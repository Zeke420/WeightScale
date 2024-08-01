using System;
using System.Collections.ObjectModel;
using System.Linq;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.Presentation.Command;

namespace WeightScale.Presentation.ViewModel
{
    public class WeightViewModel : ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly IShipmentService _shipmentService;
        private readonly IWeightService _weightService;
        private DateTime _selectedDate;

        public WeightViewModel(IWeightService weightService,
                               IShipmentService shipmentService,
                               IPackageService packageService)
        {
            _weightService = weightService;
            _shipmentService = shipmentService;
            _packageService = packageService;
            _shipmentService.PackageAdded += OnPackageAdded;

            CompleteShipmentCommand = new DelegateCommand(CompleteShipment);
            MovePackageUpCommand = new DelegateCommand(MovePackageUp);
            MovePackageDownCommand = new DelegateCommand(MovePackageDown);
            LoadShipmentsCommand = new DelegateCommand(LoadShipments);
            Shipments = new ObservableCollection<ShipmentModel>();
            SelectedDate = DateTime.Today;
            LoadShipmentsCommand.Execute(null);
        }

        public ObservableCollection<ShipmentModel> Shipments { get; set; }
        public DelegateCommand CompleteShipmentCommand { get; set; }
        public DelegateCommand MovePackageUpCommand { get; set; }
        public DelegateCommand MovePackageDownCommand { get; set; }
        public DelegateCommand LoadShipmentsCommand { get; set; }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                GetShipmentWeightByDate(_selectedDate);
            }
        }

        private void OnPackageAdded(Package obj)
        {
            GetShipmentWeightByDate(SelectedDate);
        }

        private void LoadShipments(object obj)
        {
            GetShipmentWeightByDate(SelectedDate);
        }

        private void CompleteShipment(object obj)
        {
            if (!( obj is ShipmentModel shipmentModel ))
            {
                return;
            }

            shipmentModel.IsFinished = true;
            _weightService.CompleteShipment(shipmentModel);
            OnPropertyChanged(nameof(Shipments));
            GetShipmentWeightByDate(_selectedDate);
        }

        private void GetShipmentWeightByDate(DateTime date)
        {
            Shipments?.Clear();
            var shipments = _weightService.GetShipmentWeightByDate(date);
            foreach (var shipment in shipments)
            {
                Shipments?.Add(shipment);
            }
        }

        private void MovePackageDown(object obj)
        {
            if (!( obj is PackageModel packageModel ))
            {
                return;
            }

            var shipment = Shipments.First(s => s.Id == packageModel.ShipmentId);
            var index = Shipments.IndexOf(shipment);
            var nextShipmentModel = Shipments[index + 1];

            if (nextShipmentModel == null)
            {
                return;
            }

            _packageService.MovePackage(packageModel, nextShipmentModel);
            GetShipmentWeightByDate(_selectedDate);
        }

        private void MovePackageUp(object obj)
        {
            if (!( obj is PackageModel packageModel ))
            {
                return;
            }

            var shipment = Shipments.First(s => s.Id == packageModel.ShipmentId);
            var index = Shipments.IndexOf(shipment);
            var nextShipmentModel = Shipments[index - 1];

            if (nextShipmentModel == null)
            {
                return;
            }

            _packageService.MovePackage(packageModel, nextShipmentModel);
            GetShipmentWeightByDate(_selectedDate);
        }
    }
}
