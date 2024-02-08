using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class CreateAssignmentEventHandler : AssignmentEventHandler
    {
        public override async Task ExecuteAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            if (Constants.CREATE.Equals(transitionDataModel.AssignmentEvent?.Subject, StringComparison.InvariantCultureIgnoreCase)
                || (Constants.UPDATE.Equals(transitionDataModel.AssignmentEvent?.Subject, StringComparison.InvariantCultureIgnoreCase)
                    && Constants.ACTIVE.Equals(transitionDataModel.AssignmentEvent?.data.AssignmentStatus)))
            {
                ExecuteCreate(transitionDataModel, transition);
            }

            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }

        private void ExecuteCreate(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            if (transitionDataModel.ExecuteCreateLogic == null)
                return;
            transitionDataModel.ExecuteCreateLogic(transitionDataModel, transition);
        }
    }
}
