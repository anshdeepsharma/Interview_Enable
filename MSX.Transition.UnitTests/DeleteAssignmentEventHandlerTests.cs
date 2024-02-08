using Moq;
using MSX.Common.Models.Accounts;
using MSX.Common.Models.Assignments;
using MSX.Transition.Business.Services;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class DeleteAssignmentEventHandlerTests
    {
        private DeleteAssignmentEventHandler _deleteAssignmentEventHandler;

        [SetUp]
        public void Setup()
        {
            _deleteAssignmentEventHandler = new DeleteAssignmentEventHandler();
        }

        [Test]
        public async Task Execute_WhenAssignmentEventSubjectIsDelete_ShouldSetOwnerId()
        {
            // Arrange
            TransitionDataModel transitionDataModel = new TransitionDataModel()
            {
                AssignmentEvent = new Common.Models.Assignments.AccountTeam()
                {
                    data = new Common.Models.Assignments.AssignmentData()
                    {
                        CrmAccountId = "1234",
                        Alias = "Alias"
                    },
                    
                },
               // SetIncommingUser = SetIncommingUser()
            };

            Transitions.Transition transition = new Transitions.Transition();   

            // Act
            await _deleteAssignmentEventHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            // Add your assertions here
        }

        [Test]
        public async Task Execute_WhenSetOutgoingUserIsNotNull_ShouldCallSetOutgoingUserMethod()
        {
            // Arrange
            TransitionDataModel transitionDataModel = new TransitionDataModel()
            {
                AssignmentEvent = new Common.Models.Assignments.AccountTeam()
                {
                    data = new Common.Models.Assignments.AssignmentData()
                    {
                        CrmAccountId = "1234"
                    }
                }
            };

            Transitions.Transition transition = new Transitions.Transition();
            // Act
            await _deleteAssignmentEventHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            // Add your assertions here
        }

        [Test]
        public async Task Execute_WhenSetIncommingUserIsNotNull_ShouldCallSetIncommingUserMethod()
        {
            // Arrange
            TransitionDataModel transitionDataModel = new TransitionDataModel()
            {
                AssignmentEvent = new Common.Models.Assignments.AccountTeam()
                {
                    data = new Common.Models.Assignments.AssignmentData()
                    {
                        CrmAccountId = "1234"
                    }
                }
            };

            Transitions.Transition transition = new Transitions.Transition();
            // Act
            await _deleteAssignmentEventHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            // Add your assertions here
        }

        [Test]
        public async Task Execute_WhenNextHandlerIsNotNull_ShouldCallNextHandlerExecuteMethod()
        {
            // Arrange
             TransitionDataModel transitionDataModel = new TransitionDataModel()
            {
                AssignmentEvent = new Common.Models.Assignments.AccountTeam()
                {
                    data = new Common.Models.Assignments.AssignmentData()
                    {
                        CrmAccountId = "1234"
                    }
                }
            };

            Transitions.Transition transition = new Transitions.Transition();
            // Act
            await _deleteAssignmentEventHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            // Add your assertions here
        }


        private IEnumerable<AccountUserRole> IncommingUsersFilter(TransitionDataModel transitionDataModel)
        {
            return transitionDataModel.ExistingAssignments?.Where(x => x.StandardTitle == "ATU Account Executive");
        }

        private AccountUserRole TransitionManagerFilter(TransitionDataModel transitionDataModel)
        {
            return transitionDataModel.ExistingAssignments?.Where(x => x.StandardTitle == "ATU Manager").FirstOrDefault();
        }

        private void SetIncommingUser(string userId, Transitions.Transition transition)
        {
            transition.OwnerId = userId;
        }
        private void SetOutgoingUser(string userId, Transitions.Transition transition)
        {
            transition.PreviousAccountOwner = userId;
        }
        // Add more test methods as needed
    }
}
