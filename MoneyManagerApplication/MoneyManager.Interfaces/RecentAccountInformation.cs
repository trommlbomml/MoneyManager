using System;

namespace MoneyManager.Interfaces
{
    public interface RecentAccountInformation
    {
        string Path { get; }
        DateTime LastAccessDate { get; }
    }
}