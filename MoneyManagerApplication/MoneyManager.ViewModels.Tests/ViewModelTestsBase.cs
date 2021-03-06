﻿using System;
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

        protected const int CurrentMonth = 6;
        protected const int CurrentYear = 2014;
        protected const int CurrentDay = 12;

        [SetUp]
        public virtual void Setup()
        {
            Repository = Substitute.For<Repository>();
            Repository.Name.Returns("DefaultRepositoryName");
            Repository.FilePath.Returns(@"C:\Test.mmdb");

            ApplicationContext = Substitute.For<ApplicationContext>();
            ApplicationContext.Now.Returns(new DateTime(CurrentYear, CurrentMonth, CurrentDay));
            WindowManager = Substitute.For<WindowManager>();

            Application = new ApplicationViewModel(Repository, ApplicationContext, WindowManager);
        }
    }
}
