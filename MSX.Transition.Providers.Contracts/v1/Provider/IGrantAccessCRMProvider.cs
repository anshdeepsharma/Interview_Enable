using MSX.Common.Models.GrantAccess;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface IGrantAccessCRMProvider
    {
        Task GrantAccessAsync(GrantAccessRequest grantAccessRequest);
    }
}
