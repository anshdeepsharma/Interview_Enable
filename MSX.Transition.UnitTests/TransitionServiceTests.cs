using Microsoft.Extensions.Logging;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Common;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.Assignments;
using MSX.Common.Models.Responses.v1;
using MSX.Common.Models.Transitions;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Transitions = MSX.Common.Models.Transitions;
using MSX.Transition.Business.Services;
using MSX.Common.Models.Enums;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models.MetaData;

namespace MSX.Transition.UnitTests
{
    public class TransitionServiceTests
    {
        private Mock<ILogger<ITransitionService>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;
        private Mock<ICRMTranslator> _crmTranslatorMock;
        private Mock<ITransitionFactory> _transitionFactoryMock;
        private Mock<ITransitionTeamService> _transitionTeamServiceMock;
        private Mock<IGrantAccessService> _grantAccessServiceMock;
        private Mock<IUserCRMProvider> _userCrmProviderMock;
        private Mock<ITransitionTypeService> _transitionTypeServiceMock;
        private TransitionService _transitionService;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ITransitionService>>();
            _crmTranslatorMock = new Mock<ICRMTranslator>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _transitionFactoryMock = new Mock<ITransitionFactory>();
            _transitionTeamServiceMock = new Mock<ITransitionTeamService>();
            _grantAccessServiceMock = new Mock<IGrantAccessService>();
            _transitionTypeServiceMock = new Mock<ITransitionTypeService>();
            _userCrmProviderMock = new Mock<IUserCRMProvider>();

            _transitionService = new TransitionService(
                _loggerMock.Object,
                _crmProviderMock.Object,
                _crmTranslatorMock.Object,
                _transitionFactoryMock.Object,
                _transitionTeamServiceMock.Object,
                _grantAccessServiceMock.Object,
                _userCrmProviderMock.Object
            );

            _transitionFactoryMock.Setup(x => x.GetTransitionSerice(It.IsAny<AccountTeam>())).Returns(_transitionTypeServiceMock.Object);
        }

        [Test]
        public async Task CreateTransitions_ThrowsException_WhenAccountTeamsIsEmpty()
        {
            // Arrange
            var accountTeams = new List<AccountTeam>();

            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _transitionService.CreateTransitionsAsync(null));
        }

        [Test]
        public async Task CreateTransitions_CallsCreateBatchRequestForEachAccountTeam()
        {
            // Arrange
            var accountTeams = new List<AccountTeam>
            {
                new AccountTeam(){ data=new AssignmentData(){ CrmAccountId="1234"}, Id="Id" },
                new AccountTeam() { data = new AssignmentData() { CrmAccountId = "12345" }, Id="Id" },
                new AccountTeam() { data = new AssignmentData() { CrmAccountId = "123456" } , Id = "Id"}
            };

            var batchRequests = new List<BatchRequest>
            {
                new BatchRequest(),
                new BatchRequest(),
                new BatchRequest()
            };

            _crmProviderMock.Setup(x => x.ExecuteBatchRequestAsync(It.IsAny<List<BatchRequest>>()))
                .ReturnsAsync(new List<Response<BatchRequest>>());

            _crmProviderMock.Setup(x => x.CreateBatchRequest(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new BatchRequest());

            User expectedUser = new User { UserAlias = "Alias" };
            // _userCrmProviderMock.Setup(x => x.ResolveUser(It.IsAny<string>())).ReturnsAsync<User>(expectedUser);

            // Act
            // await _transitionService.CreateTransitions(accountTeams);

            // Assert
            // _crmProviderMock.Verify(x => x.CreateBatchRequest(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(accountTeams.Count));
        }

        [Test]
        public async Task CreateTransitions_ReturnsCrmResponses()
        {
            // Arrange
            var accountTeams = new List<AccountTeam>
            {
                new AccountTeam(){ data=new AssignmentData(){ CrmAccountId="1234"}, Id="Id" },
                new AccountTeam() { data = new AssignmentData() { CrmAccountId = "12345" }, Id="Id" },
                new AccountTeam() { data = new AssignmentData() { CrmAccountId = "123456" }, Id="Id" }
            };

            var batchRequests = new List<BatchRequest>
            {
                new BatchRequest(),
                new BatchRequest(),
                new BatchRequest()
            };

            var batchRequestResponses = new List<Response<BatchRequest>>
            {
                new Response<BatchRequest>(new BatchRequest() { BatchRequestId = "1" }),
                new Response<BatchRequest>(new BatchRequest() { BatchRequestId = "2" }),
                new Response<BatchRequest>(new BatchRequest() { BatchRequestId = "3" })
            };

            var crmResponses = new List<Response<AccountTeam>>
            {
                new Response<AccountTeam>(new AccountTeam() { data = new AssignmentData() { CrmAccountId = "1234" }, Id="Id" }),
                new Response<AccountTeam>(new AccountTeam() { data = new AssignmentData() { CrmAccountId = "1234" }, Id="Id" }),
                new Response<AccountTeam>(new AccountTeam() { data = new AssignmentData() { CrmAccountId = "1234" }, Id="Id" })
            };

            _crmProviderMock.Setup(x => x.ExecuteBatchRequestAsync(It.IsAny<List<BatchRequest>>()))
                .ReturnsAsync(batchRequestResponses);

            _crmProviderMock.Setup(x => x.CreateBatchRequest(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new BatchRequest());

            // Act
            // var result = await _transitionService.CreateTransitions(accountTeams);

            // Assert
            // Assert.AreEqual(crmResponses, result);
        }
    }
}
