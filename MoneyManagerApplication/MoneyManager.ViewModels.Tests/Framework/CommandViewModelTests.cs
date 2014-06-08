using System;
using System.ComponentModel;
using MoneyManager.ViewModels.Framework;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.Framework
{
    [TestFixture]
    public class CommandViewModelTests
    {
        [Test]
        public void ConstructorWithArgumentNullThrowsException()
        {
            Assert.That(() => new CommandViewModel(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void InitialState()
        {
            var commandViewModel = new CommandViewModel(Substitute.For<Action>());

            Assert.That(commandViewModel.IsEnabled, Is.True);
            Assert.That(commandViewModel.CanExecute(null), Is.True);
        }

        [Test]
        public void InvokeCommandCallsAction()
        {
            var action = Substitute.For<Action>();
            var commandViewModel = new CommandViewModel(action);

            commandViewModel.Execute(null);
            action.Received(1).Invoke();
        }

        [Test]
        public void IsEnabledChangedInvokesCanExecuteChanged()
        {
            var canExecuteChangedHandler = Substitute.For<EventHandler>();
            var commandViewModel = new CommandViewModel(Substitute.For<Action>());
            commandViewModel.CanExecuteChanged += canExecuteChangedHandler;

            commandViewModel.IsEnabled = false;
            canExecuteChangedHandler.Received(1).Invoke(commandViewModel, EventArgs.Empty);
            canExecuteChangedHandler.ClearReceivedCalls();

            commandViewModel.IsEnabled = true;
            canExecuteChangedHandler.Received(1).Invoke(commandViewModel, EventArgs.Empty);
            canExecuteChangedHandler.ClearReceivedCalls();

            commandViewModel.IsEnabled = true;
            canExecuteChangedHandler.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<object>(), Arg.Any<EventArgs>());
        }

        [Test]
        public void IsEnabledChangedInvokesPropertyChanged()
        {
            var propertyChangedHandler = Substitute.For<PropertyChangedEventHandler>();
            var commandViewModel = new CommandViewModel(Substitute.For<Action>());
            commandViewModel.PropertyChanged += propertyChangedHandler;

            commandViewModel.IsEnabled = false;
            propertyChangedHandler.Received(1).Invoke(commandViewModel, Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == "IsEnabled"));
            propertyChangedHandler.ClearReceivedCalls();

            commandViewModel.IsEnabled = true;
            propertyChangedHandler.Received(1).Invoke(commandViewModel, Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == "IsEnabled"));
            propertyChangedHandler.ClearReceivedCalls();

            commandViewModel.IsEnabled = true;
            propertyChangedHandler.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<object>(), Arg.Any<PropertyChangedEventArgs>());
        }
    }
}
