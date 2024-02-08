using Microsoft.Extensions.Logging;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Common;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.Transitions;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Moq;
using NUnit.Framework;
using MSX.Transition.Providers.CRM;
using Newtonsoft.Json.Linq;
using MSX.Common.Models.Responses.v1;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class TransitionTeamCRMProviderTests
    {
        private Mock<ILogger<ITransitionTeamCRMProvider>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;
        private Mock<ICRMTranslator> _crmTranslatorMock;
        private TransitionTeamCRMProvider _transitionTeamCRMProvider;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ITransitionTeamCRMProvider>>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _crmTranslatorMock = new Mock<ICRMTranslator>();

            _transitionTeamCRMProvider = new TransitionTeamCRMProvider(
                _loggerMock.Object,
                _crmProviderMock.Object,
                _crmTranslatorMock.Object
            );
            _crmTranslatorMock.Setup(x => x.Translate(It.IsAny<object>(), It.IsAny<FieldMapper>()))
                .ReturnsAsync(new JObject());
            _crmProviderMock.Setup(x => x.CreateBatchRequest(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new BatchRequest());
            _crmProviderMock.Setup(x => x.ExecuteBatchRequestAsync(It.IsAny<List<BatchRequest>>())).ReturnsAsync(new List<Response<BatchRequest>>());
        }

        [Test]
        public async Task CreateTransitionTeams_Should_ExecuteBatchRequestForEachTeam()
        {
            // Arrange
            var transitionTeams = new List<TransitionTeam> { new TransitionTeam(), new TransitionTeam() };

            // Act
            await _transitionTeamCRMProvider.CreateTransitionTeamsAsync(transitionTeams);

            // Assert
            _crmProviderMock.Verify(x => x.ExecuteBatchRequestAsync(It.IsAny<List<BatchRequest>>()), Times.Once);
        }
    }
}
