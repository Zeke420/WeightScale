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
        private PackageMoveModel _selectedPackageMoveModel;

        public WeightViewModel(IWeightService weightService,
                               IShipmentService shipmentService,
                               IPackageService packageService)
        {
            _weightService = weightService;
            _shipmentService = shipmentService;
            _packageService = packageService;
            _shipmentService.PackageAdded += OnPackageAdded;

            CompleteShipmentCommand = new DelegateCommand(CompleteShipment, CanCompleteShipment);
            LoadShipmentsCommand = new DelegateCommand(LoadShipments);
            ManualMeasureCommand = new DelegateCommand(ManualMeasure);
            PackageMoveCommand = new DelegateCommand(PackageMove);
            Shipments = new ObservableCollection<ShipmentModel>();
            SelectedDate = DateTime.Today;
            LoadShipmentsCommand.Execute(null);
        }

        public ObservableCollection<ShipmentModel> Shipments { get; set; }
        public DelegateCommand CompleteShipmentCommand { get; set; }
        public DelegateCommand LoadShipmentsCommand { get; set; }
        public DelegateCommand ManualMeasureCommand { get; set; }
        public DelegateCommand PackageMoveCommand { get; set; }

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

        public PackageMoveModel SelectedPackageMoveModel
        {
            get => _selectedPackageMoveModel;
            set
            {
                _selectedPackageMoveModel = value;
                OnPropertyChanged();
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

        private bool CanCompleteShipment(object arg)
        {
            if(!( arg is ShipmentModel shipmentModel ))
            {
                return false;
            }

            return shipmentModel.Packages.All(p => !string.IsNullOrEmpty(p.EmptyWeight));
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

        private void ManualMeasure(object obj)
        {
            if (!( obj is PackageModel packageModel ))
            {
                return;
            }

            _packageService.ManualMeasure(packageModel);
            GetShipmentWeightByDate(_selectedDate);
        }

        private void PackageMove(object obj)
        {
            if (!( obj is PackageModel packageModel ))
            {
                return;
            }

            _packageService.MovePackage(packageModel, SelectedPackageMoveModel.ShipmentId);
            GetShipmentWeightByDate(_selectedDate);
        }
    }
}
