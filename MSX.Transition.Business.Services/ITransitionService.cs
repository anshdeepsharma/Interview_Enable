using MSX.Common.Models.Assignments;
using MSX.Common.Models.Responses.v1;

namespace MSX.Transition.Business.Services
{
    public interface ITransitionService
    {
        Task<List<Response<AccountTeam>>> CreateTransitionsAsync(List<AccountTeam> accountTeams);
    }
}
