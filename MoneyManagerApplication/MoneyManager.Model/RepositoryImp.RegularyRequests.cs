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
        }

        public void DeleteStandingOrder(string entityId)
        {
            EnsureRepositoryOpen("DeleteStandingOrder");

            var standingOrderEntity = _allStandingOrders.Single(r => r.PersistentId == entityId);
            _allStandingOrders.Remove(standingOrderEntity);
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
            var requests = _allStandingOrders.Where(r => r.FirstBookDate.Date <= currentMonthLastDay).ToList();

            foreach(var request in requests)
            {
                var bookDate = request.GetNextPaymentDateTime();
                while(bookDate != null && bookDate.Value <= currentMonthLastDay)
                {
                    var newRequest = request.CreateRequest(bookDate.Value);
                    _allRequests.Add(newRequest);
                    request.LastBookedDate = bookDate.Value;
                    bookDate = request.GetNextPaymentDateTime();
                }
            }
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
