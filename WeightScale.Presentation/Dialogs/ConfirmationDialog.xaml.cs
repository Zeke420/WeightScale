using System.Windows;

namespace WeightScale.Presentation.Dialogs
{
    public partial class ConfirmationDialog : Window
    {
        public ConfirmationDialog(string title, string message)
        {
            Title = title;
            Message.Text = message;
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}