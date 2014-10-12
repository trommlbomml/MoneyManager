namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Handles Files to be locked/unlocked.
    /// </summary>
    public interface SingleUserFileLock
    {
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
