
using System.IO;
using System.Windows;
using Microsoft.Win32;
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
