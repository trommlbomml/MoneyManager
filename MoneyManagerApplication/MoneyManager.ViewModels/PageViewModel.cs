
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class PageViewModel : ViewModel
    {
        private string _caption;
        public ApplicationViewModel Application { get; private set; }

        protected PageViewModel(ApplicationViewModel application)
        {
            Application = application;
        }

        public string Caption
        {
            get { return _caption; }
            protected set { SetBackingField("Caption", ref _caption, value); }
        }
    }
}
