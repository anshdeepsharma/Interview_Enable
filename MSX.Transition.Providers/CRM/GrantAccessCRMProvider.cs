using Microsoft.Extensions.Logging;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.GrantAccess;
using MSX.Transition.Providers.Contracts.v1.Provider;

namespace MSX.Transition.Providers.CRM
{
    public class GrantAccessCRMProvider : IGrantAccessCRMProvider
    {
        private readonly ILogger<IGrantAccessCRMProvider> _logger;
        private readonly ICRMProvider _crmProvider;

        public GrantAccessCRMProvider(ILogger<IGrantAccessCRMProvider> logger
            , ICRMProvider crmProvider)
        {
            _logger = logger;
            _crmProvider = crmProvider;
        }

        public async Task GrantAccessAsync(GrantAccessRequest grantAccessRequest)
        {
            List<BatchRequest> batchRequests = new List<BatchRequest>();

            var grantAccessPayload = @"{
                ""PrincipalAccess"": {
                    ""Principal"": {
                        ""systemuserid"": ""[UserId]"",
                        ""@odata.type"": ""Microsoft.Dynamics.CRM.systemuser""
                     },
                ""AccessMask"": ""[AccessType]""
                 },
                ""Target"": {
                    ""@odata.type"": ""Microsoft.Dynamics.CRM.[EntityName]"",
                    ""[EntityIdName]"": ""[EntityId]""
                }
            }";

            grantAccessPayload = grantAccessPayload.Replace("[UserId]", grantAccessRequest.UserId);

            string tempGrantAccessPayload = grantAccessPayload;

            if (grantAccessRequest.GrantAccessEntities != null)
            {
                foreach (var request in grantAccessRequest.GrantAccessEntities)
                {
                    grantAccessPayload = tempGrantAccessPayload;
                    grantAccessPayload = grantAccessPayload.Replace("[EntityName]", request.EntityName);
                    grantAccessPayload = grantAccessPayload.Replace("[EntityId]", request.EntityId);
                    grantAccessPayload = grantAccessPayload.Replace("[AccessType]", request.AccessType);

                    batchRequests.Add(_crmProvider.CreateBatchRequest(HttpMethod.Post, "GrantAccess", grantAccessPayload, request.EntityId));
                }
            }

            await _crmProvider.ExecuteBatchRequestAsync(batchRequests);
        }
    }
}