using WeightScale.Presentation.Enums;
using WeightScale.Presentation.ViewModel;

namespace WeightScale.Presentation.Services.Interfaces
{
    public interface INavigationService
    {
        ViewModelBase HeaderRegion { get; set; }
        ViewModelBase MainRegion { get; }
        ViewModelBase FooterRegion { get; set; }
        void NavigateTo<T>(NavigationRegion region, object parameter = null) where T : ViewModelBase;
        bool CanNavigate(string message);
    }
}