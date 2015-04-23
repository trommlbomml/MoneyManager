using System.Collections.Generic;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.Model.Entities;

namespace MoneyManager.Model
{
    partial class RepositoryImp
    {
        public string CreateStandingOrder(StandingOrderEntityData requestData)
        {
            EnsureRepositoryOpen("CreateStandingOrder");

            var standingOrder = new StandingOrderEntityImp();
            SetRequestEntityImpData(standingOrder, requestData);
            _allStandingOrders.Add(standingOrder);
            _persistenceHandler.SaveChanges(new SavingTask(FilePath, standingOrder.Clone()));

            return standingOrder.PersistentId;
        }

        internal void AddStandingOrder(StandingOrderEntityImp standingOrderEntity)
        {
            _allStandingOrders.Add(standingOrderEntity);
        }

        public void UpdateStandingOrder(string entityId, StandingOrderEntityData requestData)
        {
            EnsureRepositoryOpen("UpdateStandingOrder");

            var standingOrder = _allStandingOrders.Single(r => r.PersistentId == entityId);
            SetRequestEntityImpData(standingOrder, requestData);
            _persistenceHandler.SaveChanges(new SavingTask(FilePath, standingOrder.Clone()));
        }

        public void DeleteStandingOrder(string entityId)
        {
            EnsureRepositoryOpen("DeleteStandingOrder");

            var standingOrderEntity = _allStandingOrders.Single(r => r.PersistentId == entityId);
            _allStandingOrders.Remove(standingOrderEntity);
            _persistenceHandler.SaveChanges(new SavingTask(FilePath, entityId));
        }

        public StandingOrderEntity QueryStandingOrder(string entityId)
        {
            EnsureRepositoryOpen("QueryStandingOrder");

            return _allStandingOrders.Single(r => r.PersistentId == entityId);
        }

        public IEnumerable<StandingOrderEntity> QueryAllStandingOrderEntities()
        {
            EnsureRepositoryOpen("QueryAllStandingOrderEntities");

            return _allStandingOrders;
        }

        public void UpdateStandingOrdersToCurrentMonth()
        {
            var currentMonthLastDay = _applicationContext.Now.Date.LastDayOfMonth();
            var standingOrdersToUpdate = _allStandingOrders.Where(r => r.FirstBookDate.Date <= currentMonthLastDay).ToList();

            var task = new SavingTask(FilePath);

            foreach(var standingOrder in standingOrdersToUpdate)
            {
                var bookDate = standingOrder.GetNextPaymentDateTime();
                while(bookDate != null && bookDate.Value <= currentMonthLastDay)
                {
                    var newRequest = standingOrder.CreateRequest(bookDate.Value);
                    _allRequests.Add(newRequest);
                    standingOrder.LastBookedDate = bookDate.Value;
                    bookDate = standingOrder.GetNextPaymentDateTime();

                    task.RequestsToUpdate.Add(newRequest.Clone());
                    task.StandingOrdersToUpdate.Add(standingOrder.Clone());
                }
            }

            _persistenceHandler.SaveChanges(task);
        }

        private void SetRequestEntityImpData(StandingOrderEntityImp standingOrder, StandingOrderEntityData requestData)
        {
            standingOrder.Description = requestData.Description;
            standingOrder.FirstBookDate = requestData.FirstBookDate;
            standingOrder.MonthPeriodStep = requestData.MonthPeriodStep;
            standingOrder.ReferenceDay = requestData.ReferenceDay;
            standingOrder.ReferenceMonth = requestData.ReferenceMonth;
            standingOrder.Value = requestData.Value;

            if (!string.IsNullOrEmpty(requestData.CategoryEntityId))
            {
                standingOrder.Category = _allCategories.Single(c => c.PersistentId == requestData.CategoryEntityId);
            }
        }
    }
}
