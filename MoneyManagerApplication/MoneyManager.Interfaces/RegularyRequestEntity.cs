using System;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Repräsentiert einen Betrag, der regelmäßig eingetragen wird.
    /// </summary>
    public interface StandingOrderEntity
    {
        /// <summary>
        /// PersistentId of Entity.
        /// </summary>
        string PersistentId { get; }

        /// <summary>
        /// Datum, an dem der Betrag das erste Mal verrechnet werden soll.
        /// </summary>
        DateTime FirstBookDate { get; }

        /// <summary>
        /// Das letzte Datum, an dem eine Buchung eingetragen worden ist.
        /// </summary>
        DateTime LastBookedDate { get; }

        /// <summary>
        /// Das letzte Datum, an dem eine Buchung vorgenommen werden soll.
        /// </summary>
        DateTime LastBookDate { get; }

        /// <summary>
        /// Referenzmonat, an dem verrechnet wird.
        /// </summary>
        int ReferenceMonth { get; }

        /// <summary>
        /// Referenztag, an dem verrechnet wird.
        /// </summary>
        int ReferenceDay { get; }

        /// <summary>
        /// Wie viele Monate jeweils zwischen den Wiederholungen sind.
        /// </summary>
        int MonthPeriodStep { get; }

        /// <summary>
        /// Value of Money.
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Category.
        /// </summary>
        CategoryEntity Category { get; }
    }
}
