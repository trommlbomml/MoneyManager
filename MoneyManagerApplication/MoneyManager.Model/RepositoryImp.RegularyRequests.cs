
using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    partial class RepositoryImp
    {
        public string CreateRegularyRequest(RegularyRequestEntityData requestData)
        {
            EnsureRepositoryOpen("CreateRegularyRequest");

            var regularyRequest = new RegularyRequestEntityImp();
            SetRequestEntityImpData(regularyRequest, requestData);
            _allRegularyRequests.Add(regularyRequest);

            return regularyRequest.PersistentId;
        }

        internal void AddRegularyRequest(RegularyRequestEntityImp regularyRequestEntity)
        {
            _allRegularyRequests.Add(regularyRequestEntity);
        }

        public void UpdateRegularyRequest(string entityId, RegularyRequestEntityData requestData)
        {
            EnsureRepositoryOpen("UpdateRegularyRequest");

            var regularyRequest = _allRegularyRequests.Single(r => r.PersistentId == entityId);
            SetRequestEntityImpData(regularyRequest, requestData);
        }

        public void DeleteRegularyRequest(string entityId)
        {
            EnsureRepositoryOpen("DeleteRegularyRequest");

            var regularyRequestEntity = _allRegularyRequests.Single(r => r.PersistentId == entityId);

            _allRegularyRequests.Remove(regularyRequestEntity);
        }

        public RegularyRequestEntity QueryRegularyRequest(string entityId)
        {
            EnsureRepositoryOpen("QueryRegularyRequest");

            return _allRegularyRequests.Single(r => r.PersistentId == entityId);
        }

        public IEnumerable<RegularyRequestEntity> QueryAllRegularyRequestEntities()
        {
            EnsureRepositoryOpen("QueryAllRegularyRequestEntities");

            return _allRegularyRequests;
        }

        public void UpdateRegularyRequestsToCurrentMonth()
        {
            var currentMonthLastDay = _applicationContext.Now.Date.LastDayOfMonth();
            var requests = _allRegularyRequests.Where(r => r.FirstBookDate.Date <= currentMonthLastDay).ToList();

            foreach(var request in requests)
            {
                var bookDate = request.GetNextPaymentDateTime();
                while(bookDate <= currentMonthLastDay)
                {
                    var newRequest = request.CreateRequest(bookDate);
                    _allRequests.Add(newRequest);
                    request.LastBookDate = bookDate;
                    bookDate = request.GetNextPaymentDateTime();
                }
            }
        }

        private void SetRequestEntityImpData(RegularyRequestEntityImp regularyRequest, RegularyRequestEntityData requestData)
        {
            regularyRequest.Description = requestData.Description;
            regularyRequest.FirstBookDate = requestData.FirstBookDate;
            regularyRequest.MonthPeriodStep = requestData.MonthPeriodStep;
            regularyRequest.ReferenceDay = requestData.ReferenceDay;
            regularyRequest.ReferenceMonth = requestData.ReferenceMonth;
            regularyRequest.Value = requestData.Value;

            if (!string.IsNullOrEmpty(requestData.CategoryEntityId))
            {
                regularyRequest.Category = _allCategories.Single(c => c.PersistentId == requestData.CategoryEntityId);
            }
        }
    }
}
