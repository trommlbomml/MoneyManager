using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    internal class RepositoryImp : Repository
    {
        private readonly List<RequestEntityImp> _allRequests;

        public RepositoryImp()
        {
            _allRequests = new List<RequestEntityImp>();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Load(string fileName)
        {
            throw new NotImplementedException();
        }

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
            return _allRequests.Where(r => r.Date.Year == year && r.Date.Month == month);
        }

        public RequestEntity QueryRequest(string persistentId)
        {
            return _allRequests.Single(r => r.PersistentId == persistentId);
        }

        public void UpdateRequest(string persistentId, RequestEntityData data)
        {
            var request = _allRequests.Single(r => r.PersistentId == persistentId);
            request.Date = data.Date;
            request.Description = data.Description;
            request.Value = data.Value;
        }

        internal void ClearAll()
        {
            _allRequests.Clear();
        }
    }
}
