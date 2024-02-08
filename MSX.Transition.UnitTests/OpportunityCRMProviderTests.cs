using Microsoft.Extensions.Logging;
using Moq;
using MSX.Assignment.Common.Infra.Clients;
using MSX.Common.Models.Opportunities;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM;
using MSX.Transition.Providers.CRM.Contract;
using NUnit.Framework;

namespace MSX.Transition.UnitTests
{
    public class OpportunityCRMProviderTests
    {
        private Mock<ILogger<IOpportunityCRMProvider>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;
        private Mock<ICRMXMLHandler> _crmXMLHandlerMock;
        private OpportunityCRMProvider _opportunityCRMProvider;
        private Mock<ITaxonomyServiceClient> _taxonomyServiceClientMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<IOpportunityCRMProvider>>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _crmXMLHandlerMock = new Mock<ICRMXMLHandler>();
            _taxonomyServiceClientMock = new Mock<ITaxonomyServiceClient>();
            _opportunityCRMProvider = new OpportunityCRMProvider(_loggerMock.Object, _crmProviderMock.Object, _crmXMLHandlerMock.Object);
        }

        [Test]
        public async Task GetOpportunityByUser_ShouldReturnOpportunity()
        {
            // Arrange
            string userAlias = "testUserAlias";
            string accountId = "testAccountId";
            List<Opportunity> expectedOpportunity = new();

            _crmXMLHandlerMock.Setup(x => x.ConstructQuery<Opportunity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Common.Models.Opportunities.Opportunity>()))
                .Returns("testQuery");

            _crmProviderMock.Setup(x => x.ExecuteGetRequestAsync<List<Opportunity>>(It.IsAny<string>()))
                .ReturnsAsync(expectedOpportunity);

            // Act
            var result = await _opportunityCRMProvider.GetOpportunityByUserAsync(userAlias, accountId);

            // Assert
            Assert.AreEqual(expectedOpportunity, result);
            _crmXMLHandlerMock.Verify(x => x.ConstructQuery<Opportunity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Common.Models.Opportunities.Opportunity>()), Times.Once);
            _crmProviderMock.Verify(x => x.ExecuteGetRequestAsync<ResponseValueArray<OpportunityCds>>(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetOpportunityByUserInTeam_ShouldReturnOpportunity()
        {
            // Arrange
            string userAlias = "testUserAlias";
            string accountId = "testAccountId";
            List<Opportunity> expectedOpportunity = new List<Opportunity>();

            _crmXMLHandlerMock.Setup(x => x.ConstructQuery<Opportunity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Common.Models.Opportunities.Opportunity>()))
                .Returns("testQuery");

            _crmProviderMock.Setup(x => x.ExecuteGetRequestAsync<List<Opportunity>>(It.IsAny<string>()))
                .ReturnsAsync(expectedOpportunity);

            // Act
            var result = await _opportunityCRMProvider.GetOpportunityByUserInTeamAsync(userAlias, accountId);

            // Assert
            Assert.AreEqual(expectedOpportunity, result);
            _crmXMLHandlerMock.Verify(x => x.ConstructQuery<Opportunity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<Common.Models.Opportunities.Opportunity>()), Times.Once);
            _crmProviderMock.Verify(x => x.ExecuteGetRequestAsync<ResponseValueArray<OpportunityCds>>(It.IsAny<string>()), Times.Once);
        }
    }
}
