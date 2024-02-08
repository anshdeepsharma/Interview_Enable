using MSX.Common.Models.Responses.v1;
using MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public interface ITransitionTeamService
    {
        Task<List<Response<TransitionTeam>>> CreateTransitionTeamsAsync(List<TransitionTeam> transitionTeams);
    }
}
