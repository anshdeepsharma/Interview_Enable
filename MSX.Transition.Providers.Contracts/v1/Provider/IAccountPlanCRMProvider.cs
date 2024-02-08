using MSX.Common.Models.AccountPlans;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface IAccountPlanCRMProvider
    {
        Task<AccountPlan> GetAccountPlanAsync(string accountId);
    }
}
