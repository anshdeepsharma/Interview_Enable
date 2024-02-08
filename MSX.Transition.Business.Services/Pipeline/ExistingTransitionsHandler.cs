using MSX.Transition.Providers.Contracts.v1.Provider;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class ExistingTransitionsHandler : TransitionHandler
    {
        private readonly ITransitionCRMProvider _transitionCRMProvider;
        public ExistingTransitionsHandler(ITransitionCRMProvider transitionCRMProvider)
        {
            _transitionCRMProvider = transitionCRMProvider;
        }

        public override async Task ExecuteAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {

            var existingTransitions = await _transitionCRMProvider.GetTransitionAsync(transitionDataModel.TransitionType,
                                                                                 transitionDataModel.TransitionSubType,
                                                                                 transition.AccountId,
                                                                                 transitionDataModel.CheckFlags.CheckTransitionLastXHours);
            transitionDataModel.ExistingTransitions = existingTransitions;

            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }
    }
}
