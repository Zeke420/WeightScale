using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.Presentation.Command;

namespace WeightScale.Presentation.ViewModel
{
    public class ShipmentViewModel : ViewModelBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly ICourierService _courierService;
        private DateTime _selectedDate;
        private Courier _selectedCourier;

        public ShipmentViewModel(IShipmentService shipmentService,
                                 ICourierService courierService)
        {
            _shipmentService = shipmentService;
            _courierService = courierService;
            SelectedDate = DateTime.Today;
            LoadShipmentsCommand = new DelegateCommand(LoadShipments);
            AddCourierCommand = new DelegateCommand(AddCourier);
            DeleteShipmentCommand = new DelegateCommand(DeleteShipment);
            Shipments = new ObservableCollection<Shipment>();
            Couriers = new List<Courier>();
            LoadCouriers();
            LoadShipments(null);
        }
        
        public DelegateCommand LoadShipmentsCommand { get; set; }
        public DelegateCommand AddCourierCommand { get; set; }
        public DelegateCommand DeleteShipmentCommand { get; set; }
        public ObservableCollection<Shipment> Shipments { get; set; }
        public List<Courier> Couriers { get; }
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }
        
        public Courier SelectedCourier
        {
            get => _selectedCourier;
            set
            {
                _selectedCourier = value;
                OnPropertyChanged();
            }
        }

        private void AddCourier(object obj)
        {
            if (SelectedCourier is null)
            {
                return;
            }
            
            _shipmentService.AddShipment(SelectedCourier, SelectedDate);
            LoadShipments(null);
        }
        private void LoadShipments(object obj)
        {
            Shipments.Clear();
            var shipments = _shipmentService.GetShipmentsByDate(SelectedDate);
            foreach (var shipment in shipments)
            {
                Shipments.Add(shipment);
            }
        }
        
        private void DeleteShipment(object obj)
        {
            if(!(obj is Shipment shipment))
            {
                return;
            }
            
            _shipmentService.DeleteShipment(shipment);
            LoadShipmentsCommand.Execute(null);
        }
        
        private void LoadCouriers()
        {
            var couriers = _courierService.GetCouriers();
            foreach (var courier in couriers)
            {
                Couriers.Add(courier);
            }
            OnPropertyChanged(nameof(Couriers));
        }
    }
}