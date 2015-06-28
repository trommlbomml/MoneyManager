using MoneyManager.ViewModels.RequestManagement;
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
            var application = new ApplicationViewModel(Repository, ApplicationContext, WindowManager);

            Assert.That(application.Repository, Is.SameAs(Repository));
            Assert.That(application.ApplicationContext, Is.SameAs(ApplicationContext));
            Assert.That(application.WindowManager, Is.SameAs(WindowManager));
            Assert.That(application.ActivePage, Is.Null);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ActivateRequestManagementScreen(bool hasRequests)
        {
            var application = new ApplicationViewModel(Repository, ApplicationContext, WindowManager);

            application.ActivateRequestmanagementPage(hasRequests ? new [] {"TestEntity"} : null);

            var currentDateTime = ApplicationContext.Now;

            Assert.That(application.ActivePage, Is.InstanceOf<RequestManagementPageViewModel>());

            var activeScreen = (RequestManagementPageViewModel) application.ActivePage;
            Assert.That(activeScreen.Year, Is.EqualTo(currentDateTime.Year));
            Assert.That(activeScreen.Month, Is.EqualTo(currentDateTime.Month));

            if (hasRequests)
            {
                WindowManager.Received(1).ShowDialog(Arg.Is<CreatedRequestsDialogViewModel>(r => r.CreatedRequests.Count == 1 && r.CreatedRequests[0].EntityId == "TestEntity"));
            }
            else
            {
                WindowManager.DidNotReceiveWithAnyArgs().ShowDialog(Arg.Any<object>());
            }
        }

        [Test]
        public void OnCloseRequestWithNoPage()
        {
            var application = new ApplicationViewModel(Repository, ApplicationContext, WindowManager);

            Assert.That(application.OnClosingRequest(), Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnCloseRequestDelegatesFromActivePage(bool expectCancel)
        {
            var application = new ApplicationViewModel(Repository, ApplicationContext, WindowManager);
            application.ActivePage = new PageTestViewModel(application) {IsCancelOnClose = expectCancel};
            
            Assert.That(application.OnClosingRequest(), Is.EqualTo(expectCancel));
        }
    }
}
