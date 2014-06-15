
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MoneyManager.Interfaces;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
                if (SystemConstants.DatabaseExtension.Equals(Path.GetExtension(file)))
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
            Assert.That(repository.FilePath, Is.Null.Or.Empty);
        }

        [Test]
        public void CloseTwiceRepositoryThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            repository.Create(RespositoryTests.GetUniqueFilePath(), "DefaultName");
            repository.Close();

            Assert.That(repository.Close, Throws.InstanceOf<ApplicationException>());
            Assert.That(repository.FilePath, Is.Null.Or.Empty);
        }

        [Test]
        public void OpenCreatedRepositoryThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            var uniqueFilePath = RespositoryTests.GetUniqueFilePath(true);
            repository.Create(uniqueFilePath, "DefaultName");
            
            Assert.That(() => repository.Open(""), Throws.InstanceOf<ApplicationException>());
            Assert.That(repository.FilePath, Is.EqualTo(uniqueFilePath));
        }

        [Test]
        public void CreateRepositoryTwiceThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            repository.Create(RespositoryTests.GetUniqueFilePath(), "DefaultName");

            Assert.That(() => repository.Create("", "DefaultName"), Throws.InstanceOf<ApplicationException>());
        }

        internal static void AssertFileContentIsCorrect(string file, string repositoryName, List<RequestEntityImp> allRequests = null)
        {
            var document = new XmlDocument();
            document.Load(file);

            var documentElement = document.DocumentElement;
            Assert.That(documentElement, Is.Not.Null);

            // ReSharper disable PossibleNullReferenceException
            var documentName = documentElement.GetAttribute("Name");
            // ReSharper restore PossibleNullReferenceException

            Assert.That(documentName, Is.EqualTo(repositoryName));

            var requestsElement = documentElement.SelectSingleNode("Requests");
            Assert.That(requestsElement, Is.Not.Null);

            if (allRequests != null)
            {
// ReSharper disable PossibleNullReferenceException
                Assert.That(requestsElement.ChildNodes.Count, Is.EqualTo(allRequests.Count));

                foreach (XmlElement requestElement in requestsElement.ChildNodes)
                {
                    Assert.That(requestElement.Attributes.Count, Is.EqualTo(4));

                    var id = requestElement.GetAttribute("Id");
                    Assert.That(id, Is.Not.Null.Or.Empty);

                    Assert.That(allRequests.SingleOrDefault(r => r.PersistentId == id), Is.Not.Null);
                }
// ReSharper restore PossibleNullReferenceException
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateRepository(bool withExtension)
        {
            var repository = RepositoryFactory.CreateRepository();

            var targetFilePathWithoutExtension = RespositoryTests.GetUniqueFilePath();
            var targetFilePathWithExtension = targetFilePathWithoutExtension + SystemConstants.DatabaseExtension;

            repository.Create(withExtension ? targetFilePathWithExtension : targetFilePathWithoutExtension, "MyRepository");

            Assert.That(File.Exists(targetFilePathWithExtension));
            AssertFileContentIsCorrect(targetFilePathWithExtension, "MyRepository");
            Assert.That(repository.Name, Is.EqualTo("MyRepository"));
            Assert.That(repository.IsOpen, Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateRepositoryWhenFileExistsThrowsException(bool withExtension)
        {
            var repository = RepositoryFactory.CreateRepository();

            var targetFilePathWithoutExtension = RespositoryTests.GetUniqueFilePath();
            var targetFilePathWithExtension = targetFilePathWithoutExtension + SystemConstants.DatabaseExtension;

            File.WriteAllText(targetFilePathWithExtension, "Test");

            var targetFileToSet = withExtension ? targetFilePathWithExtension : targetFilePathWithoutExtension;

            Assert.That(() => repository.Create(targetFileToSet, "MyRepository"), Throws.InstanceOf<ApplicationException>());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\n   ")]
        public void CreateRepositoryWithEmptyNameThrowsExcpetion(string name)
        {
            var repository = RepositoryFactory.CreateRepository();

            Assert.That(() => repository.Create(RespositoryTests.GetUniqueFilePath(true), name), Throws.InstanceOf<ArgumentException>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OpenCreatedRepository(bool close)
        {
            var repository = RepositoryFactory.CreateRepository();
            var uniqueFilePath = RespositoryTests.GetUniqueFilePath(true);
            repository.Create(uniqueFilePath, "Test");
            if (close) repository.Close();

            Assert.That(() => repository.Open(uniqueFilePath), close ? (IResolveConstraint) Throws.Nothing : Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void OpenTwiceThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            var uniqueFilePath = RespositoryTests.GetUniqueFilePath(true);
            repository.Create(uniqueFilePath, "Test");
            repository.Close();

            repository.Open(uniqueFilePath);

            Assert.That(() => repository.Open(uniqueFilePath), Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void OpenRepositoryWhereFileDoesNotExistThrowsException()
        {
            var repository = RepositoryFactory.CreateRepository();
            Assert.That(() => repository.Open(@"X:\test.mmdb"), Throws.InstanceOf<ApplicationException>());
            Assert.That(repository.IsOpen, Is.False);
        }
    }
}

