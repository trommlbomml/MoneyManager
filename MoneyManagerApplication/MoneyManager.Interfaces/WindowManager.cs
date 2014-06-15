
using System;

namespace MoneyManager.Interfaces
{
    public interface WindowManager
    {
        void ShowDialog(object dataContext);

        void ShowError(string caption, string text);

        void ShowQuestion(string caption, string text, Action yes);

        string ShowSaveFileDialog(string initialDirectory, string fileName);

        string ShowOpenFileDialog(string initialDirectory);
    }
}
