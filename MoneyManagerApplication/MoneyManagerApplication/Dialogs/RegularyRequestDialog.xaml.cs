using System.Windows;

namespace MoneyManagerApplication.Dialogs
{
    /// <summary>
    /// Interaction logic for StandingOrderDialog.xaml
    /// </summary>
    public partial class StandingOrderDialog
    {
        public StandingOrderDialog()
        {
            InitializeComponent();
        }

        private void CloseCommandClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
