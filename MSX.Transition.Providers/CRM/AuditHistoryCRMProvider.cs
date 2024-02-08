using Microsoft.Extensions.Logging;
using MSX.Common.Models.Audits;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM.Contract;
using System.Text.Json;

namespace MSX.Transition.Providers.CRM
{
    public class AuditHistoryCRMProvider : IAuditHistoryCRMProvider
    {
        private readonly ILogger<IAuditHistoryCRMProvider> _logger;
        private readonly ICRMProvider _crmProvider;

        public AuditHistoryCRMProvider(ILogger<IAuditHistoryCRMProvider> logger,
         ICRMProvider crmProvider)
        {
            _logger = logger;
            _crmProvider = crmProvider;
        }

        public async Task<AuditHistory> GetAccountAuditHistoryAsync(string accountId)
        {
            var audityQuery = $"audits?$filter=objecttypecode eq 'account' and _objectid_value eq {accountId} &$orderby=createdon desc";
            var result = await _crmProvider.ExecuteGetRequestAsync<ResponseValueArray<AuditHistoryCds>>(audityQuery);
            return MapAuditHistoryCds(result);
        }

        private AuditHistory MapAuditHistoryCds(ResponseValueArray<AuditHistoryCds>? accountCdsList)
        {
            var auditHistory = new AuditHistory();
            auditHistory.ChangeData = new();

            if (accountCdsList?.value?.Count() > 0)
            {
                var list = accountCdsList?.value;

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (item.changedata != null)
                        {
                            ChangeData? changedata = JsonSerializer
                             .Deserialize<ChangeData>(item.changedata,
                             new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            if (changedata != null)
                                auditHistory.ChangeData.Add(changedata);
                        }
                    }
                }
            }

            return auditHistory;

        }
    }
}