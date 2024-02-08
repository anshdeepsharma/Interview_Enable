using MSX.Common.Models.Transitions;
using MSX.Transition.Providers.Contracts.v1.Provider;

namespace MSX.Transition.Business.Services
{
    public class TransitionTeamService : ITransitionTeamService
    {
        private readonly ITransitionTeamCRMProvider _transitionTeamCRMProvider;
        public TransitionTeamService(ITransitionTeamCRMProvider transitionTeamCRMProvider)
        {
            _transitionTeamCRMProvider = transitionTeamCRMProvider;
        }

        public async Task<List<Common.Models.Responses.v1.Response<TransitionTeam>>> CreateTransitionTeamsAsync(List<TransitionTeam> transitionTeams)
        {
            return await _transitionTeamCRMProvider.CreateTransitionTeamsAsync(transitionTeams);
        }
    }
}