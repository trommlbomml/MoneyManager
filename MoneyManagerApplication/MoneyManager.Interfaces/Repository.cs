using System.Collections.Generic;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Represents data of a MoneyManager Database.
    /// </summary>
    public interface Repository
    {
        /// <summary>
        /// Creates a new database at a specific path and name,
        /// </summary>
        /// <param name="path">Absolute path of the file to create.</param>
        /// <param name="name">Name of the file.</param>
        void Create(string path, string name);

        /// <summary>
        /// Opens the repository.
        /// </summary>
        /// <param name="path">Absolute path of the file.</param>
        void Open(string path);

        /// <summary>
        /// Checks whether the repository is open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// The name of the opened repository.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Absolute Path to the file.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Closes an opened repository.
        /// </summary>
        void Close();

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

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns>Collection of all categories.</returns>
        IEnumerable<CategoryEntity> QueryAllCategories();

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="name">Name of category</param>
        /// <returns>PersistentId of Category</returns>
        string CreateCategory(string name);

        /// <summary>
        /// Queries a single Category.
        /// </summary>
        /// <param name="persistentId">PersistentId</param>
        /// <returns>Category</returns>
        CategoryEntity QueryCategory(string persistentId);

        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <param name="persistentId">EntityId</param>
        /// <param name="name">Name</param>
        void UpdateCategory(string persistentId, string name);

        /// <summary>
        /// Deletes a Category.
        /// </summary>
        /// <param name="persistentId">PersistentId</param>
        void DeleteCategory(string persistentId);

        /// <summary>
        /// Creates a regulary request.
        /// </summary>
        /// <param name="requestData">Entity data</param>
        string CreateStandingOrder(StandingOrderEntityData requestData);

        /// <summary>
        /// Updates a regulary request.
        /// </summary>
        /// <param name="entityId">Entity Id</param>
        /// <param name="requestData">data</param>
        void UpdateStandingOrder(string entityId, StandingOrderEntityData requestData);

        /// <summary>
        /// Deletes a regulary request. All referencing Requests are converted to regular requests.
        /// </summary>
        /// <param name="entityId">Entity Id</param>
        void DeleteStandingOrder(string entityId);

        /// <summary>
        /// Delivers single regulary request.
        /// </summary>
        /// <param name="entityId">Entity id</param>
        StandingOrderEntity QueryStandingOrder(string entityId);

        /// <summary>
        /// Delivers all regulary requests.
        /// </summary>
        /// <returns>Regulary requests.</returns>
        IEnumerable<StandingOrderEntity> QueryAllStandingOrderEntities();

        /// <summary>
        /// Calculates the sum of Requests up to month.
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>Sum of Requests</returns>
        double CalculateSaldoForMonth(int year, int month);

        /// <summary>
        /// Updates all regulary Requests to the current month and returns all created requests.
        /// </summary>
        /// <returns>newly created Request-Entities.</returns>
        string[] UpdateStandingOrdersToCurrentMonth();
    }
}
