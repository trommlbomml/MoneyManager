using System.ComponentModel;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement.Regulary
{
    public class RegularyRequestManagementViewModel : ViewModel
    {
        private readonly ApplicationViewModel _application;
        private RegularyRequestDetailsViewModel _details;

        public EnumeratedSingleValuedProperty<RegularyRequestEntityViewModel> RegularyRequests { get; private set; }

        public CommandViewModel CreateRegularyRequestCommand { get; private set; }
        public CommandViewModel DeleteRegularyRequestCommand { get; private set; }
        public CommandViewModel EditRegularyRequestCommand { get; private set; }

        public RegularyRequestManagementViewModel(ApplicationViewModel application)
        {
            _application = application;
            RegularyRequests = new EnumeratedSingleValuedProperty<RegularyRequestEntityViewModel>();
            RegularyRequests.PropertyChanged += OnRegularyRequestPropertyChanged;

            foreach (var requestPersistentId in application.Repository.QueryAllRegularyRequestEntities().Select(r => r.PersistentId))
            {
                var request = new RegularyRequestEntityViewModel(application, requestPersistentId);
                RegularyRequests.AddValue(request);
            }

            foreach (var request in RegularyRequests.SelectableValues)
            {
                request.Refresh();
            }

            CreateRegularyRequestCommand = new CommandViewModel(OnCreateRegularyRequestCommand);
            DeleteRegularyRequestCommand = new CommandViewModel(OnDeleteRegularyRequestCommand);
            EditRegularyRequestCommand = new CommandViewModel(OnEditRegularyRequestCommand);

            UpdateCommandStates();
        }

        private void OnEditRegularyRequestCommand()
        {
            Details.IsInEditMode = true;
            UpdateCommandStates();
        }

        public RegularyRequestDetailsViewModel Details
        {
            get { return _details; }
            set { SetBackingField("Details", ref _details, value, o => UpdateCommandStates()); }
        }

        private void OnRegularyRequestPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedValue") return;
            UpdateDetails();
            UpdateCommandStates();
        }

        private void UpdateDetails()
        {
            if (RegularyRequests.SelectedValue == null)
            {
                Details = null;
            }
            else
            {
                Details = new RegularyRequestDetailsViewModel(_application, OnSaveRequestDetails, OnCancelRequestDetails)
                {
                    IsInEditMode = false,
                    EntityId = RegularyRequests.SelectedValue.EntityId
                };
                Details.Refresh();
            }
        }

        private void OnSaveRequestDetails(RegularyRequestEntityData entityData)
        {
            if (string.IsNullOrEmpty(Details.EntityId))
            {
                var entityId = _application.Repository.CreateRegularyRequest(entityData);
                Details.EntityId = entityId;

                var requestEntityViewModel = new RegularyRequestEntityViewModel(_application, entityId);
                requestEntityViewModel.Refresh();
                RegularyRequests.AddValue(requestEntityViewModel);
                RegularyRequests.SelectedValue = requestEntityViewModel;
            }
            else
            {
                _application.Repository.UpdateRegularyRequest(Details.EntityId, entityData);
                RegularyRequests.SelectedValue.Refresh();
            }

            Details.IsInEditMode = false;
            _application.Repository.UpdateRegularyRequestsToCurrentMonth();
            UpdateCommandStates();
        }

        private void OnCancelRequestDetails(RegularyRequestDetailsViewModel details)
        {
            Details.IsInEditMode = false;
            if (!string.IsNullOrEmpty(Details.EntityId))
            {
                Details.Refresh();
            }
            else
            {
                Details = null;
            }
        }

        private void OnDeleteRegularyRequestCommand()
        {
            _application.Repository.DeleteRegularyRequest(RegularyRequests.SelectedValue.EntityId);
            RegularyRequests.RemoveSelectedValue();
        }

        private void OnCreateRegularyRequestCommand()
        {
            var details = new RegularyRequestDetailsViewModel(_application, OnSaveRequestDetails, OnCancelRequestDetails)
            {
                IsInEditMode = true
            };
            Details = details;
        }

        private void UpdateCommandStates()
        {
            var isSelected = Details != null;
            var isReadOnly = Details == null || !Details.IsInEditMode;
            CreateRegularyRequestCommand.IsEnabled = isReadOnly;
            EditRegularyRequestCommand.IsEnabled = isSelected && isReadOnly;
            DeleteRegularyRequestCommand.IsEnabled = isSelected && isReadOnly;
        }
    }
}
