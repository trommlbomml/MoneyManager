
namespace MoneyManager.Model
{
    internal interface DataPersistenceHandler
    {
        void SaveChanges(string filePath, SavingTask task);
        void WaitForAllTasksFinished();
    }
}
