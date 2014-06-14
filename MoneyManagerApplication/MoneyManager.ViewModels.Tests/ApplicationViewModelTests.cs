
using System;
using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    [TestFixture]
    public class ApplicationViewModelTests : ViewModelTestsBase
    {
        [Test]
        public void InitialState()
        {
            var application = new ApplicationViewModel(Repository, ApplicationSettings);

            Assert.That(application.WindowTitle, Is.EqualTo(Properties.Resources.ApplicationMainWindowTitle));
            Assert.That(application.Repository, Is.SameAs(Repository));
            Assert.That(application.ActivePage, Is.Null);
        }

        [Test]
        public void ActivateRequestManagementScreen()
        {
            var application = new ApplicationViewModel(Repository, ApplicationSettings);

            application.ActivateRequestmanagementPage();

            var currentDateTime = DateTime.Now;

            Assert.That(application.ActivePage, Is.InstanceOf<RequestManagementPageViewModel>());

            var activeScreen = (RequestManagementPageViewModel) application.ActivePage;
            Assert.That(activeScreen.Year, Is.EqualTo(currentDateTime.Year));
            Assert.That(activeScreen.Month, Is.EqualTo(currentDateTime.Month));
        }
    }
}
