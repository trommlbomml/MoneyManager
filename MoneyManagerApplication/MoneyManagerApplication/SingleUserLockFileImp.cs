using System.IO;
 using MoneyManager.Interfaces;

namespace MoneyManagerApplication
{
    class SingleUserLockFileImp : SingleUserFileLock
    {
        public bool LockFile(string filePath)
        {
            var containingFolder = Path.GetDirectoryName(filePath);
            var targetFolder = Path.Combine(containingFolder ?? "", ".lock");

            if (Directory.Exists(targetFolder)) return false;

            Directory.CreateDirectory(targetFolder);

            return true;
        }

        public void UnlockFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            var containingFolder = Path.GetDirectoryName(filePath);
            var targetFolder = Path.Combine(containingFolder ?? "", ".lock");

            if (Directory.Exists(targetFolder)) Directory.Delete(targetFolder);
        }
    }
}
