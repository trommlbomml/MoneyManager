
using System.Windows;

namespace MoneyManagerApplication.Dialogs
{
    public partial class RequestDialog
    {
        public RequestDialog()
        {
            InitializeComponent();
        }

        private void OnCreateClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
