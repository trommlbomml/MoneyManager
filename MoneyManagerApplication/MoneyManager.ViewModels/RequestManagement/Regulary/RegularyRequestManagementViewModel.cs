﻿using System.ComponentModel;
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

            UpdateCommandStates();
        }

        public RegularyRequestDetailsViewModel Details
        {
            get { return _details; }
            set { SetBackingField("Details", ref _details, value, o => UpdateCommandStates()); }
        }

        private void OnRegularyRequestPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value") return;
            UpdateDetails();
            UpdateCommandStates();
        }

        private void UpdateDetails()
        {
            if (RegularyRequests.Value == null)
            {
                Details = null;
            }
            else
            {
                Details = new RegularyRequestDetailsViewModel(_application, OnSaveRequestDetails, OnCancelRequestDetails)
                {
                    IsInEditMode = false,
                    EntityId = RegularyRequests.Value.EntityId
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
                RegularyRequests.Value = requestEntityViewModel;
            }
            else
            {
                _application.Repository.UpdateRegularyRequest(Details.EntityId, entityData);
                RegularyRequests.Value.Refresh();
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
            _application.Repository.DeleteRegularyRequest(RegularyRequests.Value.EntityId);
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
            DeleteRegularyRequestCommand.IsEnabled = isSelected && isReadOnly;
        }
    }
}
