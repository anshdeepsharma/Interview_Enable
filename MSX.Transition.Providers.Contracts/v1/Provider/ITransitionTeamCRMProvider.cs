using MSX.Common.Models.Responses.v1;
using MSX.Common.Models.Transitions;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface ITransitionTeamCRMProvider
    {
        Task<List<Response<TransitionTeam>>> CreateTransitionTeamsAsync(List<TransitionTeam> transitionTeams);
    }
}
