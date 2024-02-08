using Microsoft.Extensions.Logging;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Common;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.ApplicationException;
using MSX.Common.Models.Assignments;
using MSX.Common.Models.Enums;
using MSX.Common.Models.Responses.v1;
using MSX.Common.Models.Transitions;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM;
using System.Net;
using System.Reflection;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class TransitionService : ITransitionService
    {
        private readonly ILogger<ITransitionService> _logger;
        private readonly ICRMProvider _crmProvider;
        private readonly ICRMTranslator _crmTranslator;
        private readonly ITransitionFactory _transitionFactory;
        private readonly ITransitionTeamService _transitionTeamService;
        private readonly IGrantAccessService _grantAccessService;
        private readonly IUserCRMProvider _userCRMProvider;

        const string map = "\\Mapper\\";
        private static readonly FieldMapper? TransitionFieldMapper = System.Text.Json.JsonSerializer.Deserialize<FieldMapper>(XMLHandler.ReadXMLFile(map + "TransitionFieldMapper.json"));

        public TransitionService(ILogger<ITransitionService> logger
            , ICRMProvider crmProvider
            , ICRMTranslator crmTranslator
            , ITransitionFactory transitionFactory
            , ITransitionTeamService transitionTeamService
            , IGrantAccessService grantAccessService
            , IUserCRMProvider userCRMProvider)
        {
            _logger = logger;
            _crmProvider = crmProvider;
            _crmTranslator = crmTranslator;
            _transitionFactory = transitionFactory;
            _transitionTeamService = transitionTeamService;
            _grantAccessService = grantAccessService;
            _userCRMProvider = userCRMProvider;
        }

        public async Task<List<Response<AccountTeam>>> CreateTransitionsAsync(List<AccountTeam> accountTeams)
        {
            List<Response<AccountTeam>> responses = new();
            List<Transitions.Transition> transitions = new();
            List<TransitionTeam> transitionTeams = new();

            List<Task<Transitions.Transition>> transtionsBatchTasks = new();
            List<Task<BatchRequest>> batchRequestTasks = new();

            // Create Transition objects
            foreach (AccountTeam accountTeam in accountTeams!)
            {
                Response<AccountTeam> response = new(accountTeam)
                {
                    correlationId = accountTeam.Id ?? string.Empty
                };
                responses.Add(response);

                // To handle special symbols like \u002B which is +
                if (!string.IsNullOrEmpty(accountTeam.data.CrmAccountId))
                    accountTeam.data.CrmAccountId = System.Text.RegularExpressions.Regex.Unescape(accountTeam.data.CrmAccountId);

                SuperImposeChangeLog(accountTeam, accountTeam.Subject!);

                transtionsBatchTasks.Add(CreateTransition(accountTeam));
            }

            transitions = await ExecuteInParallel(transtionsBatchTasks, null, responses);

            // Merge transitions
            List<Transitions.Transition> finalTransitions = MergeTransition(transitions);

            // Create batch requests for the final transitions
            foreach (Transitions.Transition transition in finalTransitions)
                batchRequestTasks.Add(CreateBatchRequest(transition));

            List<BatchRequest> batchRequests = await ExecuteInParallel(batchRequestTasks, finalTransitions, responses);

            // Create transitions in CRM by posting batch requests
            if (batchRequests.Count > 0)
            {
                List<Response<BatchRequest>> crmResponses = await _crmProvider.ExecuteBatchRequestAsync(batchRequests);

                foreach (Response<BatchRequest> crmResponse in crmResponses)
                {
                    if (crmResponse.Obj?.ContentId != null)
                    {
                        Transitions.Transition? transition = finalTransitions.Where(x => x.EventIdList.Contains(crmResponse.Obj?.ContentId!)).FirstOrDefault();

                        if (transition != null)
                        {
                            if (crmResponse.StatusCode == (int)HttpStatusCode.OK)
                            {
                                transition.Id = Guid.Parse(crmResponse.id);

                                if (transition.TransitionTeams != null && transition.TransitionTeams.Any())
                                {
                                    foreach (Transitions.TransitionTeam team in transition.TransitionTeams)
                                        team.TransitionId = transition.Id.ToString();

                                    transitionTeams.AddRange(transition.TransitionTeams);
                                }
                                else
                                {
                                    List<Response<AccountTeam>>? filteredResponseList = responses.Where(response => response.Obj?.Id != null && transition.EventIdList.Contains(response.Obj?.Id!)).ToList();

                                    foreach (Response<AccountTeam> res in filteredResponseList)
                                    {
                                        res.SuccessMessage = crmResponse.SuccessMessage;
                                        res.Message = crmResponse.Message;
                                        res.StatusCode = crmResponse.StatusCode;
                                        res.Status = crmResponse.Status;
                                    }
                                }
                            }
                            else
                            {
                                List<Response<AccountTeam>>? filteredResponseList = responses.Where(response => response.Obj?.Id != null && transition.EventIdList.Contains(response.Obj?.Id!)).ToList();

                                foreach (Response<AccountTeam> res in filteredResponseList)
                                {
                                    res.Message = crmResponse.Message;
                                    res.StatusCode = crmResponse.StatusCode;
                                    res.Status = crmResponse.Status;
                                }
                            }
                        }
                    }
                }

                // Create Transition Teams in CRM
                if (transitionTeams.Count > 0)
                {
                    List<Response<TransitionTeam>> transitionTeamsResponses = await _transitionTeamService.CreateTransitionTeamsAsync(transitionTeams);

                    foreach (Response<TransitionTeam> transitionTeamsResponse in transitionTeamsResponses)
                    {
                        if (transitionTeamsResponse.Obj?.TransitionId != null)
                        {
                            Transitions.Transition? transition = finalTransitions.Where(x => x.Id.ToString().Equals(transitionTeamsResponse.Obj?.TransitionId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                            if (transition != null)
                            {
                                List<Response<AccountTeam>>? filteredResponseList = responses.Where(response => response.Obj?.Id != null && transition.EventIdList.Contains(response.Obj?.Id!)).ToList();
                                foreach (Response<AccountTeam> res in filteredResponseList)
                                {
                                    // Skip responses that are already set with an error
                                    if (res.StatusCode > 0 && res.StatusCode != (int)HttpStatusCode.OK)
                                        continue;

                                    res.SuccessMessage = transitionTeamsResponse.SuccessMessage;
                                    res.Message = transitionTeamsResponse.Message;
                                    res.StatusCode = transitionTeamsResponse.StatusCode;
                                    res.Status = transitionTeamsResponse.Status;
                                }
                            }

                        }
                    }
                }
            }

            // if (accountTeam != null)
            //{
            //await _grantAccessService.GrantAccess(accountTeam);
            //}

            foreach (Response<AccountTeam> response in responses)
                response.Obj = null;

            return responses;
        }

        private async Task<Transitions.Transition> CreateTransition(AccountTeam accountTeam)
        {
            // Get the transition service based on transition type
            ITransitionTypeService? transitionTypeService = _transitionFactory.GetTransitionSerice(accountTeam);

            if (transitionTypeService == null)
                throw new DomainException($"No transition service found", HttpStatusCode.NoContent);

            Transitions.Transition transition = new Transitions.Transition()
            {
                EventIdList = new() { accountTeam.Id!.ToString() }
            };

            TransitionDataModel transitionDataModel = new TransitionDataModel()
            {
                AssignmentEvent = accountTeam,
            };

            await PreCheck(transitionDataModel);

            await transitionTypeService!.ExecuteTransitionAsync(transitionDataModel, transition);

            return transition;
        }

        private async Task PreCheck(TransitionDataModel transitionDataModel)
        {
            try
            {
                Common.Models.User user = await _userCRMProvider.ResolveUserAsync(transitionDataModel.AssignmentEvent?.data.Alias ?? string.Empty);
                transitionDataModel.AssignmentEventUserId = user.Id!;
            }
            catch (DataVerseTranslatorException translatorEx)
            {
                _logger.LogWarning(translatorEx, $"Unable to resolve the user : {transitionDataModel.AssignmentEvent?.data.Alias}");
                throw new DomainException($"Unable to resolve the user : {transitionDataModel.AssignmentEvent?.data.Alias}", HttpStatusCode.NotFound);
            }
        }

        private string PostHandle(string payload)
        {
            // Use schema name for lookups
            payload = payload.Replace("msp_incomingats@odata.bind", "msp_IncomingATS@odata.bind");
            payload = payload.Replace("msp_outgoingats@odata.bind", "msp_OutgoingATS@odata.bind");
            payload = payload.Replace("msp_incomingssp@odata.bind", "msp_IncomingSSP@odata.bind");
            payload = payload.Replace("msp_outgoingssp@odata.bind", "msp_OutgoingSSP@odata.bind");
            payload = payload.Replace("msp_manager@odata.bind", "msp_Manager@odata.bind");

            return payload;
        }

        private void SuperImposeChangeLog(AccountTeam assignment, String action)
        {
            PropertyInfo[] properties = assignment.data.GetType().GetProperties();
            bool isPropertyChange;

            if (assignment.data.ChangeLog != null)
            {
                foreach (PropertyInfo propertyInfo in properties)
                {
                    isPropertyChange = false;
                    foreach (var change in assignment.data.ChangeLog)
                    {
                        if (propertyInfo.Name.Equals(change.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            isPropertyChange = true;

                            if (change.Value == null || string.Empty.Equals(change.Value.ToString()))
                            {
                                switch (action.ToUpper())
                                {
                                    case Constants.CREATE:
                                        propertyInfo.SetValue(assignment.data, null);
                                        break;
                                    case Constants.UPDATE:
                                        // Blank keyword is used to indicate that this value needs to be deleted
                                        propertyInfo.SetValue(assignment.data, "Blank");
                                        break;
                                    case Constants.DELETE:
                                        // Blank keyword is used to indicate that this value needs to be deleted
                                        propertyInfo.SetValue(assignment.data, "Blank");
                                        break;
                                }
                            }
                            else
                            {
                                if (propertyInfo.PropertyType == typeof(Guid?) || propertyInfo.PropertyType == typeof(Guid))
                                {
                                    propertyInfo.SetValue(assignment.data, Guid.Parse(change.Value.ToString() ?? throw DomainException.InvalidParameterException(propertyInfo.Name, change.Value)));
                                }
                                else if (propertyInfo.PropertyType == typeof(string))
                                {
                                    propertyInfo.SetValue(assignment.data, change.Value.ToString() ?? throw DomainException.InvalidParameterException(propertyInfo.Name, change.Value));
                                }
                                else if (propertyInfo.PropertyType == typeof(int?) || propertyInfo.PropertyType == typeof(int))
                                {
                                    propertyInfo.SetValue(assignment.data, int.Parse(change.Value.ToString() ?? throw DomainException.InvalidParameterException(propertyInfo.Name, change.Value)));
                                }
                                else if (propertyInfo.PropertyType == typeof(bool?) || propertyInfo.PropertyType == typeof(bool))
                                {
                                    propertyInfo.SetValue(assignment.data, bool.Parse(change.Value.ToString() ?? throw DomainException.InvalidParameterException(propertyInfo.Name, change.Value)));
                                }
                                else
                                {
                                    propertyInfo.SetValue(assignment.data, change.Value);
                                }
                            }
                            break;
                        }
                    }

                    if (!isPropertyChange)
                    {
                        switch (propertyInfo.Name.ToLower())
                        {
                            // skip mandatory fields
                            case "crmaccountid":
                            case "alias":
                            case "changelog":
                            case "accountsalesterritoryguid":
                            case "accountsubsidiaryguid":
                            case "rolesummary":
                            case "roletype":
                            case "roleplayed":
                            case "qualifier1":
                            case "qualifier2":
                                break;
                            default:
                                propertyInfo.SetValue(assignment.data, null);
                                break;
                        }
                    }
                }
            }
        }

        private List<Transitions.Transition> MergeTransition(List<Transitions.Transition> transitions)
        {
            List<Transitions.Transition> finalTransitions = new();

            if (transitions == null || !transitions.Any())
                return finalTransitions;

            List<Transitions.Transition> groupedTransitions = transitions.GroupBy(t => new { t.AccountId, t.AccountTransitiionType, t.SolutionArea }
                                                                                 , (key, combination) => new Transitions.Transition()
                                                                                 {
                                                                                     AccountId = key.AccountId,
                                                                                     AccountTransitiionType = key.AccountTransitiionType,
                                                                                     SolutionArea = key.SolutionArea,
                                                                                     EventIdList = new(),
                                                                                     TransitionTeams = new()
                                                                                 }).ToList();

            finalTransitions.AddRange(groupedTransitions);

            foreach (var mergedTransition in groupedTransitions)
            {
                List<Transitions.Transition> matchedTransitions = transitions.OrderBy(t => t.CreatedOn)
                                                                             .Where(x => x.AccountId.Equals(mergedTransition.AccountId, StringComparison.InvariantCultureIgnoreCase)
                                                                                    && x.AccountTransitiionType.Equals(mergedTransition.AccountTransitiionType, StringComparison.InvariantCultureIgnoreCase)
                                                                                    && ((x.SolutionArea == null && mergedTransition.SolutionArea == null)
                                                                                        || (x.SolutionArea != null && x.SolutionArea.Equals(mergedTransition.SolutionArea, StringComparison.InvariantCultureIgnoreCase)))).ToList();

                // Check if there is any existing transition
                Transitions.Transition? existingTransition = matchedTransitions.Where(x => x.Id != Guid.Empty).FirstOrDefault();

                if (existingTransition != null)
                {
                    mergedTransition.Id = existingTransition.Id;
                    mergedTransition.OwnerId = existingTransition.OwnerId;
                    mergedTransition.IncomingATS = existingTransition.IncomingATS;
                    mergedTransition.OutgoingATS = existingTransition.OutgoingATS;
                    mergedTransition.IncomingSSP = existingTransition.IncomingSSP;
                    mergedTransition.OutgoingSSP = existingTransition.OutgoingSSP;
                    mergedTransition.Manager = existingTransition.Manager;
                    mergedTransition.ActivityStatusCode = existingTransition.ActivityStatusCode;
                }

                foreach (var matchedTransition in matchedTransitions)
                {
                    // Maintaining event ids corresponding to a transition
                    mergedTransition.EventIdList.AddRange(matchedTransition.EventIdList);

                    if (existingTransition != null && matchedTransition.Id == existingTransition.Id)
                        continue;

                    if (existingTransition == null)
                    {
                        // Set Transition attributes from the first available matched information
                        if (mergedTransition.Name == null && matchedTransition.Name != null)
                            mergedTransition.Name = matchedTransition.Name;

                        if (mergedTransition.OwnerId == null && matchedTransition.OwnerId != null)
                            mergedTransition.OwnerId = matchedTransition.OwnerId;

                        if (mergedTransition.ActivityTypeCode == null && matchedTransition.ActivityTypeCode != null)
                            mergedTransition.ActivityTypeCode = matchedTransition.ActivityTypeCode;

                        if (mergedTransition.AccountTPId == null && matchedTransition.AccountTPId != null)
                            mergedTransition.AccountTPId = matchedTransition.AccountTPId;

                        if (mergedTransition.PreviousSegment == null && matchedTransition.PreviousSegment != null)
                            mergedTransition.PreviousSegment = matchedTransition.PreviousSegment;

                        if (mergedTransition.PreviousSubsegment == null && matchedTransition.PreviousSubsegment != null)
                            mergedTransition.PreviousSubsegment = matchedTransition.PreviousSubsegment;

                        if (mergedTransition.CurrentSegment == null && matchedTransition.CurrentSegment != null)
                            mergedTransition.CurrentSegment = matchedTransition.CurrentSegment;

                        if (mergedTransition.CurrentSubsegment == null && matchedTransition.CurrentSubsegment != null)
                            mergedTransition.CurrentSubsegment = matchedTransition.CurrentSubsegment;

                        if (mergedTransition.DueDate == null && matchedTransition.DueDate != null)
                            mergedTransition.DueDate = matchedTransition.DueDate;

                        if (mergedTransition.ActivityStatusCode == null && matchedTransition.ActivityStatusCode != null)
                            mergedTransition.ActivityStatusCode = matchedTransition.ActivityStatusCode;
                    }

                    // Keep the lastest IncomingATS
                    if (matchedTransition.IncomingATS != null)
                        mergedTransition.IncomingATS = matchedTransition.IncomingATS;

                    // Keep the lastest OutgoingATS
                    if (matchedTransition.OutgoingATS != null)
                        mergedTransition.OutgoingATS = matchedTransition.OutgoingATS;

                    // Keep the first IncomingSSP
                    if (mergedTransition.IncomingSSP == null && matchedTransition.IncomingSSP != null)
                        mergedTransition.IncomingSSP = matchedTransition.IncomingSSP;

                    // Keep the first OutgoingSSP
                    if (mergedTransition.OutgoingSSP == null && matchedTransition.OutgoingSSP != null)
                        mergedTransition.OutgoingSSP = matchedTransition.OutgoingSSP;

                    // Keep the lastest Manger
                    if (matchedTransition.Manager != null)
                        mergedTransition.Manager = matchedTransition.Manager;

                }

                bool isValidTransitionType = Enum.TryParse(typeof(TransitionType), mergedTransition.AccountTransitiionType, true, out object? type);
                if (isValidTransitionType)
                {
                    ITransitionTypeService? transitionTypeService = _transitionFactory.GetTransitionSerice((TransitionType)type!);
                    if (transitionTypeService != null)
                    {
                        transitionTypeService.ComputeOwner(mergedTransition);
                        transitionTypeService.ComputeTransitionStatus(mergedTransition);
                    }
                }

                // Merge Transition Team
                foreach (var matchedTransition in matchedTransitions)
                {
                    if (matchedTransition.TransitionTeams != null)
                    {
                        foreach (TransitionTeam transitionTeam in matchedTransition.TransitionTeams)
                        {
                            TransitionTeam? matchedTransitionTeam = mergedTransition.TransitionTeams.Where(x => x.MemberId.Equals(transitionTeam.MemberId, StringComparison.InvariantCultureIgnoreCase)
                                                                                                                && x.Role.Equals(transitionTeam.Role, StringComparison.InvariantCultureIgnoreCase)
                                                                                                                && x.TransitionMode.Equals(transitionTeam.TransitionMode, StringComparison.InvariantCultureIgnoreCase))
                                                                                                    .FirstOrDefault();

                            if (matchedTransitionTeam == null)
                            {
                                mergedTransition.TransitionTeams.Add(
                                    new TransitionTeam()
                                    {
                                        Id = transitionTeam.Id,
                                        TransitionId = mergedTransition.Id != Guid.Empty ? mergedTransition.Id.ToString() : string.Empty,
                                        EventId = transitionTeam.EventId,
                                        MemberId = transitionTeam.MemberId,
                                        Role = transitionTeam.Role,
                                        TransitionMode = transitionTeam.TransitionMode,
                                        OwnerId = mergedTransition.OwnerId!,
                                        SolutionArea = transitionTeam.SolutionArea
                                    }
                                );
                            }
                        }
                    }
                }
            }

            return finalTransitions;
        }

        private async Task<BatchRequest> CreateBatchRequest(Transitions.Transition transition)
        {
            HttpMethod method = HttpMethod.Post;
            string endpoint = Constants.TransitionCRMEntity;
            string payload = string.Empty;

            var obj = await _crmTranslator.Translate(transition, TransitionFieldMapper);

            payload = PostHandle(obj.ToString());

            if (transition?.Id != null && transition?.Id != Guid.Empty)
            {
                method = HttpMethod.Patch;
                endpoint += "(" + transition?.Id.ToString() + ")";
            }

            return _crmProvider.CreateBatchRequest(method, endpoint, payload, transition!.EventIdList.First());
        }

        private async Task<List<T>> ExecuteInParallel<T>(List<Task<T>> tasks, List<Transitions.Transition>? transitions, List<Response<AccountTeam>> responses)
        {
            List<T> objects = new();
            int index = 0;

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during transition tasks");
            }

            foreach (var task in tasks)
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (task.Result != null)
                    {
                        objects.Add(task.Result);
                    }
                }
                else
                {
                    List<Response<AccountTeam>>? filteredResponses = null;

                    if (transitions != null)
                    {
                        filteredResponses = responses.Where(x => x.Obj?.Id != null
                                                                && index < transitions.Count
                                                                && transitions[index].EventIdList.Contains(x.Obj?.Id!)).ToList();
                    }
                    else
                    {
                        if (index < responses.Count)
                            filteredResponses = new() { responses[index] };
                    }

                    if (filteredResponses != null)
                    {
                        foreach (Response<AccountTeam> filteredResponse in filteredResponses)
                        {
                            filteredResponse.Message = task.Exception?.InnerException?.Message!;

                            if (task.Exception?.InnerException != null && task.Exception?.InnerException is DomainException)
                                filteredResponse.StatusCode = (int)((DomainException)task.Exception?.InnerException!).StatusCode;
                            else
                                filteredResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }
                    }

                }
                index++;
            }

            return objects;
        }
    }
}
