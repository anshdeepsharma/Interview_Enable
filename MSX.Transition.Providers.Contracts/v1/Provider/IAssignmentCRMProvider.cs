using MSX.Common.Models.Assignments;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface IAssignmentCRMProvider
    {
        Task<List<AccountUserRole>> GetAssignmentsByAccountIdAsync(string accountId);
    }
}
