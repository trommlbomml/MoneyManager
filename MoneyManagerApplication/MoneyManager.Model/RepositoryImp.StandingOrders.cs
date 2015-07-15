using System;
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

        public string[] UpdateStandingOrdersToCurrentMonth(int year, int month)
        {
            var currentMonthLastDay = new DateTime(year, month,1).LastDayOfMonth();
            var standingOrdersToUpdate = _allStandingOrders.Where(r => r.FirstBookDate.Date <= currentMonthLastDay).ToList();

            var task = new SavingTask(FilePath);

            var newCreatedRequestsEntityIds = new List<string>();

            foreach(var standingOrder in standingOrdersToUpdate)
            {
                var bookDate = standingOrder.GetNextPaymentDateTime();
                while(bookDate != null && bookDate.Value <= currentMonthLastDay)
                {
                    var newRequest = standingOrder.CreateRequest(bookDate.Value);
                    _allRequests.Add(newRequest);
                    newCreatedRequestsEntityIds.Add(newRequest.PersistentId);
                    standingOrder.LastBookedDate = bookDate.Value;
                    bookDate = standingOrder.GetNextPaymentDateTime();

                    task.RequestsToUpdate.Add(newRequest.Clone());
                }
                task.StandingOrdersToUpdate.Add(standingOrder.Clone());
            }

            _persistenceHandler.SaveChanges(task);

            return newCreatedRequestsEntityIds.ToArray();
        }

        private void SetRequestEntityImpData(StandingOrderEntityImp standingOrder, StandingOrderEntityData data)
        {
            standingOrder.Description = data.Description;
            standingOrder.FirstBookDate = data.FirstBookDate;
            standingOrder.MonthPeriodStep = data.MonthPeriodStep;
            standingOrder.ReferenceDay = data.ReferenceDay;
            standingOrder.ReferenceMonth = data.ReferenceMonth;
            standingOrder.Value = data.Value;

            if (data.PaymentCount.HasValue)
            {
                var monthsAddToFirstBookDate = data.MonthPeriodStep * data.PaymentCount.Value;
                standingOrder.LastBookDate = standingOrder.FirstBookDate.AddMonths(monthsAddToFirstBookDate);
            }
            else
            {
                standingOrder.LastBookDate = DateTime.MaxValue;
            }

            if (!string.IsNullOrEmpty(data.CategoryEntityId))
            {
                standingOrder.Category = _allCategories.Single(c => c.PersistentId == data.CategoryEntityId);
            }
        }
    }
}
