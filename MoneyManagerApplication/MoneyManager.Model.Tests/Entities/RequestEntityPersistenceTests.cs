using System;
using System.Globalization;
using System.Linq;
using MoneyManager.Model.Entities;
using NUnit.Framework;

namespace MoneyManager.Model.Tests.Entities
{
    [TestFixture]
    internal class RequestEntityPersistenceTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Serialize(bool descriptionIsNull)
        {
            var category = new CategoryEntityImp();
            var request = new RequestEntityImp
            {
                Date = new DateTime(2014, 5, 6),
                Description = descriptionIsNull ? null : "TestDescription",
                Value = 11.27,
                Category = category
            };

            var requestPersistence = new RequestEntityPersistenceImp(request);
            var serialized = requestPersistence.Serialize();

            Assert.That(serialized.Name.LocalName, Is.EqualTo("Request"));
            Assert.That(serialized.Attributes().Count(), Is.EqualTo(5));
            Assert.That(serialized.Attribute("Id").Value, Is.EqualTo(request.PersistentId));
            Assert.That(serialized.Attribute("Description").Value, Is.EqualTo(descriptionIsNull ? "" : request.Description));
            Assert.That(serialized.Attribute("Value").Value, Is.EqualTo(request.Value.ToString(CultureInfo.InvariantCulture)));
            Assert.That(serialized.Attribute("Date").Value, Is.EqualTo(request.Date.ToString(CultureInfo.InvariantCulture)));
            Assert.That(serialized.Attribute("CategoryId").Value, Is.EqualTo(category.PersistentId));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ConstructorWorks(bool hasCategory)
        {
            var category = new CategoryEntityImp();
            var request = new RequestEntityImp
            {
                Date = new DateTime(2014, 5, 6),
                Description = "TestDescription",
                Value = 11.27,
                Category = hasCategory ? category : null
            };

            var requestPersistence = new RequestEntityPersistenceImp(request);
            Assert.That(requestPersistence.PersistentId, Is.EqualTo(request.PersistentId));
            Assert.That(requestPersistence.Description, Is.EqualTo(request.Description));
            Assert.That(requestPersistence.Value, Is.EqualTo(request.Value));
            Assert.That(requestPersistence.Category, Is.InstanceOf<CategoryEntityStub>());
            Assert.That(requestPersistence.Category.PersistentId, Is.EqualTo( hasCategory ? category.PersistentId : string.Empty));
        }
    }
}
