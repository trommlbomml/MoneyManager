
using System;
using System.IO;
using NUnit.Framework;

namespace MoneyManager.Model.Tests
{
    //Todo: Tests für OpenTwice, CreateOpened.
    [TestFixture]
    public class RepositoryStateTests
    {
        [TearDown]
        public void TearDown()
        {
            foreach (var file in Directory.GetFiles(Path.GetTempPath()))
            {
                if (RepositoryImp.RepositoryExtension.Equals(Path.GetExtension(file)))
                {
                    try
                    {
                        File.Delete(file);
                    }
// ReSharper disable EmptyGeneralCatchClause
                    catch
// ReSharper restore EmptyGeneralCatchClause
                    {
                    }
                }
            }
        }

        [Test]
        public void CloseRepositoryWhenNotOpenedThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            Assert.That(repository.Close, Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void CloseTwiceRepositoryThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            repository.Create(RespositoryTests.GetUniqueFilePath(), "DefaultName");
            repository.Close();

            Assert.That(repository.Close, Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void OpenCreatedRepositoryThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            repository.Create(RespositoryTests.GetUniqueFilePath(), "DefaultName");
            
            Assert.That(() => repository.Open(""), Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void CreateRepositoryTwiceThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            repository.Create(RespositoryTests.GetUniqueFilePath(), "DefaultName");

            Assert.That(() => repository.Create("", "DefaultName"), Throws.InstanceOf<ApplicationException>());
        }
    }
}
