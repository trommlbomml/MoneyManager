
namespace MoneyManager.ViewModels.Framework
{
    public abstract class EntityViewModel : ViewModel
    {
        public string EntityId { get; private set; }
        
        public ApplicationViewModel Application { get; private set; }

        protected EntityViewModel(ApplicationViewModel application, string entityId)
        {
            EntityId = entityId;
            Application = application;
        }

        public abstract void Refresh();
        public abstract void Save();
    }
}
