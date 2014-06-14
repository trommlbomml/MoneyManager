
using System.Windows;

namespace MoneyManagerApplication.Dialogs
{
    public partial class CreateAccountDialog
    {
        public CreateAccountDialog()
        {
            InitializeComponent();
        }

        private void OnCreateAccountClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
