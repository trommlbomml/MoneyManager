using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    public static class RepositoryFactory
    {
        public static Repository CreateRepository(SingleUserFileLock fileLock)
        {
            return new RepositoryImp(fileLock);
        }
    }
}
