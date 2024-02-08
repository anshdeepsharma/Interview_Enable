using MSX.Common.Models.Assignments;
using MSX.Common.Models.Enums;

namespace MSX.Transition.Business.Services
{
    public interface ITransitionFactory
    {
        ITransitionTypeService? GetTransitionSerice(AccountTeam accountTeam);

        ITransitionTypeService? GetTransitionSerice(TransitionType transitionType);
    }
}
