using MSX.Common.Models.Assignments;
using MSX.Common.Models.Enums;

namespace MSX.Transition.Business.Services
{
    public class TransitionFactory : ITransitionFactory
    {
        private readonly IServiceProvider serviceProvider;

        public TransitionFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ITransitionTypeService? GetTransitionSerice(AccountTeam accountTeam)
        {
            object? transitionTypeService = null;

            if ((RoleType.ATU.ToString().Equals(accountTeam.data.RoleType, StringComparison.InvariantCultureIgnoreCase)
                && (Constants.ATSRole.Equals(accountTeam.data.RolePlayed)
                    || Constants.ATUManagerRole.Equals(accountTeam.data.RolePlayed, StringComparison.InvariantCultureIgnoreCase)))
                || Constants.CSAMRoleSummary.Equals(accountTeam.data.RoleSummary, StringComparison.InvariantCultureIgnoreCase))
            {
                transitionTypeService = serviceProvider.GetService(typeof(ATSTransitionService));
            }
            else if (RoleType.STU.ToString().Equals(accountTeam.data.RoleType, StringComparison.InvariantCultureIgnoreCase)
                && Constants.SSPRole.Equals(accountTeam.data.RolePlayed)
                && (Constants.SSPRoleSummary.Equals(accountTeam.data.RoleSummary, StringComparison.InvariantCultureIgnoreCase)
                    || Constants.SSPManagerRoleRummary.Equals(accountTeam.data.RoleSummary, StringComparison.InvariantCultureIgnoreCase)))
            {
                transitionTypeService = serviceProvider.GetService(typeof(SSPTransitionService));
            }

            return transitionTypeService != null ? (ITransitionTypeService)transitionTypeService : null;
        }

        public ITransitionTypeService? GetTransitionSerice(TransitionType transitionType)
        {
            object? transitionTypeService = null;

            if (transitionType == TransitionType.ATS)
            {
                transitionTypeService = serviceProvider.GetService(typeof(ATSTransitionService));
            }
            else if (transitionType == TransitionType.STU)
            {
                transitionTypeService = serviceProvider.GetService(typeof(SSPTransitionService));
            }

            return transitionTypeService != null ? (ITransitionTypeService)transitionTypeService : null;
        }
    }
}
