using Moq;
using MSX.Transition.Business.Services;
using MSX.Transition.Providers.Contracts.v1.Provider;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class GrantAccessServiceTests
    {
        private GrantAccessService _grantAccessService;
        private Mock<IGrantAccessCRMProvider> _mockGrantAccessCRMProvider;

        [SetUp]
        public void Setup()
        {
            _mockGrantAccessCRMProvider = new Mock<IGrantAccessCRMProvider>();
            _grantAccessService = new GrantAccessService(_mockGrantAccessCRMProvider.Object);
        }

        [Test]
        public async Task GrantAccess_ValidData_CallsGrantAccessMethod()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();

            // Act
            await _grantAccessService.GrantAccessAsync(transitionDataModel, transition);

            // Assert
            _mockGrantAccessCRMProvider.Verify(x => x.GrantAccessAsync(It.IsAny<Common.Models.GrantAccess.GrantAccessRequest>()), Times.Once);
        }
    }
}
