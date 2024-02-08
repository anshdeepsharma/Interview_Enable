using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public interface ITransitionTypeService
    {
        Task ExecuteTransitionAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition);

        void ComputeTransitionStatus(Transitions.Transition transition);

        void ComputeOwner(Transitions.Transition transition);
    }
}
