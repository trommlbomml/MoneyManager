using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManager.Interfaces;

namespace MoneyManagerApplication
{
    class ApplicationSettingsImp : ApplicationSettings
    {
        private readonly List<RecentAccountInformation> _recentAccountInformation;

        public ApplicationSettingsImp()
        {
            _recentAccountInformation = new List<RecentAccountInformation>();
            RecentAccounts = _recentAccountInformation.AsReadOnly();
        }

        public IReadOnlyCollection<RecentAccountInformation> RecentAccounts { get; private set; }

        public void UpdateRecentAccountInformation(string path, DateTime lastAccessDate)
        {
            var existingItem = _recentAccountInformation.FirstOrDefault(r => r.Path == path) as RecentAccountInformationImp;
            if (existingItem != null)
            {
                existingItem.LastAccessDate = lastAccessDate;
            }
            else
            {
                _recentAccountInformation.Add(new RecentAccountInformationImp
                {
                    LastAccessDate = lastAccessDate,
                    Path = path
                });
            }
        }

        public void DeleteRecentAccountInformation(string path)
        {
            var existingItem = _recentAccountInformation.SingleOrDefault(r => r.Path == path);
            if (existingItem == null) throw new InvalidOperationException(string.Format("Recent Account Information in path '{0}' does not exist", path));

            _recentAccountInformation.Remove(existingItem);
        }
    }
}
