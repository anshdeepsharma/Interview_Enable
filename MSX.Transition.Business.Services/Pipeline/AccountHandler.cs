using MSX.Common.Models.ApplicationException;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class AccountHandler : TransitionHandler
    {
        private readonly IAccountCRMProvider _accountCRMProvider;
        public AccountHandler(IAccountCRMProvider accountCRMProvider)
        {
            _accountCRMProvider = accountCRMProvider;
        }

        public override async Task ExecuteAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {

            if (transitionDataModel.AssignmentEvent?.data?.CrmAccountId == null)
            {
                throw new DomainException($"CRMAccountId is null", System.Net.HttpStatusCode.BadRequest);
            }

            var account = await _accountCRMProvider.GetAccountByCRMIdAsync(transitionDataModel.AssignmentEvent.data.CrmAccountId);

            if (account?.Data?.Id == null)
            {
                throw new DomainException($"No account found for CRMId {transitionDataModel.AssignmentEvent.data.CrmAccountId}", System.Net.HttpStatusCode.NotFound);
            }

            transitionDataModel.Account = account.Data;

            transition.AccountId = account.Data.Id;
            transition.CurrentSegment = account.Data.Segment!;
            transition.CurrentSubsegment = account.Data.SubSegment!;
            transition.AccountTPId = account.Data.ParentMSSalesAccountId!;

            if (transitionDataModel.IsAccountEligibleForTransition != null && !transitionDataModel.IsAccountEligibleForTransition(account))
                throw new DomainException($"Account: {account.Data.CrmAccountId} does not satisfy the criteria for transition", System.Net.HttpStatusCode.NoContent);

            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }
    }
}
