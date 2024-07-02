using System.Reflection;
using System.Windows;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.Presentation.Enums;
using WeightScale.Presentation.Resources.Constants;
using WeightScale.Presentation.Services.Interfaces;

namespace WeightScale.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService,
                             IDialogService dialogService,
                             IPackageService packageService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _navigationService.NavigateTo<HeaderViewModel>(NavigationRegion.Header);
            _navigationService.NavigateTo<WeightViewModel>(NavigationRegion.Main);
            //packageService.ConnectDevices("192.168.1.37","192.168.1.38");
        }

        public string TitleWithVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return version != null
                    ? $"{GlobalResource.ApplicationName} {version.Major}.{version.Minor}.{version.Revision}"
                    : GlobalResource.ApplicationName;
            }
        }

        public INavigationService NavigationService
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public bool CanClose()
        {
            var canNavigate = _navigationService.MainRegion.CanNavigate;
            if (canNavigate == NavigationState.Forbidden)
            {
                _dialogService.ShowMessageDialogAsync(DialogMessages.ForbiddenMessage);
                return false;
            }

            if (canNavigate != NavigationState.WithConfirmation)
            {
                return true;
            }

            return _dialogService.ShowConfirmationDialog(DialogMessages.ConfirmMessage) == MessageBoxResult.Yes;
        }
    }
}