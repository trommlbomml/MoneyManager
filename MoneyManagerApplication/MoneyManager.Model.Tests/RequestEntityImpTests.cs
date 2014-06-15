﻿using System;
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
            Assert.That(request.Date, Is.EqualTo(DateTime.Now.Date));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Serialize(bool descriptionIsNull)
        {
            var request = new RequestEntityImp
            {
                Date = new DateTime(2014, 5, 6),
                Description = descriptionIsNull ? null : "TestDescription",
                Value = 11.27
            };

            var serialized = request.Serialize();

            Assert.That(serialized.Name.LocalName, Is.EqualTo("Request"));
            Assert.That(serialized.Attributes().Count(), Is.EqualTo(4));
            Assert.That(serialized.Attribute("Id").Value, Is.EqualTo(request.PersistentId));
            Assert.That(serialized.Attribute("Description").Value, Is.EqualTo(descriptionIsNull ? "" : request.Description));
            Assert.That(serialized.Attribute("Value").Value, Is.EqualTo(request.Value.ToString(CultureInfo.InvariantCulture)));
            Assert.That(serialized.Attribute("Date").Value, Is.EqualTo(request.Date.ToString(CultureInfo.InvariantCulture)));
        }

        [Test]
        public void Deserialize()
        {
            const string persistentId = "TestId";
            var dateTime = new DateTime(2014, 7, 4);
            const double value = 12.77;
            const string description = "TestDescription";
            
            var element = new XElement("Request",
                new XAttribute("Id", persistentId),
                new XAttribute("Date", dateTime.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("Description", description),
                new XAttribute("Value", value.ToString(CultureInfo.InvariantCulture)));

            var request = new RequestEntityImp(element);

            Assert.That(request.PersistentId, Is.EqualTo(persistentId));
            Assert.That(request.Date, Is.EqualTo(dateTime));
            Assert.That(request.Description, Is.EqualTo(description));
            Assert.That(request.Value, Is.EqualTo(value));
        }
    }
}
