
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using MoneyManager.Interfaces;
using MoneyManager.ViewModels.RequestManagement;
using MoneyManager.ViewModels.RequestManagement.Regulary;
using MoneyManagerApplication.Dialogs;

namespace MoneyManagerApplication
{
    static class WinApi
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
    }

    class WindowManagerImp : WindowManager
    {
        private static readonly Dictionary<Type, Func<Window>> CreateWindowFromDataContextType = new Dictionary<Type, Func<Window>>
        {
            {typeof (RequestDialogViewModel), () => new RequestDialog()},
            {typeof (CategoryManagementDialogViewModel), () => new CategoriesManagementDialog()},
            {typeof (StandingOrderManagementViewModel), () => new StandingOrderDialog()},
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

        private static Window GetActiveWindow()
        {
            var active = WinApi.GetActiveWindow();
            return Application.Current.Windows.OfType<Window>().SingleOrDefault(window => new WindowInteropHelper(window).Handle == active);
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

        public void ShowConfirmation(string caption, string text, Action yes, Action no, Action cancel)
        {
            var result = MessageBox.Show(text, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Cancel:
                    cancel();
                    break;
                case MessageBoxResult.Yes:
                    yes();
                    break;
                case MessageBoxResult.No:
                    no();
                    break;
            }
        }

        public string ShowSaveFileDialog(string initialDirectory, string fileName, string filterExpression, string caption = null)
        {
            var saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = SystemConstants.DatabaseExtension,
                InitialDirectory = initialDirectory,
                FileName = fileName,
                Filter = filterExpression,
                Title = caption
            };

            return saveFileDialog.ShowDialog(GetActiveWindow()) == true ? saveFileDialog.FileName : string.Empty;
        }

        public string ShowOpenFileDialog(string initialDirectory, string filterExpression, string caption = null)
        {
            var openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = SystemConstants.DatabaseExtension,
                InitialDirectory = initialDirectory,
                Filter = filterExpression,
                Title = caption
            };

            return openFileDialog.ShowDialog(GetActiveWindow()) == true ? openFileDialog.FileName : string.Empty;
        }
    }
}
