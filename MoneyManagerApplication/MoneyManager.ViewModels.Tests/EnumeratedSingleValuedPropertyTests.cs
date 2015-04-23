using MoneyManager.ViewModels.Framework;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    class EnumeratedSingleValuedPropertyTests
    {
        [TestCase(new[]{ 1,2,3 }, 1, 2, 1)]
        [TestCase(new[] { 1, 2, 3 }, 1, 2, 1)]
        [TestCase(new[] { 1, 2 }, 1, 2, 1)]
        [TestCase(new[] { 1, 2 }, 1, 1, 2)]
        [TestCase(new[] { 1, 2 }, 2, 1, 2)]
        [TestCase(new[] { 1 }, 1, 1, 0)]
        public void RemoveValue(int[] available, int selected, int deleted, int expectedNewSelected)
        {
            var property = new EnumeratedSingleValuedProperty<int>();
            foreach (var i in available)
            {
                property.AddValue(i);
            }
            property.Value = selected;
            property.RemoveValue(deleted);

            Assert.That(property.Value, Is.EqualTo(expectedNewSelected));
            Assert.That(property.SelectableValues.Count, Is.EqualTo(available.Length-1));
        }
    }
}
