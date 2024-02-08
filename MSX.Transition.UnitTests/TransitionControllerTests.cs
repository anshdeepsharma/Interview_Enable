using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSX.Common.Models;
using MSX.Common.Models.Assignments;
using MSX.Common.Models.Responses.v1;
using MSX.Common.Models.Transitions;
using MSX.Transition.Business.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSX.Transition.API.Controllers.v1;
using Microsoft.Extensions.Logging;
using MSX.Common.Models.ApplicationException;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class TransitionControllerTests
    {
        private TransitionController _transitionController;
        private Mock<ILogger<TransitionController>> _loggerMock;
        private Mock<ITransitionService> _transitionServiceMock;
        private Mock<ITransitionTeamService> _transitionTeamServiceMock;
        private MSXRequestContext _msxRequestContext;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<TransitionController>>();
            _transitionServiceMock = new Mock<ITransitionService>();
            _transitionTeamServiceMock = new Mock<ITransitionTeamService>();
            _msxRequestContext = new MSXRequestContext();

            _transitionController = new TransitionController(
                _loggerMock.Object,
                _transitionServiceMock.Object,
                _transitionTeamServiceMock.Object,
                _msxRequestContext
            );
        }

        [Test]
        public async Task CreateTransitions_EmptyInput_ThrowsDomainException()
        {
            // Arrange
            var accountTeamAssignment = new List<AccountTeam>();
            _transitionServiceMock.Setup(x=>x.CreateTransitionsAsync(It.IsAny<List<AccountTeam>>()))
                .ReturnsAsync(new List<Response<AccountTeam>>());

            // Act
            Assert.ThrowsAsync<DomainException>(async () => await _transitionController.CreateTransitions(accountTeamAssignment));
        }

    }
}
