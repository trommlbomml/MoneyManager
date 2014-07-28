
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class CategoryEditViewModel : ViewModel
    {
        private string _name;

        public string EntityId { get; private set; }

        public CategoryEditViewModel(string entityId, string name)
        {
            EntityId = entityId;
            Name = name;
        }

        public CategoryEditViewModel(string name)
        {
            Name = name;
            EntityId = null;
        }

        public string Name
        {
            get { return _name; }
            set { SetBackingField("Name", ref _name, value); }
        }
    }
}
