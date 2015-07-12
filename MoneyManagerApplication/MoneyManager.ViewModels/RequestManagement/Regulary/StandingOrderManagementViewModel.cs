using System;
using System.Collections.Generic;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement.Regulary
{
    public class StandingOrderManagementViewModel : ViewModel
    {
        private readonly ApplicationViewModel _application;
        private readonly Action _onStandingOrderUpdated;
        private StandingOrderDetailsViewModel _details;
        private List<StandingOrderEntityViewModel> _allStandingOrders;

        public EnumeratedSingleValuedProperty<StandingOrderEntityViewModel> StandingOrders { get; private set; }
        public SingleValuedProperty<bool> ShowFinishedProperty { get; private set; } 

        public CommandViewModel CreateStandingOrderCommand { get; private set; }
        public CommandViewModel DeleteStandingOrderCommand { get; private set; }

        public StandingOrderManagementViewModel(ApplicationViewModel application, Action onStandingOrderUpdated)
        {
            _application = application;
            _onStandingOrderUpdated = onStandingOrderUpdated;
            StandingOrders = new EnumeratedSingleValuedProperty<StandingOrderEntityViewModel>();
            StandingOrders.OnValueChanged += OnStandingOrdersValueChangd;

            _allStandingOrders =
                application.Repository.QueryAllStandingOrderEntities()
                    .Select(r => new StandingOrderEntityViewModel(application, r.PersistentId)).ToList();

            _allStandingOrders.ForEach(s => s.Refresh());
            CreateStandingOrderCommand = new CommandViewModel(OnCreateStandingOrderCommand);
            DeleteStandingOrderCommand = new CommandViewModel(OnDeleteStandingOrderCommand);
            ShowFinishedProperty = new SingleValuedProperty<bool>();

            ShowFinishedProperty.OnValueChanged += ShowFinishedPropertyOnOnValueChanged;

            UpdateStandingOrdersWithFiltering();
            UpdateCommandStates();
        }

        private void ShowFinishedPropertyOnOnValueChanged()
        {
            UpdateStandingOrdersWithFiltering();
        }

        private void UpdateStandingOrdersWithFiltering()
        {
            var oldSelected = StandingOrders.Value;

            var available = _allStandingOrders.Where(IsMatchingFilterCriteria);
            StandingOrders.SetRange(available);

            if (oldSelected == null || StandingOrders.SelectableValues.Any(s => s.EntityId == oldSelected.EntityId))
            {
                StandingOrders.Value = oldSelected;
            }
            else
            {
                StandingOrders.Value = null;
            }
        }

        private bool IsMatchingFilterCriteria(StandingOrderEntityViewModel standingOrder)
        {
            return ShowFinishedProperty.Value || standingOrder.State != StandingOrderState.Finished;
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
                StandingOrders.AddValue(requestEntityViewModel);
                StandingOrders.Value = requestEntityViewModel;
                _allStandingOrders.Add(requestEntityViewModel);
            }
            else
            {
                _application.Repository.UpdateStandingOrder(Details.EntityId, entityData);
            }

            Details.IsInEditMode = false;
            _application.Repository.UpdateStandingOrdersToCurrentMonth();
            StandingOrders.Value.Refresh();
            UpdateCommandStates();
            _onStandingOrderUpdated();
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
            _application.WindowManager.ShowQuestion(Properties.Resources.StandingOrderManagement_DeleteCaption,
                Properties.Resources.StandingOrderManagement_DeleteMessage, DeleteStandingOrder, () => { });
        }

        private void DeleteStandingOrder()
        {
            var standingOrderEntityViewModel = StandingOrders.Value;
            _application.Repository.DeleteStandingOrder(standingOrderEntityViewModel.EntityId);
            _allStandingOrders.Remove(standingOrderEntityViewModel);
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
