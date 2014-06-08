using System;

namespace MoneyManager.Interfaces
{
    public class RequestEntityData
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Description { get; set; }
    }
}
