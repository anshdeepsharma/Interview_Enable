using MSX.Common.Models;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface IUserCRMProvider
    {
        Task<User> ResolveUserAsync(string userAlias);
    }
}
