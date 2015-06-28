using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MoneyManager.ViewModels.Framework;
using MoneyManager.ViewModels.RequestManagement;

namespace MoneyManager.ViewModels
{
    public class CreatedRequestsDialogViewModel : ViewModel
    {
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly ObservableCollection<RequestViewModel> _createdRequests;
// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        public ReadOnlyObservableCollection<RequestViewModel> CreatedRequests { get; private set; }

        public CreatedRequestsDialogViewModel(ApplicationViewModel application, IEnumerable<string> createdRequests)
        {
            var entities = (createdRequests != null
                ? createdRequests.Select(id => new RequestViewModel(application, id))
                : Enumerable.Empty<RequestViewModel>()).ToArray();

            foreach (var entity in entities)
            {
                entity.Refresh();
            }

            _createdRequests = new ObservableCollection<RequestViewModel>(entities.OrderBy(e => e.Date));

            CreatedRequests = new ReadOnlyObservableCollection<RequestViewModel>(_createdRequests);
        }
    }
}
