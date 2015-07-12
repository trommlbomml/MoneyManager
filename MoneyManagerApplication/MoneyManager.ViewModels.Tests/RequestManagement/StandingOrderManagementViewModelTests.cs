using System;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.RequestManagement.Regulary;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.RequestManagement
{
    [TestFixture]
    public class StandingOrderManagementViewModelTests : ViewModelTestsBase
    {
        private StandingOrderEntity DefineStandingOrder(string entityId, int monthPeriodStep = 1, double value = 500d)
        {
            var standingOrder = Substitute.For<StandingOrderEntity>();
            standingOrder.MonthPeriodStep.Returns(monthPeriodStep);
            standingOrder.Value.Returns(value);
            standingOrder.PersistentId.Returns(entityId);
            standingOrder.Category.Returns(default(CategoryEntity));
            return standingOrder;
        }

        [Test]
        public void ShowFinishedPropertyAfterSaveNewStandingOrder()
        {
            Repository.QueryStandingOrder(Arg.Any<string>()).Returns(ci => DefineStandingOrder(ci.Arg<string>()));
            
            var standingOrderDialog = new StandingOrderManagementViewModel(Application, () => {});

            standingOrderDialog.CreateStandingOrderCommand.Execute(null);

            var details = standingOrderDialog.Details;
            details.ValueProperty.Value = 500.0;
            details.SaveCommand.Execute(null);

            standingOrderDialog.ShowFinishedProperty.Value = true;
            Assert.That(standingOrderDialog.StandingOrders.SelectableValues.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShowFinishedPropertyAfterDeleteStandingOrder()
        {
            var entity1 = DefineStandingOrder("Entity1");
            var entity2 = DefineStandingOrder("Entity2");
            Repository.QueryAllStandingOrderEntities().Returns(ci => new []
            {
                entity1, entity2
            });

            WindowManager.When(m => m.ShowQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Action>(), Arg.Any<Action>()))
                         .Do(ci => ((Action)ci.Args()[2]).Invoke());

            Repository.QueryStandingOrder("Entity1").Returns(entity1);
            Repository.QueryStandingOrder("Entity2").Returns(entity2);

            var standingOrderDialog = new StandingOrderManagementViewModel(Application, () => { });
            standingOrderDialog.StandingOrders.Value = standingOrderDialog.StandingOrders.SelectableValues.First();
            standingOrderDialog.DeleteStandingOrderCommand.Execute(null);

            Assert.That(standingOrderDialog.StandingOrders.SelectableValues.Count, Is.EqualTo(1));
            
            standingOrderDialog.ShowFinishedProperty.Value = true;
            Assert.That(standingOrderDialog.StandingOrders.SelectableValues.Count, Is.EqualTo(1));
        }
    }
}
