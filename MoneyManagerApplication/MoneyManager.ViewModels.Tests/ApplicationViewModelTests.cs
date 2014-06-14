
using System;
using MoneyManager.ViewModels.RequestManagement;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    [TestFixture]
    public class ApplicationViewModelTests : ViewModelTestsBase
    {
        [Test]
        public void InitialState()
        {
            var application = new ApplicationViewModel(Repository, ApplicationSettings, WindowManager);

            Assert.That(application.Repository, Is.SameAs(Repository));
            Assert.That(application.ApplicationSettings, Is.SameAs(ApplicationSettings));
            Assert.That(application.WindowManager, Is.SameAs(WindowManager));
            Assert.That(application.ActivePage, Is.Null);
        }

        [Test]
        public void ActivateRequestManagementScreen()
        {
            var application = new ApplicationViewModel(Repository, ApplicationSettings, WindowManager);

            application.ActivateRequestmanagementPage();

            var currentDateTime = DateTime.Now;

            Assert.That(application.ActivePage, Is.InstanceOf<RequestManagementPageViewModel>());

            var activeScreen = (RequestManagementPageViewModel) application.ActivePage;
            Assert.That(activeScreen.Year, Is.EqualTo(currentDateTime.Year));
            Assert.That(activeScreen.Month, Is.EqualTo(currentDateTime.Month));
        }
    }
}
