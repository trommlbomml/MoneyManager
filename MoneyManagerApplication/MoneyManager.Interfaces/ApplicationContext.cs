using System;
using System.Collections.Generic;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Saving Application specific settings.
    /// </summary>
    public interface ApplicationContext
    {
        /// <summary>
        /// Collection of recent opened accounts.
        /// </summary>
        IReadOnlyCollection<RecentAccountInformation> RecentAccounts { get; }

        /// <summary>
        /// Update Recent information of account.
        /// </summary>
        /// <param name="path">Absolute path of account.</param>
        void UpdateRecentAccountInformation(string path);

        /// <summary>
        /// Removes recent account from settings.
        /// </summary>
        /// <param name="path">Absolute path of account.</param>
        void DeleteRecentAccountInformation(string path);

        /// <summary>
        /// Delivers current Date and Time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Version der Anwendung.
        /// </summary>
        Version ApplicationVersion { get; }

        /// <summary>
        /// Lock File for usage.
        /// </summary>
        /// <param name="filePath"></param>
        bool LockFile(string filePath);

        /// <summary>
        /// Unlocks file for usage.
        /// </summary>
        /// <param name="filePath"></param>
        void UnlockFile(string filePath);
    }
}
