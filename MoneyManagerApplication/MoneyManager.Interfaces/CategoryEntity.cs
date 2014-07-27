
namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Entity representing a category.
    /// </summary>
    public interface CategoryEntity
    {
        /// <summary>
        /// PersistentId of Entity.
        /// </summary>
        string PersistentId { get; }

        /// <summary>
        /// Name of the Category.
        /// </summary>
        string Name { get; }
    }
}
