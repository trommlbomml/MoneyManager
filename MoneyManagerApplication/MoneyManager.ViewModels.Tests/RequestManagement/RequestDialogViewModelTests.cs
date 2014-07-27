
using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManager.ViewModels.RequestManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.RequestManagement
{
    [TestFixture]
    public class RequestDialogViewModelTests : ViewModelTestsBase
    {
        protected IEnumerable<TestCaseData> InitialStateTestCases()
        {
            return Enumerable.Range(1, 12).Select(i => new TestCaseData(2014, i));
        }
        
        [TestCaseSource("InitialStateTestCases")]
        public void InitialState(int year, int month)
        {
            var requestDialog = new RequestDialogViewModel(Application, year, month, d => { });
            Assert.That(requestDialog.FirstPossibleDate, Is.EqualTo(new DateTime(year, month, 1)));
            Assert.That(requestDialog.LastPossibleDate, Is.EqualTo(new DateTime(year, month, DateTime.DaysInMonth(year, month))));
            Assert.That(requestDialog.Date, Is.EqualTo(new DateTime(year, month, 1)));
            Assert.That(requestDialog.CreateRequestCommand.IsEnabled, Is.True);
            Assert.That(requestDialog.DateAsString, Is.EqualTo(string.Format(Properties.Resources.RequestDayOfMonthFormat, new DateTime(year, month, 1))));
            Assert.That(requestDialog.Value, Is.EqualTo(0.0d));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(13)]
        [TestCase(14)]
        public void InvalidMonthThrowsException(int month)
        {
            Assert.That(() => new RequestDialogViewModel(Application, 2014, month, d => { }), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void CreateRequestCommandCallsAction()
        {
            var action = Substitute.For<Action<RequestDialogViewModel>>();
            var requestDialog = new RequestDialogViewModel(Application, 2014, 6, action);
            
            requestDialog.CreateRequestCommand.Execute(null);
            action.Received(1).Invoke(requestDialog);
        }

        [Test]
        public void UpdateDateAsString()
        {
            var requestDialog = new RequestDialogViewModel(Application, 2014, 6, o => { });

            requestDialog.Date = requestDialog.Date + TimeSpan.FromDays(1);
            Assert.That(requestDialog.DateAsString, Is.EqualTo(string.Format(Properties.Resources.RequestDayOfMonthFormat, new DateTime(2014, 6, 2))));
        }
    }
}
