using MSX.Transition.Providers.Contracts.v1.Provider;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class AssignmentHandler : TransitionHandler
    {
        private readonly IAssignmentCRMProvider _assignmentCRMProvider;
        private readonly IAssignmentEventandler _assignmentEventandler;
        private readonly ITransitionCRMProvider _transitionCRMProvider;
        private readonly IUserCRMProvider _userCRMProvider;
        public AssignmentHandler(IAssignmentEventandler assignmentEventandler
            , CreateAssignmentEventHandler createAssignmentHandler
            , DeleteAssignmentEventHandler deleteAssignmentHandler
            , IAssignmentCRMProvider assignmentCRMProvider
            , IUserCRMProvider userCRMProvider
            , ITransitionCRMProvider transitionCRMProvider)
        {
            _assignmentCRMProvider = assignmentCRMProvider;
            _transitionCRMProvider = transitionCRMProvider;
            _assignmentEventandler = assignmentEventandler;
            _userCRMProvider = userCRMProvider;
            assignmentEventandler.SetNextHandler(createAssignmentHandler).SetNextHandler(deleteAssignmentHandler);
        }

        public override async Task ExecuteAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {

            var existingAssingments = await _assignmentCRMProvider.GetAssignmentsByAccountIdAsync(transitionDataModel.Account?.Id!);
            transitionDataModel.ExistingAssignments = existingAssingments;

            await _assignmentEventandler.ExecuteAsync(transitionDataModel, transition);

            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }
    }
}
