using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    internal partial class RepositoryImp : Repository, IDisposable
    {
        private string _currentRepositoryName;
        private readonly SingleUserFileLock _lockFile;
        private readonly List<RequestEntityImp> _allRequests;
        private readonly List<CategoryEntityImp> _allCategories;
        private readonly List<RegularyRequestEntityImp> _allRegularyRequests; 

        public RepositoryImp(SingleUserFileLock lockFile)
        {
            if (lockFile == null) throw new ArgumentNullException("lockFile");

            _lockFile = lockFile;
            _allRequests = new List<RequestEntityImp>();
            _allCategories = new List<CategoryEntityImp>();
            _allRegularyRequests = new List<RegularyRequestEntityImp>();
        }

        private void EnsureRepositoryOpen(string action)
        {
            if (!IsOpen)
            {
                throw new ApplicationException(string.Format("Repository is not opened. Operation {0} is not possible.", action));
            }
        }

        private void LockFile(string filePath)
        {
            if (!_lockFile.LockFile(filePath)) throw new ApplicationException(string.Format(Properties.Resources.ErrorLockingFile, filePath));
        }

        private void UnlockFile(string filePath)
        {
            _lockFile.UnlockFile(filePath);
        }

        public void Create(string path, string name)
        {
            if (IsOpen) throw new ApplicationException("Repository already open. Close first to create new.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(@"Invalid repository name. Must contain visible characters", "name");

            var targetFilePath = !SystemConstants.DatabaseExtension.Equals(Path.GetExtension(path))
                                            ? path + SystemConstants.DatabaseExtension
                                            : path;

            if (File.Exists(targetFilePath)) throw new ApplicationException("File already exists.");

            InternalSafe(targetFilePath, name);

            LockFile(targetFilePath);

            _currentRepositoryName = name;
            FilePath = targetFilePath;
        }

        public void Open(string path)
        {
            if (!string.IsNullOrEmpty(FilePath)) throw new ApplicationException("Repository already open. Close first to open.");
            if (!File.Exists(path)) throw new ApplicationException("Path does not exist.");

            LockFile(path);
            FilePath = path;

            var document = XDocument.Load(path);
// ReSharper disable PossibleNullReferenceException
            _currentRepositoryName = document.Root.Attribute("Name").Value;

            _allCategories.AddRange(document.Root.Element("Categories").Elements("Category").Select(e => new CategoryEntityImp(e)));
            _allRegularyRequests.AddRange(document.Root.Element("RegularyRequests").Elements("RegularyRequest").Select(e => new RegularyRequestEntityImp(e, _allCategories)));
            _allRequests.AddRange(document.Root.Element("Requests").Elements("Request").Select(e => new RequestEntityImp(e, _allCategories, _allRegularyRequests)));

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

            UnlockFile(FilePath);
            FilePath = null;
            ClearAll();
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
                    new XElement("RegularyRequests", _allRegularyRequests.Select(r => r.Serialize())),
                    new XElement("Requests", _allRequests.Select(r => r.Serialize()))
                    )
                );

            xmlDocument.Save(fileName);
        }
        
        internal void ClearAll()
        {
            _allRequests.Clear();
            _allCategories.Clear();
            _allRegularyRequests.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RepositoryImp()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnlockFile(FilePath);
            }
        }
    }
}
