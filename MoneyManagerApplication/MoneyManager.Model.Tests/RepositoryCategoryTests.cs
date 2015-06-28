
using System;
using System.Linq;
using MoneyManager.Model.Entities;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    internal class RepositoryCategoryTests : RepositoryTestsBase
    {
        [Test]
        public void QueryAllCategories()
        {
            var categories = Repository.QueryAllCategories().ToArray();
            Assert.That(categories.Length, Is.EqualTo(5));
        }

        [Test]
        public void UpdateCategory()
        {
            var originalCategory = new CategoryEntityImp {Name = "Category"};
            var persistentId = originalCategory.PersistentId;

            Repository.AddCategory(originalCategory);
            Repository.UpdateCategory(persistentId, "New Category Name");

            var categoryQueried = Repository.QueryCategory(persistentId);

            Assert.That(categoryQueried.PersistentId, Is.EqualTo(persistentId));
            Assert.That(categoryQueried.Name, Is.EqualTo("New Category Name"));

            PersistenceHandler.Received(1).SaveChanges(Arg.Is<SavingTask>(t => t.RequestsToUpdate.Count == 0 &&
                                                                               t.CategoriesToUpdate.Count == 1 &&
                                                                               t.EntitiesToDelete.Count == 0 &&
                                                                               t.StandingOrdersToUpdate.Count == 0 &&
                                                                               t.FilePath == DatabaseFile));
        }

        [Test]
        public void UpdateRequestOfNonExistingEntityThrowsException()
        {
            Assert.That(() => Repository.UpdateCategory("InvalidId", "Test"), Throws.ArgumentException);
        }

        [Test]
        public void CreateCategory()
        {
            var persistentId = Repository.CreateCategory("My Category");

            var createdRequest = Repository.QueryAllCategories().Single(c => c.PersistentId == persistentId);
            Assert.That(createdRequest.PersistentId, Is.EqualTo(persistentId));
            Assert.That(createdRequest.Name, Is.EqualTo("My Category"));

            PersistenceHandler.Received(1).SaveChanges(Arg.Is<SavingTask>(t => t.RequestsToUpdate.Count == 0 &&
                                                                               t.CategoriesToUpdate.Count == 1 &&
                                                                               t.EntitiesToDelete.Count == 0 &&
                                                                               t.StandingOrdersToUpdate.Count == 0 &&
                                                                               t.FilePath == DatabaseFile));
        }

        [Test]
        public void DeleteCategory()
        {
            CreateCategoriesInRepository(5);

            var allCategoryIdsBeforeDelete = Repository.QueryAllCategories().Select(c => c.PersistentId).ToArray();
            var categoryPersistentIdToDelete = allCategoryIdsBeforeDelete.First();

            Repository.DeleteCategory(categoryPersistentIdToDelete);

            var persistentIdsAfterDelete = Repository.QueryAllCategories().Select(c => c.PersistentId).ToArray();

            Assert.That(persistentIdsAfterDelete.Length, Is.EqualTo(9));
            CollectionAssert.AreEquivalent(allCategoryIdsBeforeDelete.Except(new[] { categoryPersistentIdToDelete }), persistentIdsAfterDelete);

            PersistenceHandler.Received(1).SaveChanges(Arg.Is<SavingTask>(t => t.RequestsToUpdate.Count == 0 &&
                                                                               t.CategoriesToUpdate.Count == 0 &&
                                                                               t.EntitiesToDelete.Count == 1 &&
                                                                               t.StandingOrdersToUpdate.Count == 0 &&
                                                                               t.FilePath == DatabaseFile));
        }

        [Test]
        public void DeleteCategoryReferencedByRequestRemovesCategoryFromRequest()
        {
            var category = new CategoryEntityImp { Name = "My Category" };
            Repository.AddCategory(category);

            var request = CreateRequestInRepository(DateTime.Now, "Test", 12.0);
            request.Category = category;

            PersistenceHandler.ClearReceivedCalls();

            Repository.DeleteCategory(category.PersistentId);
            Assert.That(request.Category, Is.Null);

            PersistenceHandler.Received(1).SaveChanges(Arg.Is<SavingTask>(t => t.RequestsToUpdate.Count == 1 &&
                                                                               t.CategoriesToUpdate.Count == 0 &&
                                                                               t.EntitiesToDelete.Count == 1 &&
                                                                               t.StandingOrdersToUpdate.Count == 0 &&
                                                                               t.FilePath == DatabaseFile));
        }
    }
}
