
using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using MoneyManager.Interfaces;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    internal class RespositoryTests : RepositoryTestsBase
    {
        [Test]
        public void OpenWhenRepositoryAlreadyOpenedThrowsException()
        {
            Assume.That(Repository.IsOpen, "Repository must be opened to test.");

            Assert.That(() => Repository.Open("C:\test.mmbd"), Throws.InstanceOf<ApplicationException>());
            Assert.That(Repository.FilePath, Is.EqualTo(DatabaseFile));
        }

        [Test]
        public void CreateWhenRepositoryAlreadyOpenedThrowsException()
        {
            Assume.That(Repository.IsOpen, "Repository must be opened to test.");

            Assert.That(() => Repository.Create("C:\test.mmbd", "DefaultName"), Throws.InstanceOf<ApplicationException>());
            Assert.That(Repository.FilePath, Is.EqualTo(DatabaseFile));
        }

        [TestCase(2012, 8, 0.0)]
        [TestCase(2012, 9, 50.55)]
        [TestCase(2013, 5, 50.55)]
        [TestCase(2013, 6, 50.55 + 12.5)]
        [TestCase(2013, 11, 50.55 + 12.5)]
        [TestCase(2013, 12, 50.55 + 12.5 + 11.2)]
        [TestCase(2014, 1, 50.55 + 12.5 + 11.2)]
        [TestCase(2014, 2, 50.55 + 12.5 + 11.2 + 7.77)]
        public void CalculateSaldoForMonth(int year, int month, double expectedSaldo)
        {
            Repository.CreateRequest(new RequestEntityData { Date = new DateTime(2012, 9, 6), Value = 50.55 });
            Repository.CreateRequest(new RequestEntityData {Date = new DateTime(2013, 6, 1), Value = 12.5});
            Repository.CreateRequest(new RequestEntityData { Date = new DateTime(2013, 12, 15), Value = 11.20 });
            Repository.CreateRequest(new RequestEntityData { Date = new DateTime(2014, 2, 1), Value = 7.77 });

            var saldo = Repository.CalculateSaldoForMonth(year, month);
            Assert.That(saldo, Is.EqualTo(expectedSaldo));
        }

        [Test]
        public void SaveNotOpenedDatabaseThrowsExceptio()
        {
            var repository = RepositoryFactory.CreateRepository(Context);
            Assert.That(repository.Save, Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void Save()
        {
            CreateRequestsInRepository(new[] { new DateTime(2014, 1, 1) }, new[] { 10 });

            Repository.Save();
            RepositoryStateTests.AssertFileContentIsCorrect(DatabaseFile, Repository.Name, Repository.AllRequests);
            Assert.That(Repository.FilePath, Is.EqualTo(DatabaseFile));
        }

        [Test]
        public void Close()
        {
            CreateRequestsInRepository(new[] { new DateTime(2014, 1, 1) }, new[] { 10 });
            Repository.Save();
            Repository.Close();

            Assert.That(Repository.AllRequests, Is.Empty);
            Assert.That(Repository.Name, Is.Null.Or.Empty);
            Assert.That(Repository.IsOpen, Is.False);
            Assert.That(Repository.FilePath, Is.Null.Or.Empty);
        }

        [Test]
        public void Open()
        {
            CreateRequestsInRepository(new[] { new DateTime(2014, 1, 1) }, new[] { 10 });
            Repository.Save();
            Repository.Close();

            Repository.Open(DatabaseFile);
            AssertRepositoryEqualsFileContent(DatabaseFile);
            Assert.That(Repository.FilePath, Is.EqualTo(DatabaseFile));
        }

        private void AssertRepositoryEqualsFileContent(string databaseFile)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(databaseFile);

// ReSharper disable PossibleNullReferenceException
            var name = xmlDocument.DocumentElement.GetAttribute("Name");
            Assert.That(Repository.Name, Is.EqualTo(name));

            var requestsElement = xmlDocument.DocumentElement.SelectSingleNode("Requests");
            Assert.That(Repository.AllRequests.Count, Is.EqualTo(requestsElement.ChildNodes.Count));
            
            foreach (XmlElement element in requestsElement.ChildNodes)
            {
                var id = element.GetAttribute("Id");
                var request = Repository.AllRequests.SingleOrDefault(r => r.PersistentId == id);
                Assert.That(request, Is.Not.Null);

                Assert.That(request.PersistentId, Is.EqualTo(id));
                Assert.That(request.Date, Is.EqualTo(DateTime.Parse(element.GetAttribute("Date"), CultureInfo.InvariantCulture)));
                Assert.That(request.Description, Is.EqualTo(element.GetAttribute("Description")));
                Assert.That(request.Value, Is.EqualTo(Double.Parse(element.GetAttribute("Value"), CultureInfo.InvariantCulture)));
            }
// ReSharper restore PossibleNullReferenceException
        }
    }
}
