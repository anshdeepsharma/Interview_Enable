using Moq;
using MSX.Common.Models.Transitions;
using MSX.Transition.Business.Services;
using MSX.Transition.Providers.Contracts.v1.Provider;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class TransitionTeamServiceTests
    {
        private Mock<ITransitionTeamCRMProvider> _transitionTeamCRMProviderMock;
        private TransitionTeamService _transitionTeamService;

        [SetUp]
        public void Setup()
        {
            _transitionTeamCRMProviderMock = new Mock<ITransitionTeamCRMProvider>();
            _transitionTeamService = new TransitionTeamService(_transitionTeamCRMProviderMock.Object);
        }

        [Test]
        public async Task CreateTransitionTeams_ShouldCall_CreateTransitionTeams_Method()
        {
            // Arrange
            var transitionTeams = new List<TransitionTeam>();

            // Act
            await _transitionTeamService.CreateTransitionTeamsAsync(transitionTeams);

            // Assert
            _transitionTeamCRMProviderMock.Verify(x => x.CreateTransitionTeamsAsync(transitionTeams), Times.Once);
        }
    }
}
