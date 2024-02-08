
using MSX.Common.Models.Audits;

namespace MSX.Transition.Business.Services
{
    public class PreviousSubsegmentGetter : IPreviousSubsegmentGetter
    {
        public string Get(AuditHistory accountAuditHistory)
        {
            var previousSubSegement = accountAuditHistory?.ChangeData?.Where(x => x.ChangedAttributes != null)
                            .SelectMany(changeddata => changeddata.ChangedAttributes)
                           .FirstOrDefault(attr => attr.LogicalName.Equals("msp_endcustomersubsegmentcode"));
            return previousSubSegement?.OldValue ?? string.Empty;
        }
    }
}
