
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
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
        private static Dictionary<Type, Func<Window>> CreateWindowFromDataContextType = new Dictionary
            <Type, Func<Window>>
        {
            {typeof (CreateAccountDialogViewModel), () => new CreateAccountDialog()},
            {typeof (RequestDialogViewModel), () => new RequestDialog()},
            {typeof (CategoryManagementDialogViewModel), () => new CategoriesManagementDialog()},
        };

        private static Window GetWindowFromViewModelType(Type type)
        {
            Func<Window> createFunc;
            if (CreateWindowFromDataContextType.TryGetValue(type, out createFunc))
            {
                return createFunc();
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

        public void ShowQuestion(string caption, string text, Action yes, Action no)
        {
            var result = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                yes();
            }
            else if (result == MessageBoxResult.No)
            {
                no();
            }
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
