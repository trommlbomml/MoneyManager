using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    partial class RepositoryImp
    {
        internal List<RequestEntityImp> AllRequests { get { return _allRequests; } }

        internal void AddRequest(RequestEntityImp requestEntityImp)
        {
            if (requestEntityImp == null) throw new ArgumentNullException("requestEntityImp");
            if (_allRequests.Any(r => r.PersistentId == requestEntityImp.PersistentId))
            {
                throw new InvalidOperationException(string.Format("Entity with Id {0} alread exists.", requestEntityImp.PersistentId));
            }

            _allRequests.Add(requestEntityImp);
        }

        public IEnumerable<RequestEntity> QueryRequestsForSingleMonth(int year, int month)
        {
            EnsureRepositoryOpen("QueryRequestsForSingleMonth");

            return _allRequests.Where(r => r.Date.Year == year && r.Date.Month == month);
        }

        public RequestEntity QueryRequest(string persistentId)
        {
            EnsureRepositoryOpen("QueryRequest");

            var entity = _allRequests.SingleOrDefault(r => r.PersistentId == persistentId);
            if (entity == null) throw new ArgumentException(@"Entity with Id does not exist", persistentId);

            return entity;
        }

        public void UpdateRequest(string persistentId, RequestEntityData data)
        {
            EnsureRepositoryOpen("UpdateRequest");

            var request = _allRequests.Single(r => r.PersistentId == persistentId);
            SetRequestData(request, data);
        }

        public string CreateRequest(RequestEntityData data)
        {
            EnsureRepositoryOpen("CreateRequest");

            var request = new RequestEntityImp();
            SetRequestData(request, data);
            _allRequests.Add(request);

            return request.PersistentId;
        }

        private void SetRequestData(RequestEntityImp request, RequestEntityData data)
        {
            request.Date = data.Date;
            request.Description = data.Description;
            request.Value = data.Value;
            request.Category = _allCategories.SingleOrDefault(c => c.PersistentId == data.CategoryPersistentId);
        }

        public void DeleteRequest(string persistentId)
        {
            EnsureRepositoryOpen("DeleteRequest");
            _allRequests.Remove(_allRequests.Single(r => r.PersistentId == persistentId));
        }

        public double CalculateSaldoForMonth(int year, int month)
        {
            EnsureRepositoryOpen("CalculateSaldoForMonth");

            return _allRequests.Where(r => r.Date.Year <= year && (r.Date.Month <= month || r.Date.Year < year))
                               .Sum(r => r.Value);
        }
    }
}
