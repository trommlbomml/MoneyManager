using System.Windows;

namespace MoneyManagerApplication.Dialogs
{
    public partial class CreatedRequestsDialog
    {
        public CreatedRequestsDialog()
        {
            InitializeComponent();
        }

        private void OnClickOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
