
using MSX.Common.Models.Audits;

namespace MSX.Transition.Business.Services
{
    public interface IPreviousSubsegmentGetter
    {
        string Get(AuditHistory accountAuditHistory);
    }
}
