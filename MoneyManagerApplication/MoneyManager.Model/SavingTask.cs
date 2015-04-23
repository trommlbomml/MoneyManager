using System.Collections.Generic;
using MoneyManager.Model.Entities;

namespace MoneyManager.Model
{
    internal class SavingTask
    {
        public List<string> EntitiesToDelete { get; private set; }
        public List<RequestEntityPersistenceImp> RequestsToUpdate { get; private set; }
        public List<CategoryEntityPersistenceImp> CategoriesToUpdate { get; private set; }
        public List<StandingOrderEntityPersistence> StandingOrdersToUpdate { get; private set; }
        public string FilePath { get; private set; }

        public SavingTask(string filePath)
        {
            FilePath = filePath;
            EntitiesToDelete = new List<string>();
            RequestsToUpdate = new List<RequestEntityPersistenceImp>();
            CategoriesToUpdate = new List<CategoryEntityPersistenceImp>();
            StandingOrdersToUpdate = new List<StandingOrderEntityPersistence>();
        }

        public SavingTask(string filePath, CategoryEntityPersistenceImp singleSave):this(filePath)
        {
            FilePath = filePath;
            CategoriesToUpdate.Add(singleSave);
        }

        public SavingTask(string filePath, RequestEntityPersistenceImp singleSave)
            : this(filePath)
        {
            RequestsToUpdate.Add(singleSave);
        }

        public SavingTask(string filePath, StandingOrderEntityPersistence singleSave)
            : this(filePath)
        {
            StandingOrdersToUpdate.Add(singleSave);
        }

        public SavingTask(string filePath, string entityToDelete)
            : this(filePath)
        {
            EntitiesToDelete.Add(entityToDelete);
        }
    }
}