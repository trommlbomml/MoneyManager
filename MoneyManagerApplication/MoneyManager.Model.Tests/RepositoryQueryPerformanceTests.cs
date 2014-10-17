﻿using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    internal class RepositoryQueryPerformanceTests : RepositoryTestsBase
    {
        private void CreateTestData()
        {
            var random = new Random();

            const int years = 10;
            const int categoriesCount = 50;
            const int regularyRequests = 100;
            const int requestsPerMonth = 100;
            var startDateTime = new DateTime(2001, 1, 1);


            var allCategories = Enumerable.Range(1, categoriesCount).Select(i => new CategoryEntityImp { Name = string.Format("Category {0}", i) }).ToArray();

            var allRegularyRequests = Enumerable.Range(1, regularyRequests).Select(i => new RegularyRequestEntityImp
            {
                Category = allCategories.ElementAt(random.Next(0, categoriesCount)),
                FirstBookDate = startDateTime,
                Value = 55,
                MonthPeriodStep = random.Next(1, 13),
                ReferenceDay = random.Next(1, 28),
                ReferenceMonth = random.Next(1, 13)
            }).ToArray();

            var allRequests = Enumerable.Range(1, years).SelectMany(y => Enumerable.Range(1, 12)
                                                        .SelectMany(m => Enumerable.Range(1, requestsPerMonth).Select(r => new RequestEntityImp
                                                        {
                                                            Date = new DateTime(2000 + y, m, random.Next(1, 28)), 
                                                            Value = random.NextDouble() * 200
                                                        }))).ToArray();

            foreach (var categoryEntityImp in allCategories) Repository.AddCategory(categoryEntityImp);
            foreach (var regularRequest in allRegularyRequests) Repository.AddRegularyRequest(regularRequest);
            foreach (var request in allRequests) Repository.AddRequest(request);
        }

        [Test]
        public void QueryForCurrentMonthWithLotOfDataIn10Years()
        {
            CreateTestData();
            var sw = Stopwatch.StartNew();
            var elements = Repository.QueryRequestsForSingleMonth(2010, 12).ToArray();
            var elapsedMilliseconds = sw.Elapsed.TotalMilliseconds;
            Trace.TraceInformation("Elapsed Time to query: {0} milliseconds", elapsedMilliseconds);
            Assert.That(elapsedMilliseconds, Is.LessThanOrEqualTo(50), "Performance to slow.");
        }

        [Test]
        public void UpdateSaldoforCurrentMonthWithLotOfDataIn10Years()
        {
            CreateTestData();
            var sw = Stopwatch.StartNew();
            var elements = Repository.CalculateSaldoForMonth(2010, 12);
            var elapsedMilliseconds = sw.Elapsed.TotalMilliseconds;
            Trace.TraceInformation("Elapsed Time to query: {0} milliseconds", elapsedMilliseconds);
            Assert.That(elapsedMilliseconds, Is.LessThanOrEqualTo(50), "Performance to slow.");
        }
    }
}