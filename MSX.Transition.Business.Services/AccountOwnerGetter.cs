
using MSX.Common.Models.Audits;

namespace MSX.Transition.Business.Services
{
    public class AccountOwnerGetter : IAccountOwnerGetter
    {
        public string Get(AuditHistory accountAuditHistory)
        {
            var previousAccountOwner = accountAuditHistory?.ChangeData?.Where(x => x.ChangedAttributes != null)
                            .SelectMany(changeddata => changeddata.ChangedAttributes)
                           .FirstOrDefault(attr => attr.LogicalName.Equals("msp_previousaccountowner"));
            return previousAccountOwner?.OldValue ?? string.Empty;
        }
    }
}
