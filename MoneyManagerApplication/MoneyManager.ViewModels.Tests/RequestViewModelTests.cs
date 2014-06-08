﻿using System;
using MoneyManager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests
{
    [TestFixture]
    public class RequestViewModelTests : ViewModelTestsBase
    {
        private const string DefaultEntityId = "DefaultEntityId";

        [Test]
        public void UpdateLocalizedProperties()
        {
            var testDate = new DateTime(2014, 4, 6);
            const double testValue = 11.2;

            var viewModel = new RequestViewModel(Application, DefaultEntityId);
            var expectedDateAsString = string.Format(Properties.Resources.RequestDateFormat, testDate);
            var expectedValueAsString = string.Format(Properties.Resources.MoneyValueFormat, testValue);

            viewModel.Date = testDate;
            Assert.That(viewModel.DateAsString, Is.EqualTo(expectedDateAsString));

            viewModel.Value = testValue;
            Assert.That(viewModel.ValueAsString, Is.EqualTo(expectedValueAsString));
        }

        [Test]
        public void Refresh()
        {
            var viewModel = new RequestViewModel(Application, DefaultEntityId);

            var requestEntity = Substitute.For<RequestEntity>();
            requestEntity.Date.Returns(new DateTime(2014, 5, 5));
            requestEntity.Value.Returns(12.5);
            requestEntity.Description.Returns("TestDescription");

            Repository.QueryRequest(DefaultEntityId).Returns(requestEntity);

            viewModel.Refresh();
            Repository.Received(1).QueryRequest(DefaultEntityId);
            Assert.That(viewModel.Date, Is.EqualTo(new DateTime(2014, 5, 5)));
            Assert.That(viewModel.Value, Is.EqualTo(12.5));
            Assert.That(viewModel.Description, Is.EqualTo("TestDescription"));
        }

        [Test]
        public void Save()
        {
            var viewModel = new RequestViewModel(Application, DefaultEntityId)
            {
                Date = new DateTime(2014, 7, 12),
                Description = "Test",
                Value = -11
            };

            viewModel.Save();
            Repository.Received(1).UpdateRequest(DefaultEntityId, Arg.Is<RequestEntityData>(r => r.Date == viewModel.Date && r.Description == "Test" && Math.Abs(r.Value - (-11)) < double.Epsilon));
        }
    }
}