using Moq;
using MSX.Common.Models.ApplicationException;
using MSX.Common.Models.Assignments;
using MSX.Transition.Business.Services;
using MSX.Transition.Providers.Contracts.v1.Provider;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class AccountHandlerTests
    {
        private Mock<IAccountCRMProvider> _accountCRMProviderMock;
        private AccountHandler _accountHandler;

        [SetUp]
        public void Setup()
        {
            _accountCRMProviderMock = new Mock<IAccountCRMProvider>();
            _accountHandler = new AccountHandler(_accountCRMProviderMock.Object);
        }

        [Test]
        public async Task Execute_ValidTransitionDataModelAndTransition_CallsGetAccountByCRMId()
        {
            var account = new Common.Models.Accounts.Account()
            {
                Data = new Common.Models.Accounts.Data()
                {
                    Id = "AccountId",
                    Segment = "Segment",
                    SubSegment = "SubSegment",
                    MSSalesAccountId = "12345",
                    ParentMSSalesAccountId = "12345"
                }
            };
            // Arrange
            var transitionDataModel = new TransitionDataModel()
            {
                AssignmentEvent = new AccountTeam
                {
                    data = new AssignmentData
                    {
                        CrmAccountId = "CRMAccountId"
                    }
                }
            };
            var transition = new Transitions.Transition();

            _accountCRMProviderMock.Setup(p => p.GetAccountByCRMIdAsync(It.IsAny<string>()))
                .ReturnsAsync(account);

            // Act
            await _accountHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            _accountCRMProviderMock.Verify(p => p.GetAccountByCRMIdAsync(It.IsAny<string>()), Times.Once);
            Assert.That(transition.AccountId, Is.EqualTo(account.Data.Id));
            Assert.That(transition.CurrentSegment, Is.EqualTo(account.Data.Segment));
            Assert.That(transition.CurrentSubsegment, Is.EqualTo(account.Data.SubSegment));
            Assert.That(transition.AccountTPId, Is.EqualTo(account.Data.ParentMSSalesAccountId));
            Assert.That(transitionDataModel.Account, Is.EqualTo(account.Data));
        }

        [Test]
        public async Task Execute_NullCrmAccountId_ThrowsDomainException()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel
            {
                AssignmentEvent = new AccountTeam
                {
                    data = new AssignmentData
                    {
                        CrmAccountId = null
                    }
                }
            };
            var transition = new Transitions.Transition();

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _accountHandler.ExecuteAsync(transitionDataModel, transition));
        }

        [Test]
        public async Task Execute_NoAccountFound_ThrowsDomainException()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel
            {
                AssignmentEvent = new AccountTeam
                {
                    data = new AssignmentData
                    {
                        CrmAccountId = "123"
                    }
                }
            };
            var transition = new Transitions.Transition();
            _accountCRMProviderMock.Setup(p => p.GetAccountByCRMIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new Common.Models.Accounts.Account() { Data = new Common.Models.Accounts.Data() });

            // Act & Assert
            Assert.ThrowsAsync<DomainException>(() => _accountHandler.ExecuteAsync(transitionDataModel, transition));
        }

        // Add more unit tests as needed
    }
}
