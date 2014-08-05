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
        private readonly List<CategoryEntityImp> _allCategories; 

        public RepositoryImp()
        {
            _allRequests = new List<RequestEntityImp>();
            _allCategories = new List<CategoryEntityImp>();
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

        internal void AddCategory(CategoryEntityImp categoryEntity)
        {
            if (categoryEntity == null) throw new ArgumentNullException("categoryEntity");
            if (_allCategories.Any(r => r.PersistentId == categoryEntity.PersistentId))
            {
                throw new InvalidOperationException(string.Format("Entity with Id {0} alread exists.", categoryEntity.PersistentId));
            }

            _allCategories.Add(categoryEntity);
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

            _allCategories.AddRange(document.Root.Element("Categories").Elements("Category").Select(e => new CategoryEntityImp(e)));

            _allRequests.AddRange(document.Root.Element("Requests").Elements("Request").Select(e => new RequestEntityImp(e, _allCategories)));

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
                Value = data.Value,
                Category = string.IsNullOrEmpty(data.CategoryPersistentId) ? null : _allCategories.Single(c => c.PersistentId == data.CategoryPersistentId)
            };
            _allRequests.Add(request);

            return request.PersistentId;
        }

        public void DeleteRequest(string persistentId)
        {
            EnsureRepositoryOpen("DeleteRequest");

            _allRequests.Remove(_allRequests.Single(r => r.PersistentId == persistentId));
        }

        public IEnumerable<CategoryEntity> QueryAllCategories()
        {
            EnsureRepositoryOpen("CreateRequest");

            return _allCategories;
        }

        public string CreateCategory(string name)
        {
            EnsureRepositoryOpen("CreateRequest");

            var categoryImp = new CategoryEntityImp { Name = name };
            _allCategories.Add(categoryImp);

            return categoryImp.PersistentId;
        }

        public CategoryEntity QueryCategory(string persistentId)
        {
            EnsureRepositoryOpen("QueryCategory");

            var entity = _allCategories.SingleOrDefault(c => c.PersistentId == persistentId);
            if (entity == null) throw new ArgumentException("Entity with Id does not exist", persistentId);

            return entity;
        }

        public void UpdateCategory(string persistentId, string name)
        {
            EnsureRepositoryOpen("CreateRequest");

            var category = _allCategories.SingleOrDefault(c => c.PersistentId == persistentId);
            if (category == null) throw new ArgumentException("The category does not exist or more than once", "persistentId");

            category.Name = name;
        }

        public void DeleteCategory(string persistentId)
        {
            EnsureRepositoryOpen("CreateRequest");

            var categoryEntityImp = _allCategories.Single(r => r.PersistentId == persistentId);
            _allCategories.Remove(categoryEntityImp);

            foreach (var request in _allRequests.Where(r => r.Category != null && r.Category.PersistentId == categoryEntityImp.PersistentId))
            {
                request.Category = null;
            }
        }

        public double CalculateSaldoForMonth(int year, int month)
        {
            EnsureRepositoryOpen("CalculateSaldoForMonth");

            return _allRequests.Where(r => r.Date.Year <= year && (r.Date.Month <= month || r.Date.Year < year))
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
                    new XElement("Categories", _allCategories.Select(c => c.Serialize())),
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
            request.Category = _allCategories.SingleOrDefault(c => c.PersistentId == data.CategoryPersistentId);
        }

        internal void ClearAll()
        {
            _allRequests.Clear();
            _allCategories.Clear();
        }
    }
}
