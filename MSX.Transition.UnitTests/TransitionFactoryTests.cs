using Moq;
using MSX.Common.Models.Assignments;
using MSX.Common.Models.Enums;
using MSX.Transition.Business.Services;
using NUnit.Framework;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class TransitionFactoryTests
    {
        private Mock<IServiceProvider> serviceProviderMock;
        private TransitionFactory transitionFactory;

        [SetUp]
        public void Setup()
        {
            serviceProviderMock = new Mock<IServiceProvider>();
            transitionFactory = new TransitionFactory(serviceProviderMock.Object);
        }

        [Test]
        public void GetTransitionService_Returns_ATSTransitionService_When_TransitionType_Is_ATS()
        {
            // Arrange
            AccountTeam accountTeam = new() { data = new AssignmentData() { RoleType = RoleType.ATU.ToString(), RolePlayed = Constants.ATSRole, RoleSummary = Constants.CSAMRoleSummary } };
            var atsTransitionServiceMock = new Mock<ITransitionTypeService>();
            serviceProviderMock.Setup(s => s.GetService(typeof(ATSTransitionService))).Returns(atsTransitionServiceMock.Object);

            // Act
            var result = transitionFactory.GetTransitionSerice(accountTeam);

            // Assert
            Assert.That(result, Is.EqualTo(atsTransitionServiceMock.Object));
        }

        [Test]
        public void GetTransitionService_Returns_STUTransitionService_When_TransitionType_Is_STU()
        {
            // Arrange
            AccountTeam accountTeam = new() { data = new AssignmentData() { RoleType = RoleType.STU.ToString(), RolePlayed = Constants.SSPRole, RoleSummary = Constants.SSPRoleSummary } };
            var sspTransitionServiceMock = new Mock<ITransitionTypeService>();
            serviceProviderMock.Setup(s => s.GetService(typeof(SSPTransitionService))).Returns(sspTransitionServiceMock.Object);

            // Act
            var result = transitionFactory.GetTransitionSerice(accountTeam);

            // Assert
            Assert.That(result, Is.EqualTo(sspTransitionServiceMock.Object));
        }
    }
}
