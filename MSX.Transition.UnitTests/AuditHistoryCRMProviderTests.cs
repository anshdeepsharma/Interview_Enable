using Microsoft.Extensions.Logging;
using MSX.Common.Models.Audits;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM.Contract;
using Moq;
using NUnit.Framework;
using System.Text.Json;
using System.Threading.Tasks;
using MSX.Transition.Providers.CRM;

namespace MSX.Transition.UnitTests
{
    public class AuditHistoryCRMProviderTests
    {
        private Mock<ILogger<IAuditHistoryCRMProvider>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;
        private AuditHistoryCRMProvider _auditHistoryCRMProvider;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<IAuditHistoryCRMProvider>>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _auditHistoryCRMProvider = new AuditHistoryCRMProvider(_loggerMock.Object, _crmProviderMock.Object);
        }

        [Test]
        public async Task GetAccountAuditHistory_InValidAccountId_ReturnsNullAuditHistory()
        {
            // Arrange
            string accountId = "invalidAccountId";
            var expectedQuery = $"audits?$filter=objecttypecode eq 'account' and _objectid_value eq {accountId} &$orderby=createdon desc";
            var expectedResult = new ResponseValueArray<AuditHistoryCds>();
            _crmProviderMock.Setup(x => x.ExecuteGetRequestAsync<ResponseValueArray<AuditHistoryCds>>(expectedQuery))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _auditHistoryCRMProvider.GetAccountAuditHistoryAsync(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ChangeData);
            Assert.That(result.ChangeData.Count(),Is.EqualTo(0));
        }
    }
}
