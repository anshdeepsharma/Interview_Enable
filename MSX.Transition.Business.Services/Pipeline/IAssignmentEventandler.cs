using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public interface IAssignmentEventandler
    {
        IAssignmentEventandler SetNextHandler(IAssignmentEventandler nextHandler);
        Task ExecuteAsync(TransitionDataModel transitionDataModel, Transitions.Transition transition);
    }
}
