using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
