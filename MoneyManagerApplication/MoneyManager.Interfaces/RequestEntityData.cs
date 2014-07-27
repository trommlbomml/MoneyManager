using System;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Data Object for updating Request Entity.
    /// </summary>
    public class RequestEntityData
    {
        /// <summary>
        /// Date of Booking.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Referenced category. Can be null.
        /// </summary>
        public string CategoryPersistentId { get; set; }
    }
}
