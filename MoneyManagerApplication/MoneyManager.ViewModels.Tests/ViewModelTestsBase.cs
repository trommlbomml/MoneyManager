using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    public abstract class ViewModelTestsBase
    {
        protected ApplicationViewModel Application { get; private set; }
        protected Repository Repository { get; private set; }
        protected ApplicationSettings ApplicationSettings { get; private set; }
        protected WindowManager WindowManager { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            Repository = Substitute.For<Repository>();
            Repository.Name.Returns("DefaultRepositoryName");

            ApplicationSettings = Substitute.For<ApplicationSettings>();

            Application = new ApplicationViewModel(Repository, ApplicationSettings, WindowManager);
        }
    }
}
