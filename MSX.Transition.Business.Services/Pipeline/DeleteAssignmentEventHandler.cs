using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class DeleteAssignmentEventHandler : AssignmentEventHandler
    {
        public override async Task ExecuteAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            if (Constants.DELETE.Equals(transitionDataModel.AssignmentEvent?.Subject, StringComparison.InvariantCultureIgnoreCase)
                || (Constants.UPDATE.Equals(transitionDataModel.AssignmentEvent?.Subject, StringComparison.InvariantCultureIgnoreCase)
                    && Constants.EXPIRED.Equals(transitionDataModel.AssignmentEvent?.data.AssignmentStatus, StringComparison.InvariantCultureIgnoreCase)))
            {
                ExecuteDelete(transitionDataModel, transition);
            }

            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }

        private void ExecuteDelete(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            if (transitionDataModel.ExecuteDeleteLogic == null)
                return;
            transitionDataModel.ExecuteDeleteLogic(transitionDataModel, transition);
        }
    }
}
