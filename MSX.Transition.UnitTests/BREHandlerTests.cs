using Microsoft.Extensions.Logging;
using MSX.Assignment.Common.Infra.Clients;
using MSX.Common.Models.ApplicationException;
using Moq;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;
using MSX.Transition.Business.Services;
using MSX.Common.Models.BRE;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class BREHandlerTests
    {
        private Mock<ILogger<BREHandler>> _loggerMock;
        private Mock<IBREServiceClient> _breServiceClientMock;

        private BREHandler _breHandler;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<BREHandler>>();
            _breServiceClientMock = new Mock<IBREServiceClient>();

            _breHandler = new BREHandler(_loggerMock.Object, _breServiceClientMock.Object);
        }

        [Test]
        public async Task Execute_ValidTransitionDataModelAndTransition_CallsBreServiceClientExecute()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            transitionDataModel.CheckFlags = new CheckFlags() { RuleName="RuleName"};
            var transition = new Transitions.Transition();

            // Act
            await _breHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            _breServiceClientMock.Verify(x => x.Execute(It.IsAny<BRERequest>()), Times.Once);
        }

        [Test]
        public async Task Execute_EmptyTransitionDataModelAndTransition_ReturnNUll()
        {
            // Arrange
            var transitionDataModel = new TransitionDataModel();
            transitionDataModel.CheckFlags = new CheckFlags() { RuleName = "RuleName" };
            var transition = new Transitions.Transition();

            // Act
            await _breHandler.ExecuteAsync(transitionDataModel, transition);

            // Assert
            Assert.Null(transitionDataModel.BREData);
        }

        // Add more unit tests as needed

    }
}
