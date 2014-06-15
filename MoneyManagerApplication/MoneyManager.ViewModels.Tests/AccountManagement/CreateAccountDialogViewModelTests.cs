
using System;
using MoneyManager.ViewModels.AccountManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.AccountManagement
{
    [TestFixture]
    public class CreateAccountDialogViewModelTests
    {
        [Test]
        public void InitialState()
        {
            var dialog = new CreateAccountDialogViewModel(o => { }, o => { });
            Assert.That(dialog.CancelCommand.IsEnabled, Is.True);
            Assert.That(dialog.CreateAccountCommand.IsEnabled, Is.True);
            Assert.That(dialog.Name, Is.Not.Null.Or.Empty);
            Assert.That(dialog.Path, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void InvokeCommandsInvokesDelegates()
        {
            var cancelAction = Substitute.For<Action<CreateAccountDialogViewModel>>();
            var okAction = Substitute.For<Action<CreateAccountDialogViewModel>>();

            var dialog = new CreateAccountDialogViewModel(cancelAction, okAction);

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
            var dialog = new CreateAccountDialogViewModel(o => { }, o => { })
            {
                Name = name, 
                Path = path
            };

            Assert.That(dialog.CreateAccountCommand.IsEnabled, Is.EqualTo(expectedIsEnabled));
        }

        [Test]
        public void CreateAccountCommandEnableWhenTextsAreCorrect()
        {
            var dialog = new CreateAccountDialogViewModel(o => { }, o => { })
            {
                Name = string.Empty,
                Path = null
            };

            Assert.That(dialog.CreateAccountCommand.IsEnabled, Is.False);

            dialog.Name = "Test";
            dialog.Path = "Test";
            Assert.That(dialog.CreateAccountCommand.IsEnabled, Is.True);
        }
    }
}
