using WeightScale.Presentation.Command;
using WeightScale.Presentation.Enums;
using WeightScale.Presentation.Services.Interfaces;

namespace WeightScale.Presentation.ViewModel
{
    public class HeaderViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        public HeaderViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateToCourierCommand = new DelegateCommand(NavigateToCourier);
            NavigateToShipmentsCommand = new DelegateCommand(NavigateToShipments);
            NavigateToWeightCommand = new DelegateCommand(NavigateToWeight);
            NavigateToReportCommand = new DelegateCommand(NavigateToReport);
        }

        public DelegateCommand NavigateToCourierCommand { get; set; }
        public DelegateCommand NavigateToShipmentsCommand { get; set; }
        public DelegateCommand NavigateToWeightCommand { get; set; }
        public DelegateCommand NavigateToReportCommand { get; set; }

        public INavigationService NavigationService
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        private void NavigateToCourier(object obj)
        {
            _navigationService.NavigateTo<CourierViewModel>(NavigationRegion.Main);
        }

        private void NavigateToShipments(object obj)
        {
            _navigationService.NavigateTo<ShipmentViewModel>(NavigationRegion.Main);
        }

        private void NavigateToWeight(object obj)
        {
            _navigationService.NavigateTo<WeightViewModel>(NavigationRegion.Main);
        }

        private void NavigateToReport(object obj)
        {
            _navigationService.NavigateTo<ReportViewModel>(NavigationRegion.Main);
        }
    }
}