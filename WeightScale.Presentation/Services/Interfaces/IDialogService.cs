using System.Threading.Tasks;
using System.Windows;

namespace WeightScale.Presentation.Services.Interfaces
{
    public interface IDialogService
    {
        MessageBoxResult ShowConfirmationDialog(string message);
        MessageBoxResult SuccessMessage(string message);
        Task<MessageBoxResult> ShowMessageDialogAsync(string message);
    }
}
