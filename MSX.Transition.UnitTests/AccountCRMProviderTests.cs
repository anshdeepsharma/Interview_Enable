using Microsoft.Extensions.Logging;
using Moq;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM;
using MSX.Transition.Providers.CRM.Contract;
using NUnit.Framework;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class AccountCRMProviderTests
    {
        private Mock<ILogger<IAccountCRMProvider>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;
        private AccountCRMProvider _accountCRMProvider;
        private Mock<CRMXMLHandler> _crmXMLHandlerMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<IAccountCRMProvider>>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _crmXMLHandlerMock = new Mock<CRMXMLHandler>();
            _accountCRMProvider = new AccountCRMProvider(_loggerMock.Object, _crmProviderMock.Object, _crmXMLHandlerMock.Object);
        }

        [Test]
        public async Task GetAccountByCRMId_InvalidObjectInResponse_ReturnsNullAccount()
        {
            // Arrange
            string crmAccountId = "123456";
            var accountCdsList = new ResponseValueArray<AccountCds>();
            _crmProviderMock.Setup(x => x.ExecuteGetRequestAsync<ResponseValueArray<AccountCds>>(It.IsAny<string>()))
                .ReturnsAsync(accountCdsList);

            // Act
            var result = await _accountCRMProvider.GetAccountByCRMIdAsync(crmAccountId);

            // Assert
            Assert.IsNull(result);
            // Add more assertions as needed
        }

        // Add more unit tests as needed
    }
}
