
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class TransitionHandler : ITranstionHandler
    {
        public ITranstionHandler _nextHandler { get; private set; }
        public ITranstionHandler SetNextHandler(ITranstionHandler next)
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
