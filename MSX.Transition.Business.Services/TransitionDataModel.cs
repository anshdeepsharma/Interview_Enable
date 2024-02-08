using MSX.Common.Models.AccountPlans;
using MSX.Common.Models.Accounts;
using MSX.Common.Models.Assignments;
using MSX.Common.Models.Audits;
using MSX.Common.Models.BRE;
using MSX.Common.Models.GrantAccess;
using MSX.Common.Models.Opportunities;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class TransitionDataModel
    {
        public string AssignmentEventUserId { get; set; } = string.Empty;
        public string TransitionType { get; set; } = string.Empty;
        public string? TransitionSubType { get; set; }
        public Data? Account { get; set; }
        public GrantAccessRequest? GrantAccessRequest { get; set; }
        public CheckFlags CheckFlags { get; set; }
        public BRERequest? BRERequest { get; set; }
        public BREResponse? BREData { get; set; }
        public AccountPlan? AccountPlan { get; set; }
        public Opportunity? Opportunity { get; set; }
        public AuditHistory? AuditHistory { get; set; }
        public AccountTeam? AssignmentEvent { get; set; }
        public List<AccountUserRole>? ExistingAssignments { get; set; }
        public List<Transitions.Transition>? ExistingTransitions { get; set; }
        public List<AccountUserRole>? FilteredAssignment { get; set; }
        public Func<Account, bool>? IsAccountEligibleForTransition;
        public Func<TransitionDataModel, AccountUserRole?>? TransitionManagerFilter;
        public Action<TransitionDataModel, Transitions.Transition>? ExecuteCreateLogic;
        public Action<TransitionDataModel, Transitions.Transition>? ExecuteDeleteLogic;
    }
    public class CheckFlags
    {
        public string RuleName { get; set; } = string.Empty;
        public string CheckTransitionLastXHours { get; set; } = "24";

        public bool CheckArea { get; set; }
        public bool CheckPreviousSegment { get; set; }
        public bool CheckSegment { get; set; }

        public bool CheckOpportunity { get; set; }
    }
}
