using MSX.Common.Models.Accounts;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface IAccountCRMProvider
    {
        Task<Account?> GetAccountByCRMIdAsync(string crmAccountId);
    }
}
