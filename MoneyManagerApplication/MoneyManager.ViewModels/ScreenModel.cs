
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels
{
    public class ScreenModel : ViewModel
    {
        public ApplicationViewModel Application { get; private set; }

        protected ScreenModel(ApplicationViewModel application)
        {
            Application = application;
        }
    }
}
