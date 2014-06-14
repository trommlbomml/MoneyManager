using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyManager.ViewModels.AccountManagement;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    [TestFixture]
    public class RecentAccountViewModelTests : ViewModelTestsBase
    {
        [Test]
        public void InitialState()
        {
            var viewModel = new RecentAccountViewModel {LastAccessDate = new DateTime(2014, 4, 4)};

            var expectedLastAccessDateAsString = string.Format(Properties.Resources.LastAccesDateFormat, new DateTime(2014, 4, 4));
            Assert.That(viewModel.LastAccessDateAsString, Is.EqualTo(expectedLastAccessDateAsString));
        }
    }
}
