using Moq;
using MSX.Transition.Business.Services;
using MSX.Transition.Providers.Contracts.v1.Provider;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class AssignmentHandlerTests
    {
        private Mock<IAssignmentCRMProvider> _assignmentCRMProviderMock;
        private Mock<IAssignmentEventandler> _assignmentEventandlerMock;
        private Mock<ITransitionCRMProvider> _transitionCRMProviderMock;
        private Mock<IUserCRMProvider> _userCRMProviderMock;
        private AssignmentHandler _assignmentHandler;
        private Mock<CreateAssignmentEventHandler> _createAssignmentEventHandlerMock;
        private Mock<DeleteAssignmentEventHandler> _deleteAssignmentEventHandlerMock;

        [SetUp]
        public void Setup()
        {
            _assignmentCRMProviderMock = new Mock<IAssignmentCRMProvider>();
            _assignmentEventandlerMock = new Mock<IAssignmentEventandler>();
            _transitionCRMProviderMock = new Mock<ITransitionCRMProvider>();
            _userCRMProviderMock = new Mock<IUserCRMProvider>();
            _createAssignmentEventHandlerMock = new Mock<CreateAssignmentEventHandler>();
            _deleteAssignmentEventHandlerMock = new Mock<DeleteAssignmentEventHandler>();

            _assignmentEventandlerMock.Setup(x => x.SetNextHandler(It.IsAny<IAssignmentEventandler>())).Returns(_assignmentEventandlerMock.Object);

            _assignmentHandler = new AssignmentHandler(
                _assignmentEventandlerMock.Object,
                _createAssignmentEventHandlerMock.Object,
                _deleteAssignmentEventHandlerMock.Object,
                _assignmentCRMProviderMock.Object,
                _userCRMProviderMock.Object,
                _transitionCRMProviderMock.Object
            );
        }

        [Test]
        public async Task Execute_ShouldGetAssignmentsByAccountId()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();

            // Act
            await _assignmentHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            _assignmentCRMProviderMock.Verify(x => x.GetAssignmentsByAccountIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Execute_ShouldExecuteAssignmentEventandler()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();

            // Act
            await _assignmentHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            _assignmentEventandlerMock.Verify(x => x.ExecuteAsync(It.IsAny<TransitionDataModel>(), It.IsAny<Transitions.Transition>()), Times.Once);
        }

        [Test]
        public async Task Execute_ShouldExecuteNextHandler()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();
            var nextHandlerMock = new Mock<TransitionHandler>();
            _assignmentHandler.SetNextHandler(nextHandlerMock.Object);

            // Act
            await _assignmentHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            nextHandlerMock.Verify(x => x.ExecuteAsync(It.IsAny<TransitionDataModel>(), It.IsAny<Transitions.Transition>()), Times.Once);
        }
    }
}
