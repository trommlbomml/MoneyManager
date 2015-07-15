using System;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.AccountManagement;
using MoneyManager.ViewModels.RequestManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.RequestManagement
{
    [TestFixture]
    public class RequestManagementScreenModelTests : ViewModelTestsBase
    {
        [Test]
        public void InitalState([Values(0, 12)]double expectedSaldo, [Values(true, false)]bool currentMonth)
        {
            var currentDate = currentMonth ? ApplicationContext.Now : ApplicationContext.Now.AddMonths(-1);
            DefineRequestsForMonth(currentDate.Year, currentDate.Month, 3);
            Repository.CalculateSaldoForMonth(currentDate.Year, currentDate.Month).Returns(expectedSaldo);

            var screenModel = new RequestManagementPageViewModel(Application, currentDate.Year, currentDate.Month);

            Assert.That(screenModel.SwitchAccountCommand.IsEnabled, Is.True);
            Assert.That(screenModel.AddRequestCommand.IsEnabled, Is.True);
            Assert.That(screenModel.DeleteRequestCommand.IsEnabled, Is.False);
            Assert.That(screenModel.EditRequestCommand.IsEnabled, Is.False);
            Assert.That(screenModel.SwitchAccountCommand.IsEnabled, Is.True);
            Assert.That(screenModel.NextMonthCommand.IsEnabled, Is.EqualTo(!currentMonth));
            Assert.That(screenModel.PreviousMonthCommand.IsEnabled, Is.True);
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.EqualTo(!currentMonth));

            Assert.That(screenModel.Month, Is.EqualTo(currentDate.Month));
            Assert.That(screenModel.Year, Is.EqualTo(currentDate.Year));

            Assert.That(screenModel.Months.SelectableValues.Count, Is.EqualTo(12));
            Assert.That(screenModel.Months.Value, Is.EqualTo(screenModel.Months.SelectableValues.Single(s => s.Index == currentDate.Month)));
            Assert.That(screenModel.Months.SelectableValues.Select(m => m.Index).ToArray(), Is.EquivalentTo(Enumerable.Range(1,12).ToArray()));

            Assert.That(screenModel.Saldo, Is.EqualTo(expectedSaldo));
            Assert.That(screenModel.SaldoAsString, Is.EqualTo(string.Format(Properties.Resources.MoneyValueFormat, expectedSaldo)));

            Assert.That(screenModel.Caption, Is.EqualTo(string.Format(Properties.Resources.RequestManagementPageCaptionFormat, Repository.Name)));

            Repository.Received(1).CalculateSaldoForMonth(currentDate.Year, currentDate.Month);
            Repository.Received(1).QueryRequestsForSingleMonth(currentDate.Year, currentDate.Month);

            Assert.That(screenModel.Requests.SelectableValues.Count, Is.EqualTo(3));
            Assert.That(screenModel.Requests.SelectableValues[0].Category, Is.EqualTo(Properties.Resources.NoCategory));
            Assert.That(screenModel.Requests.SelectableValues[1].Category, Is.EqualTo("Category2"));
            Assert.That(screenModel.Requests.SelectableValues[2].Category, Is.EqualTo(Properties.Resources.NoCategory));

            foreach (var request in screenModel.Requests.SelectableValues)
            {
                Assert.That(request.ValueAsString, Is.EqualTo(string.Format(Properties.Resources.MoneyValueFormat, request.Value)));
            }
        }

        [TestCase(2012, 3, 2012, 4)]
        [TestCase(2012, 12, 2013, 1)]
        public void NextMonthCommandCallsRepositoryAndUpdatesProperties(int currentYear, int currentMonth, int nextYear, int nextMonth)
        {
            var screenModel = new RequestManagementPageViewModel(Application, currentYear, currentMonth);

            Repository.ClearReceivedCalls();

            screenModel.NextMonthCommand.Execute(null);
            Repository.Received(1).CalculateSaldoForMonth(nextYear, nextMonth);
            Repository.Received(1).UpdateStandingOrdersToCurrentMonth(nextYear, nextMonth);
            Repository.Received(1).QueryRequestsForSingleMonth(nextYear, nextMonth);
            Assert.That(screenModel.Month, Is.EqualTo(nextMonth));
            Assert.That(screenModel.Year, Is.EqualTo(nextYear));
            Assert.That(screenModel.Months.Value, Is.EqualTo(screenModel.Months.SelectableValues.Single(s => s.Index == nextMonth)));
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void NextMonthCommandUpdatesGotoCurrentMonthState(bool nextCommandGoesToCurrentMonth)
        {
            var currentDate = ApplicationContext.Now;
            if (nextCommandGoesToCurrentMonth) currentDate = currentDate.AddMonths(-1);

            var screenModel = new RequestManagementPageViewModel(Application, currentDate.Year, currentDate.Month);
            
            Repository.ClearReceivedCalls();

            screenModel.NextMonthCommand.Execute(null);
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.EqualTo(!nextCommandGoesToCurrentMonth));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PreviousMonthCommandUpdatesGotoCurrentMonthState(bool previousCommandGoesToCurrentMonth)
        {
            var currentDate = ApplicationContext.Now;
            if (previousCommandGoesToCurrentMonth) currentDate = currentDate.AddMonths(1);

            var screenModel = new RequestManagementPageViewModel(Application, currentDate.Year, currentDate.Month);

            Repository.ClearReceivedCalls();

            screenModel.PreviousMonthCommand.Execute(null);
            Repository.DidNotReceiveWithAnyArgs().UpdateStandingOrdersToCurrentMonth(Arg.Any<int>(), Arg.Any<int>());
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.EqualTo(!previousCommandGoesToCurrentMonth));
        }

        [Test]
        public void GotoCurrentMonthCommand()
        {
            var currentDate = ApplicationContext.Now.AddMonths(-1);
            var screenModel = new RequestManagementPageViewModel(Application, currentDate.Year, currentDate.Month);

            Repository.ClearReceivedCalls();

            screenModel.GotoCurrentMonthCommand.Execute(null);
            Repository.Received(1).CalculateSaldoForMonth(ApplicationContext.Now.Year, ApplicationContext.Now.Month);
            Repository.Received(1).QueryRequestsForSingleMonth(ApplicationContext.Now.Year, ApplicationContext.Now.Month);
            Repository.DidNotReceiveWithAnyArgs().UpdateStandingOrdersToCurrentMonth(Arg.Any<int>(), Arg.Any<int>());
            Assert.That(screenModel.Month, Is.EqualTo(ApplicationContext.Now.Month));
            Assert.That(screenModel.Year, Is.EqualTo(currentDate.Year));
            Assert.That(screenModel.Months.Value, Is.EqualTo(screenModel.Months.SelectableValues.Single(s => s.Index == ApplicationContext.Now.Month)));
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.False);
            Assert.That(screenModel.NextMonthCommand.IsEnabled, Is.False);
        }

        [TestCase(2013, 4, 2013, 3 )]
        [TestCase(2013, 1, 2012, 12)]
        public void PreviousMonthCommandCallsRepositoryAndUpdatesProperties(int currentYear, int currentMonth, int nextYear, int nextMonth)
        {
            var screenModel = new RequestManagementPageViewModel(Application, currentYear, currentMonth);

            Repository.ClearReceivedCalls();

            screenModel.PreviousMonthCommand.Execute(null);
            Repository.Received(1).CalculateSaldoForMonth(nextYear, nextMonth);
            Repository.Received(1).QueryRequestsForSingleMonth(nextYear, nextMonth);
            Assert.That(screenModel.Month, Is.EqualTo(nextMonth));
            Assert.That(screenModel.Year, Is.EqualTo(nextYear));
            Assert.That(screenModel.Months.Value, Is.EqualTo(screenModel.Months.SelectableValues.Single(s => s.Index == nextMonth)));
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.True);
        }

        [Test]
        public void ChangeYearUpdatesGotoCurrentMonthCommand()
        {
            var screenModel = new RequestManagementPageViewModel(Application, ApplicationContext.Now.Year, ApplicationContext.Now.Month);
            screenModel.Year -= 1;
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.True);
            Repository.ClearReceivedCalls();

            screenModel.Year += 1;
            Repository.Received(1).QueryRequestsForSingleMonth(screenModel.Year, screenModel.Month);
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.False);
        }

        [Test]
        public void MonthYearUpdatesGotoCurrentMonthCommand()
        {
            var screenModel = new RequestManagementPageViewModel(Application, ApplicationContext.Now.Year, ApplicationContext.Now.Month);
            
            var currentMonth = screenModel.Months.Value;
            screenModel.Months.Value = screenModel.Months.SelectableValues.First(m => m.Index != currentMonth.Index);
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.True);
            
            screenModel.Months.Value = currentMonth;
            Assert.That(screenModel.GotoCurrentMonthCommand.IsEnabled, Is.False);
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

            Assert.That(requestDialogViewModel.Caption, Is.EqualTo(Properties.Resources.RequestDialogCaptionCreate));
            Assert.That(requestDialogViewModel.CreateRequestCommand.IsEnabled, Is.False);
            Assert.That(requestDialogViewModel.DateProperty.Value, Is.EqualTo(new DateTime(2014, 6, 1)));
            Assert.That(requestDialogViewModel.FirstPossibleDate, Is.EqualTo(new DateTime(2014, 6, 1)));
            Assert.That(requestDialogViewModel.LastPossibleDate, Is.EqualTo(new DateTime(2014, 6, 30)));
            Assert.That(requestDialogViewModel.ValueProperty.Value, Is.EqualTo(0.0d));
        }

        [TestCase(false, 11.1)]
        [TestCase(true, 11.1)]
        [TestCase(false, -11.1)]
        [TestCase(true, -11.1)]
        public void AddRequestCommandWithCreateRequestExecuted(bool isCategorySelected, double expectedValue)
        {
            var expectedDate = new DateTime(2014, 6, 14);
            const string entityIdOfRequest = "NewEntityId";
            const string expectedDescription = "TestDescription";

            object dialogViewModel = null;
            WindowManager.When(w => w.ShowDialog(Arg.Any<object>())).Do(c => dialogViewModel = c[0]);

            Application.Repository.QueryAllCategories().Returns(c => Enumerable.Range(1, 3).Select(i =>
            {
                var category = Substitute.For<CategoryEntity>();
                category.PersistentId.Returns("Category" + i);
                category.Name.Returns("Category" + i);
                return category;
            }));

            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);
            var requestsBeforeUpdate = screenModel.Requests.SelectableValues.Count;
            screenModel.AddRequestCommand.Execute(null);

            var requestDialogViewModel = (RequestDialogViewModel)dialogViewModel;
            requestDialogViewModel.DescriptionProperty.Value = expectedDescription;
            requestDialogViewModel.ValueProperty.Value = Math.Abs(expectedValue);
            requestDialogViewModel.RequestKind.Value = expectedValue < 0.0
                ? RequestKind.Expenditure
                : RequestKind.Earning;
            requestDialogViewModel.DateProperty.Value = expectedDate;
            if (isCategorySelected)
            {
                requestDialogViewModel.Categories.Value = requestDialogViewModel.Categories.SelectableValues.First();
            }

            Repository.CalculateSaldoForMonth(2014, 6).Returns(99.99);
            Repository.CreateRequest(Arg.Any<RequestEntityData>()).Returns(entityIdOfRequest);
            Repository.QueryRequest(entityIdOfRequest).Returns(c =>
            {
                var entity = Substitute.For<RequestEntity>();
                entity.PersistentId.Returns(entityIdOfRequest);
                entity.Description.Returns(expectedDescription);
                entity.Value.Returns(expectedValue);
                entity.Date.Returns(expectedDate);
                if (isCategorySelected)
                {
                    var category = Substitute.For<CategoryEntity>();
                    category.PersistentId.Returns("Category1");
                    category.Name.Returns("Category1");
                    entity.Category.Returns(category);
                }
                else
                {
                    entity.Category.Returns((CategoryEntity)null);
                }
                
                return entity;
            });
            Repository.ClearReceivedCalls();

            requestDialogViewModel.CreateRequestCommand.Execute(null);

            var requestViewModel = screenModel.Requests.SelectableValues.SingleOrDefault(r => r.EntityId == entityIdOfRequest);

// ReSharper disable ImplicitlyCapturedClosure
            Repository.Received(1).CreateRequest(Arg.Is<RequestEntityData>(r => r.Date == expectedDate
                                                                             && Math.Abs(r.Value - expectedValue) < double.Epsilon
                                                                             && r.Description == expectedDescription
                                                                             && r.CategoryPersistentId == (isCategorySelected ? "Category1" : null)));
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
            Assert.That(requestViewModel.Category, Is.EqualTo(isCategorySelected ? "Category1" : Properties.Resources.NoCategory));
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
                entity.Value.Returns((i-1)*2.30);

                Repository.QueryRequest(entity.PersistentId).Returns(entity);

                if (i%2 == 0)
                {
                    var category = Substitute.For<CategoryEntity>();
                    category.Name.Returns("Category" + i);
                    entity.Category.Returns(category);
                }
                else
                {
                    entity.Category.Returns(c => null);
                }

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
                       .When(w => w.ShowQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Action>(), Arg.Any<Action>()))
                       .Do(c => { if (confirmDelete) ((Action)c[2]).Invoke(); });

            screenModel.Requests.Value = screenModel.Requests.SelectableValues.First();
            var persistentId = screenModel.Requests.Value.EntityId;
            
            Repository.ClearReceivedCalls();
            DefineRequestsForMonth(2014, 6, 2);

            screenModel.DeleteRequestCommand.Execute(null);

            if (confirmDelete)
            {
                Repository.Received(1).QueryRequestsForSingleMonth(2014, 6);
                Repository.Received(1).CalculateSaldoForMonth(2014, 6);
                Repository.Received(1).DeleteRequest(persistentId);
                Assert.That(screenModel.Requests.SelectableValues.Count, Is.EqualTo(2));
                Assert.That(screenModel.Requests.Value, Is.Null);    
            }
            else
            {
                Repository.DidNotReceiveWithAnyArgs().QueryRequestsForSingleMonth(Arg.Any<int>(), Arg.Any<int>());
                Repository.DidNotReceiveWithAnyArgs().CalculateSaldoForMonth(Arg.Any<int>(), Arg.Any<int>());
                Repository.DidNotReceiveWithAnyArgs().DeleteRequest(Arg.Any<string>());
                Assert.That(screenModel.Requests.SelectableValues.Count, Is.EqualTo(3));
                Assert.That(screenModel.Requests.Value, Is.EqualTo(screenModel.Requests.SelectableValues.First()));    
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

            screenModel.Requests.Value = screenModel.Requests.SelectableValues.First();
            screenModel.EditRequestCommand.Execute(null);

            WindowManager.Received(1).ShowDialog(Arg.Any<object>());
            Assert.That(dialogViewModel, Is.InstanceOf<RequestDialogViewModel>());

            var requestDialogViewModel = (RequestDialogViewModel)dialogViewModel;

            Assert.That(requestDialogViewModel.Caption, Is.EqualTo(Properties.Resources.RequestDialogCaptionEdit));
            Assert.That(requestDialogViewModel.CreateRequestCommand.IsEnabled, Is.False);
        }

        [TestCase(RequestKind.Earning)]
        [TestCase(RequestKind.Expenditure)]
        public void EditRequest(RequestKind requestKind)
        {
            DefineRequestsForMonth(2014, 6, 1);
            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);
            var currentRequestEntityId = screenModel.Requests.SelectableValues.First().EntityId;

            object dialogViewModel = null;
            Application.WindowManager
                       .When(w => w.ShowDialog(Arg.Any<object>()))
                       .Do(c => { dialogViewModel = c[0]; });

            screenModel.Requests.Value = screenModel.Requests.SelectableValues.First();
            screenModel.EditRequestCommand.Execute(null);

            Assert.That(dialogViewModel, Is.InstanceOf<RequestDialogViewModel>());

            var dialog = (RequestDialogViewModel) dialogViewModel;

            var newDate = dialog.DateProperty.Value.AddDays(2);
            dialog.DateProperty.Value = newDate;
            dialog.DescriptionProperty.Value = "New Description added";
            dialog.RequestKind.Value = requestKind;
            dialog.ValueProperty.Value = 77.77;

            Application.Repository.ClearReceivedCalls();
            dialog.CreateRequestCommand.Execute(null);

            var expectedValue = requestKind == RequestKind.Earning ? 77.77 : -77.77;

            Application.Repository.Received(1).UpdateRequest(currentRequestEntityId,
                Arg.Is<RequestEntityData>(r => r.Description == "New Description added" && Math.Abs(r.Value - expectedValue) < double.Epsilon && dialog.DateProperty.Value == newDate));

            Application.Repository.Received(1).CalculateSaldoForMonth(2014, 6);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SwitchAccountCommand(bool saveOnExit)
        {
            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);

            Application.Repository.ClearReceivedCalls();
            screenModel.SwitchAccountCommand.Execute(null);

            Application.Repository.Received(1).Close();
            Assert.That(Application.ActivePage, Is.InstanceOf<AccountManagementPageViewModel>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnCloseRequestShowsMessageBox(bool yesNo)
        {
            WindowManager.When(w => w.ShowQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Action>(), Arg.Any<Action>()))
                         .Do(c =>
                         {
                             if (yesNo) ((Action)c[2]).Invoke();
                             else ((Action)c[3]).Invoke();
                         });

            var screenModel = new RequestManagementPageViewModel(Application, 2014, 6);

            screenModel.OnClosingRequest();

            WindowManager.Received(1).ShowQuestion(Properties.Resources.RequestManagementOnClosingRequestConfirmationCaption, 
                                                       Properties.Resources.RequestManagementOnClosingRequestConfirmationMessage, 
                                                       Arg.Any<Action>(), Arg.Any<Action>());

            if (yesNo)
            {
                Repository.Received(1).Close();
            }
            else
            {
                Repository.DidNotReceive().Close();
            }
        }
    }
}