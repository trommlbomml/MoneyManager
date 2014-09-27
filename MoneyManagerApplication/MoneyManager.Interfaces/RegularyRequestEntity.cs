using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Repräsentiert einen Betrag, der regelmäßig eingetragen wird.
    /// </summary>
    public interface RegularyRequestEntity
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
