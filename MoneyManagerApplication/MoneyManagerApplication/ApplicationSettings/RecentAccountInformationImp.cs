using System;
using MoneyManager.Interfaces;

namespace MoneyManagerApplication.ApplicationSettings
{
    class RecentAccountInformationImp : RecentAccountInformation
    {
        public string Path { get; set; }
        public DateTime LastAccessDate { get; set; }
    }
}
