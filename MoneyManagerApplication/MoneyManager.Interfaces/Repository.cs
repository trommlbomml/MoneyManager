
using System.Collections.Generic;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Represents data of a MoneyManager Database.
    /// </summary>
    public interface Repository
    {
        /// <summary>
        /// Queries all Requests for a Single Month of a Year.
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>Collection of Requests of this Month.</returns>
        IEnumerable<RequestEntity> QueryRequestsForSingleMonth(int year, int month);

        /// <summary>
        /// Updates data of a Request.
        /// </summary>
        /// <param name="persistentId">EntityId of Entry to update.</param>
        /// <param name="data">Data to add.</param>
        void UpdateRequest(string persistentId, RequestEntityData data);

        /// <summary>
        /// Gets Request with Id.
        /// </summary>
        /// <param name="persistentId">EntityId</param>
        /// <returns>Entity with Id.</returns>
        RequestEntity QueryRequest(string persistentId);

        /// <summary>
        /// Creates New Request Entity.
        /// </summary>
        /// <param name="data">Data of Request</param>
        /// <returns>PersistentId of Request.</returns>
        string CreateRequest(RequestEntityData data);

        /// <summary>
        /// Deletes Request Entry.
        /// </summary>
        /// <param name="persistentId"></param>
        void DeleteRequest(string persistentId);

        double CalculateSaldoForMonth(int year, int month);
    }
}
