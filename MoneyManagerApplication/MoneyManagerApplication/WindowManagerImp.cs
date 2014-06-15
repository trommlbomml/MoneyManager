
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.AccountManagement;
using MoneyManager.ViewModels.RequestManagement;
using MoneyManagerApplication.Dialogs;

namespace MoneyManagerApplication
{
    class WindowManagerImp : WindowManager
    {
        private static Window GetWindowFromViewModelType(Type type)
        {
            if (type == typeof (CreateAccountDialogViewModel))
            {
                return new CreateAccountDialog();
            }
            if (type == typeof (RequestDialogViewModel))
            {
                return new RequestDialog();
            }

            throw new InvalidOperationException(string.Format("There is no Window for type {0}", type.FullName));
        }

        public void ShowDialog(object dataContext)
        {
            var window = GetWindowFromViewModelType(dataContext.GetType());
            window.DataContext = dataContext;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public void ShowError(string caption, string text)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public void ShowQuestion(string caption, string text, Action yes)
        {
            if (MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) yes();
        }

        public string ShowSaveFileDialog(string initialDirectory, string fileName)
        {
            var saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = SystemConstants.DatabaseExtension,
                InitialDirectory = initialDirectory,
                FileName = fileName
            };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : string.Empty;
        }

        public string ShowOpenFileDialog(string initialDirectory)
        {
            var openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = SystemConstants.DatabaseExtension,
                InitialDirectory = initialDirectory
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
        }
    }
}
