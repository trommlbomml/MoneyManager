using MoneyManager.Interfaces;

namespace MoneyManager.Model
{
    public static class RepositoryFactory
    {
        public static Repository CreateRepository(ApplicationContext context)
        {
            return new RepositoryImp(context);
        }
    }
}
