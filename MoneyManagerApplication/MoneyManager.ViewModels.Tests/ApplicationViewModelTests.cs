
using System;
using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    [TestFixture]
    public class ApplicationViewModelTests
    {
        private Repository Repository { get; set; }

        [SetUp]
        public void Setup()
        {
            Repository = Substitute.For<Repository>();
        }

        [Test]
        public void InitialState()
        {
            var application = new ApplicationViewModel(Repository);

            Assert.That(application.WindowTitle, Is.EqualTo(Properties.Resources.ApplicationMainWindowTitle));
            Assert.That(application.Repository, Is.SameAs(Repository));
            Assert.That(application.ActiveScreen, Is.Null);
        }

        [Test]
        public void ActivateRequestManagementScreen()
        {
            var application = new ApplicationViewModel(Repository);

            application.ActivateRequestmanagementScreen();

            var currentDateTime = DateTime.Now;

            Assert.That(application.ActiveScreen, Is.InstanceOf<RequestManagementScreenModel>());

            var activeScreen = (RequestManagementScreenModel) application.ActiveScreen;
            Assert.That(activeScreen.Year, Is.EqualTo(currentDateTime.Year));
            Assert.That(activeScreen.Month, Is.EqualTo(currentDateTime.Month));
        }
    }
}
