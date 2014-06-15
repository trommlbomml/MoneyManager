using System;
using System.Diagnostics;
using System.Linq;
using MoneyManager.Interfaces;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class RepositoryPerformanceTests
    {
        [Explicit]
        [Test]
        public void Save()
        {
            const int usageInYears = 50;
            const int requestserMonth = 200;

            var repository = (RepositoryImp)RepositoryFactory.CreateRepository();

            foreach (var requestData in Enumerable.Range(1,usageInYears * 12 * requestserMonth).Select(i => new RequestEntityData()))
            {
                repository.CreateRequest(requestData);
            }

            var stopwatch = Stopwatch.StartNew();

            repository.Save();

            var elapsedTime = stopwatch.Elapsed;

            Trace.TraceInformation(stopwatch.Elapsed.TotalSeconds.ToString());
            Assert.That(elapsedTime, Is.LessThan(TimeSpan.FromSeconds(1)), "Performance for Save to low. ");
        }
    }
}
