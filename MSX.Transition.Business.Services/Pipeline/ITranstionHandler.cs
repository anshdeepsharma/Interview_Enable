using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public interface ITranstionHandler
    {
        ITranstionHandler SetNextHandler(ITranstionHandler nextHandler);
        Task ExecuteAsync(TransitionDataModel transitionDataModel, Transitions.Transition transition);
    }
}
