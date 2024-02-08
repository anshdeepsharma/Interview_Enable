using MSX.Common.Models.Opportunities;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface IOpportunityCRMProvider
    {
        Task<List<Opportunity>> GetOpportunityByUserAsync(string userId, string accountId);
        Task<List<Opportunity>> GetOpportunityByUserInTeamAsync(string userId, string accountId);
    }
}
