using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    public abstract class ViewModelTestsBase
    {
        protected ApplicationViewModel Application { get; private set; }
        protected Repository Repository { get; private set; }
        protected ApplicationContext ApplicationContext { get; private set; }
        protected WindowManager WindowManager { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            Repository = Substitute.For<Repository>();
            Repository.Name.Returns("DefaultRepositoryName");
            Repository.FilePath.Returns(@"C:\Test.mmdb");

            ApplicationContext = Substitute.For<ApplicationContext>();
            WindowManager = Substitute.For<WindowManager>();

            Application = new ApplicationViewModel(Repository, ApplicationContext, WindowManager);
        }
    }
}
