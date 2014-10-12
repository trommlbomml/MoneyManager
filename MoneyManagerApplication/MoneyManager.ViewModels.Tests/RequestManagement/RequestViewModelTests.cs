using System;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.RequestManagement;
using NSubstitute;
using NUnit.Framework;

namespace MoneyManager.ViewModels.Tests.RequestManagement
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

        [TestCase(0.0d)]
        [TestCase(12.5d)]
        public void Refresh(double value)
        {
            var viewModel = new RequestViewModel(Application, DefaultEntityId);

            var requestDate = new DateTime(2014, 5, 5);

            var requestEntity = Substitute.For<RequestEntity>();
            requestEntity.Date.Returns(requestDate);
            requestEntity.Value.Returns(value);
            requestEntity.Description.Returns("TestDescription");
            requestEntity.RegularyRequest.Returns(c => null);

            Repository.QueryRequest(DefaultEntityId).Returns(requestEntity);

            viewModel.Refresh();
            Repository.Received(1).QueryRequest(DefaultEntityId);
            Assert.That(viewModel.Date, Is.EqualTo(requestDate));
            Assert.That(viewModel.IsRegularyRequest, Is.False);
            Assert.That(viewModel.DateAsString, Is.EqualTo(string.Format(Properties.Resources.RequestDateFormat, requestDate)));
            Assert.That(viewModel.Value, Is.EqualTo(value));
            Assert.That(viewModel.ValueAsString, Is.EqualTo(string.Format(Properties.Resources.MoneyValueFormat, value)));
            Assert.That(viewModel.Description, Is.EqualTo("TestDescription"));
        }

        [TestCase(null, "Category1")]
        [TestCase("Castegory2", null)]
        [TestCase("Castegory2", "Category1")]
        [TestCase(null, null)]
        public void RefreshFromRegularyRequestTakesPropertiesOfRegularyRequest(string requestCategoryName, string regularyRequestCategoryName)
        {
            var viewModel = new RequestViewModel(Application, DefaultEntityId);

            var requestDate = new DateTime(2014, 5, 5);
            CategoryEntity categoryOfRequest = null;
            CategoryEntity categoryOfRegularyRequest = null;

            if (!string.IsNullOrEmpty(requestCategoryName))
            {
                categoryOfRequest = Substitute.For<CategoryEntity>();
                categoryOfRequest.Name.Returns(requestCategoryName);    
            }
            if (!string.IsNullOrEmpty(regularyRequestCategoryName))
            {
                categoryOfRegularyRequest = Substitute.For<CategoryEntity>();
                categoryOfRegularyRequest.Name.Returns(regularyRequestCategoryName);    
            }
            
            var regularyRequest = Substitute.For<RegularyRequestEntity>();
            regularyRequest.Value.Returns(22.4);
            regularyRequest.Description.Returns("Description Of Regulary Request");
            regularyRequest.Category.Returns(categoryOfRegularyRequest);

            var requestEntity = Substitute.For<RequestEntity>();
            requestEntity.Date.Returns(requestDate);
            requestEntity.Value.Returns(11.2);
            requestEntity.Description.Returns("Description Of Request");
            requestEntity.Category.Returns(categoryOfRequest);
            requestEntity.RegularyRequest.Returns(regularyRequest);

            Repository.QueryRequest(DefaultEntityId).Returns(requestEntity);

            viewModel.Refresh();
            Assert.That(viewModel.IsRegularyRequest, Is.True);
            Assert.That(viewModel.Date, Is.EqualTo(requestDate));
            Assert.That(viewModel.DateAsString, Is.EqualTo(string.Format(Properties.Resources.RequestDateFormat, requestDate)));
            Assert.That(viewModel.Value, Is.EqualTo(22.4));
            Assert.That(viewModel.Description, Is.EqualTo("Description Of Regulary Request"));
            Assert.That(viewModel.Category, Is.EqualTo(regularyRequestCategoryName ?? Properties.Resources.NoCategory));
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