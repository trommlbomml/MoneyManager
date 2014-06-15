
using System.Windows;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.AccountManagement;
using MoneyManagerApplication.Dialogs;

namespace MoneyManagerApplication
{
    class WindowManagerImp : WindowManager
    {
        public void ShowDialog(object dataContext)
        {
            if (dataContext.GetType() == typeof (CreateAccountDialogViewModel))
            {
                var window = new CreateAccountDialog
                {
                    DataContext = dataContext,
                    Owner = Application.Current.MainWindow
                };
                window.ShowDialog();
            }
        }

        public void ShowError(string caption, string text)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
