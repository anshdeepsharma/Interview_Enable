using MSX.Common.Models.Audits;
using MSX.Transition.Business.Services;
using NUnit.Framework;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class PreviousSegmentGetterTests
    {
        private PreviousSegmentGetter _previousSegmentGetter;

        [SetUp]
        public void SetUp()
        {
            _previousSegmentGetter = new PreviousSegmentGetter();
        }

        [Test]
        public void Get_ReturnsPreviousSegment_WhenChangeDataIsNotEmpty()
        {
            // Arrange
            var auditHistory = new AuditHistory
            {
                ChangeData = new List<ChangeData>
                {
                    new ChangeData
                    {
                        ChangedAttributes = new List<ChangedAttribute>
                        {
                            new ChangedAttribute
                            {
                                LogicalName = "msp_endcustomersegmentcode",
                                OldValue = "Segment1"
                            }
                        }
                    }
                }
            };

            // Act
            var result = _previousSegmentGetter.Get(auditHistory);

            // Assert
            Assert.That(result, Is.EqualTo("Segment1"));
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
            var result = _previousSegmentGetter.Get(auditHistory);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
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
            var result = _previousSegmentGetter.Get(auditHistory);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
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
            var result = _previousSegmentGetter.Get(auditHistory);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Get_ReturnsEmptyString_WhenNoMatchingAttributeFound()
        {
            // Arrange
            var auditHistory = new AuditHistory
            {
                ChangeData = new List<ChangeData>
                {
                    new ChangeData
                    {
                        ChangedAttributes = new List<ChangedAttribute>
                        {
                            new ChangedAttribute
                            {
                                LogicalName = "other_attribute",
                                NewValue = "Value1"
                            }
                        }
                    }
                }
            };

            // Act
            var result = _previousSegmentGetter.Get(auditHistory);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Get_ReturnsEmptyString_WhenAccountAuditHistoryIsNull()
        {
            // Act
            var result = _previousSegmentGetter.Get(null);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Get_ReturnsEmptyString_WhenAccountChangedDataIsNull()
        {
            // Act
            var result = _previousSegmentGetter.Get(new AuditHistory());

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }
    }
}
