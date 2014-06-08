using System;
using System.ComponentModel;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.Framework
{
    [TestFixture]
    public class ViewModelTests
    {
        [Test]
        public void OnPropertyChangedWithNoHandler()
        {
            var testViewModel = new TestViewModel();

            Assert.That(() => testViewModel.TestOnPropertyChanged("PropertyName"), Throws.Nothing);
        }

        [Test]
        public void OnPropertyChangedWithHandler()
        {
            var testViewModel = new TestViewModel();
            var propertyChangedHandler = Substitute.For<PropertyChangedEventHandler>();
            testViewModel.PropertyChanged += propertyChangedHandler;

            testViewModel.TestOnPropertyChanged("PropertyName");

            propertyChangedHandler.Received(1).Invoke(testViewModel, Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == "PropertyName"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetBackingFieldInvokesPropertyChanged(bool changeValue)
        {
            var backingField = "Hello";

            var testViewModel = new TestViewModel();
            var propertyChangedHandler = Substitute.For<PropertyChangedEventHandler>();
            testViewModel.PropertyChanged += propertyChangedHandler;

            var newValue = changeValue ? "Hello2" : "Hello";

            testViewModel.TestSetBackingField("MyProperty", ref backingField, newValue);

            if (changeValue)
            {
                propertyChangedHandler.Received(1).Invoke(testViewModel, Arg.Is<PropertyChangedEventArgs>(e => e.PropertyName == "MyProperty"));
            }
            else
            {
                propertyChangedHandler.DidNotReceiveWithAnyArgs().Invoke(testViewModel, Arg.Any<PropertyChangedEventArgs>());    
            }
            
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetBackingFieldCallsOnChangedHandler(bool changeValue)
        {
            var backingField = "Hello";

            var testViewModel = new TestViewModel();
            var onChangedHandler = Substitute.For<Action<string>>();

            var newValue = changeValue ? "Hello2" : "Hello";
            testViewModel.TestSetBackingField("MyProperty", ref backingField, newValue, onChangedHandler);

            if (changeValue)
            {
                onChangedHandler.Received(1).Invoke("Hello");
            }
            else
            {
                onChangedHandler.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<string>());
            }
        }
    }
}
