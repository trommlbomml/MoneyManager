using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Assert.That(viewModel.CreateNewAccountCommand.IsEnabled, Is.False);
            Assert.That(viewModel.OpenAccountCommand.IsEnabled, Is.True);
            Assert.That(viewModel.Accounts, Is.Empty);
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

            Assert.That(viewModel.Accounts.Count, Is.EqualTo(2));

            Assert.That(viewModel.Accounts[0].LastAccessDate, Is.EqualTo(new DateTime(2014, 4, 1)));
            Assert.That(viewModel.Accounts[0].Path, Is.EqualTo("C:\test\test.txt"));

            Assert.That(viewModel.Accounts[1].LastAccessDate, Is.EqualTo(new DateTime(2014, 8, 14)));
            Assert.That(viewModel.Accounts[1].Path, Is.EqualTo("C:\test2\test2.txt"));
        }

        [Test]
        public void CreateRepositoryWithRepositoryThrowsErrorShowsMessageBox()
        {
            Repository.When(r => r.Create(Arg.Any<string>(), Arg.Any<string>())).Do(c => {throw new ApplicationException("ErrorMessage");});

            var viewModel = new AccountManagementPageViewModel(Application)
            {
                NewAccountFilePath = "Test",
                NewAccountNameProperty = {Value = "Test"}
            };

            viewModel.CreateNewAccountCommand.Execute(null);

            Repository.Received(1).Create("Test", "Test");
            WindowManager.Received(1).ShowError("Error", "ErrorMessage");
        }

        [Test]
        public void CreateAccount()
        {
            var viewModel = new AccountManagementPageViewModel(Application)
            {
                NewAccountFilePath = "Test",
                NewAccountNameProperty = {Value = "Test"}
            };

            viewModel.CreateNewAccountCommand.Execute(null);

            Repository.Received(1).Create("Test", "Test");
            Assert.That(Application.ActivePage, Is.InstanceOf<RequestManagementPageViewModel>());

            var requestManagementPage = (RequestManagementPageViewModel) Application.ActivePage;
            Assert.That(requestManagementPage.Month, Is.EqualTo(ApplicationContext.Now.Month));
            Assert.That(requestManagementPage.Year, Is.EqualTo(ApplicationContext.Now.Year));

            ApplicationContext.DidNotReceiveWithAnyArgs().UpdateRecentAccountInformation("Test");
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
            viewModel.Accounts.First().OpenCommand.Execute(null);

            Repository.Received(1).Open(@"C:\test\test.txt");
            Repository.Received(1).UpdateStandingOrdersToCurrentMonth();
            ApplicationContext.DidNotReceiveWithAnyArgs().UpdateRecentAccountInformation(@"C:\test\test.txt");
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
                Repository.Received(1).UpdateStandingOrdersToCurrentMonth();
                Assert.That(Application.ActivePage, Is.InstanceOf<RequestManagementPageViewModel>());
            }
            else
            {
                Repository.DidNotReceiveWithAnyArgs().Open(Arg.Any<string>());
                Repository.DidNotReceiveWithAnyArgs().UpdateStandingOrdersToCurrentMonth();
                Assert.That(Application.ActivePage, Is.Not.InstanceOf<RequestManagementPageViewModel>());
            }

            ApplicationContext.DidNotReceiveWithAnyArgs().UpdateRecentAccountInformation(Arg.Any<string>());
        }

        [Test]
        public void OpenCommandWithException()
        {
            WindowManager.ShowOpenFileDialog(Arg.Any<string>(), Arg.Any<string>()).Returns("TestPath");
            Repository.When(r => r.Open(Arg.Any<string>())).Do(c => {throw new ApplicationException("TestMessage");});

            var viewModel = new AccountManagementPageViewModel(Application);
            viewModel.OpenAccountCommand.Execute(null);

            WindowManager.Received(1).ShowError(Properties.Resources.ErrorOpenRecentAccount,string.Format(Properties.Resources.RecentAccountUnexpectedErrorFormat, "TestMessage"));
            Repository.Received(1).Open(Arg.Any<string>());
            ApplicationContext.DidNotReceiveWithAnyArgs().UpdateRecentAccountInformation(Arg.Any<string>());
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
            Repository.When(r => r.Open(Arg.Any<string>())).Do(c => { throw new FileNotFoundException("Text"); });
            
            if (answerYes) 
            {
                WindowManager.When(w => w.ShowQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Action>(), Arg.Any<Action>()))
                             .Do(c => ((Action)c[2]).Invoke());
            }

            var viewModel = new AccountManagementPageViewModel(Application);
            viewModel.Accounts.First().OpenCommand.Execute(null);

            Repository.Received(1).Open(accountPath);
            ApplicationContext.DidNotReceiveWithAnyArgs().UpdateRecentAccountInformation(Arg.Any<string>());

            WindowManager.Received(1).ShowQuestion(Properties.Resources.ErrorOpenRecentAccount,
                                                   string.Format(Properties.Resources.RecentAccountNotFoundFormat, accountPath), 
                                                   Arg.Any<Action>(), Arg.Any<Action>());

            if (answerYes)
            {
                ApplicationContext.Received(1).DeleteRecentAccountInformation(accountPath);
                Assert.That(viewModel.Accounts.Count, Is.EqualTo(0));
            }
            else
            {
                ApplicationContext.DidNotReceiveWithAnyArgs().DeleteRecentAccountInformation(Arg.Any<string>());
                Assert.That(viewModel.Accounts.Count, Is.EqualTo(1));
            }

            Assert.That(Application.ActivePage, Is.Not.InstanceOf<AccountManagementPageViewModel>());
        }
    }
}
