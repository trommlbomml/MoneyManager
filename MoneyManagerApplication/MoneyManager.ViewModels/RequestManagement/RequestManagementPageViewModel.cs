using System;
using System.ComponentModel;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

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

        public RequestManagementPageViewModel(ApplicationViewModel application, int year, int month) : base(application)
        {
            Months = new EnumeratedSingleValuedProperty<MonthNameViewModel>();
            Months.AddValue(new MonthNameViewModel("Januar", 0));
            Months.AddValue(new MonthNameViewModel("Februar", 1));
            Months.AddValue(new MonthNameViewModel("März", 2));
            Months.AddValue(new MonthNameViewModel("April", 3));
            Months.AddValue(new MonthNameViewModel("Mai", 4));
            Months.AddValue(new MonthNameViewModel("Juni", 5));
            Months.AddValue(new MonthNameViewModel("Juli", 6));
            Months.AddValue(new MonthNameViewModel("August", 7));
            Months.AddValue(new MonthNameViewModel("September", 8));
            Months.AddValue(new MonthNameViewModel("Oktober", 9));
            Months.AddValue(new MonthNameViewModel("November", 10));
            Months.AddValue(new MonthNameViewModel("Dezember", 11));

            Requests = new EnumeratedSingleValuedProperty<RequestViewModel>();
            
            _year = year;
            Months.SelectedValue = Months.SelectableValues[month];
            
            AddRequestCommand = new CommandViewModel(OnAddRequestCommand);
            DeleteRequestCommand = new CommandViewModel(OnDeleteRequestCommand);
            SaveCommand = new CommandViewModel(OnSaveCommand);
            PreviousMonthCommand = new CommandViewModel(OnPreviousMonthCommand);
            NextMonthCommand = new CommandViewModel(OnNextMonthCommand);

            Months.PropertyChanged += OnMonthsPropertyChanged;
            Requests.PropertyChanged += OnSelectedRequestChanged;

            UpdateCurrentMonth();
            UpdateCommandStates();
            UpdateSaldoAsString();

            Caption = string.Format(Properties.Resources.RequestManagementPageCaptionFormat, Application.Repository.Name);
        }

        private void OnSelectedRequestChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedValue") return;
            UpdateCommandStates();
        }

        private void OnMonthsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_preventUpdateCurrentMonth) return;

            UpdateCurrentMonth();
        }

        private void OnNextMonthCommand()
        {
            _preventUpdateCurrentMonth = true;

            if (Month == 11)
            {
                Year++;
                Months.SelectedValue = Months.SelectableValues.First();
            }
            else
            {
                Months.SelectedValue = Months.SelectableValues[Month+1];
            }
            _preventUpdateCurrentMonth = false;

            UpdateCurrentMonth();
        }

        private void OnPreviousMonthCommand()
        {
            _preventUpdateCurrentMonth = true;
            if (Month == 0)
            {
                Year--;
                Months.SelectedValue = Months.SelectableValues.Last();
            }
            else
            {
                Months.SelectedValue = Months.SelectableValues[Month-1];
            }
            _preventUpdateCurrentMonth = false;

            UpdateCurrentMonth();
        }

        private void OnSaveCommand()
        {
            Application.Repository.Save();
            Application.ApplicationSettings.UpdateRecentAccountInformation(Application.Repository.FilePath, DateTime.Now);
        }

        private void OnDeleteRequestCommand()
        {
            Application.Repository.DeleteRequest(Requests.SelectedValue.EntityId);
            Requests.SelectedValue = null;
            UpdateCurrentMonth();
        }

        private void OnAddRequestCommand()
        {
            var viewModel = new RequestDialogViewModel(Year, Month, OnCreateRequest);
            Application.WindowManager.ShowDialog(viewModel);
        }

        private void OnCreateRequest(RequestDialogViewModel requestDialog)
        {
            var requestEntityId = Application.Repository.CreateRequest(new RequestEntityData
            {
                Date = requestDialog.Date,
                Description = requestDialog.Description,
                Value = requestDialog.Value
            });

            var requestViewModel = new RequestViewModel(Application, requestEntityId);
            requestViewModel.Refresh();
            Requests.AddValue(requestViewModel);
            UpdateSaldoForCurrentMonth();
        }

        public CommandViewModel AddRequestCommand { get; private set; }
        public CommandViewModel DeleteRequestCommand { get; private set; }
        public CommandViewModel SaveCommand { get; private set; }
        public CommandViewModel PreviousMonthCommand { get; private set; }
        public CommandViewModel NextMonthCommand { get; private set; }

        public int Year
        {
            get { return _year; }
            set { SetBackingField("Year", ref _year, value, o => UpdateCurrentMonth()); }
        }

        public int Month
        {
            get { return Months.SelectedValue.Index; }
        }

        public double Saldo
        {
            get { return _saldo; }
            private set { SetBackingField("Saldo", ref _saldo, value, o => UpdateSaldoAsString()); }
        }

        private void UpdateSaldoAsString()
        {
            SaldoAsString = string.Format(Properties.Resources.MoneyValueFormat, Saldo);
        }

        public string SaldoAsString
        {
            get { return _saldoAsString; }
            private set { SetBackingField("SaldoAsString", ref _saldoAsString, value); }
        }

        private void UpdateCommandStates()
        {
            DeleteRequestCommand.IsEnabled = Requests.SelectedValue != null;
        }

        private void UpdateCurrentMonth()
        {
            if (_preventUpdateCurrentMonth) return;

            var requests = Application.Repository.QueryRequestsForSingleMonth(Year, Month)
                                                 .Select(requestEntity => new RequestViewModel(Application, requestEntity.PersistentId)
                                                    {
                                                        Date = requestEntity.Date,
                                                        Description = requestEntity.Description,
                                                        Value = requestEntity.Value
                                                    })
                                                 .OrderBy(r => r.Date);

            Requests.SetRange(requests);

            UpdateSaldoForCurrentMonth();
        }

        private void UpdateSaldoForCurrentMonth()
        {
            Saldo = Application.Repository.CalculateSaldoForMonth(Year, Month);
        }
    }
}
