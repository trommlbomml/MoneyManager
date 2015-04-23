using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Model
{
    internal interface DataPersistenceHandler
    {
        void SaveChanges(string filePath, SavingTask task);
        void WaitForAllTasksFinished();
    }
}
