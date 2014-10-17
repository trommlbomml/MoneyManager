using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneyManager.Model
{
    static class Extensions
    {
        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            var lastDay = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTime(dateTime.Year, dateTime.Month, lastDay);
        }
    }
}
