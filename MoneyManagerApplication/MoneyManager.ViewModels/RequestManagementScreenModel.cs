using System;
using System.Collections.ObjectModel;
using System.Linq;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class RequestManagementScreenModel : ScreenModel
    {
        private readonly ObservableCollection<RequestViewModel> _requests;
        private int _year;
        private int _month;
        private double _saldo;
        private string _saldoAsString;
        private RequestViewModel _selectedRequest;

        public RequestManagementScreenModel(ApplicationViewModel application, int year, int month) : base(application)
        {
            _requests = new ObservableCollection<RequestViewModel>();
            Requests = new ReadOnlyObservableCollection<RequestViewModel>(_requests);
            _year = year;
            _month = month;
            
            AddRequestCommand = new CommandViewModel(OnAddRequestCommand);
            DeleteRequestCommand = new CommandViewModel(OnDeleteRequestCommand);
            SaveCommand = new CommandViewModel(OnSaveCommand);

            UpdateCurrentMonth();
            UpdateSaldoForCurrentMonth();
            UpdateCommandStates();
        }

        private void OnSaveCommand()
        {
            Application.Repository.Save("Test.xml");
        }

        private void OnDeleteRequestCommand()
        {
            Application.Repository.DeleteRequest(SelectedRequest.EntityId);
            SelectedRequest = null;
            UpdateCurrentMonth();
        }

        private void OnAddRequestCommand()
        {
            var requestEntityId = Application.Repository.CreateRequest(new RequestEntityData
            {
                Date = new DateTime(Year, Month, 15),
                Description = "My Description",
                Value = 12.95
            });

            var requestViewModel = new RequestViewModel(Application, requestEntityId);
            requestViewModel.Refresh();
            _requests.Add(requestViewModel);
            UpdateSaldoForCurrentMonth();
        }

        public ReadOnlyObservableCollection<RequestViewModel> Requests { get; private set; }

        public CommandViewModel AddRequestCommand { get; private set; }
        public CommandViewModel DeleteRequestCommand { get; private set; }
        public CommandViewModel SaveCommand { get; private set; }

        public int Year
        {
            get { return _year; }
            set { SetBackingField("Year", ref _year, value, o => UpdateCurrentMonth()); }
        }

        public int Month
        {
            get { return _month; }
            set { SetBackingField("Month", ref _month, value, o => UpdateCurrentMonth()); }
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

        public RequestViewModel SelectedRequest
        {
            get { return _selectedRequest; }
            set { SetBackingField("SelectedRequest", ref _selectedRequest, value, o => UpdateCommandStates()); }
        }

        private void UpdateCommandStates()
        {
            DeleteRequestCommand.IsEnabled = SelectedRequest != null;
        }

        private void UpdateCurrentMonth()
        {
            var requests = Application.Repository.QueryRequestsForSingleMonth(Year, Month)
                                                 .Select(requestEntity => new RequestViewModel(Application, requestEntity.PersistentId)
                                                    {
                                                        Date = requestEntity.Date,
                                                        Description = requestEntity.Description,
                                                        Value = requestEntity.Value
                                                    })
                                                 .OrderBy(r => r.Date);

            _requests.Clear();
            foreach (var requestViewModel in requests)
            {
                _requests.Add(requestViewModel);
            }

            SelectedRequest = null;

            UpdateSaldoForCurrentMonth();
        }

        private void UpdateSaldoForCurrentMonth()
        {
            Saldo = Application.Repository.CalculateSaldoForMonth(Year, Month);
        }
    }
}
