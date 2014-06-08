using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
            var entity = _allRequests.SingleOrDefault(r => r.PersistentId == persistentId);
            if (entity == null) throw new ArgumentException("Entity with Id does not exist", persistentId);

            return entity;
        }

        public string CreateRequest(RequestEntityData data)
        {
            var request = new RequestEntityImp
            {
                Date = data.Date.Date,
                Description = data.Description,
                Value = data.Value
            };
            _allRequests.Add(request);

            return request.PersistentId;
        }

        public void DeleteRequest(string persistentId)
        {
            _allRequests.Remove(_allRequests.Single(r => r.PersistentId == persistentId));
        }

        public double CalculateSaldoForMonth(int year, int month)
        {
            return _allRequests.Where(r => r.Date.Year <= year && r.Date.Month <= month)
                               .Sum(r => r.Value);
        }

        public void Save(string fileName)
        {
            var xmlDocument = new XDocument(
                                    new XElement("MoneyManagerAccount",
                                            new XElement("Requests", _allRequests.Select(r => r.Serialize()))
                                            )
                                    );

            xmlDocument.Save(fileName);
        }

        public void UpdateRequest(string persistentId, RequestEntityData data)
        {
            var request = _allRequests.SingleOrDefault(r => r.PersistentId == persistentId);
            if (request == null) throw new ArgumentException("Entity with Id does not exist", persistentId);

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
