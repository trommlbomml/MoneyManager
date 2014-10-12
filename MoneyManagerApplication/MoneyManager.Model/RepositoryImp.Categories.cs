using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    partial class RepositoryImp
    {
        internal void AddCategory(CategoryEntityImp categoryEntity)
        {
            if (categoryEntity == null) throw new ArgumentNullException("categoryEntity");
            if (_allCategories.Any(r => r.PersistentId == categoryEntity.PersistentId))
            {
                throw new InvalidOperationException(string.Format("Entity with Id {0} alread exists.", categoryEntity.PersistentId));
            }

            _allCategories.Add(categoryEntity);
        }

        public IEnumerable<CategoryEntity> QueryAllCategories()
        {
            EnsureRepositoryOpen("CreateRequest");

            return _allCategories;
        }

        public string CreateCategory(string name)
        {
            EnsureRepositoryOpen("CreateRequest");

            var categoryImp = new CategoryEntityImp { Name = name };
            _allCategories.Add(categoryImp);

            return categoryImp.PersistentId;
        }

        public CategoryEntity QueryCategory(string persistentId)
        {
            EnsureRepositoryOpen("QueryCategory");

            var entity = _allCategories.SingleOrDefault(c => c.PersistentId == persistentId);
            if (entity == null) throw new ArgumentException(@"Entity with Id does not exist", persistentId);

            return entity;
        }

        public void UpdateCategory(string persistentId, string name)
        {
            EnsureRepositoryOpen("CreateRequest");

            var category = _allCategories.SingleOrDefault(c => c.PersistentId == persistentId);
            if (category == null) throw new ArgumentException(@"The category does not exist or more than once", "persistentId");

            category.Name = name;
        }

        public void DeleteCategory(string persistentId)
        {
            EnsureRepositoryOpen("CreateRequest");

            var categoryEntityImp = _allCategories.Single(r => r.PersistentId == persistentId);
            _allCategories.Remove(categoryEntityImp);

            foreach (var request in _allRequests.Where(r => r.Category != null && r.Category.PersistentId == categoryEntityImp.PersistentId))
            {
                request.Category = null;
            }
        }
    }
}
