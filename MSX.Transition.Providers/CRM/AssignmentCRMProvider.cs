using Microsoft.Extensions.Logging;
using MSX.Assignment.Common.Infra.Clients;
using MSX.Common.Models.Assignments;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM.Contract;
using System.Text.Json;

namespace MSX.Transition.Providers.CRM
{
    public class AssignmentCRMProvider : IAssignmentCRMProvider
    {
        private readonly ILogger<IAssignmentCRMProvider> _logger;
        private readonly IRoleServiceClient _roleServiceClient;
        public AssignmentCRMProvider(ILogger<IAssignmentCRMProvider> logger
            , IRoleServiceClient roleServiceClient)
        {
            _logger = logger;
            _roleServiceClient = roleServiceClient;
        }

        public async Task<List<AccountUserRole>> GetAssignmentsByAccountIdAsync(string accountId)
        {
            string strApiEndPoint = $"odata/AccountUserRoles?$filter=accountid eq {accountId}";

            HttpResponseMessage response = await _roleServiceClient.GetRoleServiceAPI(strApiEndPoint);

            string strResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(strResponse);
            }

            try
            {
                var data = JsonSerializer.Deserialize<ResponseValueArray<AccountUserRole>>(strResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

                return data.value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deserializing response from CRM API. Response: {strResponse}");
                throw;
            }
        }
    }
}