
using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    [TestFixture]
    public class RepositoryFactoryTests
    {
        [Test]
        public void FactoryCreatesInstance()
        {
            var repository = RepositoryFactory.CreateRepository(Substitute.For<ApplicationContext>());
            Assert.That(repository, Is.Not.Null);
        }
    }
}
