using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Options;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.Presentation.Enums;
using WeightScale.Presentation.Resources.Constants;
using WeightScale.Presentation.Services.Interfaces;

namespace WeightScale.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly IDialogService _dialogService;
        private INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService,
                             IDialogService dialogService,
                             IPackageService packageService,
                             IOptions<ApplicationSettings> applicationSettings)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _applicationSettings = applicationSettings.Value;
            _navigationService.NavigateTo<HeaderViewModel>(NavigationRegion.Header);
            _navigationService.NavigateTo<WeightViewModel>(NavigationRegion.Main);
            _navigationService.NavigateTo<FooterViewModel>(NavigationRegion.Footer);
            // packageService.ConnectDevices(_applicationSettings.IpAddressFullWeight,
            //                               _applicationSettings.IpAddressEmptyWeight);
        }

        public string TitleWithVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly()
                                      .GetName()
                                      .Version;
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
