
using MSX.Common.Models.Audits;

namespace MSX.Transition.Business.Services
{
    public interface IPreviousSegmentGetter
    {
        string Get(AuditHistory accountAuditHistory);
    }
}
