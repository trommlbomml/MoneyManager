﻿using System.Linq;
using System.Xml.Linq;
using MoneyManager.Model.Entities;
using NUnit.Framework;

namespace MoneyManager.Model.Tests.Entities
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
            Assert.That(request.HasChanged, Is.True);
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
            Assert.That(categoryImp.HasChanged, Is.False);
        }

        [Test]
        public void ChangePropertyUpdatesHasChanged()
        {
            var element = new XElement("Category",
                new XAttribute("Id", "testId"),
                new XAttribute("Name", "Name"));

            var categoryImp = new CategoryEntityImp(element);
            Assert.That(categoryImp.HasChanged, Is.False);

            categoryImp.Name = "My New Name";
            Assert.That(categoryImp.HasChanged, Is.True);
        }
    }
}
