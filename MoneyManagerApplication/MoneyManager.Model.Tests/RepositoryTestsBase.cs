using System;
using System.IO;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.Model.Entities;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    internal abstract class RepositoryTestsBase
    {
        protected string DatabaseFile { get; private set; }
        protected DataPersistenceHandler PersistenceHandler { get; private set; }

        public static string GetUniqueFilePath(bool withExtension = false)
        {
            return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")) + (withExtension ? SystemConstants.DatabaseExtension : string.Empty);
        }

        protected RepositoryImp Repository { get; set; }
        protected ApplicationContext Context { get; private set; }

        internal static ApplicationContext CreateApplicationContextMockup(bool lockFileIsSuccessful)
        {
            var applicationContext = Substitute.For<ApplicationContext>();
            applicationContext.LockFile(Arg.Any<string>()).Returns(true);
            return applicationContext;
        }

        [SetUp]
        public void Setup()
        {
            Context = CreateApplicationContextMockup(true);
            PersistenceHandler = Substitute.For<DataPersistenceHandler>();
            Repository = new RepositoryImp(Context, PersistenceHandler);
            DatabaseFile = GetUniqueFilePath(true);
            Repository.Create(DatabaseFile, "DefaultName");
            PersistenceHandler.ClearReceivedCalls();
        }

        [TearDown]
        public void TearDown()
        {
            if (Repository.IsOpen) Repository.Close();
        }

        protected CategoryEntityImp CreateCategory(string name)
        {
            var category = new CategoryEntityImp { Name = name };
            Repository.AddCategory(category);
            return category;
        }

        protected RequestEntityImp CreateRequestInRepository(DateTime date, string description, double value)
        {
            var request = new RequestEntityImp
            {
                Date = date,
                Description = description,
                Value = value
            };
            Repository.AddRequest(request);

            return request;
        }

        protected void CreateRequestsInRepository(DateTime[] monthsToCreate, int[] requestsPerMonth)
        {
            for (var i = 0; i < monthsToCreate.Length; i++)
            {
                for (var j = 0; j < requestsPerMonth[i]; j++)
                {
                    Repository.CreateRequest(new RequestEntityData { Date = monthsToCreate[i] });
                }
            }
        }

        protected CategoryEntityImp[] CreateCategoriesInRepository(int count)
        {
            var allCategories = Enumerable.Range(1, count).Select(i => new CategoryEntityImp { Name = "Category" + i }).ToArray();
            foreach (var categoryEntityImp in allCategories)
            {
                Repository.AddCategory(categoryEntityImp);
            }
            PersistenceHandler.ClearReceivedCalls();
            return allCategories;
        }
    }
}
