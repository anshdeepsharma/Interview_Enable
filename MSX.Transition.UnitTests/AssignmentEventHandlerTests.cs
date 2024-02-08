using Moq;
using MSX.Transition.Business.Services;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class AssignmentEventHandlerTests
    {
        private Mock<IAssignmentEventandler> _nextHandlerMock;
        private AssignmentEventHandler _assignmentEventHandler;

        [SetUp]
        public void Setup()
        {
            _nextHandlerMock = new Mock<IAssignmentEventandler>();
            _assignmentEventHandler = new AssignmentEventHandler();
        }

        [Test]
        public async Task Execute_WhenNextHandlerIsNotNull_CallsNextHandlerExecute()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();

            _assignmentEventHandler.SetNextHandler(_nextHandlerMock.Object);

            // Act
            await _assignmentEventHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            _nextHandlerMock.Verify(x => x.ExecuteAsync(transitionDataModel, transition), Times.Once);
        }
    }
}
