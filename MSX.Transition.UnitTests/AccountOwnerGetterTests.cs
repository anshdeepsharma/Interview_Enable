using MSX.Common.Models.Audits;
using MSX.Transition.Business.Services;
using NUnit.Framework;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class AccountOwnerGetterTests
    {
        private AccountOwnerGetter _accountOwnerGetter;

        [SetUp]
        public void Setup()
        {
            _accountOwnerGetter = new AccountOwnerGetter();
        }

        [Test]
        public void Get_ReturnsPreviousAccountOwner_WhenAccountAuditHistoryIsNotNull()
        {
            // Arrange
            var accountAuditHistory = new AuditHistory
            {
                ChangeData = new List<ChangeData>
                {
                    new ChangeData
                    {
                        ChangedAttributes = new List<ChangedAttribute>
                        {
                            new ChangedAttribute
                            {
                                LogicalName = "msp_previousaccountowner",
                                OldValue = "John Doe",
                            }
                        }
                    }
                }
            };

            // Act
            var result = _accountOwnerGetter.Get(accountAuditHistory);

            // Assert
            Assert.That(result, Is.EqualTo("John Doe"));
        }

        [Test]
        public void Get_ReturnsEmptyString_WhenAccountAuditHistoryIsNull()
        {
            // Arrange
            // Act
            var result = _accountOwnerGetter.Get(null);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Get_ReturnsEmptyString_WhenChangedDataIsNull()
        {
            // Arrange
            AuditHistory accountAuditHistory = new AuditHistory();

            // Act
            var result = _accountOwnerGetter.Get(accountAuditHistory);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Get_ReturnsEmptyString_WhenNoPreviousAccountOwnerFound()
        {
            // Arrange
            var accountAuditHistory = new AuditHistory
            {
                ChangeData = new List<ChangeData>
                {
                    new ChangeData
                    {
                        ChangedAttributes = new List<ChangedAttribute>
                        {
                            new ChangedAttribute
                            {
                                LogicalName = "some_other_attribute",
                                NewValue = "Some Value"
                            }
                        }
                    }
                }
            };

            // Act
            var result = _accountOwnerGetter.Get(accountAuditHistory);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Get_ReturnsEmptyString_WhenChangeDataIsEmpty()
        {
            // Arrange
            var auditHistory = new AuditHistory
            {
                ChangeData = new List<ChangeData>()
            };

            // Act
            var result = _accountOwnerGetter.Get(auditHistory);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Get_ReturnsEmptyString_WhenChangeAttributeIsNull()
        {
            // Arrange
            var auditHistory = new AuditHistory
            {
                ChangeData = new List<ChangeData>()
                {
                    new ChangeData(),
                }
            };

            // Act
            var result = _accountOwnerGetter.Get(auditHistory);

            // Assert
            Assert.That(result, Is.Empty);
        }
        [Test]
        public void Get_ReturnsEmptyString_WhenChangeAttributeIsEmpty()
        {
            // Arrange
            var auditHistory = new AuditHistory
            {
                ChangeData = new List<ChangeData>()
                {
                    new ChangeData()
                    {
                        ChangedAttributes=new List<ChangedAttribute>()
                    }
                }
            };

            // Act
            var result = _accountOwnerGetter.Get(auditHistory);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }


    }
}
