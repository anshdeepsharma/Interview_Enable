using MSX.Common.Models.ApplicationException;
using MSX.Common.Models.Opportunities;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class OpportunityHandler : TransitionHandler
    {
        private readonly IOpportunityCRMProvider _opportunityCRMProvider;
        public OpportunityHandler(IOpportunityCRMProvider opportunityCRMProvider)
        {
            _opportunityCRMProvider = opportunityCRMProvider;
        }

        public override async Task ExecuteAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            var checkOpportunity = transitionDataModel.CheckFlags?.CheckOpportunity ?? false;
            if (checkOpportunity)
            {
                var userOpportunityTask = _opportunityCRMProvider.GetOpportunityByUserAsync(transitionDataModel.AssignmentEventUserId, transitionDataModel.Account?.Id!);
                var userTeamOpportunityTask = _opportunityCRMProvider.GetOpportunityByUserInTeamAsync(transitionDataModel.AssignmentEventUserId, transitionDataModel.Account?.Id!);

                await Task.WhenAll(userOpportunityTask, userTeamOpportunityTask);

                List<Opportunity> userOpportunities = userOpportunityTask.Result;
                List<Opportunity> userTeamOpportunities = userTeamOpportunityTask.Result;

                if ((userOpportunities == null && userTeamOpportunities == null)
                    || (userOpportunities?.Count == 0 && userTeamOpportunities?.Count == 0))
                {
                    throw new DomainException($"No opportunity found for user {transitionDataModel.AssignmentEvent?.data?.Alias}", System.Net.HttpStatusCode.NoContent);
                }
            }

            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }
    }
}
