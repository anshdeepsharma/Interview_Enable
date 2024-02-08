using Microsoft.Extensions.Logging;
using MSX.Common.Models.Enums;
using MSX.Common.Models.Transitions;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM.Contract;
using Transitions = MSX.Common.Models.Transitions;
namespace MSX.Transition.Providers.CRM
{
    public class TransitionCRMProvider : ITransitionCRMProvider
    {
        private readonly ILogger<ITransitionCRMProvider> _logger;
        private readonly ICRMProvider _crmProvider;
        private readonly ICRMXMLHandler _crmXMLHandler;

        private readonly Dictionary<string, string> _transitionTypeCodeDict;

        private const string AccountTransitionActivityTypeCode = "861980000"; // Account Transition - 861980000

        private static string TransitionFetchXML = XMLHandler.ReadXMLFile(Constants.FetchXMLPath + Constants.TransitionFetchXMLFile);

        public TransitionCRMProvider(ILogger<ITransitionCRMProvider> logger,
            ICRMProvider crmProvider,
            ICRMXMLHandler crmXMLHandler)
        {
            _logger = logger;
            _crmProvider = crmProvider;
            _transitionTypeCodeDict = new Dictionary<string, string>()
            {
                { TransitionType.ATS.ToString(), "606820001" },
                { TransitionType.STU.ToString(), "606820004" }
            };
            _crmXMLHandler = crmXMLHandler;
        }

        public async Task<List<Transitions.Transition>> GetTransitionAsync(string transitionType
            , string? transitionSubtype
            , string accountId
            , string checkTransitionLastXHours)
        {
            Transitions.Transition transition = new()
            {
                AccountTransitiionType = _transitionTypeCodeDict[transitionType],
                SolutionArea = transitionSubtype,
                ActivityTypeCode = AccountTransitionActivityTypeCode,
                AccountId = accountId
            };

            string query = _crmXMLHandler.ConstructQuery("msp_relationshipmanagements"
                , TransitionFetchXML
                , Entity.Transition.ToString()
                , Constants.TransitionSelectAttributes
                , new Dictionary<string, string>()
                    {
                        { "AccountTransitiionType", "msp_accounttransitiiontype" },
                        { "AccountId", "msp_accountid" },
                        { "ActivityTypeCode", "msp_activitytypecode" },
                        { "SolutionArea", "msp_transitionsolutionarea" }
                    }
                , transition);

            query = query.Replace("{0}", checkTransitionLastXHours);

            ResponseValueArray<TransitionCds> transitionCdsList = await _crmProvider.ExecuteGetRequestAsync<ResponseValueArray<TransitionCds>>(query);
            List<Transitions.Transition> transitionList = MapToTransition(transitionCdsList);
            return transitionList;
        }

        public List<Transitions.Transition> MapToTransition(ResponseValueArray<TransitionCds> transitionCdsList)
        {
            List<Transitions.Transition> transitions = new();

            if (transitionCdsList?.value?.Count > 0)
            {
                foreach (TransitionCds transitionCds in transitionCdsList.value)
                {
                    Transitions.Transition? transition = transitions.Where(t => t.Id == Guid.Parse(transitionCds.Id!)).FirstOrDefault();
                    if (transition == null)
                    {
                        transition = new()
                        {
                            Id = transitionCds.Id != null ? Guid.Parse(transitionCds.Id) : Guid.Empty,
                            AccountId = transitionCds.AccountId!,
                            Name = transitionCds.Name!,
                            AccountTransitiionType = transitionCds.AccountTransitiionType!,
                            ActivityStatusCode = transitionCds.ActivityStatusCode!,
                            IncomingATS = transitionCds.IncomingATS,
                            OutgoingATS = transitionCds.OutgoingATS,
                            IncomingSSP = transitionCds.IncomingSSP,
                            OutgoingSSP = transitionCds.OutgoingSSP,
                            Manager = transitionCds.Manager,
                            SolutionArea = transitionCds.SolutionArea,
                            OwnerId = transitionCds.OwnerId!,
                            CreatedOn = transitionCds.CreatedOn
                        };

                        transitions.Add(transition);
                    }

                    if (transitionCds.TeamId != null)
                    {
                        TransitionTeam transitionTeam = new()
                        {
                            Id = transitionCds.TeamId != null ? Guid.Parse(transitionCds.TeamId) : Guid.Empty,
                            TransitionId = transitionCds.Id!,
                            OwnerId = transitionCds.TeamOwnerId!,
                            Role = transitionCds.TeamRole!,
                            MemberId = transitionCds.TeamMemberId!,
                            TransitionMode = transitionCds.TeamTransitionMode!,
                            SolutionArea = transitionCds.SolutionArea
                        };

                        if (transition.TransitionTeams == null)
                            transition.TransitionTeams = new();
                        transition.TransitionTeams.Add(transitionTeam);
                    }
                }
            }

            return transitions;
        }
    }
}