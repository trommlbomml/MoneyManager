using System.Collections.Generic;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    internal class SavingTask
    {
        public List<string> EntitiesToDelete { get; private set; }
        public List<RequestEntity> RequestsToUpdate { get; private set; }
        public List<CategoryEntity> CategoriesToUpdate { get; private set; }
        public List<StandingOrderEntity> StandingOrdersToUpdate { get; private set; }

        public SavingTask()
        {
            EntitiesToDelete = new List<string>();
            RequestsToUpdate = new List<RequestEntity>();
            CategoriesToUpdate = new List<CategoryEntity>();
            StandingOrdersToUpdate = new List<StandingOrderEntity>();
        }
    }
}