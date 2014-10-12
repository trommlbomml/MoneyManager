using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.AccountManagement;
using MoneyManager.ViewModels.RequestManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.AccountManagement
{
    [TestFixture]
    public class AccountManagementPageViewModelTests : ViewModelTestsBase
    {
        private static RecentAccountInformation CreateRecentAccountInformation(string path, DateTime date)
        {
            var accountInfo = Substitute.For<RecentAccountInformation>();
            accountInfo.LastAccessDate.Returns(date);
            accountInfo.Path.Returns(path);
            return accountInfo;
        }

        [Test]
        public void InitalState()
        {
            var viewModel = new AccountManagementPageViewModel(Application);

            Assert.That(viewModel.Caption, Is.EqualTo(Properties.Resources.AccountManagementPageCaption));
            Assert.That(viewModel.CreateNewAccountCommand.IsEnabled, Is.True);
            Assert.That(viewModel.OpenAccountCommand.IsEnabled, Is.True);
            Assert.That(viewModel.OpenRecentAccountCommand.IsEnabled, Is.False);
            Assert.That(viewModel.Accounts.SelectableValues, Is.Empty);
            Assert.That(viewModel.Accounts.SelectedValue, Is.Null);
        }

        [Test]
        public void LoadsRecentAccounts()
        {
            var values = new List<RecentAccountInformation>
            {
                CreateRecentAccountInformation("C:\test\test.txt", new DateTime(2014, 4, 1)),
                CreateRecentAccountInformation("C:\test2\test2.txt", new DateTime(2014, 8, 14))
            };

            ApplicationContext.RecentAccounts.Returns(values.AsReadOnly());

            var viewModel = new AccountManagementPageViewModel(Application);

            Assert.That(viewModel.Accounts.SelectableValues.Count, Is.EqualTo(2));
            Assert.That(viewModel.Accounts.SelectedValue, Is.Null);

            Assert.That(viewModel.Accounts.SelectableValues[0].LastAccessDate, Is.EqualTo(new DateTime(2014, 4, 1)));
            Assert.That(viewModel.Accounts.SelectableValues[0].Path, Is.EqualTo("C:\test\test.txt"));

            Assert.That(viewModel.Accounts.SelectableValues[1].LastAccessDate, Is.EqualTo(new DateTime(2014, 8, 14)));
            Assert.That(viewModel.Accounts.SelectableValues[1].Path, Is.EqualTo("C:\test2\test2.txt"));
        }

        [Test]
        public void CreateRepositoryWithRepositoryThrowsErrorShowsMessageBox()
        {
            object currentDialog = null;

            WindowManager.When(w => w.ShowDialog(Arg.Any<object>())).Do(c => currentDialog = c[0]);
            Repository.When(r => r.Create(Arg.Any<string>(), Arg.Any<string>())).Do(c => {throw new ApplicationException("ErrorMessage");});

            var viewModel = new AccountManagementPageViewModel(Application);

            viewModel.CreateNewAccountCommand.Execute(null);
            Assert.That(currentDialog, Is.InstanceOf<CreateAccountDialogViewModel>());

            var createAccountDialog = (CreateAccountDialogViewModel) currentDialog;

            createAccountDialog.Name = "Test";
            createAccountDialog.Path = "Test";
            createAccountDialog.CreateAccountCommand.Execute(null);

            Repository.Received(1).Create("Test", "Test");
            WindowManager.Received(1).ShowError("Error", "ErrorMessage");
        }

        [Test]
        public void CreateAccount()
        {
            object currentDialog = null;

            WindowManager.When(w => w.ShowDialog(Arg.Any<object>())).Do(c => currentDialog = c[0]);

            var viewModel = new AccountManagementPageViewModel(Application);

            viewModel.CreateNewAccountCommand.Execute(null);
            Assert.That(currentDialog, Is.InstanceOf<CreateAccountDialogViewModel>());

            var createAccountDialog = (CreateAccountDialogViewModel)currentDialog;

            createAccountDialog.Name = "Test";
            createAccountDialog.Path = "Test";
            createAccountDialog.CreateAccountCommand.Execute(null);

            Repository.Received(1).Create("Test", "Test");
            
            Assert.That(Application.ActivePage, Is.InstanceOf<RequestManagementPageViewModel>());

            var requestManagementPage = (RequestManagementPageViewModel) Application.ActivePage;
            Assert.That(requestManagementPage.Month, Is.EqualTo(ApplicationContext.Now.Month));
            Assert.That(requestManagementPage.Year, Is.EqualTo(ApplicationContext.Now.Year));

            ApplicationContext.Received(1).UpdateRecentAccountInformation("Test", Arg.Any<DateTime>());
        }

        [Test]
        public void OpenRecentAccount()
        {
            var values = new List<RecentAccountInformation>
            {
                CreateRecentAccountInformation(@"C:\test\test.txt", new DateTime(2014, 4, 1)),
            };
            ApplicationContext.RecentAccounts.Returns(values.AsReadOnly());

            var viewModel = new AccountManagementPageViewModel(Application);
            viewModel.Accounts.SelectedValue = viewModel.Accounts.SelectableValues.First();

            viewModel.OpenRecentAccountCommand.Execute(null);

            Repository.Received(1).Open(@"C:\test\test.txt");
            ApplicationContext.Received(1).UpdateRecentAccountInformation(@"C:\test\test.txt", Arg.Any<DateTime>());
            Assert.That(Application.ActivePage, Is.InstanceOf<RequestManagementPageViewModel>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OpenCommand(bool accept)
        {
            var expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Konto.mmdb");

            if (accept)
            {
                WindowManager.ShowOpenFileDialog(Arg.Any<string>(), Arg.Any<string>()).Returns(expectedPath);
            }
            else
            {
                WindowManager.ShowOpenFileDialog(Arg.Any<string>(), Arg.Any<string>()).Returns("");
                expectedPath = "";
            }

            var viewModel = new AccountManagementPageViewModel(Application);
            viewModel.OpenAccountCommand.Execute(null);

            WindowManager.Received(1).ShowOpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Properties.Resources.AccountManagementFilterOpenAccount);

            if (accept)
            {
                Repository.Received(1).Open(expectedPath);
                ApplicationContext.Received(1).UpdateRecentAccountInformation(expectedPath, Arg.Any<DateTime>());
                Assert.That(Application.ActivePage, Is.InstanceOf<RequestManagementPageViewModel>());
            }
            else
            {
                Repository.DidNotReceiveWithAnyArgs().Open(Arg.Any<string>());
                ApplicationContext.DidNotReceiveWithAnyArgs().UpdateRecentAccountInformation(Arg.Any<string>(), Arg.Any<DateTime>());
                Assert.That(Application.ActivePage, Is.Not.InstanceOf<RequestManagementPageViewModel>());
            }
        }

        [Test]
        public void OpenCommandWithException()
        {
            WindowManager.ShowOpenFileDialog(Arg.Any<string>(), Arg.Any<string>()).Returns("TestPath");
            Repository.When(r => r.Open(Arg.Any<string>())).Do(c => {throw new ApplicationException("TestMessage");});

            var viewModel = new AccountManagementPageViewModel(Application);
            viewModel.OpenAccountCommand.Execute(null);
            
            WindowManager.Received(1).ShowError("Error", "TestMessage");
            Repository.Received(1).Open(Arg.Any<string>());
            ApplicationContext.DidNotReceiveWithAnyArgs().UpdateRecentAccountInformation(Arg.Any<string>(), Arg.Any<DateTime>());
            Assert.That(Application.ActivePage, Is.Not.InstanceOf<RequestManagementPageViewModel>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OpenRecentAccountWithException(bool answerYes)
        {
            const string accountPath = @"C:\test\test.txt";
            var values = new List<RecentAccountInformation>
            {
                CreateRecentAccountInformation(accountPath, new DateTime(2014, 4, 1)),
            };
            ApplicationContext.RecentAccounts.Returns(values.AsReadOnly());
            Repository.When(r => r.Open(Arg.Any<string>())).Do(c => { throw new Exception("Text"); });
            
            if (answerYes) 
            {
                WindowManager.When(w => w.ShowQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Action>(), Arg.Any<Action>()))
                             .Do(c => ((Action)c[2]).Invoke());
            }

            var viewModel = new AccountManagementPageViewModel(Application);
            viewModel.Accounts.SelectedValue = viewModel.Accounts.SelectableValues.First();

            viewModel.OpenRecentAccountCommand.Execute(null);

            Repository.Received(1).Open(accountPath);
            ApplicationContext.DidNotReceiveWithAnyArgs().UpdateRecentAccountInformation(Arg.Any<string>(), Arg.Any<DateTime>());

            WindowManager.Received(1).ShowQuestion(Properties.Resources.RemoveRecentAccountMessageCaption, 
                                                   string.Format(Properties.Resources.RemoveRecentAccountMessageFormat, accountPath), 
                                                   Arg.Any<Action>(), Arg.Any<Action>());

            if (answerYes)
            {
                ApplicationContext.Received(1).DeleteRecentAccountInformation(accountPath);
                Assert.That(viewModel.Accounts.SelectableValues.Count, Is.EqualTo(0));
                Assert.That(viewModel.Accounts.SelectedValue, Is.Null);
            }
            else
            {
                ApplicationContext.DidNotReceiveWithAnyArgs().DeleteRecentAccountInformation(Arg.Any<string>());
                Assert.That(viewModel.Accounts.SelectableValues.Count, Is.EqualTo(1));
                Assert.That(viewModel.Accounts.SelectedValue, Is.EqualTo(viewModel.Accounts.SelectableValues.First()));
            }

            Assert.That(Application.ActivePage, Is.Not.InstanceOf<AccountManagementPageViewModel>());
        }
    }
}
