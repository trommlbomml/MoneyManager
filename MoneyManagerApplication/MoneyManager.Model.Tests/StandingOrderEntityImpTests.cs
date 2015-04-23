
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class StandingOrderEntityImpTests
    {
        [Test]
        public void InitialState()
        {
            var standingOrder = new StandingOrderEntityImp();
            Assert.That(standingOrder.HasChanged, Is.True);
        }

        [TestCase(1, 1, 1, true)]
        [TestCase(1, 1, 2, true)]
        [TestCase(1, 1, 3, true)]
        [TestCase(1, 1, 7, true)]
        [TestCase(2, 1, 3, true)]
        [TestCase(2, 1, 5, true)]
        [TestCase(2, 1, 7, true)]
        [TestCase(2, 1, 11, true)]
        [TestCase(3, 1, 1, true)]
        [TestCase(3, 1, 4, true)]
        [TestCase(3, 1, 10, true)]
        [TestCase(6, 1, 1, true)]
        [TestCase(6, 1, 7, true)]
        [TestCase(12, 1, 1, true)]
        [TestCase(2, 1, 4, false)]
        [TestCase(2, 2, 5, false)]
        [TestCase(3, 1, 3, false)]
        [TestCase(3, 2, 4, false)]
        [TestCase(6, 1, 3, false)]
        [TestCase(6, 4, 1, false)]
        [TestCase(12, 4, 5, false)]
        public void IsMonthOfPeriod(int period, int referenceMonth, int month, bool expectedIsPeriodMonth)
        {
            var entity = new StandingOrderEntityImp
            {
                MonthPeriodStep = period,
                ReferenceDay = 1,
                ReferenceMonth = referenceMonth
            };

            Assert.That(entity.IsMonthOfPeriod(month), Is.EqualTo(expectedIsPeriodMonth));
        }

        [Test]
        public void GetNextPeriodDateTimeForFirstCall([Values(1,3,6,12)]int period)
        {
            var entity = new StandingOrderEntityImp
            {
                MonthPeriodStep = period,
                ReferenceDay = 1,
                ReferenceMonth = 4,
                FirstBookDate = new DateTime(2014, 4, 1)
            };

            var expectedDateTime = new DateTime(2014, 4, 1);
            var nextDateTime = entity.GetNextPaymentDateTime();

            Assert.That(nextDateTime, Is.EqualTo(expectedDateTime));
        }

        [Test]
        public void GetNextPeriodForLastBookDateReturnsNull([Values(1, 3, 6, 12)]int period)
        {
            var entity = new StandingOrderEntityImp
            {
                MonthPeriodStep = period,
                ReferenceDay = 1,
                ReferenceMonth = 4,
                FirstBookDate = new DateTime(2014, 2, 1),
                LastBookedDate = new DateTime(2014, 4, 1),
                LastBookDate =  new DateTime(2014, 4, 1)
            };

            Assert.That(entity.GetNextPaymentDateTime(), Is.Null);
        }

        [Test]
        public void GetNextPeriodDateTime([Values(1, 3, 6, 12)]int period)
        {
            var entity = new StandingOrderEntityImp
            {
                MonthPeriodStep = period,
                ReferenceDay = 1,
                ReferenceMonth = 4,
                FirstBookDate = new DateTime(2014, 4, 1),
                LastBookedDate = new DateTime(2014, 4, 1)
            };

            var expectedDateTime = new DateTime(2014, 4, 1).AddMonths(period);
            var nextDateTime = entity.GetNextPaymentDateTime();

            Assert.That(nextDateTime, Is.EqualTo(expectedDateTime));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Deserialize(bool withCategory)
        {
            const string persistentId = "TestId";
            var dateTime = new DateTime(2014, 7, 4);
            const double value = 12.77;
            const string description = "TestDescription";

            var categories = Enumerable.Range(1, 3).Select(i => new CategoryEntityImp()).ToArray();
            var categoryId = withCategory ? categories.First().PersistentId : "";
            
            var element = new XElement("Request",
                new XAttribute("Id", persistentId),
                new XAttribute("FirstBookDate", dateTime.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("ReferenceMonth", 1),
                new XAttribute("ReferenceDay", 2),
                new XAttribute("MonthPeriodStep", 3),
                new XAttribute("Description", description),
                new XAttribute("LastBookDate", dateTime),
                new XAttribute("LastBookedDate", dateTime),
                new XAttribute("Value", value),
                new XAttribute("CategoryId", categoryId));

            var standingOrder = new StandingOrderEntityImp(element, categories);

            Assert.That(standingOrder.PersistentId, Is.EqualTo(persistentId));
            Assert.That(standingOrder.FirstBookDate, Is.EqualTo(dateTime));
            Assert.That(standingOrder.ReferenceMonth, Is.EqualTo(1));
            Assert.That(standingOrder.ReferenceDay, Is.EqualTo(2));
            Assert.That(standingOrder.MonthPeriodStep, Is.EqualTo(3));
            Assert.That(standingOrder.Description, Is.EqualTo(description));
            Assert.That(standingOrder.LastBookDate, Is.EqualTo(dateTime));
            Assert.That(standingOrder.LastBookedDate, Is.EqualTo(dateTime));
            Assert.That(standingOrder.Value, Is.EqualTo(value));
            Assert.That(standingOrder.Category, Is.EqualTo(withCategory ? categories.First() : null));
            Assert.That(standingOrder.HasChanged, Is.False);
        }
    }
}

