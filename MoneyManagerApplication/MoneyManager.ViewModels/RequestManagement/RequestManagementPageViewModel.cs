using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;
using MoneyManager.ViewModels.RequestManagement.Regulary;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class RequestManagementPageViewModel : PageViewModel
    {
        private int _year;
        private double _saldo;
        private string _saldoAsString;
        private bool _preventUpdateCurrentMonth;

        public EnumeratedSingleValuedProperty<RequestViewModel> Requests { get; private set; } 
        public EnumeratedSingleValuedProperty<MonthNameViewModel> Months { get; private set; }
        
        public CommandViewModel AddRequestCommand { get; private set; }
        public CommandViewModel DeleteRequestCommand { get; private set; }
        public CommandViewModel PreviousMonthCommand { get; private set; }
        public CommandViewModel NextMonthCommand { get; private set; }
        public CommandViewModel EditRequestCommand { get; private set; }
        public CommandViewModel SwitchAccountCommand { get; private set; }
        public CommandViewModel EditCategoriesCommand { get; private set; }
        public CommandViewModel EditStandingOrdersCommand { get; private set; }
        public CommandViewModel GotoCurrentMonthCommand { get; private set; }

        public RequestManagementPageViewModel(ApplicationViewModel application, int year, int month) : base(application)
        {
            Months = new EnumeratedSingleValuedProperty<MonthNameViewModel>();
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameJanuary, 1));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameFebuary, 2));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameMarch, 3));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameApril, 4));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameMay, 5));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameJune, 6));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameJuly, 7));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameAugust, 8));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameSeptember, 9));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameOctober, 10));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameNovember, 11));
            Months.AddValue(new MonthNameViewModel(Properties.Resources.MonthNameDecember, 12));

            Requests = new EnumeratedSingleValuedProperty<RequestViewModel>();
            
            _year = year;
            Months.Value = Months.SelectableValues.Single(m => m.Index == month);
            
            AddRequestCommand = new CommandViewModel(OnAddRequestCommand);
            DeleteRequestCommand = new CommandViewModel(OnDeleteRequestCommand);
            PreviousMonthCommand = new CommandViewModel(OnPreviousMonthCommand);
            NextMonthCommand = new CommandViewModel(OnNextMonthCommand);
            EditRequestCommand = new CommandViewModel(OnEditRequestCommand);
            SwitchAccountCommand = new CommandViewModel(OnSwitchAccountCommand);
            EditCategoriesCommand = new CommandViewModel(OnEditCategoriesCommand);
            GotoCurrentMonthCommand = new CommandViewModel(OnGotoCurrentMonthCommand);
            EditStandingOrdersCommand = new CommandViewModel(OnEditStandingOrdersCommand);

            Months.PropertyChanged += OnMonthsPropertyChanged;
            Requests.PropertyChanged += OnSelectedRequestChanged;

            UpdateCurrentMonth();
            UpdateCommandStates();
            UpdateSaldoAsString();

            Caption = string.Format(Properties.Resources.RequestManagementPageCaptionFormat, Application.Repository.Name);
        }

        private void OnEditStandingOrdersCommand()
        {
            Application.WindowManager.ShowDialog(new StandingOrderManagementViewModel(Application, OnStandingOrderUpdated));
        }

        private void OnStandingOrderUpdated()
        {
            UpdateCurrentMonth();
            UpdateCommandStates();
        }

        private void OnGotoCurrentMonthCommand()
        {
            var currentDate = Application.ApplicationContext.Now;
            _preventUpdateCurrentMonth = true;
            Year = currentDate.Year;
            Months.Value = Months.SelectableValues.Single(m => m.Index == currentDate.Month);
            _preventUpdateCurrentMonth = false;

            UpdateCurrentMonth();
            UpdateCommandStates();
        }

        public int Year
        {
            get { return _year; }
            set 
            {
                SetBackingField("Year", ref _year, value, o => YearChanged()); 
            }
        }

        private void YearChanged()
        {
            UpdateCurrentMonth(true);
            UpdateCommandStates();
        }

        public int Month
        {
            get { return Months.Value.Index; }
        }

        public double Saldo
        {
            get { return _saldo; }
            private set { SetBackingField("Saldo", ref _saldo, value, o => UpdateSaldoAsString()); }
        }

        public string SaldoAsString
        {
            get { return _saldoAsString; }
            private set { SetBackingField("SaldoAsString", ref _saldoAsString, value); }
        }

        private void UpdateSaldoAsString()
        {
            SaldoAsString = string.Format(Properties.Resources.MoneyValueFormat, Saldo);
        }

        private void OnEditCategoriesCommand()
        {
            Application.WindowManager.ShowDialog(new CategoryManagementDialogViewModel(Application, OnEditCategoriesOk));
        }

        private void OnEditCategoriesOk(CategoryManagementDialogViewModel categoryManagement)
        {
            foreach (var currentCategory in categoryManagement.Categories.SelectableValues)
            {
                if (!string.IsNullOrEmpty(currentCategory.EntityId))
                {
                    Application.Repository.UpdateCategory(currentCategory.EntityId, currentCategory.Name);
                }
                else
                {
                    Application.Repository.CreateCategory(currentCategory.Name);
                }
            }

            foreach (var categoryToDelete in categoryManagement.CategoriesToDelete)
            {
                Application.Repository.DeleteCategory(categoryToDelete.EntityId);
            }

            UpdateCurrentMonth();
        }

        private void OnSwitchAccountCommand()
        {
            Application.Repository.Close();
            Application.ActivateAccountManagementPage();
        }

        private void OnEditRequestCommand()
        {
            var currentRequest = Requests.Value;

            var viewModel = new RequestDialogViewModel(Application, currentRequest.EntityId, OnEditRequest)
            {
                Caption = Properties.Resources.RequestDialogCaptionEdit
            };
            Application.WindowManager.ShowDialog(viewModel);
        }

        private void OnEditRequest(RequestDialogViewModel requestDialog)
        {
            var currentRequestEntityId = Requests.Value.EntityId;
            var requestEntityData = new RequestEntityData
            {
                Date = requestDialog.DateProperty.Value,
                Description = requestDialog.DescriptionProperty.Value,
                Value = requestDialog.CalculateValue,
                CategoryPersistentId = requestDialog.Categories.Value != null ? requestDialog.Categories.Value.EntityId : null
            };
            Application.Repository.UpdateRequest(currentRequestEntityId, requestEntityData);

            Requests.Value.Refresh();
            UpdateSaldoForCurrentMonth();
        }

        private void OnSelectedRequestChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value") return;
            UpdateCommandStates();
        }

        private void OnMonthsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_preventUpdateCurrentMonth) return;

            UpdateCurrentMonth(true);
            UpdateCommandStates();
        }

        private void OnNextMonthCommand()
        {
            _preventUpdateCurrentMonth = true;

            if (Month == 12)
            {
                Year++;
                Months.Value = Months.SelectableValues.First();
            }
            else
            {
                Months.Value = Months.SelectableValues.Single(m => m.Index == Month+1);
            }
            _preventUpdateCurrentMonth = false;

            UpdateCurrentMonth(true);
            UpdateCommandStates();
        }

        private void OnPreviousMonthCommand()
        {
            _preventUpdateCurrentMonth = true;
            if (Month == 1)
            {
                Year--;
                Months.Value = Months.SelectableValues.Last();
            }
            else
            {
                Months.Value = Months.SelectableValues.Single(m => m.Index == Month-1);
            }
            _preventUpdateCurrentMonth = false;

            UpdateCurrentMonth();
            UpdateCommandStates();
        }

        private void OnDeleteRequestCommand()
        {
            Application.WindowManager.ShowQuestion(Properties.Resources.RequestManagementDeleteRequestQuestionCaption,
                                                   Properties.Resources.RequestManagementDeleteRequestQuestionMessage,
                                                   DeleteRequest, () => {});
        }

        private void DeleteRequest()
        {
            Application.Repository.DeleteRequest(Requests.Value.EntityId);
            Requests.Value = null;
            UpdateCurrentMonth();
        }

        private void OnAddRequestCommand()
        {
            var viewModel = new RequestDialogViewModel(Application, Year, Month, OnCreateRequest)
            {
                Caption = Properties.Resources.RequestDialogCaptionCreate
            };
            Application.WindowManager.ShowDialog(viewModel);
        }

        private void OnCreateRequest(RequestDialogViewModel requestDialog)
        {
            var requestEntityId = Application.Repository.CreateRequest(new RequestEntityData
            {
                Date = requestDialog.DateProperty.Value,
                Description = requestDialog.DescriptionProperty.Value,
                Value = requestDialog.CalculateValue,
                CategoryPersistentId = requestDialog.Categories.Value != null ? requestDialog.Categories.Value.EntityId : null
            });

            var requestViewModel = new RequestViewModel(Application, requestEntityId);
            requestViewModel.Refresh();
            Requests.AddValue(requestViewModel);
            UpdateSaldoForCurrentMonth();
        }

        private void UpdateCommandStates()
        {
            var isCurrentMonthSelected = Application.ApplicationContext.Now.Year == Year &&
                                         Application.ApplicationContext.Now.Month == Month;
            var isRequestSelected = Requests.Value != null;
            var isStandingOrder = isRequestSelected && Requests.Value.IsStandingOrder;
            DeleteRequestCommand.IsEnabled = isRequestSelected && !isStandingOrder;
            EditRequestCommand.IsEnabled = isRequestSelected && !isStandingOrder;
            GotoCurrentMonthCommand.IsEnabled = !isCurrentMonthSelected;
        }

        private void UpdateCurrentMonth(bool updateStandingOrders = false)
        {
            if (_preventUpdateCurrentMonth) return;

            var sw = Stopwatch.StartNew();

            if (updateStandingOrders)
            {
                Application.Repository.UpdateStandingOrdersToCurrentMonth(Year, Month);
            }

            var requests = Application.Repository.QueryRequestsForSingleMonth(Year, Month)
                                                 .Select(CreateRequestViewModelFromEntity)
                                                 .OrderBy(r => r.Date);

            Requests.SetRange(requests);

            UpdateSaldoForCurrentMonth();

            Debug.WriteLine("UpdateCurrentMonth update standing Orders={0}, {1}ms", updateStandingOrders, sw.ElapsedMilliseconds);
        }

        private RequestViewModel CreateRequestViewModelFromEntity(RequestEntity requestEntity)
        {
            var requestViewModel = new RequestViewModel(Application, requestEntity.PersistentId);
            requestViewModel.Refresh();
            return requestViewModel;
        }

        private void UpdateSaldoForCurrentMonth()
        {
            Saldo = Application.Repository.CalculateSaldoForMonth(Year, Month);
        }

        public override bool OnClosingRequest()
        {
            var cancel = false;
            Application.WindowManager.ShowQuestion(Properties.Resources.RequestManagementOnClosingRequestConfirmationCaption,
                                                       Properties.Resources.RequestManagementOnClosingRequestConfirmationMessage,
                () => Application.Repository.Close(), () => { cancel = true; });

            return cancel;
        }
    }
}
