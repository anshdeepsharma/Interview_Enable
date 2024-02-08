using Microsoft.Extensions.DependencyInjection;
using Moq;
using MSX.Transition.API;
using NUnit.Framework;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class DIContainterTests
    {
        private IServiceCollection _services;

        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();
            DIContainter.RegisterDependencies(_services);
        }

        [Test]
        public void RegisterDependencies_ShouldRegisterDependencies()
        {
            // Arrange

            // Act

            // Assert
            Assert.Pass();
        }
    }
}
