using Microsoft.Extensions.Logging;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.GrantAccess;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Moq;
using NUnit.Framework;
using MSX.Transition.Providers.CRM;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class GrantAccessCRMProviderTests
    {
        private GrantAccessCRMProvider _grantAccessCRMProvider;
        private Mock<ILogger<IGrantAccessCRMProvider>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<IGrantAccessCRMProvider>>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _grantAccessCRMProvider = new GrantAccessCRMProvider(_loggerMock.Object, _crmProviderMock.Object);
        }

        [Test]
        public async Task GrantAccess_WhenCalled_ShouldExecuteBatchRequest()
        {
            // Arrange
            var grantAccessRequest = new GrantAccessRequest
            {
                UserId = "testUserId",
                GrantAccessEntities = new List<GrantAccessEntityDetail>
                {
                    new GrantAccessEntityDetail
                    {
                        EntityName = "testEntityName",
                        EntityId = "testEntityId",
                        AccessType = "testAccessType"
                    }
                }
            };

            var batchRequests = new List<BatchRequest>
            {
                new BatchRequest
                {
                    HttpMethod = HttpMethod.Post.ToString(),
                    URL = "GrantAccess",
                    Payload = @"{
                        ""PrincipalAccess"": {
                            ""Principal"": {
                                ""systemuserid"": ""testUserId"",
                                ""@odata.type"": ""Microsoft.Dynamics.CRM.systemuser""
                            },
                            ""AccessMask"": ""testAccessType""
                        },
                        ""Target"": {
                            ""@odata.type"": ""Microsoft.Dynamics.CRM.testEntityName"",
                            ""testEntityId"": ""testEntityId""
                        }
                    }"
                }
            };

            _crmProviderMock.Setup(x => x.CreateBatchRequest(HttpMethod.Post, "GrantAccess", It.IsAny<string>(), It.IsAny<string>()))
                .Returns(batchRequests[0]);

            // Act
            await _grantAccessCRMProvider.GrantAccessAsync(grantAccessRequest);

            // Assert
            _crmProviderMock.Verify(x => x.ExecuteBatchRequestAsync(batchRequests), Times.Once);
        }
    }
}
