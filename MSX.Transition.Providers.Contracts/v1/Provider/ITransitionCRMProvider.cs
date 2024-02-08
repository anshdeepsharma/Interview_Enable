using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface ITransitionCRMProvider
    {
        Task<List<Transitions.Transition>> GetTransitionAsync(string transitionType
            , string? transitionSubtype
            , string accountId
            , string checkTransitionLastXHours);
    }
}
