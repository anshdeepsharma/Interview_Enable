using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public interface IGrantAccessService
    {
        Task GrantAccessAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition);
    }
}
