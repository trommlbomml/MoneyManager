using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace MoneyManager.Model
{
    class DataPersistenceHandlerAsyncImp : DataPersistenceHandler
    {
        private readonly Queue<SavingTask> _tasksToWork = new Queue<SavingTask>();
        private readonly object _lockObject = new object();
        private bool _isWorkerThreadRunning;

        public void SaveChanges(SavingTask task)
        {
            lock (_lockObject)
            {
                _tasksToWork.Enqueue(task);
            }
            if (!_isWorkerThreadRunning)
            {
                _isWorkerThreadRunning = true;
                ThreadPool.QueueUserWorkItem(OnWorkTasks);
            }
        }

        private void OnWorkTasks(object state)
        {
            while (true)
            {
                SavingTask currentTask;
                lock (_lockObject)
                {
                    if (_tasksToWork.Count == 0) break;
                    currentTask = _tasksToWork.Dequeue();
                }
                SaveTaskChanges(currentTask);
            }
            _isWorkerThreadRunning = false;
        }

        private static bool TryRemoveElementFromParentById(XDocument document, string parentName, string persistentId)
        {
            var element = document.Root.Element(parentName).Elements().SingleOrDefault(e => e.Attribute("Id").Value == persistentId);

            if (element == null) return false;

            element.Remove();
            return true;
        }

        private static void UpdateElementInParent(XDocument document, string parentName, string persistentId, XElement toInsert)
        {
            TryRemoveElementFromParentById(document, parentName, persistentId);
            document.Root.Element(parentName).Add(toInsert);
        }

        private static void SaveTaskChanges(SavingTask task)
        {
            var document = XDocument.Load(task.FilePath);

            foreach (var entityToDelete in task.EntitiesToDelete)
            {
                if (TryRemoveElementFromParentById(document, PersistenceConstants.Categories, entityToDelete)) continue;
                if (TryRemoveElementFromParentById(document, PersistenceConstants.StandingOrders, entityToDelete)) continue;
                TryRemoveElementFromParentById(document, PersistenceConstants.StandingOrders, entityToDelete);
            }

            foreach (var categoryEntity in task.CategoriesToUpdate)
            {
                UpdateElementInParent(document, PersistenceConstants.Categories, categoryEntity.PersistentId, categoryEntity.Serialize());
            }
            foreach (var requestEntity in task.RequestsToUpdate)
            {
                UpdateElementInParent(document, PersistenceConstants.Requests, requestEntity.PersistentId, requestEntity.Serialize());
            }
            foreach (var standingOrderEntity in task.StandingOrdersToUpdate)
            {
                UpdateElementInParent(document, PersistenceConstants.StandingOrders, standingOrderEntity.PersistentId, standingOrderEntity.Serialize());
            }

            document.Save(task.FilePath);
        }

        public void WaitForAllTasksFinished()
        {
        }
    }
}