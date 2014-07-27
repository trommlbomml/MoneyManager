using System;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Information about a recent opened account.
    /// </summary>
    public interface RecentAccountInformation
    {
        /// <summary>
        /// Absolute path to file.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Last access date.
        /// </summary>
        DateTime LastAccessDate { get; }
    }
}