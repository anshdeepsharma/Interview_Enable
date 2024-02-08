using Moq;
using Transitions = MSX.Common.Models.Transitions;
using MSX.Transition.Providers.Contracts.v1.Provider;
using NUnit.Framework;
using MSX.Transition.Business.Services;
using MSX.Common.Models.Audits;
using MSX.Assignment.Common.Infra.Clients;

namespace MSX.Transition.UnitTests
{
    public class AuditHistoryHandlerTests
    {
        [Test]
        public async Task Execute_ShouldGetAccountAuditHistory()
        {
            // Arrange
            var auditHistoryCRMProviderMock = new Mock<IAuditHistoryCRMProvider>();
            var previousSegmentGetterMock = new Mock<IPreviousSegmentGetter>();
            var previousSubsegmentGetterMock = new Mock<IPreviousSubsegmentGetter>();
            var accountOwnerGetterMock = new Mock<IAccountOwnerGetter>();
            var crmProviderMock = new Mock<ICRMProvider>();
            var taxonomyServiceClientMock = new Mock<ITaxonomyServiceClient>();

            var handler = new AuditHistoryHandler(auditHistoryCRMProviderMock.Object,
                previousSegmentGetterMock.Object,
                previousSubsegmentGetterMock.Object,
                accountOwnerGetterMock.Object,
                crmProviderMock.Object,
                taxonomyServiceClientMock.Object);

            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition { AccountId = "1" };

            // Act
            await handler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            Assert.That(transition.PreviousSegment, Is.Null);
            Assert.That(transition.PreviousSubsegment, Is.Null);
            Assert.That(transition.PreviousAccountOwner, Is.Null);
            auditHistoryCRMProviderMock.Verify(p => p.GetAccountAuditHistoryAsync(transition.AccountId), Times.Once);
        }

        [Test]
        public async Task Execute_ShouldSetValuesAsNullIfAuditHistoryIsEmpty()
        {
            // Arrange
            var auditHistoryCRMProviderMock = new Mock<IAuditHistoryCRMProvider>();
            var previousSegmentGetterMock = new Mock<IPreviousSegmentGetter>();
            var previousSubsegmentGetterMock = new Mock<IPreviousSubsegmentGetter>();
            var accountOwnerGetterMock = new Mock<IAccountOwnerGetter>();
            var crmProviderMock = new Mock<ICRMProvider>();
            var taxonomyServiceClientMock = new Mock<ITaxonomyServiceClient>();

            var handler = new AuditHistoryHandler(auditHistoryCRMProviderMock.Object,
                previousSegmentGetterMock.Object,
                previousSubsegmentGetterMock.Object,
                accountOwnerGetterMock.Object,
                crmProviderMock.Object,
                taxonomyServiceClientMock.Object);

            auditHistoryCRMProviderMock.Setup(x => x.GetAccountAuditHistoryAsync(It.IsAny<string>())).ReturnsAsync(new Common.Models.Audits.AuditHistory());

            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition { AccountId = "1" };

            // Act
            await handler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            Assert.That(transition.PreviousSegment, Is.Null);
            Assert.That(transition.PreviousSubsegment, Is.Null);
            Assert.That(transition.PreviousAccountOwner, Is.Null);
            auditHistoryCRMProviderMock.Verify(p => p.GetAccountAuditHistoryAsync(transition.AccountId), Times.Once);
        }

        [Test]
        public async Task Execute_ShouldSetValuesAuditHistoryIsReturned()
        {
            // Arrange
            var auditHistoryCRMProviderMock = new Mock<IAuditHistoryCRMProvider>();
            var previousSegmentGetterMock = new Mock<IPreviousSegmentGetter>();
            var previousSubsegmentGetterMock = new Mock<IPreviousSubsegmentGetter>();
            var accountOwnerGetterMock = new Mock<IAccountOwnerGetter>();
            var crmProviderMock = new Mock<ICRMProvider>();
            var taxonomyServiceClientMock = new Mock<ITaxonomyServiceClient>();

            var handler = new AuditHistoryHandler(auditHistoryCRMProviderMock.Object,
                previousSegmentGetterMock.Object,
                previousSubsegmentGetterMock.Object,
                accountOwnerGetterMock.Object,
                crmProviderMock.Object,
                taxonomyServiceClientMock.Object);

            var emptyAuditHistory = new Common.Models.Audits.AuditHistory();
            auditHistoryCRMProviderMock.Setup(x => x.GetAccountAuditHistoryAsync(It.IsAny<string>())).ReturnsAsync(emptyAuditHistory);

            previousSegmentGetterMock.Setup(x=>x.Get(It.IsAny<Common.Models.Audits.AuditHistory>())).Returns("PreviousSegment");
            previousSubsegmentGetterMock.Setup(x => x.Get(It.IsAny<Common.Models.Audits.AuditHistory>())).Returns("PreviousSubSegment");
            accountOwnerGetterMock.Setup(x => x.Get(It.IsAny<Common.Models.Audits.AuditHistory>())).Returns("PreviousOwner");

            var transitionDataModel = new TransitionDataModel();
            var transition = new Transitions.Transition { AccountId = "1" };

            // Act
            await handler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            Assert.That(transition.PreviousSegment, Is.EqualTo("PreviousSegment"));
            Assert.That(transition.PreviousSubsegment, Is.EqualTo("PreviousSubSegment"));
            Assert.That(transition.PreviousAccountOwner, Is.EqualTo("PreviousOwner"));

            auditHistoryCRMProviderMock.Verify(p => p.GetAccountAuditHistoryAsync(transition.AccountId), Times.Once);
            previousSegmentGetterMock.Verify(p => p.Get(emptyAuditHistory), Times.Once);
            previousSubsegmentGetterMock.Verify(p => p.Get(emptyAuditHistory), Times.Once);
            accountOwnerGetterMock.Verify(p => p.Get(emptyAuditHistory), Times.Once);
        }
    }
}
