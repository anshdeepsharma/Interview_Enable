using Moq;
using Transitions = MSX.Common.Models.Transitions;
using MSX.Transition.Business.Services;
using MSX.Transition.Providers.Contracts.v1.Provider;
using NUnit.Framework;
using MSX.Common.Models.ApplicationException;
using MSX.Common.Models.Opportunities;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class OpportunityHandlerTests
    {
        private Mock<IOpportunityCRMProvider> _opportunityCRMProviderMock;
        private OpportunityHandler _opportunityHandler;

        [SetUp]
        public void Setup()
        {
            _opportunityCRMProviderMock = new Mock<IOpportunityCRMProvider>();
            _opportunityHandler = new OpportunityHandler(_opportunityCRMProviderMock.Object);
        }

        [Test]
        public async Task Execute_WhenUserOpportunityIsNullAndUserTeamOpportunityIsNull_ThrowsDomainException()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel()
            {
                CheckFlags = new CheckFlags()
                {
                    CheckOpportunity = true
                }
            };
            var transition = new Transitions.Transition();

            _opportunityCRMProviderMock.Setup(x => x.GetOpportunityByUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Opportunity>());
            _opportunityCRMProviderMock.Setup(x => x.GetOpportunityByUserInTeamAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Opportunity>());

            // Act and Assert
            Assert.ThrowsAsync<DomainException>(async () =>
            {
                await _opportunityHandler.ExecuteAsync(transitionDataModel, transition);
            });
        }

        [Test]
        public async Task Execute_WhenUserOpportunityIsNotNullAndUserTeamOpportunityIsNull_DoesNotThrowException()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();
            var userOpportunities = new List<Opportunity>();

            _opportunityCRMProviderMock.Setup(x => x.GetOpportunityByUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userOpportunities);
            _opportunityCRMProviderMock.Setup(x => x.GetOpportunityByUserInTeamAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Opportunity>());

            // Act and Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                await _opportunityHandler.ExecuteAsync(transitionDataModel, transition);
            });
        }

        [Test]
        public async Task Execute_WhenUserOpportunityIsNullAndUserTeamOpportunityIsNotNull_DoesNotThrowException()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();
            var userTeamOpportunities = new List<Opportunity>();

            _opportunityCRMProviderMock.Setup(x => x.GetOpportunityByUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Opportunity>());
            _opportunityCRMProviderMock.Setup(x => x.GetOpportunityByUserInTeamAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userTeamOpportunities);

            // Act and Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                await _opportunityHandler.ExecuteAsync(transitionDataModel, transition);
            });
        }

        [Test]
        public async Task Execute_WhenUserOpportunityIsNotNullAndUserTeamOpportunityIsNotNull_DoesNotThrowException()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();
            var userOpportunities = new List<Opportunity>();
            var userTeamOpportunities = new List<Opportunity>();

            _opportunityCRMProviderMock.Setup(x => x.GetOpportunityByUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userOpportunities);
            _opportunityCRMProviderMock.Setup(x => x.GetOpportunityByUserInTeamAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userTeamOpportunities);

            // Act and Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                await _opportunityHandler.ExecuteAsync(transitionDataModel, transition);
            });
        }
    }
}
