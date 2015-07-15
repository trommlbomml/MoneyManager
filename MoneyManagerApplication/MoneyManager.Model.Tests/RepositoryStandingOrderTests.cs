using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    internal class RepositoryStandingOrderTests : RepositoryTestsBase
    {
        protected static IEnumerable<TestCaseData> CreateStandingOrderSetsDataCorrectTestCases()
        {
            yield return new TestCaseData(new DateTime(2015, 4, 1), null, 1, DateTime.MaxValue);
            yield return new TestCaseData(new DateTime(2015, 4, 1), null, 2, DateTime.MaxValue);
            yield return new TestCaseData(new DateTime(2015, 4, 1), null, 6, DateTime.MaxValue);
            yield return new TestCaseData(new DateTime(2015, 4, 1), 1, 1, new DateTime(2015, 5, 1));
            yield return new TestCaseData(new DateTime(2015, 4, 1), 1, 2, new DateTime(2015, 6, 1));
            yield return new TestCaseData(new DateTime(2015, 4, 1), 1, 6, new DateTime(2015, 10, 1));
            yield return new TestCaseData(new DateTime(2015, 4, 1), 3, 1, new DateTime(2015, 7, 1));
            yield return new TestCaseData(new DateTime(2015, 4, 1), 3, 2, new DateTime(2015, 10, 1));
            yield return new TestCaseData(new DateTime(2015, 4, 1), 3, 6, new DateTime(2016, 10, 1));
        }

        [TestCaseSource("CreateStandingOrderSetsDataCorrectTestCases")]
        public void CreateStandingOrderSetsDataCorrect(DateTime firstBookDate, int? paymentCount, int monthStep, DateTime expectedLastBookDate)
        {
            var id = Repository.CreateStandingOrder(new StandingOrderEntityData
            {
                Description = "Description1",
                FirstBookDate = firstBookDate,
                MonthPeriodStep = monthStep,
                ReferenceDay = 6,
                ReferenceMonth = firstBookDate.Month,
                Value = 55.70,
                PaymentCount = paymentCount
            });

            var standingOrder = Repository.QueryStandingOrder(id);
            Assert.That(standingOrder.Description, Is.EqualTo("Description1"));
            Assert.That(standingOrder.FirstBookDate, Is.EqualTo(firstBookDate));
            Assert.That(standingOrder.ReferenceDay, Is.EqualTo(6));
            Assert.That(standingOrder.ReferenceMonth, Is.EqualTo(firstBookDate.Month));
            Assert.That(standingOrder.Value, Is.EqualTo(55.70));
            Assert.That(standingOrder.LastBookDate, Is.EqualTo(expectedLastBookDate));
        }

        [Test]
        public void UpdateStandingOrdersToCurrentMonth()
        {
            Context.Now.Returns(new DateTime(2015, 8, 6));
            var start = Context.Now.AddMonths(-6);
            Repository.CreateStandingOrder(new StandingOrderEntityData
            {
                Description = "Description1",
                FirstBookDate = start,
                MonthPeriodStep = 1,
                ReferenceDay = 6,
                ReferenceMonth = start.Month,
                Value = 55.70
            });
            PersistenceHandler.ClearReceivedCalls();
            
            var requestIds = Repository.UpdateStandingOrdersToCurrentMonth(2015, 8);

            var requests = Repository.AllRequests.OrderBy(r => r.Date).ToArray();
            Assert.That(requests.Length, Is.EqualTo(7));
            var monthIndex = 0;
            foreach (var request in requests)
            {
                Assert.That(request.Category, Is.Null);
                Assert.That(request.Description, Is.EqualTo("Description1"));
                Assert.That(request.Value, Is.EqualTo(55.70));
                Assert.That(request.Date, Is.EqualTo(new DateTime(2015, 2+monthIndex, 6)));
                monthIndex++;
            }

            PersistenceHandler.Received(1).SaveChanges(Arg.Is<SavingTask>(t => t.RequestsToUpdate.Count == 7 &&
                                                                               t.CategoriesToUpdate.Count == 0 &&
                                                                               t.EntitiesToDelete.Count == 0 &&
                                                                               t.StandingOrdersToUpdate.Count == 1 &&
                                                                               t.FilePath == DatabaseFile));

            Assert.That(requestIds.Length, Is.EqualTo(7));
            CollectionAssert.AreEquivalent(requestIds, requests.Select(r => r.PersistentId).ToArray());
        }
    }
}
