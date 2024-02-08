using Microsoft.Extensions.Logging;
using Moq;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM;
using MSX.Transition.Providers.CRM.Contract;
using NUnit.Framework;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class TransitionCRMProviderTests
    {
        private Mock<ILogger<ITransitionCRMProvider>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;
        private Mock<ICRMXMLHandler> _crmXMLHandlerMock;
        private TransitionCRMProvider _transitionCRMProvider;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ITransitionCRMProvider>>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _crmXMLHandlerMock = new Mock<ICRMXMLHandler>();
            _transitionCRMProvider = new TransitionCRMProvider(_loggerMock.Object, _crmProviderMock.Object, _crmXMLHandlerMock.Object);
        }

        [Test]
        public async Task GetTransition_ShouldReturnTransitions()
        {
            // Arrange
            string transitionType = "ATS";
            string transitionSubtype = "sampleTransitionSubtype";
            string accountId = "sampleAccountId";
            var expectedQuery = $"msp_relationshipmanagements?$filter=msp_accounttransitiiontype eq {transitionType}" +
                $" and statecode eq 0 and _msp_accountid_value eq {accountId}";
            var expectedTransitions = new List<Transitions.Transition>();

            _crmProviderMock.Setup(x => x.ExecuteGetRequestAsync<List<Transitions.Transition>>(expectedQuery))
                .ReturnsAsync(expectedTransitions);
            _crmXMLHandlerMock.Setup(x => x.ConstructQuery<Transitions.Transition>(It.IsAny<string>()
                , It.IsAny<string>()
                , It.IsAny<string>()
                , It.IsAny<string>()
                , It.IsAny<Dictionary<string, string>>()
                , It.IsAny<Transitions.Transition>())).Returns(expectedQuery);
            // Act
            var result = await _transitionCRMProvider.GetTransitionAsync(transitionType, transitionSubtype, accountId, "24");

            // Assert
            Assert.AreEqual(expectedTransitions, result);
            _crmProviderMock.Verify(x => x.ExecuteGetRequestAsync<ResponseValueArray<TransitionCds>> (It.IsAny<string>()), Times.Once);
        }
    }
}
