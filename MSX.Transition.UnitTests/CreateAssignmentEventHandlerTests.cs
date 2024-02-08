using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSX.Common.Models.Transitions;
using Transitions = MSX.Common.Models.Transitions;
using MSX.Transition.Business.Services;
using MSX.Common.Models.Assignments;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class CreateAssignmentEventHandlerTests
    {
        private Mock<AssignmentEventHandler> _nextHandlerMock;
        private CreateAssignmentEventHandler _createAssignmentEventHandler;

        [SetUp]
        public void Setup()
        {
            _nextHandlerMock = new Mock<AssignmentEventHandler>();
            _createAssignmentEventHandler = new CreateAssignmentEventHandler();
            _createAssignmentEventHandler.SetNextHandler(_nextHandlerMock.Object);
        }

        [Test]
        public async Task Execute_WhenAssignmentEventSubjectIsCreateAndExistingTransitionsIsNotNull_ShouldSetTransitionIdAndTransitionTeams()
        {
            // Arrange

            var transtionId = Guid.NewGuid();
            var transitionDataModel = new TransitionDataModel
            {
                AssignmentEvent = new AccountTeam()
                {
                    data = new AssignmentData()
                    {
                        CrmAccountId = "123"
                    },
                    Subject = "CREATE"
                },
                ExistingTransitions = new List<Transitions.Transition>
                {
                    new Transitions.Transition
                    {
                        Id = transtionId,
                        OwnerId = "2"
                    }
                },
                AssignmentEventUserId = "3",
                ExecuteCreateLogic = (transitionDataModel, transition) =>
                {
                    transition.Id = transitionDataModel.ExistingTransitions.First().Id;
                    transition.TransitionTeams = new List<TransitionTeam>
                    {
                        new TransitionTeam
                        {
                            OwnerId = transitionDataModel.AssignmentEventUserId
                        }
                    };
                }
            };
            var transition = new Transitions.Transition();

            // Act
            await _createAssignmentEventHandler.ExecuteAsync(transitionDataModel, transition);
            _nextHandlerMock.Verify(x => x.ExecuteAsync(It.IsAny<TransitionDataModel>(), It.IsAny<Transitions.Transition>()), Times.Once);
        }

        [Test]
        public async Task Execute_WhenAssignmentEventSubjectIsNotCreate_ShouldNotSetTransitionIdAndTransitionTeams()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel
            {
                AssignmentEvent = new AccountTeam()
                {
                    data = new AssignmentData()
                    {
                        CrmAccountId = "123"
                    },
                    Subject = "CREATE"
                }
            };
            var transition = new Transitions.Transition();

            // Act
            await _createAssignmentEventHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            Assert.That(transition.Id, Is.EqualTo(Guid.Empty));
            Assert.IsNull(transition.TransitionTeams);
        }

        [Test]
        public async Task Execute_WhenExistingTransitionsIsNull_ShouldNotSetTransitionIdAndTransitionTeams()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel
            {
                AssignmentEvent = new AccountTeam()
                {
                    data = new AssignmentData()
                    {
                        CrmAccountId = "123"
                    },
                    Subject = "CREATE"
                },
                ExistingTransitions = null
            };
            var transition = new Transitions.Transition();

            // Act
            await _createAssignmentEventHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            Assert.That(transition.Id, Is.EqualTo(Guid.Empty));
            Assert.IsNull(transition.TransitionTeams);
        }

        [Test]
        public async Task Execute_WhenNextHandlerIsNotNull_ShouldCallNextHandlerExecuteMethod()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel
            {
                AssignmentEvent = new AccountTeam()
                {
                    data = new AssignmentData()
                    {
                        CrmAccountId = "123"
                    },
                    Subject = "CREATE"
                }
            };
            var transition = new Transitions.Transition();

            // Act
            await _createAssignmentEventHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            _nextHandlerMock.Verify(x => x.ExecuteAsync(transitionDataModel, transition), Times.Once);
        }
    }
}
