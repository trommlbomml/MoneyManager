﻿
using System;

namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Interface for the view to show Windows in the GUI.
    /// </summary>
    public interface WindowManager
    {
        /// <summary>
        /// Shows a custom Dialog for custom Context.
        /// </summary>
        /// <param name="dataContext">DataContext to show Window for.</param>
        void ShowDialog(object dataContext);

        /// <summary>
        /// Shows Error MessageBox.
        /// </summary>
        /// <param name="caption">Caption of MessageBox</param>
        /// <param name="text">Text of MessageBox.</param>
        void ShowError(string caption, string text);

        /// <summary>
        /// Shows Question MessageBox.
        /// </summary>
        /// <param name="caption">Caption of MessageBox</param>
        /// <param name="text">Text of MessageBox</param>
        /// <param name="yes">Action to do when choosing yes</param>
        /// <param name="no">Action to do when chossing no</param>
        void ShowQuestion(string caption, string text, Action yes, Action no);

        /// <summary>
        /// Shows a confirmation Dialog.
        /// </summary>
        /// <param name="caption">Caption of MessageBox</param>
        /// <param name="text">Text of MessageBox</param>
        /// <param name="yes">Action to do when choosing yes</param>
        /// <param name="no">Action to do when choosing no</param>
        /// <param name="cancel">Action to do when cancelling</param>
        void ShowConfirmation(string caption, string text, Action yes, Action no, Action cancel);

        /// <summary>
        /// Shows a Save File Dialog.
        /// </summary>
        /// <param name="initialDirectory">Startup directory</param>
        /// <param name="fileName">Provided Filename for default</param>
        /// <param name="filtersExpression">Filter expression</param>
        /// <param name="caption">Caption of Dialog</param>
        /// <returns>Selectd File Name. Is Null when canceled.</returns>
        string ShowSaveFileDialog(string initialDirectory, string fileName, string filtersExpression, string caption = null);

        /// <summary>
        /// Shows a Open File Dialog
        /// </summary>
        /// <param name="initialDirectory">Startup directory</param>
        /// <param name="filtersExpression">Filter expression</param>
        /// <param name="caption">Caption of Dialog</param>
        /// <returns>Selectd File Name. Is Null when canceled.</returns>
        string ShowOpenFileDialog(string initialDirectory, string filtersExpression, string caption = null);
    }
}
