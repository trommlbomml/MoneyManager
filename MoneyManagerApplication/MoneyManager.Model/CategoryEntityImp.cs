using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    internal class CategoryEntityImp : Entity, CategoryEntity
    {
        public string Name { get; set; }

        public CategoryEntityImp()
        {
            Name = string.Empty;
        }

        public CategoryEntityImp(XElement requestElement) :
            base(requestElement.Attribute("Id").Value)
        {
            Name = requestElement.Attribute("Name").Value;
        }

        public XElement Serialize()
        {
            return new XElement("Category", new XAttribute("Id", PersistentId),
                                            new XAttribute("Description", Name ?? ""));
        }
    }
}
