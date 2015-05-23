using System;
using System.Linq;
using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    internal class RepositoryStandingOrderTests : RepositoryTestsBase
    {
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
            
            Repository.UpdateStandingOrdersToCurrentMonth();

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
        }
    }
}
