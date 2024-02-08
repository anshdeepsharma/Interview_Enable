using Microsoft.AspNetCore.Mvc;
using Moq;
using MSX.Transition.API.Controllers;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class HealthControllerTests
    {
        private HealthController _healthController;

        [SetUp]
        public void Setup()
        {
            _healthController = new HealthController();
        }

        [Test]
        public async Task PingAsync_ReturnsOkResult()
        {
            // Arrange

            // Act
            var result = await _healthController.PingAsync();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}
