using System;
using MoneyManager.Interfaces;

namespace MoneyManager.Model.Entities
{
    internal class CategoryEntityStub : CategoryEntity
    {
        public string PersistentId { get; private set; }
        public string Name { get { throw new InvalidOperationException("Not Valid for Stub.");} }

        public CategoryEntityStub(CategoryEntity category)
        {
            PersistentId = category == null ? string.Empty : category.PersistentId;
        }
    }
}