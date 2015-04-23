using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model.Entities
{
    internal class CategoryEntityPersistenceImp : CategoryEntity
    {
        public string PersistentId { get; private set; }
        public string Name { get; private set; }

        public CategoryEntityPersistenceImp(CategoryEntity entity)
        {
            PersistentId = entity.PersistentId;
            Name = entity.Name;
        }

        public XElement Serialize()
        {
            return new XElement("Category", new XAttribute("Id", PersistentId),
                                            new XAttribute("Name", Name ?? ""));
        }
    }
}