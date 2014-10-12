using System.Windows;

namespace MoneyManagerApplication.Dialogs
{
    /// <summary>
    /// Interaction logic for RegularyRequestDialog.xaml
    /// </summary>
    public partial class RegularyRequestDialog
    {
        public RegularyRequestDialog()
        {
            InitializeComponent();
        }

        private void CloseCommandClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
