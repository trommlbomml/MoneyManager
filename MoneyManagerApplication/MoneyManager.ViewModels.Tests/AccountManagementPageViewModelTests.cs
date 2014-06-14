
using System;
using System.Collections.Generic;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.AccountManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    [TestFixture]
    public class AccountManagementPageViewModelTests : ViewModelTestsBase
    {
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

            ApplicationSettings.RecentAccounts.Returns(values.AsReadOnly());

            var viewModel = new AccountManagementPageViewModel(Application);

            Assert.That(viewModel.Accounts.SelectableValues.Count, Is.EqualTo(2));
            Assert.That(viewModel.Accounts.SelectedValue, Is.Null);

            Assert.That(viewModel.Accounts.SelectableValues[0].LastAccessDate, Is.EqualTo(new DateTime(2014, 4, 1)));
            Assert.That(viewModel.Accounts.SelectableValues[0].Path, Is.EqualTo("C:\test\test.txt"));

            Assert.That(viewModel.Accounts.SelectableValues[1].LastAccessDate, Is.EqualTo(new DateTime(2014, 8, 14)));
            Assert.That(viewModel.Accounts.SelectableValues[1].Path, Is.EqualTo("C:\test2\test2.txt"));
        }

        private RecentAccountInformation CreateRecentAccountInformation(string path, DateTime date)
        {
            var accountInfo = Substitute.For<RecentAccountInformation>();
            accountInfo.LastAccessDate.Returns(date);
            accountInfo.Path.Returns(path);
            return accountInfo;
        }
    }
}
