
using System;
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class CategoryEditViewModel : ViewModel
    {
        private string _name;

        public string EntityId { get; private set; }
        public CommandViewModel DeleteCommand { get; private set; }

        public CategoryEditViewModel(string entityId, string name, Action<CategoryEditViewModel> onDelete)
        {
            EntityId = entityId;
            Name = name;
            DeleteCommand = new CommandViewModel(() => onDelete(this));
        }

        public CategoryEditViewModel(string name, Action<CategoryEditViewModel> onDelete)
        {
            Name = name;
            EntityId = null;
            DeleteCommand = new CommandViewModel(() => onDelete(this));
        }

        public string Name
        {
            get { return _name; }
            set { SetBackingField("Name", ref _name, value); }
        }
    }
}
