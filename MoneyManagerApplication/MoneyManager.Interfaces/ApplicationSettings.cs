using System;
using System.Collections.Generic;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Saving Application specific settings.
    /// </summary>
    public interface ApplicationSettings
    {
        /// <summary>
        /// Collection of recent opened accounts.
        /// </summary>
        IReadOnlyCollection<RecentAccountInformation> RecentAccounts { get; }

        /// <summary>
        /// Update Recent information of account.
        /// </summary>
        /// <param name="path">Absolute path of account.</param>
        /// <param name="lastAccessDate">Last access date.</param>
        void UpdateRecentAccountInformation(string path, DateTime lastAccessDate);

        /// <summary>
        /// Removes recent account from settings.
        /// </summary>
        /// <param name="path">Absolute path of account.</param>
        void DeleteRecentAccountInformation(string path);
    }
}
