using System.Threading.Tasks;
using System.Windows;

namespace WeightScale.Presentation.Dialogs
{
    public partial class MessageDialog : Window
    {
        public MessageDialog(string title, string message)
        {
            InitializeComponent();
            Title = title;
            Message.Text = message;
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public async Task<bool?> ShowDialogAsync()
        {
            await Task.Yield();
            return ShowDialog();
        }
    }
}
