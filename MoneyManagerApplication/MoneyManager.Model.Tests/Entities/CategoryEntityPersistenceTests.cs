using System.Linq;
using MoneyManager.Model.Entities;
using NUnit.Framework;

namespace MoneyManager.Model.Tests.Entities
{
    [TestFixture]
    class CategoryEntityPersistenceTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Serialize(bool nameisNull)
        {
            var category = new CategoryEntityImp { Name = nameisNull ? null : "Category1" };

            var categoryPersistence = new CategoryEntityPersistenceImp(category);
            var serialized = categoryPersistence.Serialize();

            Assert.That(serialized.Name.LocalName, Is.EqualTo("Category"));
            Assert.That(serialized.Attributes().Count(), Is.EqualTo(2));
            Assert.That(serialized.Attribute("Id").Value, Is.EqualTo(category.PersistentId));
            Assert.That(serialized.Attribute("Name").Value, Is.EqualTo(nameisNull ? "" : category.Name));
        }

        [Test]
        public void ConstructorWorks()
        {
            var category = new CategoryEntityImp { Name = "Category1" };

            var categoryPersistence = new CategoryEntityPersistenceImp(category);
            Assert.That(categoryPersistence.PersistentId, Is.EqualTo(category.PersistentId));
            Assert.That(categoryPersistence.Name, Is.EqualTo(category.Name));
        }
    }
}
