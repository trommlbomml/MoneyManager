
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

            var allRequestsReferencing = _allRequests.Where(r => r.RegularyRequest == regularyRequestEntity).ToArray();
            foreach (var requestEntityImp in allRequestsReferencing)
            {
                requestEntityImp.Category = regularyRequestEntity.Category;
                requestEntityImp.Description = regularyRequestEntity.Description;
                requestEntityImp.Value = regularyRequestEntity.Value;
                requestEntityImp.RegularyRequest = null;
            }

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

        private IEnumerable<RegularyRequestEntity> QueryRegularyRequestsForMonth(int year, int month)
        {
            EnsureRepositoryOpen("QueryAllRegularyRequestEntities");

            var currentMonthDateTime = new DateTime(year, month, 1);
            return _allRegularyRequests.Where(r => r.FirstBookDate <= currentMonthDateTime && r.IsMonthOfPeriod(month));
        }

        private IEnumerable<RegularyRequestEntity> QueryRegularRequestsUpToMonth(int year, int month)
        {
            var currentMonthDateTime = new DateTime(year, month, 1);
            return _allRegularyRequests.Where(r => r.FirstBookDate <= currentMonthDateTime && r.IsMonthOfPeriod(month));
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
