using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class CategoryManagementDialogViewModel : ViewModel
    {
        public EnumeratedSingleValuedProperty<CategoryEditViewModel> Categories { get; private set; }

        public List<CategoryEditViewModel> CategoriesToDelete { get; private set; }

        public CategoryManagementDialogViewModel(ApplicationViewModel application, Action<CategoryManagementDialogViewModel> ok)
        {
            CategoriesToDelete = new List<CategoryEditViewModel>();
            Categories = new EnumeratedSingleValuedProperty<CategoryEditViewModel>();
            Categories.PropertyChanged += CategoriesOnPropertyChanged;

            foreach (var categoryViewModel in application.Repository.QueryAllCategories().Select(c => new CategoryEditViewModel(c.PersistentId, c.Name, OnDeleteCategory)))
            {
                Categories.AddValue(categoryViewModel);
            }

            OkCommand = new CommandViewModel(() => ok(this));
            NewCategoryCommand = new CommandViewModel(OnNewCategoryCommand);

            UpdateCommandStates();
        }

        private void OnDeleteCategory(CategoryEditViewModel categoryEditViewModel)
        {
            if (!string.IsNullOrEmpty(categoryEditViewModel.EntityId)) CategoriesToDelete.Add(categoryEditViewModel);
            Categories.RemoveValue(categoryEditViewModel);
        }

        private void CategoriesOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            OkCommand.IsEnabled = true;
            NewCategoryCommand.IsEnabled = true;
        }

        private void OnNewCategoryCommand()
        {
            Categories.AddValue(new CategoryEditViewModel(Properties.Resources.CategoryManagementNewCategoryDefaultName, OnDeleteCategory));
        }

        public CommandViewModel OkCommand { get; private set; }
        public CommandViewModel NewCategoryCommand { get; private set; }
    }
}
