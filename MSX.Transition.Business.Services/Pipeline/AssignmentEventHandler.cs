using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class AssignmentEventHandler : IAssignmentEventandler
    {
        public IAssignmentEventandler _nextHandler { get; private set; }
        public IAssignmentEventandler SetNextHandler(IAssignmentEventandler next)
        {
            _nextHandler = next;
            return next;
        }
        public virtual async Task ExecuteAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }
    }
}
