using System.ComponentModel;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement.Regulary
{
    public class StandingOrderManagementViewModel : ViewModel
    {
        private readonly ApplicationViewModel _application;
        private StandingOrderDetailsViewModel _details;

        public EnumeratedSingleValuedProperty<StandingOrderEntityViewModel> StandingOrders { get; private set; }

        public CommandViewModel CreateStandingOrderCommand { get; private set; }
        public CommandViewModel DeleteStandingOrderCommand { get; private set; }

        public StandingOrderManagementViewModel(ApplicationViewModel application)
        {
            _application = application;
            StandingOrders = new EnumeratedSingleValuedProperty<StandingOrderEntityViewModel>();
            StandingOrders.OnValueChanged += OnStandingOrdersValueChangd;

            foreach (var standingOrderEntity in application.Repository.QueryAllStandingOrderEntities().Select(r => new StandingOrderEntityViewModel(application, r.PersistentId)))
            {
                standingOrderEntity.Refresh();
                StandingOrders.AddValue(standingOrderEntity);
            }

            CreateStandingOrderCommand = new CommandViewModel(OnCreateStandingOrderCommand);
            DeleteStandingOrderCommand = new CommandViewModel(OnDeleteStandingOrderCommand);

            UpdateCommandStates();
        }

        private void OnStandingOrdersValueChangd()
        {
            UpdateDetails();
            UpdateCommandStates();
        }

        public StandingOrderDetailsViewModel Details
        {
            get { return _details; }
            set { SetBackingField("Details", ref _details, value, o => UpdateCommandStates()); }
        }

        private void UpdateDetails()
        {
            if (StandingOrders.Value == null)
            {
                Details = null;
            }
            else
            {
                Details = new StandingOrderDetailsViewModel(_application, OnSaveRequestDetails, OnCancelRequestDetails)
                {
                    IsInEditMode = false,
                    EntityId = StandingOrders.Value.EntityId
                };
                Details.Refresh();
            }
        }

        private void OnSaveRequestDetails(StandingOrderEntityData entityData)
        {
            if (string.IsNullOrEmpty(Details.EntityId))
            {
                var entityId = _application.Repository.CreateStandingOrder(entityData);
                Details.EntityId = entityId;

                var requestEntityViewModel = new StandingOrderEntityViewModel(_application, entityId);
                requestEntityViewModel.Refresh();
                StandingOrders.AddValue(requestEntityViewModel);
                StandingOrders.Value = requestEntityViewModel;
            }
            else
            {
                _application.Repository.UpdateStandingOrder(Details.EntityId, entityData);
                StandingOrders.Value.Refresh();
            }

            Details.IsInEditMode = false;
            _application.Repository.UpdateStandingOrdersToCurrentMonth();
            UpdateCommandStates();
        }

        private void OnCancelRequestDetails(StandingOrderDetailsViewModel details)
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

        private void OnDeleteStandingOrderCommand()
        {
            _application.Repository.DeleteStandingOrder(StandingOrders.Value.EntityId);
            StandingOrders.RemoveSelectedValue();
        }

        private void OnCreateStandingOrderCommand()
        {
            var details = new StandingOrderDetailsViewModel(_application, OnSaveRequestDetails, OnCancelRequestDetails)
            {
                IsInEditMode = true
            };
            Details = details;
        }

        private void UpdateCommandStates()
        {
            var isSelected = Details != null;
            var isReadOnly = Details == null || !Details.IsInEditMode;
            CreateStandingOrderCommand.IsEnabled = isReadOnly;
            DeleteStandingOrderCommand.IsEnabled = isSelected && isReadOnly;
        }
    }
}
