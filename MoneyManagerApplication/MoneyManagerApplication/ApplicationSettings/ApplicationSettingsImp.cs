using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MoneyManager.Interfaces;

namespace MoneyManagerApplication.ApplicationSettings
{
    class ApplicationSettingsImp : MoneyManager.Interfaces.ApplicationSettings
    {
        private readonly List<RecentAccountInformation> _recentAccountInformation;

        public ApplicationSettingsImp()
        {
            _recentAccountInformation = new List<RecentAccountInformation>();
            ParseRecentAccounts();
            RecentAccounts = _recentAccountInformation.AsReadOnly();
        }

        private void ParseRecentAccounts()
        {
            var text = Properties.Settings.Default.RecentAccounts;
            if (string.IsNullOrEmpty(text)) return;
            
            foreach (var token in text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                DateTime date;
                var expectedData = token.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                if (expectedData.Length < 0) continue;
                if (string.IsNullOrEmpty(expectedData[0])) continue;
                if (!DateTime.TryParse(expectedData[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) continue;

                CreateRecentAccount(expectedData[0], date);
            }
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
                CreateRecentAccount(path, lastAccessDate);
            }

            SaveToAppSettings();
        }

        private void CreateRecentAccount(string path, DateTime lastAccessDate)
        {
            _recentAccountInformation.Add(new RecentAccountInformationImp
            {
                LastAccessDate = lastAccessDate,
                Path = path
            });
        }

        private void SaveToAppSettings()
        {
            var text = string.Join(";",
                _recentAccountInformation.Select(
                    r => string.Format("{0}|{1}", r.Path, r.LastAccessDate.ToString(CultureInfo.InvariantCulture))));

            Properties.Settings.Default.RecentAccounts = text;
            Properties.Settings.Default.Save();
        }

        public void DeleteRecentAccountInformation(string path)
        {
            var existingItem = _recentAccountInformation.SingleOrDefault(r => r.Path == path);
            if (existingItem == null) throw new InvalidOperationException(string.Format("Recent Account Information in path '{0}' does not exist", path));

            _recentAccountInformation.Remove(existingItem);

            SaveToAppSettings();
        }
    }
}
