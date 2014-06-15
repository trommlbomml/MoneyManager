using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    internal class RepositoryImp : Repository
    {
        private string _currentRepositoryName;
        private readonly List<RequestEntityImp> _allRequests;

        public RepositoryImp()
        {
            _allRequests = new List<RequestEntityImp>();
        }

        internal List<RequestEntityImp> AllRequests {get { return _allRequests; }}

        internal void AddRequest(RequestEntityImp requestEntityImp)
        {
            if (requestEntityImp == null) throw new ArgumentNullException("requestEntityImp");
            if (_allRequests.Any(r => r.PersistentId == requestEntityImp.PersistentId))
            {
                throw new InvalidOperationException(string.Format("Entity with Id {0} alread exists.", requestEntityImp.PersistentId));
            }

            _allRequests.Add(requestEntityImp);
        }

        private void EnsureRepositoryOpen(string action)
        {
            if (!IsOpen)
            {
                throw new ApplicationException(string.Format("Repository is not opened. Operation {0} is not possible.", action));
            }
        }

        public void Create(string path, string name)
        {
            if (IsOpen) throw new ApplicationException("Repository already open. Close first to create new.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid repository name. Must contain visible characters", "name");

            var targetFilePath = !SystemConstants.DatabaseExtension.Equals(Path.GetExtension(path))
                                            ? path + SystemConstants.DatabaseExtension
                                            : path;

            if (File.Exists(targetFilePath)) throw new ApplicationException("File already exists.");

            InternalSafe(targetFilePath, name);

            _currentRepositoryName = name;
            FilePath = targetFilePath;
        }

        public void Open(string path)
        {
            if (!string.IsNullOrEmpty(FilePath)) throw new ApplicationException("Repository already open. Close first to open.");

            if (!File.Exists(path)) throw new ApplicationException("Path does not exist.");

            FilePath = path;

            var document = XDocument.Load(path);
// ReSharper disable PossibleNullReferenceException
            _currentRepositoryName = document.Root.Attribute("Name").Value;

            _allRequests.AddRange(document.Root.Element("Requests").Elements("Request").Select(e => new RequestEntityImp(e)));

// ReSharper restore PossibleNullReferenceException
        }

        public bool IsOpen { get { return !string.IsNullOrEmpty(FilePath); } }

        public string Name 
        {
            get { return IsOpen ? _currentRepositoryName : string.Empty; }
        }

        public string FilePath { get; private set; }

        public void Close()
        {
            EnsureRepositoryOpen("Close");

            FilePath = null;
            ClearAll();
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
            if (entity == null) throw new ArgumentException("Entity with Id does not exist", persistentId);

            return entity;
        }

        public string CreateRequest(RequestEntityData data)
        {
            EnsureRepositoryOpen("CreateRequest");

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
            EnsureRepositoryOpen("DeleteRequest");

            _allRequests.Remove(_allRequests.Single(r => r.PersistentId == persistentId));
        }

        public double CalculateSaldoForMonth(int year, int month)
        {
            EnsureRepositoryOpen("CalculateSaldoForMonth");

            return _allRequests.Where(r => r.Date.Year <= year && r.Date.Month <= month)
                               .Sum(r => r.Value);
        }

        public void Save()
        {
            EnsureRepositoryOpen("Save");

            InternalSafe(FilePath, _currentRepositoryName);
        }

        private void InternalSafe(string fileName, string name)
        {
            var xmlDocument = new XDocument(
                new XElement("MoneyManagerAccount",
                    new XAttribute("Name", name),
                    new XElement("Requests", _allRequests.Select(r => r.Serialize()))
                    )
                );

            xmlDocument.Save(fileName);
        }

        public void UpdateRequest(string persistentId, RequestEntityData data)
        {
            EnsureRepositoryOpen("UpdateRequest");

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
