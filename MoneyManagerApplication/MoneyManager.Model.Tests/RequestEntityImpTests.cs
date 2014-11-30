using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class RequestEntityImpTests
    {
        [Test]
        public void InitialState()
        {
            var request = new RequestEntityImp();
            Assert.That(request.PersistentId, Is.Not.Null.Or.Empty);
            Assert.That(request.Description, Is.EqualTo(""));
            Assert.That(request.Category, Is.Null);
        }

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

            var serialized = request.Serialize();

            Assert.That(serialized.Name.LocalName, Is.EqualTo("Request"));
            Assert.That(serialized.Attributes().Count(), Is.EqualTo(5));
            Assert.That(serialized.Attribute("Id").Value, Is.EqualTo(request.PersistentId));
            Assert.That(serialized.Attribute("Description").Value, Is.EqualTo(descriptionIsNull ? "" : request.Description));
            Assert.That(serialized.Attribute("Value").Value, Is.EqualTo(request.Value.ToString(CultureInfo.InvariantCulture)));
            Assert.That(serialized.Attribute("Date").Value, Is.EqualTo(request.Date.ToString(CultureInfo.InvariantCulture)));
            Assert.That(serialized.Attribute("CategoryId").Value, Is.EqualTo(category.PersistentId));
        }

        [Test]
        public void Deserialize([Values(true, false)]bool withCategory, [Values(true, false)]bool withStandingOrder)
        {
            const string persistentId = "TestId";
            var dateTime = new DateTime(2014, 7, 4);
            const double value = 12.77;
            const string description = "TestDescription";

            var categories = Enumerable.Range(1, 3).Select(i => new CategoryEntityImp()).ToArray();
            var categoryId = withCategory ? categories.First().PersistentId : "";

            var element = new XElement("Request",
                new XAttribute("Id", persistentId),
                new XAttribute("Date", dateTime.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Description", description),
                new XAttribute("Value", value.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("CategoryId", categoryId));

            var request = new RequestEntityImp(element, categories);

            Assert.That(request.PersistentId, Is.EqualTo(persistentId));
            Assert.That(request.Date, Is.EqualTo(dateTime));
            Assert.That(request.Description, Is.EqualTo(description));
            Assert.That(request.Value, Is.EqualTo(value));

            var categoryToReference = withCategory ? categories.First() : null;

            Assert.That(request.Category, Is.EqualTo(categoryToReference));
        }
    }
}
