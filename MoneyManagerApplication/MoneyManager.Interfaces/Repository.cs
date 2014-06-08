
using System.Collections.Generic;

namespace MoneyManager.Interfaces
{
    public interface Repository
    {
        void Save();

        void Load(string fileName);

        IEnumerable<RequestEntity> QueryRequestsForSingleMonth(int year, int month);

        void UpdateRequest(string persistentId, RequestEntityData data);

        RequestEntity QueryRequest(string persistentId);
    }
}
