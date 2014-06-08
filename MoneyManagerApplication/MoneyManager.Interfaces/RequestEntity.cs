using System;

namespace MoneyManager.Interfaces
{
    public interface RequestEntity
    {
        double Value { get; }
        string Description { get; }
        DateTime Date { get; }
        string PersistentId { get; }
    }
}