
using System;
using System.IO;
using MoneyManager.ViewModels.AccountManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.AccountManagement
{
    [TestFixture]
    public class CreateAccountDialogViewModelTests : ViewModelTestsBase
    {
        [Test]
        public void InitialState()
        {
            var expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Konto.mmdb");

            var dialog = new CreateAccountDialogViewModel(Application, o => { }, o => { });
            Assert.That(dialog.CancelCommand.IsEnabled, Is.True);
            Assert.That(dialog.CreateAccountCommand.IsEnabled, Is.True);
            Assert.That(dialog.SelectFileCommand.IsEnabled, Is.True);
            Assert.That(dialog.Name, Is.EqualTo("Mein Konto"));
            Assert.That(dialog.Path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void InvokeCommandsInvokesDelegates()
        {
            var cancelAction = Substitute.For<Action<CreateAccountDialogViewModel>>();
            var okAction = Substitute.For<Action<CreateAccountDialogViewModel>>();

            var dialog = new CreateAccountDialogViewModel(Application, cancelAction, okAction);

            dialog.CancelCommand.Execute(null);
            cancelAction.Received(1).Invoke(dialog);

            dialog.CreateAccountCommand.Execute(null);
            okAction.Received(1).Invoke(dialog);
        }

        [TestCase(null, null, false)]
        [TestCase("", null, false)]
        [TestCase(null, "", false)]
        [TestCase("", "", false)]
        [TestCase(null, "text", false)]
        [TestCase("", "text", false)]
        [TestCase("text", "text", true)]
        [TestCase("text", null, false)]
        [TestCase("text", "", false)]
        [TestCase("text", "text", true)]
        public void CreateAccountCommandIsEnabled(string name, string path, bool expectedIsEnabled)
        {
            var dialog = new CreateAccountDialogViewModel(Application, o => { }, o => { })
            {
                Name = name, 
                Path = path
            };

            Assert.That(dialog.CreateAccountCommand.IsEnabled, Is.EqualTo(expectedIsEnabled));
        }

        [Test]
        public void CreateAccountCommandEnableWhenTextsAreCorrect()
        {
            var dialog = new CreateAccountDialogViewModel(Application, o => { }, o => { })
            {
                Name = string.Empty,
                Path = null
            };

            Assert.That(dialog.CreateAccountCommand.IsEnabled, Is.False);

            dialog.Name = "Test";
            dialog.Path = "Test";
            Assert.That(dialog.CreateAccountCommand.IsEnabled, Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SelectFileCommand(bool accept)
        {
            var expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Konto.mmdb");

            if (accept)
            {
                expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "test.mmdb");
                WindowManager.ShowSaveFileDialog(Arg.Any<string>(), Arg.Any<string>())
                    .Returns(expectedPath);
            }
            else
            {
                WindowManager.ShowSaveFileDialog(Arg.Any<string>(), Arg.Any<string>())
                    .Returns("");
            }
            
            var dialog = new CreateAccountDialogViewModel(Application, o => { }, o => { });
            dialog.SelectFileCommand.Execute(null);

            if (accept)
            {
                Assert.That(dialog.Path, Is.EqualTo(expectedPath));
            }
        }
    }
}
