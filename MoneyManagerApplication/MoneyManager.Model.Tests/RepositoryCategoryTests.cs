
using System;
using System.Linq;
using MoneyManager.Model.Entities;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    internal class RepositoryCategoryTests : RepositoryTestsBase
    {
        [Test]
        public void QueryAllCategories()
        {
            var allCategories = CreateCategoriesInRepository(5);

            var categories = Repository.QueryAllCategories().ToArray();
            Assert.That(categories.Length, Is.EqualTo(allCategories.Length));
            Assert.That(categories.OrderBy(c => c.Name).Select(c => c.PersistentId), Is.EqualTo(allCategories.Select(c => c.PersistentId)));
        }

        [Test]
        public void UpdateCategory()
        {
            var originalCategory = new CategoryEntityImp {Name = "Category"};
            var persistentId = originalCategory.PersistentId;

            Repository.AddCategory(originalCategory);

            Repository.UpdateCategory(persistentId, "New Category Name");

            var categoryQueried = Repository.QueryAllCategories().First();

            Assert.That(categoryQueried.PersistentId, Is.EqualTo(persistentId));
            Assert.That(categoryQueried.Name, Is.EqualTo("New Category Name"));
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
        }

        [Test]
        public void DeleteCategory()
        {
            CreateCategoriesInRepository(5);

            var allCategoryIdsBeforeDelete = Repository.QueryAllCategories().Select(c => c.PersistentId).ToArray();
            var categoryPersistentIdToDelete = allCategoryIdsBeforeDelete.First();

            Repository.DeleteCategory(categoryPersistentIdToDelete);

            var persistentIdsAfterDelete = Repository.QueryAllCategories().Select(c => c.PersistentId).ToArray();

            Assert.That(persistentIdsAfterDelete.Length, Is.EqualTo(4));
            CollectionAssert.AreEquivalent(allCategoryIdsBeforeDelete.Except(new[] { categoryPersistentIdToDelete }), persistentIdsAfterDelete);
        }

        [Test]
        public void DeleteCategoryReferencedByRequestRemovesCategoryFromRequest()
        {
            var category = new CategoryEntityImp { Name = "My Category" };
            Repository.AddCategory(category);

            var request = CreateRequestInRepository(DateTime.Now, "Test", 12.0);
            request.Category = category;

            Repository.DeleteCategory(category.PersistentId);
            Assert.That(request.Category, Is.Null);
        }
    }
}
