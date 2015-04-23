
namespace MoneyManager.Model
{
    internal interface DataPersistenceHandler
    {
        void SaveChanges(SavingTask task);
        void WaitForAllTasksFinished();
    }
}
