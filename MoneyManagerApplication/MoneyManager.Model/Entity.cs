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

        protected void SetProperty<T>(ref T backingField, T value)
        {
            if (Equals(backingField, value)) return;
            backingField = value;
            HasChanged = true;
        }

        public bool HasChanged { get; private set; }
    }
}
