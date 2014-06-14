
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class PageViewModel : ViewModel
    {
        public ApplicationViewModel Application { get; private set; }

        protected PageViewModel(ApplicationViewModel application)
        {
            Application = application;
        }
    }
}
