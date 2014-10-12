
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
    [TestFixture]
    public class RepositoryStateTests
    {
        private Repository CreateRepositoryFromFactory()
        {
            return RepositoryFactory.CreateRepository(RepositoryTestsBase.CreateApplicationContextMockup(true));
        }

        [SetUp]
        public void SetUp()
        {
            var targetFolder = Path.GetTempPath();
            var lockDirectory = Path.Combine(targetFolder, ".lock");
            if (Directory.Exists(lockDirectory)) Directory.Delete(lockDirectory);
        }

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
            var repository = CreateRepositoryFromFactory();
            Assert.That(repository.Close, Throws.InstanceOf<ApplicationException>());
            Assert.That(repository.FilePath, Is.Null.Or.Empty);
        }

        [Test]
        public void CloseTwiceRepositoryThrowsException()
        {
            var repository = CreateRepositoryFromFactory();
            repository.Create(RepositoryTestsBase.GetUniqueFilePath(), "DefaultName");
            repository.Close();

            Assert.That(repository.Close, Throws.InstanceOf<ApplicationException>());
            Assert.That(repository.FilePath, Is.Null.Or.Empty);
        }

        [Test]
        public void OpenCreatedRepositoryThrowsException()
        {
            var repository = CreateRepositoryFromFactory();
            var uniqueFilePath = RepositoryTestsBase.GetUniqueFilePath(true);
            repository.Create(uniqueFilePath, "DefaultName");
            
            Assert.That(() => repository.Open(""), Throws.InstanceOf<ApplicationException>());
            Assert.That(repository.FilePath, Is.EqualTo(uniqueFilePath));
        }

        [Test]
        public void CreateRepositoryTwiceThrowsException()
        {
            var repository = CreateRepositoryFromFactory();
            repository.Create(RepositoryTestsBase.GetUniqueFilePath(), "DefaultName");

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
            var repository = CreateRepositoryFromFactory();
            var targetFilePathWithoutExtension = RepositoryTestsBase.GetUniqueFilePath();
            var targetFilePathWithExtension = targetFilePathWithoutExtension + SystemConstants.DatabaseExtension;

            repository.Create(withExtension ? targetFilePathWithExtension : targetFilePathWithoutExtension, "MyRepository");

            Assert.That(File.Exists(targetFilePathWithExtension));
            AssertFileContentIsCorrect(targetFilePathWithExtension, "MyRepository");
            Assert.That(repository.Name, Is.EqualTo("MyRepository"));
            Assert.That(repository.IsOpen, Is.True);

            repository.Close();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateRepositoryWhenFileExistsThrowsException(bool withExtension)
        {
            var repository = CreateRepositoryFromFactory();
            var targetFilePathWithoutExtension = RepositoryTestsBase.GetUniqueFilePath();
            var targetFilePathWithExtension = targetFilePathWithoutExtension + SystemConstants.DatabaseExtension;

            File.WriteAllText(targetFilePathWithExtension, @"Test");

            var targetFileToSet = withExtension ? targetFilePathWithExtension : targetFilePathWithoutExtension;

            Assert.That(() => repository.Create(targetFileToSet, "MyRepository"), Throws.InstanceOf<ApplicationException>());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("\n   ")]
        public void CreateRepositoryWithEmptyNameThrowsExcpetion(string name)
        {
            var repository = CreateRepositoryFromFactory();

            Assert.That(() => repository.Create(RepositoryTestsBase.GetUniqueFilePath(true), name), Throws.InstanceOf<ArgumentException>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OpenCreatedRepository(bool close)
        {
            var repository = CreateRepositoryFromFactory();
            var uniqueFilePath = RepositoryTestsBase.GetUniqueFilePath(true);
            repository.Create(uniqueFilePath, "Test");
            if (close) repository.Close();

            Assert.That(() => repository.Open(uniqueFilePath), close ? (IResolveConstraint) Throws.Nothing : Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void OpenTwiceThrowsException()
        {
            var repository = CreateRepositoryFromFactory();
            var uniqueFilePath = RepositoryTestsBase.GetUniqueFilePath(true);
            repository.Create(uniqueFilePath, "Test");
            repository.Close();

            repository.Open(uniqueFilePath);

            Assert.That(() => repository.Open(uniqueFilePath), Throws.InstanceOf<ApplicationException>());
        }

        [Test]
        public void OpenRepositoryWhereFileDoesNotExistThrowsException()
        {
            var repository = CreateRepositoryFromFactory();
            Assert.That(() => repository.Open(@"X:\test.mmdb"), Throws.InstanceOf<ApplicationException>());
            Assert.That(repository.IsOpen, Is.False);
        }
    }
}

