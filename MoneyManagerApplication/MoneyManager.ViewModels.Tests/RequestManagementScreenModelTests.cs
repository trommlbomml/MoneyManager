
using System.Linq;
using MoneyManager.ViewModels.RequestManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    [TestFixture]
    public class RequestManagementScreenModelTests : ViewModelTestsBase
    {
        [TestCase(0)]
        [TestCase(12)]
        public void InitalState(double expectedSaldo)
        {
            Repository.CalculateSaldoForMonth(2014, 3).Returns(expectedSaldo);

            var screenModel = new RequestManagementPageViewModel(Application, 2014, 3);

            Assert.That(screenModel.AddRequestCommand.IsEnabled, Is.True);
            Assert.That(screenModel.DeleteRequestCommand.IsEnabled, Is.False);
            Assert.That(screenModel.NextMonthCommand.IsEnabled, Is.True);
            Assert.That(screenModel.PreviousMonthCommand.IsEnabled, Is.True);
            Assert.That(screenModel.SaveCommand.IsEnabled, Is.True);

            Assert.That(screenModel.Month, Is.EqualTo(3));
            Assert.That(screenModel.Year, Is.EqualTo(2014));

            Assert.That(screenModel.Months.SelectableValues.Count, Is.EqualTo(12));
            Assert.That(screenModel.Months.SelectedValue, Is.EqualTo(screenModel.Months.SelectableValues.Single(s => s.Index == 3)));

            Assert.That(screenModel.Saldo, Is.EqualTo(expectedSaldo));
            Assert.That(screenModel.SaldoAsString, Is.EqualTo(string.Format(Properties.Resources.MoneyValueFormat, expectedSaldo)));

            Assert.That(screenModel.Caption, Is.EqualTo(Properties.Resources.RequestManagementPageCaption));

            Repository.Received(1).CalculateSaldoForMonth(2014, 3);
            Repository.Received(1).QueryRequestsForSingleMonth(2014, 3);
        }

        [TestCase(2014, 3, 2014, 4)]
        [TestCase(2014, 11, 2015, 0)]
        public void NextMonthCommandCallsRepositoryAndUpdatesProperties(int currentYear, int currentMonth, int nextYear, int nextMonth)
        {
            var screenModel = new RequestManagementPageViewModel(Application, currentYear, currentMonth);

            Repository.ClearReceivedCalls();

            screenModel.NextMonthCommand.Execute(null);
            Repository.Received(1).CalculateSaldoForMonth(nextYear, nextMonth);
            Repository.Received(1).QueryRequestsForSingleMonth(nextYear, nextMonth);
            Assert.That(screenModel.Month, Is.EqualTo(nextMonth));
            Assert.That(screenModel.Year, Is.EqualTo(nextYear));
            Assert.That(screenModel.Months.SelectedValue, Is.EqualTo(screenModel.Months.SelectableValues.Single(s => s.Index == nextMonth)));
        }

        [TestCase(2014, 4, 2014, 3 )]
        [TestCase(2014, 0, 2013, 11)]
        public void PreviousMonthCommandCallsRepositoryAndUpdatesProperties(int currentYear, int currentMonth, int nextYear, int nextMonth)
        {
            var screenModel = new RequestManagementPageViewModel(Application, currentYear, currentMonth);

            Repository.ClearReceivedCalls();

            screenModel.PreviousMonthCommand.Execute(null);
            Repository.Received(1).CalculateSaldoForMonth(nextYear, nextMonth);
            Repository.Received(1).QueryRequestsForSingleMonth(nextYear, nextMonth);
            Assert.That(screenModel.Month, Is.EqualTo(nextMonth));
            Assert.That(screenModel.Year, Is.EqualTo(nextYear));
            Assert.That(screenModel.Months.SelectedValue, Is.EqualTo(screenModel.Months.SelectableValues.Single(s => s.Index == nextMonth)));
        }
    }
}
