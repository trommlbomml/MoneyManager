
using System.Windows;

namespace MoneyManagerApplication.Dialogs
{
    public partial class CategoriesManagementDialog
    {
        public CategoriesManagementDialog()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
