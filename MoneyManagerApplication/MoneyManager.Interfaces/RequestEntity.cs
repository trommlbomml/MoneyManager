using System;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Represents a Reqest Entity.
    /// </summary>
    public interface RequestEntity
    {
        /// <summary>
        /// PersistentId of Entity.
        /// </summary>
        string PersistentId { get; }

        /// <summary>
        /// Value of Money.
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Date of Value booking.
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        /// Category.
        /// </summary>
        CategoryEntity Category { get; }
    }
}