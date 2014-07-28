using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class CategoryManagementDialogViewModel : ViewModel
    {
        private readonly ApplicationViewModel _application;
        public EnumeratedSingleValuedProperty<CategoryEditViewModel> Categories { get; private set; }

        public List<CategoryEditViewModel> CategoriesToDelete { get; private set; }

        public CategoryManagementDialogViewModel(ApplicationViewModel application, Action<CategoryManagementDialogViewModel> ok)
        {
            _application = application;
            CategoriesToDelete = new List<CategoryEditViewModel>();
            Categories = new EnumeratedSingleValuedProperty<CategoryEditViewModel>();
            Categories.PropertyChanged += CategoriesOnPropertyChanged;

            foreach (var categoryViewModel in application.Repository.QueryAllCategories().Select(c => new CategoryEditViewModel(c.PersistentId, c.Name)))
            {
                Categories.AddValue(categoryViewModel);
            }

            OkCommand = new CommandViewModel(() => ok(this));
            NewCategoryCommand = new CommandViewModel(OnNewCategoryCommand);
            DeleteCategoryCommand = new CommandViewModel(OnDeleteCategoryCommand);

            UpdateCommandStates();
        }

        private void CategoriesOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            OkCommand.IsEnabled = true;
            NewCategoryCommand.IsEnabled = true;
            DeleteCategoryCommand.IsEnabled = Categories.SelectedValue != null;
        }

        private void OnDeleteCategoryCommand()
        {
            var selectedItem = Categories.SelectedValue;
            if (!string.IsNullOrEmpty(selectedItem.EntityId)) CategoriesToDelete.Add(selectedItem);
            Categories.RemoveSelectedValue();
        }

        private void OnNewCategoryCommand()
        {
            Categories.AddValue(new CategoryEditViewModel("<New Category>"));
        }

        public CommandViewModel OkCommand { get; private set; }
        public CommandViewModel NewCategoryCommand { get; private set; }
        public CommandViewModel DeleteCategoryCommand { get; private set; }
    }
}
