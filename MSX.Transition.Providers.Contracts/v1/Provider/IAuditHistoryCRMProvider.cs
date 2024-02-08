using MSX.Common.Models.Audits;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface IAuditHistoryCRMProvider
    {
        Task<AuditHistory> GetAccountAuditHistoryAsync(string accountId);
    }
}
