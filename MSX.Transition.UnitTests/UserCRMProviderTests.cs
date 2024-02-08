using Microsoft.Extensions.Logging;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Moq;
using NUnit.Framework;
using System.Text.Json;
using MSX.Transition.Providers.CRM;

namespace MSX.Transition.UnitTests
{
    public class UserCRMProviderTests
    {
        private Mock<ILogger<IUserCRMProvider>> _loggerMock;
        private Mock<ICRMProvider> _crmProviderMock;
        private UserCRMProvider _userCRMProvider;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<IUserCRMProvider>>();
            _crmProviderMock = new Mock<ICRMProvider>();
            _userCRMProvider = new UserCRMProvider(_loggerMock.Object, _crmProviderMock.Object);
        }

        [Test]
        public async Task ResolveUser_WhenUserAliasIsNull_ReturnsUserWithAlias()
        {
            // Arrange
            string userAlias = null;
            User expectedUser = new User { Alias = userAlias };

            // Act
            User actualUser = await _userCRMProvider.ResolveUserAsync(userAlias);

            // Assert
            Assert.AreEqual(expectedUser.Alias, actualUser.Alias);
        }
    }
}
