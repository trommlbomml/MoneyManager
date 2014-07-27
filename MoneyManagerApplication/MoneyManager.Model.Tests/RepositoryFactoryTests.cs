
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class RepositoryFactoryTests
    {
        [Test]
        public void FactoryCreatesInstance()
        {
            var repository = RepositoryFactory.CreateRepository();
            Assert.That(repository, Is.Not.Null);
        }
    }
}
