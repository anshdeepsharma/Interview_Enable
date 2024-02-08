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
    public class ATSTransitionService : ITransitionTypeService
    {
        private readonly ITranstionHandler _transtionHandler;
        private readonly BREConfig _breConfig;
        private readonly TransitionConfig _transitionConfig;

        public ATSTransitionService(ITranstionHandler transtionHandler
            , AccountHandler accountHandler
            , AuditHistoryHandler auditHistoryHandler
            , BREHandler breHandler
            , AssignmentHandler assignmentHandler
            , ExistingTransitionsHandler existingTransitionsHandler
            , IOptions<BREConfig> breConfig
            , IOptions<TransitionConfig> transitionConfig)
        {
            _transtionHandler = transtionHandler;
            _transtionHandler.SetNextHandler(accountHandler)
                .SetNextHandler(auditHistoryHandler)
                .SetNextHandler(breHandler)
                .SetNextHandler(existingTransitionsHandler)
                .SetNextHandler(assignmentHandler);
            _breConfig = breConfig.Value;
            _transitionConfig = transitionConfig.Value;
        }

        public async Task ExecuteTransitionAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            transitionDataModel.IsAccountEligibleForTransition = IsAccountEligibleForTransition;
            transitionDataModel.TransitionManagerFilter = TransitionManagerFilter;
            transitionDataModel.TransitionType = TransitionType.ATS.ToString();
            transitionDataModel.ExecuteCreateLogic = ExecuteCreateLogic;
            transitionDataModel.ExecuteDeleteLogic = ExecuteDeleteLogic;

            transitionDataModel.CheckFlags = new CheckFlags()
            {
                CheckArea = true,
                CheckPreviousSegment = true,
                CheckSegment = true,
                RuleName = _breConfig?.RuleName!,
                CheckTransitionLastXHours = _transitionConfig.CheckTransitionLastXHours
            };

            transition.ActivityTypeCode = "Account Transition";
            await _transtionHandler.ExecuteAsync(transitionDataModel, transition);
        }

        private AccountUserRole? TransitionManagerFilter(TransitionDataModel transitionDataModel)
        {
            return transitionDataModel.ExistingAssignments?.Where(x => x.StandardTitle == "ATU Manager").FirstOrDefault();
        }

        private bool IsAccountEligibleForTransition(Account account)
        {
            List<string> ATSEligibleNonGPAccountSegments = new List<string>() { "Strategic Public Sector", "Major Public Sector" };
            List<string> ATSEligibleNonGPAccountSubSegments = new List<string>() {
                "Strategic - Public Sector",
                "Major - Federal Government",
                "Major - Government Other",
                "Major - State & Local Governments",
                "Major - Education",
            };
            List<string> ATSEligibleGPAccountSegments = new List<string>() { "Strategic Commercial", "Major Commercial" };
            List<string> ATSEligibleGPAccountSubSegments = new List<string>() {
                "Strategic - Health",
                "Strategic - Commercial Other",
                "Major - Health",
                "Major - Commercial Other"
            };

            if (string.IsNullOrEmpty(account.Data.Segment) || string.IsNullOrEmpty(account.Data.SubSegment))
                return false;

            if (ATSEligibleNonGPAccountSegments.Contains(account.Data.Segment)
                && ATSEligibleNonGPAccountSubSegments.Contains(account.Data.SubSegment)
                && string.IsNullOrEmpty(account.Data.GlobalParentAccountId))
                return true;

            if (ATSEligibleGPAccountSegments.Contains(account.Data.Segment)
                && ATSEligibleGPAccountSubSegments.Contains(account.Data.SubSegment)
                && ((!string.IsNullOrEmpty(account.Data.GlobalParentAccountId) && "Yes".Equals(account.Data.IsGlobalHQ, StringComparison.InvariantCultureIgnoreCase))
                    || (string.IsNullOrEmpty(account.Data.GlobalParentAccountId) && "No".Equals(account.Data.IsGlobalHQ, StringComparison.InvariantCultureIgnoreCase))))
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
                    transition.OwnerId = transition.Manager;
                    hasOwnerUpdate = true;
                }

                // Set Incoming ATS
                if (IsATS(transitionDataModel))
                    transition.IncomingATS = transitionDataModel.AssignmentEventUserId;

                // Set Incoming ATS/Incoming CSAM in transition team
                if (!IsManager(transitionDataModel))
                {
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

                if (transition.Manager != null)
                {
                    // Set manager as owner
                    transition.OwnerId = transition.Manager;
                }

                // Set Incoming ATS
                if (IsATS(transitionDataModel))
                    transition.IncomingATS = transitionDataModel.AssignmentEventUserId;

                // If not manager, add this user to transition team
                if (!IsManager(transitionDataModel))
                    AddToTransitionTeam(transitionDataModel, transition, "Incoming");
            }
        }

        private void ExecuteDeleteLogic(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
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
                        transition.OwnerId = transition.Manager;
                        hasOwnerUpdate = true;
                    }
                }

                // Set Outgoing ATS
                if (IsATS(transitionDataModel))
                    transition.OutgoingATS = transitionDataModel.AssignmentEventUserId;

                if (!IsManager(transitionDataModel))
                {
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
                // Search manager in assignments in Virtual Entity
                AccountUserRole? manager = TransitionManagerFilter(transitionDataModel);

                if (manager != null)
                {
                    transition.Manager = manager.SystemUserId;
                    // Set manager as owner
                    transition.OwnerId = manager.SystemUserId;
                }

                // Set Outgoing ATS
                if (IsATS(transitionDataModel))
                    transition.OutgoingATS = transitionDataModel.AssignmentEventUserId;

                // If not manager, add this user to transition team
                if (!IsManager(transitionDataModel))
                    AddToTransitionTeam(transitionDataModel, transition, "Outgoing");
            }
        }

        private bool IsManager(TransitionDataModel transitionDataModel)
        {
            return Constants.ATUManagerRole.Equals(transitionDataModel.AssignmentEvent?.data.RolePlayed, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsATS(TransitionDataModel transitionDataModel)
        {
            return Constants.ATSRole.Equals(transitionDataModel.AssignmentEvent?.data.RolePlayed, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsCSAM(TransitionDataModel transitionDataModel)
        {
            return Constants.CSAMRoleSummary.Equals(transitionDataModel.AssignmentEvent?.data.RoleSummary, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetTransitionRole(TransitionDataModel transitionDataModel)
        {
            if (IsATS(transitionDataModel))
                return "ATS";
            else if (IsCSAM(transitionDataModel))
                return "CSAM";

            return string.Empty;
        }

        private void SetTransitionBasicAttributes(TransitionDataModel transitionDataModel, Transitions.Transition transition)
        {
            // Set Transition attributes
            transition.Name = $"ATS Account Transition - {transitionDataModel.Account?.SalesAccountName} - {DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt")}";
            transition.AccountTransitiionType = transitionDataModel.TransitionType;
            transition.ActivityTypeCode = "Account Transition";
            transition.ActivityStatusCode = "Draft";
            transition.DueDate = DateTime.UtcNow.AddDays(30).ToString();
            transition.CreatedOn = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            //Set Account Executive as owner
            transition.OwnerId = transitionDataModel.Account?.CRMOwnerId ?? transitionDataModel.AssignmentEventUserId;
        }

        private void CopyAttributes(Transitions.Transition transition, Transitions.Transition existingTransition)
        {
            transition.Id = existingTransition.Id;
            transition.AccountTransitiionType = existingTransition.AccountTransitiionType;
            transition.OwnerId = existingTransition.OwnerId;
            transition.IncomingATS = existingTransition.IncomingATS;
            transition.OutgoingATS = existingTransition.OutgoingATS;
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
                if (transition.IncomingATS != null && !transition.IncomingATS.Equals(transition.OutgoingATS, StringComparison.InvariantCultureIgnoreCase))
                {
                    transition.ActivityStatusCode = "In Progress";
                }
            }
        }

        public void ComputeOwner(Transitions.Transition transition)
        {
            // Set manager as owner
            if (transition.Manager != null)
                transition.OwnerId = transition.Manager;
        }
    }
}
