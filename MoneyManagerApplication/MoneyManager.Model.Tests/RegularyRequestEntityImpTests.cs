
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class RegularyRequestEntityImpTests
    {
        [TestCase(1, 1, 1, true)]
        [TestCase(1, 1, 2, true)]
        [TestCase(1, 1, 3, true)]
        [TestCase(1, 1, 7, true)]
        [TestCase(2, 1, 3, true)]
        [TestCase(2, 1, 5, true)]
        [TestCase(2, 1, 7, true)]
        [TestCase(2, 1, 11, true)]
        [TestCase(3, 1, 1, true)]
        [TestCase(3, 1, 4, true)]
        [TestCase(3, 1, 10, true)]
        [TestCase(6, 1, 1, true)]
        [TestCase(6, 1, 7, true)]
        [TestCase(12, 1, 1, true)]
        [TestCase(2, 1, 4, false)]
        [TestCase(2, 2, 5, false)]
        [TestCase(3, 1, 3, false)]
        [TestCase(3, 2, 4, false)]
        [TestCase(6, 1, 3, false)]
        [TestCase(6, 4, 1, false)]
        [TestCase(12, 4, 5, false)]
        public void IsMonthOfPeriod(int period, int referenceMonth, int month, bool expectedIsPeriodMonth)
        {
            var entity = new RegularyRequestEntityImp
            {
                MonthPeriodStep = period,
                ReferenceDay = 1,
                ReferenceMonth = referenceMonth
            };

            Assert.That(entity.IsMonthOfPeriod(month), Is.EqualTo(expectedIsPeriodMonth));
        }
    }
}

