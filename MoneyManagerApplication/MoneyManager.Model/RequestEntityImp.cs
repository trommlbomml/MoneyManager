using System;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    internal class RequestEntityImp : Entity, RequestEntity
    {
        public double Value { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }
    }
}
