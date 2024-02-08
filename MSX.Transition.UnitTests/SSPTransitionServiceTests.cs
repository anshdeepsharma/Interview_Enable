using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MSX.Assignment.Common.Infra.Clients;
using MSX.Common.Infra;
using MSX.Transition.Business.Services;
using MSX.Transition.Providers.Contracts.v1.Provider;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class SSPTransitionServiceTests
    {
        private Mock<ITranstionHandler> _transtionHandlerMock;
        private Mock<IGrantAccessService> _grantAccessServiceMock;
        private SSPTransitionService _sut;
        private Mock<IOptions<BREConfig>> _breConfigMock;
        private Mock<IOptions<TransitionConfig>> _transitionConfigMock;

        [SetUp]
        public void Setup()
        {
            _transtionHandlerMock = new Mock<ITranstionHandler>();
            _grantAccessServiceMock = new Mock<IGrantAccessService>();
            _breConfigMock = new Mock<IOptions<BREConfig>>();
            _transitionConfigMock = new Mock<IOptions<TransitionConfig>>();
            var accountCRMProviderMock = new Mock<IAccountCRMProvider>();
            var opportunityCRMProviderMock = new Mock<IOpportunityCRMProvider>();
            var auditHistoryCRMProviderMock = new Mock<IAuditHistoryCRMProvider>();
            var previousSegmentGetterMock = new Mock<IPreviousSegmentGetter>();
            var previousSubsegmentGetterMock = new Mock<IPreviousSubsegmentGetter>();
            var accountOwnerGetterMock = new Mock<IAccountOwnerGetter>();
            var crmProviderMock = new Mock<ICRMProvider>();
            var taxonomyServiceClientMock = new Mock<ITaxonomyServiceClient>();
            var breServiceClientMock = new Mock<IBREServiceClient>();
            var breLoggerMock = new Mock<ILogger<BREHandler>>();
            var transitionCRMProviderMock = new Mock<ITransitionCRMProvider>();
            var accountHandlerMock = new Mock<AccountHandler>(accountCRMProviderMock.Object);
            var opportunityHandlerMock = new Mock<OpportunityHandler>(opportunityCRMProviderMock.Object);
            var auditHistoryHandlerMock = new Mock<AuditHistoryHandler>(auditHistoryCRMProviderMock.Object
                , previousSegmentGetterMock.Object
                , previousSubsegmentGetterMock.Object
                , accountOwnerGetterMock.Object
                , crmProviderMock.Object
                , taxonomyServiceClientMock.Object);
            var breHandlerMock = new Mock<BREHandler>(breLoggerMock.Object, breServiceClientMock.Object);

            var assignmentEventandlerMock = new Mock<IAssignmentEventandler>();
            var createAssignmentEventHandlerMock = new Mock<CreateAssignmentEventHandler>();
            var deleteAssignmentEventHandlerrMock = new Mock<DeleteAssignmentEventHandler>();
            var assignmentCRMProviderMock = new Mock<IAssignmentCRMProvider>();
            var userCRMProviderMock = new Mock<IUserCRMProvider>();
            var assignmentHanlderMock = new Mock<AssignmentHandler>(assignmentEventandlerMock.Object
                , createAssignmentEventHandlerMock.Object
                , deleteAssignmentEventHandlerrMock.Object
                , assignmentCRMProviderMock.Object
                , userCRMProviderMock.Object
                , transitionCRMProviderMock.Object);

            var existingTransitionsHandlerMock = new Mock<ExistingTransitionsHandler>(transitionCRMProviderMock.Object);
            
            assignmentEventandlerMock.Setup(x => x.SetNextHandler(It.IsAny<IAssignmentEventandler>()))
                .Returns(assignmentEventandlerMock.Object);

            _transtionHandlerMock.Setup(x => x.SetNextHandler(It.IsAny<ITranstionHandler>()))
               .Returns(_transtionHandlerMock.Object);

            _breConfigMock.Setup(config => config.Value).Returns(new BREConfig
            {
                RuleName = "TestRule"
            });

            _transitionConfigMock.Setup(config => config.Value).Returns(new TransitionConfig
            {
                CheckTransitionLastXHours = "24"
            });

            _sut = new SSPTransitionService(_transtionHandlerMock.Object
                , accountHandlerMock.Object
                , opportunityHandlerMock.Object
                , auditHistoryHandlerMock.Object
                , breHandlerMock.Object
                , assignmentHanlderMock.Object
                , existingTransitionsHandlerMock.Object
                , _grantAccessServiceMock.Object
                , _breConfigMock.Object
                , _transitionConfigMock.Object);
        }

        [Test]
        public async Task ExecuteTransition_ShouldCallTranstionHandlerExecute()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition();

            // Act
            await _sut.ExecuteTransitionAsync(transitionDataModel, transition);

            // Assert
            _transtionHandlerMock.Verify(h => h.ExecuteAsync(transitionDataModel, transition), Times.Once);
        }
    }
}
