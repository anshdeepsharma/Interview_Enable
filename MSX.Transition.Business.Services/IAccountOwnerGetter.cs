
using MSX.Common.Models.Audits;

namespace MSX.Transition.Business.Services
{
    public interface IAccountOwnerGetter
    {
        string Get(AuditHistory accountAuditHistory);
    }
}
