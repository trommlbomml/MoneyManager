using System;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.RequestManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.RequestManagement
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
            Assert.That(screenModel.Months.SelectableValues.Select(m => m.Index).ToArray(), Is.EquivalentTo(Enumerable.Range(1,12).ToArray()));

            Assert.That(screenModel.Saldo, Is.EqualTo(expectedSaldo));
            Assert.That(screenModel.SaldoAsString, Is.EqualTo(string.Format(Properties.Resources.MoneyValueFormat, expectedSaldo)));

            Assert.That(screenModel.Caption, Is.EqualTo(string.Format(Properties.Resources.RequestManagementPageCaptionFormat, Repository.Name)));

            Repository.Received(1).CalculateSaldoForMonth(2014, 3);
            Repository.Received(1).QueryRequestsForSingleMonth(2014, 3);
        }

        [TestCase(2014, 3, 2014, 4)]
        [TestCase(2014, 12, 2015, 1)]
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
        [TestCase(2014, 1, 2013, 12)]
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

        [Test]
        public void SaveCommand()
        {
            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);

            Repository.ClearReceivedCalls();

            screenModel.SaveCommand.Execute(null);
            Repository.Received(1).Save();
            ApplicationSettings.Received(1).UpdateRecentAccountInformation(Repository.FilePath, Arg.Any<DateTime>());
        }

        [Test]
        public void AddRequestCommandShowsDialog()
        {
            object dialogViewModel = null;
            WindowManager.When(w => w.ShowDialog(Arg.Any<object>())).Do(c => dialogViewModel = c[0]);

            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);
            screenModel.AddRequestCommand.Execute(null);

            WindowManager.Received(1).ShowDialog(Arg.Any<object>());
            Assert.That(dialogViewModel, Is.InstanceOf<RequestDialogViewModel>());

            var requestDialogViewModel = (RequestDialogViewModel) dialogViewModel;

            Assert.That(requestDialogViewModel.Caption, Is.EqualTo("Neue Transaktion"));
            Assert.That(requestDialogViewModel.CreateRequestCommand.IsEnabled, Is.True);
            Assert.That(requestDialogViewModel.Date, Is.EqualTo(new DateTime(2014, 6, 1)));
            Assert.That(requestDialogViewModel.FirstPossibleDate, Is.EqualTo(new DateTime(2014, 6, 1)));
            Assert.That(requestDialogViewModel.LastPossibleDate, Is.EqualTo(new DateTime(2014, 6, 30)));
            Assert.That(requestDialogViewModel.Value, Is.EqualTo(0.0d));
        }

        [Test]
        public void AddRequestCommandWithCreateRequestExecuted()
        {
            var expectedDate = new DateTime(2014, 6, 14);
            const string entityIdOfRequest = "NewEntityId";
            const double expectedValue = 11.2;
            const string expectedDescription = "TestDescription";

            object dialogViewModel = null;
            WindowManager.When(w => w.ShowDialog(Arg.Any<object>())).Do(c => dialogViewModel = c[0]);

            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);
            var requestsBeforeUpdate = screenModel.Requests.SelectableValues.Count;
            screenModel.AddRequestCommand.Execute(null);

            var requestDialogViewModel = (RequestDialogViewModel)dialogViewModel;
            requestDialogViewModel.Description = expectedDescription;
            requestDialogViewModel.Value = expectedValue;
            requestDialogViewModel.Date = expectedDate;

            Repository.CalculateSaldoForMonth(2014, 6).Returns(99.99);
            Repository.CreateRequest(Arg.Any<RequestEntityData>()).Returns(entityIdOfRequest);
            Repository.QueryRequest(entityIdOfRequest).Returns(c =>
            {
                var entity = Substitute.For<RequestEntity>();
                entity.PersistentId.Returns(entityIdOfRequest);
                entity.Description.Returns(expectedDescription);
                entity.Value.Returns(expectedValue);
                entity.Date.Returns(expectedDate);
                return entity;
            });
            Repository.ClearReceivedCalls();

            requestDialogViewModel.CreateRequestCommand.Execute(null);

            var requestViewModel = screenModel.Requests.SelectableValues.SingleOrDefault(r => r.EntityId == entityIdOfRequest);

// ReSharper disable ImplicitlyCapturedClosure
            Repository.Received(1).CreateRequest(Arg.Is<RequestEntityData>(r => r.Date == expectedDate
                                                                             && Math.Abs(r.Value - expectedValue) < double.Epsilon
                                                                             && r.Description == expectedDescription));
// ReSharper restore ImplicitlyCapturedClosure

            Repository.Received(1).QueryRequest(entityIdOfRequest);
            Repository.Received(1).CalculateSaldoForMonth(2014, 6);
            Assert.That(screenModel.Requests.SelectableValues.Count, Is.EqualTo(requestsBeforeUpdate + 1));
            Assert.That(requestViewModel, Is.Not.Null);
            Assert.That(screenModel.Saldo, Is.EqualTo(99.99));
// ReSharper disable PossibleNullReferenceException
            Assert.That(requestViewModel.Description, Is.EqualTo(expectedDescription));
            Assert.That(requestViewModel.Date, Is.EqualTo(expectedDate));
            Assert.That(requestViewModel.Value, Is.EqualTo(expectedValue));
// ReSharper restore PossibleNullReferenceException
        }

        private void DefineRequestsForMonth(int year, int month, int requestCount)
        {
            var list = Enumerable.Range(1, requestCount).Select(i =>
            {
                var entity = Substitute.For<RequestEntity>();
                entity.PersistentId.Returns(string.Format("Entity={0}", i));
                entity.Date.Returns(new DateTime(year, month, 5));
                entity.Description.Returns("Description");
                entity.Value.Returns(i*2.56);

                return entity;
            }).ToList();

            Repository.QueryRequestsForSingleMonth(Arg.Any<int>(), Arg.Any<int>()).Returns(list);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DeleteRequest(bool confirmDelete)
        {
            DefineRequestsForMonth(2014, 6, 3);
            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);

            Application.WindowManager
                       .When(w => w.ShowQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Action>()))
                       .Do(c => { if (confirmDelete) ((Action)c[2]).Invoke(); });

            screenModel.Requests.SelectedValue = screenModel.Requests.SelectableValues.First();
            var persistentId = screenModel.Requests.SelectedValue.EntityId;
            
            Repository.ClearReceivedCalls();
            DefineRequestsForMonth(2014, 6, 2);

            screenModel.DeleteRequestCommand.Execute(null);

            if (confirmDelete)
            {
                Repository.Received(1).QueryRequestsForSingleMonth(2014, 6);
                Repository.Received(1).CalculateSaldoForMonth(2014, 6);
                Repository.Received(1).DeleteRequest(persistentId);
                Assert.That(screenModel.Requests.SelectableValues.Count, Is.EqualTo(2));
                Assert.That(screenModel.Requests.SelectedValue, Is.Null);    
            }
            else
            {
                Repository.DidNotReceiveWithAnyArgs().QueryRequestsForSingleMonth(Arg.Any<int>(), Arg.Any<int>());
                Repository.DidNotReceiveWithAnyArgs().CalculateSaldoForMonth(Arg.Any<int>(), Arg.Any<int>());
                Repository.DidNotReceiveWithAnyArgs().DeleteRequest(Arg.Any<string>());
                Assert.That(screenModel.Requests.SelectableValues.Count, Is.EqualTo(3));
                Assert.That(screenModel.Requests.SelectedValue, Is.EqualTo(screenModel.Requests.SelectableValues.First()));    
            }
        }

        [Test]
        public void EditRequestCommandShowsDialog()
        {
            DefineRequestsForMonth(2014, 6, 1);
            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);

            object dialogViewModel = null;
            Application.WindowManager
                       .When(w => w.ShowDialog(Arg.Any<object>()))
                       .Do(c => { dialogViewModel = c[0]; });

            screenModel.Requests.SelectedValue = screenModel.Requests.SelectableValues.First();
            screenModel.EditRequestCommand.Execute(null);

            WindowManager.Received(1).ShowDialog(Arg.Any<object>());
            Assert.That(dialogViewModel, Is.InstanceOf<RequestDialogViewModel>());

            var requestDialogViewModel = (RequestDialogViewModel)dialogViewModel;

            Assert.That(requestDialogViewModel.Caption, Is.EqualTo("Transaktion bearbeiten"));
            Assert.That(requestDialogViewModel.CreateRequestCommand.IsEnabled, Is.True);
        }

        [Test]
        public void EditRequest()
        {
            DefineRequestsForMonth(2014, 6, 1);
            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);
            var currentRequestEntityId = screenModel.Requests.SelectableValues.First().EntityId;

            object dialogViewModel = null;
            Application.WindowManager
                       .When(w => w.ShowDialog(Arg.Any<object>()))
                       .Do(c => { dialogViewModel = c[0]; });

            screenModel.Requests.SelectedValue = screenModel.Requests.SelectableValues.First();
            screenModel.EditRequestCommand.Execute(null);

            Assert.That(dialogViewModel, Is.InstanceOf<RequestDialogViewModel>());

            var dialog = (RequestDialogViewModel) dialogViewModel;

            var newDate = dialog.Date.AddDays(2);
            dialog.Date = newDate;
            dialog.Description = "New Description added";
            dialog.Value = 77.77;

            Application.Repository.ClearReceivedCalls();
            dialog.CreateRequestCommand.Execute(null);

            Application.Repository.Received(1).UpdateRequest(currentRequestEntityId, 
                Arg.Is<RequestEntityData>(r => r.Description == "New Description added" && Math.Abs(r.Value - 77.77) < double.Epsilon && dialog.Date == newDate));

            Application.Repository.Received(1).CalculateSaldoForMonth(2014, 6);
        }
    }
}