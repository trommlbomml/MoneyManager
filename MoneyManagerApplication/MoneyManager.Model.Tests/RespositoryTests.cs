
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using MoneyManager.Interfaces;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class RespositoryTests
    {
        RepositoryImp Repository { get; set; }

        private string _databaseFile;

        public static string GetUniqueFilePath()
        {
            return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        }

        [SetUp]
        public void Setup()
        {
            Repository = (RepositoryImp)RepositoryFactory.CreateRepository();
            _databaseFile = GetUniqueFilePath() + SystemConstants.DatabaseExtension;
            Repository.Create(_databaseFile, "DefaultName");
        }

        [TearDown]
        public void TearDown()
        {
            if (Repository.IsOpen) Repository.Close();
        }

        private void CreateRequestsInRepository(DateTime[] monthsToCreate, int[] requestsPerMonth)
        {
            for (var i = 0; i < monthsToCreate.Length; i++)
            {
                for (var j = 0; j < requestsPerMonth[i]; j++)
                {
                    Repository.CreateRequest(new RequestEntityData {Date = monthsToCreate[i]});
                }
            }
        }

        protected IEnumerable<TestCaseData> QueryRequestsForSingleMonthTestCases()
        {
            yield return new TestCaseData(new [] { new DateTime(2014, 6, 1) }, 
                                          new[] {10}, 
                                          2014, 6, 
                                          10).SetName("Query all Entites of single month");

            yield return new TestCaseData(new[] { new DateTime(2014, 6, 1), new DateTime(2014, 8, 1) },
                                          new[] { 10, 6 },
                                          2014, 8,
                                          6).SetName("Query some Entites of single month");

            yield return new TestCaseData(new[] { new DateTime(2014, 6, 1), new DateTime(2013, 6, 1) },
                                          new[] { 7, 6 },
                                          2014, 6,
                                          7).SetName("Query some Entites of different years of single month");
        }

        [Test]
        public void OpenWhenRepositoryAlreadyOpenedThrowsException()
        {
            Assume.That(Repository.IsOpen, "Repository must be opened to test.");

            Assert.That(() => Repository.Open("C:\test.mmbd"), Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void CreateWhenRepositoryAlreadyOpenedThrowsException()
        {
            Assume.That(Repository.IsOpen, "Repository must be opened to test.");

            Assert.That(() => Repository.Create("C:\test.mmbd", "DefaultName"), Throws.InstanceOf<ApplicationException>());
        }

        [TestCaseSource("QueryRequestsForSingleMonthTestCases")]
        public void QueryRequestsForSingleMonth(DateTime[] monthsToCreate, int [] requestsPerMonth, int year, int month, int expectedQueriedRequestCount)
        {
            Assume.That(monthsToCreate.Length, Is.EqualTo(requestsPerMonth.Length), "Testdata Corrupted. Field length must be equal.");

            CreateRequestsInRepository(monthsToCreate, requestsPerMonth);

            var requests = Repository.QueryRequestsForSingleMonth(year, month).ToArray();
            Assert.That(requests.Length, Is.EqualTo(expectedQueriedRequestCount));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void QueryRequestsForMonthWithNoEntries(bool createRequestsInDifferentMonth)
        {
            if (createRequestsInDifferentMonth) CreateRequestsInRepository(new[] { new DateTime(2014, 5, 1) }, new[] { 3 });

            var requests = Repository.QueryRequestsForSingleMonth(2014, 6);

            Assert.That(requests, Is.Not.Null);
            Assert.That(requests, Is.Empty);
        }

        private RequestEntityImp CreateRequestInRepository(DateTime date, string description, double value)
        {
            var request = new RequestEntityImp
            {
                Date = date,
                Description = description,
                Value = value
            };
            Repository.AddRequest(request);

            return request;
        }

        [Test]
        public void QuerySingleRequest()
        {
            var originalRequest = CreateRequestInRepository(DateTime.Now.Date, "Description", 12.75);
            var persistentId = originalRequest.PersistentId;

            var queriedRequest = Repository.QueryRequest(persistentId);
            Assert.That(queriedRequest.PersistentId, Is.EqualTo(persistentId));
            Assert.That(queriedRequest.Description, Is.EqualTo(originalRequest.Description));
            Assert.That(queriedRequest.Value, Is.EqualTo(originalRequest.Value));
        }

        [Test]
        public void UpdateRequest()
        {
            var originalRequest = CreateRequestInRepository(DateTime.Now.Date, "Testdescription", 12.75);
            var persistentId = originalRequest.PersistentId;

            var newData = new RequestEntityData
            {
                Date = new DateTime(2013, 7, 6),
                Description = "New Description",
                Value = 11.11
            };
            Repository.UpdateRequest(persistentId, newData);

            var queriedRequest = Repository.QueryRequest(persistentId);

            Assert.That(queriedRequest.Description, Is.EqualTo("New Description"));
            Assert.That(queriedRequest.Value, Is.EqualTo(11.11));
        }

        [Test]
        public void UpdateRequestOfNonExistingEntityThrowsException()
        {
            Assert.That(() => Repository.UpdateRequest("InvalidId", new RequestEntityData()), Throws.ArgumentException);
        }

        [Test]
        public void QueryRequestOfNonExistingEntityThrowsException()
        {
            Assert.That(() => Repository.QueryRequest("InvalidId"), Throws.ArgumentException);
        }

        [Test]
        public void CreateRequest()
        {
            CreateRequestsInRepository(new [] { new DateTime(2014, 1, 1) }, new [] {10});

            var requestData = new RequestEntityData
            {
                Date = new DateTime(2014, 6, 1),
                Description = "My Description",
                Value = -11
            };

            var persistentId = Repository.CreateRequest(requestData);

            var createdRequest = Repository.QueryRequest(persistentId);

            Assert.That(createdRequest.PersistentId, Is.EqualTo(persistentId));
            Assert.That(createdRequest.Description, Is.EqualTo(requestData.Description));
            Assert.That(createdRequest.Value, Is.EqualTo(requestData.Value));
        }

        [Test]
        public void DeleteRequest()
        {
            CreateRequestsInRepository(new[] { new DateTime(2014, 1, 1) }, new[] { 10 });

            var allRequestIdsBeforeDelete = Repository.QueryRequestsForSingleMonth(2014, 1).Select(r => r.PersistentId).ToArray();
            var requestPersistentIdToDelete = allRequestIdsBeforeDelete.First();

            Repository.DeleteRequest(requestPersistentIdToDelete);

            var persistentIdsAfterDelete = Repository.QueryRequestsForSingleMonth(2014, 1).Select(r => r.PersistentId).ToArray();

            Assert.That(persistentIdsAfterDelete.Length, Is.EqualTo(9));
            CollectionAssert.AreEquivalent(allRequestIdsBeforeDelete.Except(new[]{requestPersistentIdToDelete}), persistentIdsAfterDelete);
        }

        [Test]
        public void SaveNotOpenedDatabaseThrowsExceptio()
        {
            var repository = RepositoryFactory.CreateRepository();
            Assert.That(repository.Save, Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void Save()
        {
            CreateRequestsInRepository(new[] { new DateTime(2014, 1, 1) }, new[] { 10 });

            Repository.Save();
            RepositoryStateTests.AssertFileContentIsCorrect(_databaseFile, Repository.Name, Repository.AllRequests);
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
        }

        [Test]
        public void Open()
        {
            CreateRequestsInRepository(new[] { new DateTime(2014, 1, 1) }, new[] { 10 });
            Repository.Save();
            Repository.Close();

            Repository.Open(_databaseFile);
            AssertRepositoryEqualsFileContent(_databaseFile);
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
