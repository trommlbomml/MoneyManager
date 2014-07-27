
using MoneyManager.ViewModels.Framework;

namespace MoneyManager.ViewModels.RequestManagement
{
    public class CategoryViewModel : EntityViewModel
    {
        private string _name;

        public CategoryViewModel(ApplicationViewModel application, string entityId) : base(application, entityId)
        {
        }

        public string Name
        {
            get { return _name; }
            private set { SetBackingField("Name", ref _name, value); }
        }

        public override void Refresh()
        {
            var category = Application.Repository.QueryCategory(EntityId);
            Name = category.Name;
        }

        public override void Save()
        {
            Application.Repository.UpdateCategory(EntityId, Name);
        }
    }
}
