
using MSX.Common.Models.Audits;

namespace MSX.Transition.Business.Services
{
    public class PreviousSegmentGetter : IPreviousSegmentGetter
    {
        public string Get(AuditHistory accountAuditHistory)
        {
            var previousSegment = accountAuditHistory?.ChangeData?.Where(x => x.ChangedAttributes != null)
                            .SelectMany(changeddata => changeddata.ChangedAttributes)
                            .FirstOrDefault(attr => attr.LogicalName.Equals("msp_endcustomersegmentcode"));
            return previousSegment?.OldValue ?? string.Empty;
        }
    }
}
