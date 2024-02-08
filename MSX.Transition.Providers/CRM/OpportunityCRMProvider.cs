using Microsoft.Extensions.Logging;
using MSX.Common.Models.Enums;
using MSX.Common.Models.Opportunities;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM.Contract;

namespace MSX.Transition.Providers.CRM
{
    public class OpportunityCRMProvider : IOpportunityCRMProvider
    {
        private readonly ILogger<IOpportunityCRMProvider> _logger;
        private readonly ICRMProvider _crmProvider;
        private readonly ICRMXMLHandler _crmXMLHandler;

        private static string OpportunityByUserFetchXML = XMLHandler.ReadXMLFile(Constants.FetchXMLPath + Constants.OpportunityByUserFetchXMLFile);
        private static string OpportunityByUserInTeamFetchXML = XMLHandler.ReadXMLFile(Constants.FetchXMLPath + Constants.OpportunityByUserInTeamFetchXMLFile);
        private static string SelectAttributes = Constants.OpportunitySelectAttributes;

        public OpportunityCRMProvider(ILogger<IOpportunityCRMProvider> logger
            , ICRMProvider crmProvider
            , ICRMXMLHandler crmXMLHandler)
        {
            _logger = logger;
            _crmProvider = crmProvider;
            _crmXMLHandler = crmXMLHandler;
        }

        public async Task<List<Opportunity>> GetOpportunityByUserAsync(string userId, string accountId)
        {
            return await GetOpportunity(userId, accountId, OpportunityByUserFetchXML);
        }

        public async Task<List<Opportunity>> GetOpportunityByUserInTeamAsync(string userId, string accountId)
        {
            return await GetOpportunity(userId, accountId, OpportunityByUserInTeamFetchXML);
        }

        private async Task<List<Opportunity>> GetOpportunity(string userId, string accountId, string fetchXml)
        {
            Opportunity search = new() { AccountId = accountId };

            string query = _crmXMLHandler.ConstructQuery<Opportunity>("opportunities"
                , fetchXml
                , Entity.Opportunity.ToString()
                , SelectAttributes
                , new Dictionary<string, string>()
                    {
                        { "AccountId", "accountid" }
                    }
                , search);

            query = query.Replace("{0}", userId.ToString());

            var opportunityCdsList = await _crmProvider.ExecuteGetRequestAsync<ResponseValueArray<OpportunityCds>>(query);
            return MapOpportunityCds(opportunityCdsList);
        }

        private List<Opportunity> MapOpportunityCds(ResponseValueArray<OpportunityCds>? opportunityCdsList)
        {
            var opportunityList = new List<Opportunity>();

            if (opportunityCdsList?.value?.Count > 0)
            {
                for (int index = 0; index < opportunityCdsList.value.Count; index++)
                {
                    var opportunityCds = opportunityCdsList.value[index];

                    var transformedOpportunity = new Opportunity()
                    {
                        OpportunityNumber = opportunityCds.OpportunityNumber
                    };
                    opportunityList.Add(transformedOpportunity);
                }
            }
            return opportunityList;
        }

    }
}