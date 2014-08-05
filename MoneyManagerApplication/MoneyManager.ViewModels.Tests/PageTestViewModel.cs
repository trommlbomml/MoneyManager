namespace MoneyManager.ViewModels.Tests
{
    public class PageTestViewModel : PageViewModel
    {
        public PageTestViewModel(ApplicationViewModel application) : base(application)
        {
        }

        public bool IsCancelOnClose { get; set; }

        public override bool OnClosingRequest()
        {
            return IsCancelOnClose;
        }
    }
}