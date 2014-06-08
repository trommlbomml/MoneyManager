using System.Collections.ObjectModel;
using System.Linq;

namespace MoneyManager.ViewModels
{
    public class RequestManagementScreenModel : ScreenModel
    {
        private readonly ObservableCollection<RequestViewModel> _requests;
        private int _year;
        private int _month;

        public RequestManagementScreenModel(ApplicationViewModel application, int year, int month) : base(application)
        {
            _requests = new ObservableCollection<RequestViewModel>();
            Requests = new ReadOnlyObservableCollection<RequestViewModel>(_requests);
            _year = year;
            _month = month;
            OnMonthChanged();
        }

        public ReadOnlyObservableCollection<RequestViewModel> Requests { get; private set; }

        public int Year
        {
            get { return _year; }
            set { SetBackingField("Year", ref _year, value, o => OnMonthChanged()); }
        }

        public int Month
        {
            get { return _month; }
            set { SetBackingField("Month", ref _month, value, o => OnMonthChanged()); }
        }

        private void OnMonthChanged()
        {
            var requests = Application.Repository.QueryRequestsForSingleMonth(Year, Month)
                                                 .Select(requestEntity => new RequestViewModel(Application, requestEntity.PersistentId)
                                                    {
                                                        Date = requestEntity.Date,
                                                        Description = requestEntity.Description,
                                                        Value = requestEntity.Value
                                                    })
                                                 .OrderBy(r => r.Date);


            foreach (var requestViewModel in requests)
            {
                _requests.Add(requestViewModel);
            }
        }
    }
}
