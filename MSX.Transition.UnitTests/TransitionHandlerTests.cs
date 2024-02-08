using Moq;
using MSX.Transition.Business.Services;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class TransitionHandlerTests
    {
        private Mock<ITranstionHandler> _nextHandlerMock;
        private TransitionHandler _transitionHandler;

        [SetUp]
        public void Setup()
        {
            _nextHandlerMock = new Mock<ITranstionHandler>();
            _transitionHandler = new TransitionHandler();
        }

        [Test]
        public async Task Execute_WhenNextHandlerIsNotNull_CallsNextHandlerExecute()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();

            _transitionHandler.SetNextHandler(_nextHandlerMock.Object);

            // Act
            await _transitionHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            _nextHandlerMock.Verify(h => h.ExecuteAsync(transitionDataModel, transition), Times.Once);
        }
    }
}
