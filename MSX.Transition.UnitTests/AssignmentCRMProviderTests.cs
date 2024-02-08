using Microsoft.Extensions.Logging;
using MSX.Assignment.Common.Infra.Clients;
using MSX.Common.Models.Assignments;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM.Contract;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MSX.Transition.Providers.CRM;
using Newtonsoft.Json;

namespace MSX.Transition.UnitTests
{
    public class AssignmentCRMProviderTests
    {
        private Mock<ILogger<IAssignmentCRMProvider>> _loggerMock;
        private Mock<IRoleServiceClient> _roleServiceClientMock;
        private AssignmentCRMProvider _assignmentCRMProvider;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<IAssignmentCRMProvider>>();
            _roleServiceClientMock = new Mock<IRoleServiceClient>();
            _assignmentCRMProvider = new AssignmentCRMProvider(_loggerMock.Object, _roleServiceClientMock.Object);
        }

        [Test]
        public async Task GetAssignmentsByAccountId_ValidAccountId_ReturnsAccountUserRoles()
        {
            // Arrange
            string accountId = "123";

            AccountUserRole accountUserRole = new AccountUserRole
            {
                AccountId = accountId,
            };

            ResponseValueArray<AccountUserRole> responseObj = new ResponseValueArray<AccountUserRole>()
            {
                value = new List<AccountUserRole>()
                {
                    accountUserRole
                }
            };

            string strApiEndPoint = $"odata/AccountUserRoles?$filter=accountid eq {accountId}";
            HttpResponseMessage successResponse = new HttpResponseMessage(HttpStatusCode.OK);
            successResponse.Content = new StringContent(JsonConvert.SerializeObject(responseObj));

            _roleServiceClientMock.Setup(c => c.GetRoleServiceAPI(strApiEndPoint)).ReturnsAsync(successResponse);

            // Act
            List<AccountUserRole> result = await _assignmentCRMProvider.GetAssignmentsByAccountIdAsync(accountId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task GetAssignmentsByAccountId_InvalidAccountId_ThrowsException()
        {
            // Arrange
            string accountId = "invalid";

            string strApiEndPoint = $"odata/AccountUserRoles?$filter=accountid eq {accountId}";
            HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            errorResponse.Content = new StringContent("{'error': 'Invalid account ID'}");

            _roleServiceClientMock.Setup(c => c.GetRoleServiceAPI(strApiEndPoint)).ReturnsAsync(errorResponse);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _assignmentCRMProvider.GetAssignmentsByAccountIdAsync(accountId));
        }
    }
}
