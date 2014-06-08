using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    public abstract class ViewModelTestsBase
    {
        protected ApplicationViewModel Application { get; private set; }
        protected Repository Repository { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            Repository = Substitute.For<Repository>();

            Application = new ApplicationViewModel(Repository);
        }
    }
}
