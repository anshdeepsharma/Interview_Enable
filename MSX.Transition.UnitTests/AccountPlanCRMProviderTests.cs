using Microsoft.Extensions.Logging;
using MSX.Common.Models.AccountPlans;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Moq;
using NUnit.Framework;
using MSX.Transition.Providers.CRM;
using MSX.Transition.Providers.CRM.Contract;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class AccountPlanCRMProviderTests
    {
        private Mock<ILogger<IAccountPlanCRMProvider>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;
        private AccountPlanCRMProvider _accountPlanCRMProvider;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<IAccountPlanCRMProvider>>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _accountPlanCRMProvider = new AccountPlanCRMProvider(_loggerMock.Object, _crmProviderMock.Object);
        }

        [Test]
        public async Task GetAccountPlan_ValidAccountId_ReturnsAccountPlan()
        {
            // Arrange
            string accountId = "123";

            var accountPlan = new AccountPlan() { AccountPlanId = "123456" };
            _crmProviderMock.Setup(x => x.ExecuteGetRequestAsync<AccountPlan>(It.IsAny<string>()))
                .ReturnsAsync(accountPlan);

            // Act
            var result = await _accountPlanCRMProvider.GetAccountPlanAsync(accountId);

            // Assert
            Assert.IsNotNull(result);
            // Add more assertions as needed
        }
    }
}
