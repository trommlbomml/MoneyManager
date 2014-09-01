using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MoneyManagerApplication.Extensions
{
    internal class Commands
    {
        public static readonly DependencyProperty DataGridDoubleClickProperty = 
            DependencyProperty.RegisterAttached("DataGridDoubleClickCommand", typeof (ICommand), typeof (Commands), new PropertyMetadata(AttachOrRemoveDataGridDoubleClickEvent));

        public static ICommand GetDataGridDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(DataGridDoubleClickProperty);
        }

        public static void SetDataGridDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DataGridDoubleClickProperty, value);
        }

        public static void AttachOrRemoveDataGridDoubleClickEvent(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = obj as DataGrid;
            if (dataGrid == null) return;

            if (args.OldValue == null && args.NewValue != null)
            {
                dataGrid.MouseDoubleClick += ExecuteDataGridDoubleClick;
            }
            else if (args.OldValue != null && args.NewValue == null)
            {
                dataGrid.MouseDoubleClick -= ExecuteDataGridDoubleClick;
            }
        }

        private static void ExecuteDataGridDoubleClick(object sender, MouseButtonEventArgs args)
        {
            var dependencyObject = sender as DependencyObject;
            if (dependencyObject == null) return;

            var command = (ICommand) dependencyObject.GetValue(DataGridDoubleClickProperty);
            if (command == null) return;

            if (command.CanExecute(dependencyObject)) command.Execute(dependencyObject);
        }
    }
}