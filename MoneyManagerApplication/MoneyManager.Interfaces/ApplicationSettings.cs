using System;
using System.Collections.Generic;

namespace MoneyManager.Interfaces
{
    public interface ApplicationSettings
    {
        IReadOnlyCollection<RecentAccountInformation> RecentAccounts { get; }

        void UpdateRecentAccountInformation(string path, DateTime lastAccessDate);

        void DeleteRecentAccountInformation(string path);
    }
}
