using System.Xml.Linq;
using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    internal class CategoryEntityImp : Entity, CategoryEntity
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public CategoryEntityImp()
        {
            Name = string.Empty;
        }

        public CategoryEntityImp(XElement requestElement) :
            base(requestElement.Attribute("Id").Value)
        {
            _name = requestElement.Attribute("Name").Value;
        }

        public XElement Serialize()
        {
            return new XElement("Category", new XAttribute("Id", PersistentId),
                                            new XAttribute("Name", Name ?? ""));
        }
    }
}
