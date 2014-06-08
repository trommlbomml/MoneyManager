
using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManager.Interfaces;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class RespositoryTests
    {
        RepositoryImp Repository { get; set; }

        [SetUp]
        public void Setup()
        {
            Repository = (RepositoryImp)RepositoryFactory.CreateRepository();
        }

        [TearDown]
        public void TearDown()
        {
            Repository.ClearAll();
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
    }
}
