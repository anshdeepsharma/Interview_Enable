using Microsoft.Extensions.Options;
using MSX.Common.Infra;
using MSX.Common.Models.Accounts;
using MSX.Common.Models.ApplicationException;
using MSX.Common.Models.Assignments;
using MSX.Common.Models.Enums;
using MSX.Common.Models.Transitions;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class SSPTransitionService : ITransitionTypeService
    {
        private readonly ITranstionHandler _transtionHandler;
        private readonly IGrantAccessService _grantAccessService;
        private readonly BREConfig _breConfig;
        private readonly TransitionConfig _transitionConfig;

        public SSPTransitionService(ITranstionHandler transtionHandler
            , AccountHandler accountHandler
            , OpportunityHandler opportunityHandler
            , AuditHistoryHandler auditHistoryHandler
            , BREHandler breHandler
            , AssignmentHandler assignmentHandler
            , ExistingTransitionsHandler existingTransitionsHandler
            , IGrantAccessService grantAccessService
            , IOptions<BREConfig> breConfig
            , IOptions<TransitionConfig> transitionConfig)
        {
            _transtionHandler = transtionHandler;
            _transtionHandler.SetNextHandler(accountHandler)
                .SetNextHandler(opportunityHandler)
                .SetNextHandler(auditHistoryHandler)
                .SetNextHandler(breHandler)
                .SetNextHandler(existingTransitionsHandler)
                .SetNextHandler(assignmentHandler);
            _grantAccessService = grantAccessService;
            _breConfig = breConfig.Value;
            _transitionConfig = transitionConfig.Value;
        }

        public async Task ExecuteTransitionAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            // Setting Transition Data Model
            transitionDataModel.IsAccountEligibleForTransition = IsAccountEligibleForTransition;
            transitionDataModel.TransitionManagerFilter = TransitionManagerFilter;
            transitionDataModel.TransitionType = TransitionType.STU.ToString();
            transitionDataModel.TransitionSubType = transitionDataModel.AssignmentEvent?.data.Qualifier2;
            transitionDataModel.ExecuteCreateLogic = ExecuteCreateLogic;
            transitionDataModel.ExecuteDeleteLogic = ExecuteDeleteLogic;

            string subject = transitionDataModel.AssignmentEvent?.Subject!;
            string assignmentStatus = transitionDataModel.AssignmentEvent?.data.AssignmentStatus!;

            bool checkOpportunity = false;
            if ((Constants.UPDATE.Equals(subject, StringComparison.InvariantCultureIgnoreCase)
                && Constants.EXPIRED.Equals(assignmentStatus, StringComparison.InvariantCultureIgnoreCase))
                || Constants.DELETE.Equals(subject, StringComparison.InvariantCultureIgnoreCase))
                checkOpportunity = true;


            transitionDataModel.CheckFlags = new CheckFlags()
            {
                CheckArea = true,
                CheckPreviousSegment = false,
                CheckSegment = false,
                RuleName = _breConfig.RuleName!,
                CheckOpportunity = checkOpportunity,
                CheckTransitionLastXHours = _transitionConfig.CheckTransitionLastXHours
            };

            await _transtionHandler.ExecuteAsync(transitionDataModel, transition);

            //await _grantAccessService.GrantAccess(transitionDataModel, transition);
        }

        private AccountUserRole? TransitionManagerFilter(TransitionDataModel transitionDataModel)
        {
            return transitionDataModel.ExistingAssignments?.Where(x => Constants.SSPManagerRoleRummary.Equals(x.StandardTitle, StringComparison.InvariantCultureIgnoreCase)
            && x.Qualifier2.Equals(transitionDataModel.TransitionSubType, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
        private bool IsAccountEligibleForTransition(Account account)
        {
            List<string> SSPEligibleAccountAreas = new List<string>() { Area.UnitedStates, Area.Canada, Area.Latam };
            if (account.Data.Area == null || !SSPEligibleAccountAreas.Contains(account.Data.Area))
                return false;

            List<string> SSPEligibleAccountSubSegments = new List<string>() {
                "Major - Commercial Other",
                "Major - Education",
                "Major - Government Other",
                "Major - Health",
                "Major - State & Local Governments",
                "Strategic - Commercial Other",
                "Strategic - Health",
                "Strategic - Public Sector"
            };

            if (!string.IsNullOrEmpty(account.Data.SubSegment) && SSPEligibleAccountSubSegments.Contains(account.Data.SubSegment))
                return true;

            return false;
        }

        private void ExecuteCreateLogic(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            if (transitionDataModel.ExistingTransitions != null && transitionDataModel.ExistingTransitions.Any())
            {
                Transitions.Transition existingTransition = transitionDataModel.ExistingTransitions.First();
                CopyAttributes(transition, existingTransition);

                transition.TransitionTeams = new List<TransitionTeam>();
                bool hasOwnerUpdate = false;

                // Set Manager
                if (IsManager(transitionDataModel))
                {
                    transition.Manager = transitionDataModel.AssignmentEventUserId;
                }
                else
                {
                    if (transition.Manager == null)
                    {
                        // Search manager in assignments in Virtual Entity
                        AccountUserRole? manager = TransitionManagerFilter(transitionDataModel);

                        if (manager != null)
                            transition.Manager = manager.SystemUserId;
                    }
                }

                if (transition.Manager != null)
                {
                    if (transition.IncomingSSP == null)
                    {
                        transition.OwnerId = transition.Manager!;
                        transition.OwnerRoleType = Constants.STUM;
                        hasOwnerUpdate = true;
                    }
                }

                // Set Incoming SSP
                if (IsSSP(transitionDataModel))
                {
                    if (transition.IncomingSSP == null)
                    {
                        transition.IncomingSSP = transitionDataModel.AssignmentEventUserId;
                        transition.OwnerId = transition.IncomingSSP!;
                        transition.OwnerRoleType = Constants.SSP;
                        hasOwnerUpdate = true;
                    }

                    // Search if the member already exists in transition team
                    TransitionTeam? existingTransitionTeam = existingTransition.TransitionTeams?.Where(x => x.MemberId.Equals(transitionDataModel.AssignmentEventUserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (existingTransitionTeam == null)
                        AddToTransitionTeam(transitionDataModel, transition, "Incoming");
                }

                if (hasOwnerUpdate)
                {
                    // Need to update owner all existing team records
                    if (existingTransition.TransitionTeams != null)
                    {
                        foreach (TransitionTeam team in existingTransition.TransitionTeams)
                        {
                            if (!team.OwnerId.Equals(transition.OwnerId, StringComparison.InvariantCultureIgnoreCase))
                            {
                                transition.TransitionTeams.Add(
                                    new TransitionTeam()
                                    {
                                        EventId = transitionDataModel.AssignmentEvent?.Id!,
                                        Id = team.Id,
                                        OwnerId = transition.OwnerId
                                    }
                                );
                            }
                        }
                    }
                }

            }
            else
            {
                SetTransitionBasicAttributes(transitionDataModel, transition);

                // Set Manager
                if (IsManager(transitionDataModel))
                    transition.Manager = transitionDataModel.AssignmentEventUserId;
                else
                {
                    // Search manager in assignments in Virtual Entity
                    AccountUserRole? manager = TransitionManagerFilter(transitionDataModel);

                    if (manager != null)
                        transition.Manager = manager.SystemUserId;
                }

                // Set Incoming SSP
                if (IsSSP(transitionDataModel))
                    transition.IncomingSSP = transitionDataModel.AssignmentEventUserId;

                // If not manager, add this user to transition team
                if (!IsManager(transitionDataModel))
                    AddToTransitionTeam(transitionDataModel, transition, "Incoming");
            }
        }

        private void ExecuteDeleteLogic(TransitionDataModel transitionDataModel, Transitions.Transition transition)
        {
            // Nothing to process in case of Outgoing Manager
            if (IsManager(transitionDataModel))
                throw new DomainException("No need to process for Outgoing Manager", System.Net.HttpStatusCode.NoContent);

            if (transitionDataModel.ExistingTransitions != null && transitionDataModel.ExistingTransitions.Any())
            {
                var existingTransition = transitionDataModel.ExistingTransitions.First();
                CopyAttributes(transition, existingTransition);

                bool hasOwnerUpdate = false;

                // Set Manager
                if (transition.Manager == null)
                {
                    // Search manager in assignments in Virtual Entity
                    AccountUserRole? manager = TransitionManagerFilter(transitionDataModel);

                    if (manager != null)
                    {
                        transition.Manager = manager.SystemUserId;
                        if (transition.IncomingSSP == null)
                        {
                            transition.OwnerId = transition.Manager!;
                            transition.OwnerRoleType = Constants.STUM;
                            hasOwnerUpdate = true;
                        }
                    }
                }

                // Set Outgoing SSP
                if (IsSSP(transitionDataModel))
                {
                    if (transition.OutgoingSSP == null)
                        transition.OutgoingSSP = transitionDataModel.AssignmentEventUserId;

                    // Search if the member already exists in transition team
                    TransitionTeam? existingTransitionTeam = existingTransition.TransitionTeams?.Where(x => x.MemberId.Equals(transitionDataModel.AssignmentEventUserId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (existingTransitionTeam == null)
                        AddToTransitionTeam(transitionDataModel, transition, "Outgoing");
                }

                if (hasOwnerUpdate)
                {
                    // Need to update owner all existing team records
                    if (existingTransition.TransitionTeams != null)
                    {
                        foreach (TransitionTeam team in existingTransition.TransitionTeams)
                        {
                            if (!team.OwnerId.Equals(transition.OwnerId, StringComparison.InvariantCultureIgnoreCase))
                            {
                                transition.TransitionTeams.Add(
                                    new TransitionTeam()
                                    {
                                        EventId = transitionDataModel.AssignmentEvent?.Id!,
                                        Id = team.Id,
                                        OwnerId = transition.OwnerId
                                    }
                                );
                            }
                        }
                    }
                }
            }
            else
            {
                SetTransitionBasicAttributes(transitionDataModel, transition);

                // Set Manager
                AccountUserRole? manager = TransitionManagerFilter(transitionDataModel);

                if (manager != null)
                {
                    transition.Manager = manager.SystemUserId;
                    // Set manager as owner
                    transition.OwnerId = manager.SystemUserId;
                }

                // Set Outgoing SSP
                if (IsSSP(transitionDataModel))
                    transition.OutgoingSSP = transitionDataModel.AssignmentEventUserId;

                // If not manager, add this user to transition team
                if (!IsManager(transitionDataModel))
                    AddToTransitionTeam(transitionDataModel, transition, "Outgoing");
            }
        }

        private bool IsManager(TransitionDataModel transitionDataModel)
        {
            return Constants.SSPManagerRoleRummary.Equals(transitionDataModel.AssignmentEvent?.data.RoleSummary!, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsSSP(TransitionDataModel transitionDataModel)
        {
            return Constants.SSPRoleSummary.Equals(transitionDataModel.AssignmentEvent?.data.RoleSummary!, StringComparison.InvariantCultureIgnoreCase);
        }
        private string GetTransitionRole(TransitionDataModel transitionDataModel)
        {
            if (IsSSP(transitionDataModel))
                return "SSP";

            return string.Empty;
        }
        private void SetTransitionBasicAttributes(TransitionDataModel transitionDataModel, Transitions.Transition transition)
        {
            // Set Transition attributes
            transition.Name = $"STU Account Transition - {transitionDataModel.Account?.SalesAccountName} - {transitionDataModel.TransitionSubType} - {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt")}";
            transition.AccountTransitiionType = transitionDataModel.TransitionType;
            transition.SolutionArea = transitionDataModel.TransitionSubType;
            transition.ActivityTypeCode = "Account Transition";
            transition.ActivityStatusCode = "Draft";
            transition.DueDate = DateTime.UtcNow.AddDays(60).ToString();
            transition.CreatedOn = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            //Set owner
            transition.OwnerId = transitionDataModel.AssignmentEventUserId!;
            transition.OwnerRoleType = IsManager(transitionDataModel) ? Constants.STUM : Constants.SSP;
        }

        private void CopyAttributes(Transitions.Transition transition, Transitions.Transition existingTransition)
        {
            transition.Id = existingTransition!.Id;
            transition.AccountTransitiionType = existingTransition.AccountTransitiionType;
            transition.SolutionArea = existingTransition.SolutionArea;
            transition.OwnerId = existingTransition.OwnerId;
            transition.IncomingSSP = existingTransition.IncomingSSP;
            transition.OutgoingSSP = existingTransition.OutgoingSSP;
            transition.Manager = existingTransition.Manager;
            transition.ActivityStatusCode = existingTransition.ActivityStatusCode;
        }

        private void AddToTransitionTeam(TransitionDataModel transitionDataModel, Transitions.Transition transition, string transitionMode)
        {
            if (transition != null)
            {
                if (transition.TransitionTeams == null)
                    transition.TransitionTeams = new();

                transition.TransitionTeams.Add(
                    new TransitionTeam()
                    {
                        EventId = transitionDataModel.AssignmentEvent?.Id!,
                        MemberId = transitionDataModel.AssignmentEventUserId!,
                        Role = GetTransitionRole(transitionDataModel),
                        TransitionMode = transitionMode,
                        OwnerId = transition.OwnerId,
                        SolutionArea = transitionDataModel.AssignmentEvent?.data.Qualifier2
                    }
                );
            }
        }

        public void ComputeTransitionStatus(Transitions.Transition transition)
        {
            if (transition.OwnerId != null)
            {
                if ((transition.OwnerId.Equals(transition.IncomingSSP, StringComparison.InvariantCultureIgnoreCase)
                    || transition.OwnerId.Equals(transition.Manager, StringComparison.InvariantCultureIgnoreCase))
                    && transition.OutgoingSSP != null)
                {
                    transition.ActivityStatusCode = "In Progress";
                }
            }
        }

        public void ComputeOwner(Transitions.Transition transition)
        {
            // First Incoming SSP should be owner
            if (transition.IncomingSSP != null)
                transition.OwnerId = transition.IncomingSSP;
            else if (transition.Manager != null)
                transition.OwnerId = transition.Manager;
        }
    }
}
