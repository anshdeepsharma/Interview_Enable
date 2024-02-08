using Microsoft.Extensions.Logging;
using MSX.Common.Models.AccountPlans;
using MSX.Transition.Providers.Contracts.v1.Provider;

namespace MSX.Transition.Providers.CRM
{
    public class AccountPlanCRMProvider : IAccountPlanCRMProvider
    {
        private readonly ILogger<IAccountPlanCRMProvider> _logger;
        private readonly ICRMProvider _crmProvider;

        public AccountPlanCRMProvider(ILogger<IAccountPlanCRMProvider> logger,
            ICRMProvider crmProvider)
        {
            _logger = logger;
            _crmProvider = crmProvider;
        }

        public async Task<AccountPlan> GetAccountPlanAsync(string accountId)
        {
            var query = $"msp_accountplans?$filter=_msp_parentaccountid_value eq {accountId}";
            return await _crmProvider.ExecuteGetRequestAsync<AccountPlan>(query);
        }
    }
}