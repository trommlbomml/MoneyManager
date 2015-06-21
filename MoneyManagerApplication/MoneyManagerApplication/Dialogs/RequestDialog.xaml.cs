
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MoneyManager.ViewModels.RequestManagement;

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
            var viewModel = (RequestDialogViewModel)DataContext;
            if (viewModel.IsEditingExistingRequest)
            {
                DialogResult = true;
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (Mouse.Captured is Calendar || Mouse.Captured is System.Windows.Controls.Primitives.CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
