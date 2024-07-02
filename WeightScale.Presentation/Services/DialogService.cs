using System.Threading.Tasks;
using System.Windows;
using WeightScale.Presentation.Dialogs;
using WeightScale.Presentation.Resources.Constants;
using WeightScale.Presentation.Services.Interfaces;

namespace WeightScale.Presentation.Services
{
    public class DialogService : IDialogService
    {
        public MessageBoxResult ShowConfirmationDialog(string message)
        {
            return new ConfirmationDialog(DialogMessages.DialogTitle, message)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Application.Current.MainWindow
                }.ShowDialog()
                 .GetValueOrDefault()
                ? MessageBoxResult.Yes
                : MessageBoxResult.No;
        }

        public async Task<MessageBoxResult> ShowMessageDialogAsync(string message)
        {
            var dialogResult = await new MessageDialog(DialogMessages.DialogTitle, message)
                                     {
                                         WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                         Owner = Application.Current.MainWindow
                                     }.ShowDialogAsync();

            return dialogResult.GetValueOrDefault() ? MessageBoxResult.Yes : MessageBoxResult.No;
        }
    }
}