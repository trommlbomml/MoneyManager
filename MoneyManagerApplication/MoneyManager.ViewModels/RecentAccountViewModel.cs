using System;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class RecentAccountViewModel : ViewModel
    {
        private DateTime _lastAccessDate;
        private string _path;
        private string _lastAccessDateAsString;

        public RecentAccountViewModel()
        {
            UpdateLocalizedProperties();
        }

        private void UpdateLocalizedProperties()
        {
            LastAccessDateAsString = string.Format(Properties.Resources.LastAccesDateFormat, LastAccessDate);
        }

        public string LastAccessDateAsString
        {
            get { return _lastAccessDateAsString; }
            private set { SetBackingField("LastAccessDateAsString", ref _lastAccessDateAsString, value, o => UpdateLocalizedProperties()); }
        }

        public DateTime LastAccessDate
        {
            get { return _lastAccessDate; }
            set { SetBackingField("LastAccessDate", ref _lastAccessDate, value); }
        }

        public string Path
        {
            get { return _path; }
            set { SetBackingField("Path", ref _path, value); }
        }
    }
}
