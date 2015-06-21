using System;

namespace MoneyManager.Interfaces
{
    public class StandingOrderEntityData
    {
        public DateTime FirstBookDate { get; set; }
        public int ReferenceMonth { get; set; }
        public int ReferenceDay { get; set; }
        public int MonthPeriodStep { get; set; }
        public double Value { get; set; }
        public string Description { get; set; }
        public string CategoryEntityId { get; set; }
    }
}