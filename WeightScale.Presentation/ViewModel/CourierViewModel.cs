using System.Collections.ObjectModel;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.Presentation.Command;

namespace WeightScale.Presentation.ViewModel
{
    public class CourierViewModel : ViewModelBase
    {
        private readonly ICourierService _courierService;
        private string _courierName;

        public CourierViewModel(ICourierService courierService)
        {
            _courierService = courierService;
            CreateCourierCommand = new DelegateCommand(CreateCourier);
            DeleteCourierCommand = new DelegateCommand(DeleteCourier);
            Couriers = new ObservableCollection<Courier>();
            LoadCouriers();
        }

        public DelegateCommand CreateCourierCommand { get; }
        public DelegateCommand DeleteCourierCommand { get; }
        public ObservableCollection<Courier> Couriers { get; set; }

        public string CourierName
        {
            get => _courierName;
            set
            {
                _courierName = value;
                OnPropertyChanged();
            }
        }

        private void CreateCourier(object obj)
        {
            _courierService.CreateCourier(CourierName);
            LoadCouriers();
            CourierName = string.Empty;
        }

        private void DeleteCourier(object obj)
        {
            if (!( obj is Courier courier ))
            {
                return;
            }

            _courierService.DeleteCourier(courier);
            Couriers.Remove(courier);
        }

        private void LoadCouriers()
        {
            Couriers.Clear();
            var couriers = _courierService.GetCouriers();
            foreach (var courier in couriers)
            {
                Couriers.Add(courier);
            }
        }
    }
}