using MSX.Transition.Providers.Contracts.v1.Provider;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class GrantAccessService : IGrantAccessService
    {
        private readonly IGrantAccessCRMProvider _grantAccessCRMProvider;
        public GrantAccessService(IGrantAccessCRMProvider grantAccessCRMProvider)
        {
            _grantAccessCRMProvider = grantAccessCRMProvider;
        }

        public async Task GrantAccessAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            transitionDataModel.GrantAccessRequest = new Common.Models.GrantAccess.GrantAccessRequest()
            {
                UserId = transitionDataModel.AssignmentEventUserId,
                GrantAccessEntities = new List<Common.Models.GrantAccess.GrantAccessEntityDetail>()
                {
                    new Common.Models.GrantAccess.GrantAccessEntityDetail()
                    {
                        EntityIdName="msp_accountplanid",
                        EntityId=transitionDataModel.AccountPlan?.AccountPlanId!,
                        EntityName="msp_accountplan",
                        AccessType="ReadAccess"
                    },
                    new Common.Models.GrantAccess.GrantAccessEntityDetail()
                    {
                        EntityIdName="msp_relationshipmanagementid",
                        EntityId=transition.Id.ToString(),
                        EntityName="msp_relationshipmanagement",
                        AccessType="WriteAccess"
                    }
                }
            };

            await _grantAccessCRMProvider.GrantAccessAsync(transitionDataModel.GrantAccessRequest);
        }
    }
}

