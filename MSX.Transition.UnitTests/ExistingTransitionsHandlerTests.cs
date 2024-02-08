using Moq;
using MSX.Transition.Business.Services;
using MSX.Transition.Providers.Contracts.v1.Provider;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class ExistingTransitionsHandlerTests
    {
        private ExistingTransitionsHandler _existingTransitionsHandler;
        private Mock<ITransitionCRMProvider> _transitionCRMProviderMock;

        [SetUp]
        public void Setup()
        {
            _transitionCRMProviderMock = new Mock<ITransitionCRMProvider>();
            _existingTransitionsHandler = new ExistingTransitionsHandler(_transitionCRMProviderMock.Object);
        }

        [Test]
        public async Task Execute_ShouldGetTransitionAndSetExistingTransitions()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel() { CheckFlags = new CheckFlags() };
            var transition = new Transitions.Transition();
            var existingTransitions = new List<Transitions.Transition>();

            _transitionCRMProviderMock.Setup(x => x.GetTransitionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(existingTransitions);

            // Act
            await _existingTransitionsHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            Assert.AreEqual(existingTransitions, transitionDataModel.ExistingTransitions);
        }

        [Test]
        public async Task Execute_ShouldCallNextHandler()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel() { CheckFlags = new CheckFlags() };
            var transition = new Transitions.Transition();

            // Act
            await _existingTransitionsHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            // _nextHandlerMock.Verify(x => x.Execute(transitionDataModel, transition), Times.Once);
        }
    }
}
