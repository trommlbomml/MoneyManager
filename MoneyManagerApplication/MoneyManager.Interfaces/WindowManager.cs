
namespace MoneyManager.Interfaces
{
    public interface WindowManager
    {
        void ShowDialog(object dataContext);

        void ShowError(string caption, string text);
    }
}
