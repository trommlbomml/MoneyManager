using System;
using System.Globalization;
using MoneyManager.Interfaces;
using MoneyManager.Model.Entities;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.Model.Tests.Entities
{
    [TestFixture]
    class StandingOrderEntityPersistenceTests
    {
        public ApplicationContext ApplicationContext { get; set; }

        public void Setup()
        {
            ApplicationContext = Substitute.For<ApplicationContext>();
            ApplicationContext.Now.Returns(new DateTime(2014, 6, 5));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Serialize(bool hasCategory)
        {
            var category = new CategoryEntityImp();
            var standingOrder = new StandingOrderEntityImp(ApplicationContext)
            {
                Category = hasCategory ? category : null,
                Description =  "Test",
                FirstBookDate = new DateTime(2015, 4, 1),
                LastBookDate = new DateTime(2016, 4, 1),
                LastBookedDate = new DateTime(2015, 5, 1),
                MonthPeriodStep = 1,
                ReferenceDay = 1,
                ReferenceMonth = 4,
                Value = 45.22
            };
            var standingOrderPersistence = new StandingOrderEntityPersistence(standingOrder);

            var data = standingOrderPersistence.Serialize();
            Assert.That(data.Attribute("CategoryId").Value, Is.EqualTo(hasCategory ? category.PersistentId : string.Empty));
            Assert.That(data.Attribute("Description").Value, Is.EqualTo("Test"));
            Assert.That(data.Attribute("FirstBookDate").Value, Is.EqualTo(standingOrder.FirstBookDate.ToString(CultureInfo.InvariantCulture)));
            Assert.That(data.Attribute("LastBookDate").Value, Is.EqualTo(standingOrder.LastBookDate.ToString(CultureInfo.InvariantCulture)));
            Assert.That(data.Attribute("LastBookedDate").Value, Is.EqualTo(standingOrder.LastBookedDate.ToString(CultureInfo.InvariantCulture)));
            Assert.That(data.Attribute("MonthPeriodStep").Value, Is.EqualTo(standingOrder.MonthPeriodStep.ToString(CultureInfo.InvariantCulture)));
            Assert.That(data.Attribute("ReferenceDay").Value, Is.EqualTo(standingOrder.ReferenceDay.ToString(CultureInfo.InvariantCulture)));
            Assert.That(data.Attribute("ReferenceMonth").Value, Is.EqualTo(standingOrder.ReferenceMonth.ToString(CultureInfo.InvariantCulture)));
            Assert.That(data.Attribute("Value").Value, Is.EqualTo(standingOrder.Value.ToString(CultureInfo.InvariantCulture)));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ConstructorWorks(bool hasCategory)
        {
            var category = new CategoryEntityImp();
            var standingOrder = new StandingOrderEntityImp(ApplicationContext)
            {
                Category = hasCategory ? category : null,
                Description = "Test",
                FirstBookDate = new DateTime(2015, 4, 1),
                LastBookDate = new DateTime(2016, 4, 1),
                LastBookedDate = new DateTime(2015, 5, 1),
                MonthPeriodStep = 1,
                ReferenceDay = 1,
                ReferenceMonth = 4,
                Value = 45.22
            };
            var standingOrderPersistence = new StandingOrderEntityPersistence(standingOrder);

            Assert.That(standingOrderPersistence.PersistentId, Is.EqualTo(standingOrder.PersistentId));
            Assert.That(standingOrderPersistence.Description, Is.EqualTo(standingOrder.Description));
            Assert.That(standingOrderPersistence.FirstBookDate, Is.EqualTo(standingOrder.FirstBookDate));
            Assert.That(standingOrderPersistence.LastBookDate, Is.EqualTo(standingOrder.LastBookDate));
            Assert.That(standingOrderPersistence.LastBookedDate, Is.EqualTo(standingOrder.LastBookedDate));
            Assert.That(standingOrderPersistence.MonthPeriodStep, Is.EqualTo(standingOrder.MonthPeriodStep));
            Assert.That(standingOrderPersistence.ReferenceDay, Is.EqualTo(standingOrder.ReferenceDay));
            Assert.That(standingOrderPersistence.ReferenceMonth, Is.EqualTo(standingOrder.ReferenceMonth));
            Assert.That(standingOrderPersistence.Value, Is.EqualTo(standingOrder.Value));
            Assert.That(standingOrderPersistence.Category, Is.InstanceOf<CategoryEntityStub>());
            Assert.That(standingOrderPersistence.Category.PersistentId, Is.EqualTo(hasCategory ? category.PersistentId : ""));
        }
    }
}
