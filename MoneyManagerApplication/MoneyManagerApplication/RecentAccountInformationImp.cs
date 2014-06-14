using System;
using MoneyManager.Interfaces;

namespace MoneyManagerApplication
{
    class RecentAccountInformationImp : RecentAccountInformation
    {
        public string Path { get; set; }
        public DateTime LastAccessDate { get; set; }
    }
}
