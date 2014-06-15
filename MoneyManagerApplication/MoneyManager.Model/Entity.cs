using System;

namespace MoneyManager.Model
{
    public abstract class Entity
    {
        public string PersistentId { get; private set; }

        protected Entity()
        {
            PersistentId = Guid.NewGuid().ToString("D");
        }

        protected Entity(string id)
        {
            PersistentId = id;
        }
    }
}
