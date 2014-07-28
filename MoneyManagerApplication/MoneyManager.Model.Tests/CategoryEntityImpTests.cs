using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class CategoryEntityImpTests
    {
        [Test]
        public void InitialState()
        {
            var request = new CategoryEntityImp();
            Assert.That(request.PersistentId, Is.Not.Null.Or.Empty);
            Assert.That(request.Name, Is.EqualTo(""));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Serialize(bool nameisNull)
        {
            var category = new CategoryEntityImp {Name = nameisNull ? null : "Category1"};

            var serialized = category.Serialize();

            Assert.That(serialized.Name.LocalName, Is.EqualTo("Category"));
            Assert.That(serialized.Attributes().Count(), Is.EqualTo(2));
            Assert.That(serialized.Attribute("Id").Value, Is.EqualTo(category.PersistentId));
            Assert.That(serialized.Attribute("Name").Value, Is.EqualTo(nameisNull ? "" : category.Name));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Deserialize(bool withCategory)
        {
            const string persistentId = "TestId";
            const string name = "Category1";

            var element = new XElement("Category",
                new XAttribute("Id", persistentId),
                new XAttribute("Name", name));

            var categoryImp = new CategoryEntityImp(element);

            Assert.That(categoryImp.PersistentId, Is.EqualTo(persistentId));
            Assert.That(categoryImp.Name, Is.EqualTo(name));
        }
    }
}
