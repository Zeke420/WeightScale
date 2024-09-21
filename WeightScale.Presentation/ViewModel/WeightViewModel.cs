using System;
using System.Collections.ObjectModel;
using System.Linq;
using WeightScale.BusinessLogicLayer.Mappers;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.Integration.Services;
using WeightScale.Presentation.Command;
using WeightScale.Presentation.Services.Interfaces;

namespace WeightScale.Presentation.ViewModel
{
    public class WeightViewModel : ViewModelBase
    {
        private readonly IPackageService _packageService;
        private readonly IShipmentService _shipmentService;
        private readonly IWeightService _weightService;
        private readonly IDialogService _dialogService;
        private readonly ILogger _logger;

        private DateTime _selectedDate;
        private ObservableCollection<ShipmentModel> _shipmentModels;

        public WeightViewModel(IWeightService weightService,
                               IShipmentService shipmentService,
                               IPackageService packageService,
                               IDialogService dialogService,
                               ILogger logger)
        {
            _weightService = weightService;
            _shipmentService = shipmentService;
            _packageService = packageService;
            _dialogService = dialogService;
            _logger = logger;
            _shipmentService.PackageAdded += OnPackageAdded;

            CompleteShipmentCommand = new DelegateCommand(CompleteShipment, CanCompleteShipment);
            LoadShipmentsCommand = new DelegateCommand(LoadShipments);
            ManualMeasureCommand = new DelegateCommand(ManualMeasure);
            PackageMoveCommand = new DelegateCommand(PackageMove);
            Shipments = new ObservableCollection<ShipmentModel>();
            SelectedDate = DateTime.Today;
            LoadShipmentsCommand.Execute(null);
        }

        public ObservableCollection<ShipmentModel> Shipments
        {
            get => _shipmentModels;
            set
            {
                _shipmentModels = value;
                OnPropertyChanged();
            }
        }

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

        private void OnPackageAdded(Package obj)
        {
            _logger.LogInfo("UI: Package received.");
            var shipment = Shipments.FirstOrDefault(s => s.Id == obj.ShipmentId);
            if (shipment == null)
            {
                return;
            }

            var packageModel = PackageMapper.Map(obj, Shipments.ToList());

            var existingPackage = shipment.Packages.FirstOrDefault(p => p.Id == obj.Id);
            if (existingPackage != null)
            {
                existingPackage.FullWeight = packageModel.FullWeight;
                existingPackage.EmptyWeight = packageModel.EmptyWeight;
                existingPackage.WeightDifference = packageModel.WeightDifference;
                existingPackage.FullPackageDate = packageModel.FullPackageDate;
                existingPackage.EmptyPackageDate = packageModel.EmptyPackageDate;
            }
            else
            {
                shipment.Packages.Add(packageModel);
            }

            OnPropertyChanged(nameof(Shipments));
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

            try
            {
                _packageService.ManualMeasure(packageModel);
                var shipment = Shipments.FirstOrDefault(s => s.Id == packageModel.ShipmentId);
                if (shipment == null)
                {
                    return;
                }

                var existingPackage = shipment.Packages.FirstOrDefault(p => p.Id == packageModel.Id);
                var package = PackageMapper.MapToEntity(packageModel);
                packageModel = PackageMapper.Map(package, Shipments.ToList());
                if (existingPackage != null)
                {
                    existingPackage.EmptyWeight = packageModel.EmptyWeight;
                    existingPackage.FullWeight = packageModel.FullWeight;
                    existingPackage.WeightDifference = packageModel.WeightDifference;
                    existingPackage.EmptyPackageDate = packageModel.EmptyPackageDate;
                    existingPackage.FullPackageDate = packageModel.FullPackageDate;
                }

                OnPropertyChanged(nameof(Shipments));
                CompleteShipmentCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageDialogAsync(e.Message);
            }
        }

        private void PackageMove(object obj)
        {
            if (!(obj is PackageModel packageModel))
            {
                return;
            }

            try
            {
                _packageService.MovePackage(packageModel);
                var currentShipment = Shipments.FirstOrDefault(s => s.Id == packageModel.ShipmentId);
                var targetShipment = Shipments.FirstOrDefault(x => x.Id == packageModel.SelectedPackageMoveModel.ShipmentId);

                currentShipment?.Packages.Remove(packageModel);
                packageModel.ShipmentId = packageModel.SelectedPackageMoveModel.ShipmentId;
                var package = PackageMapper.MapToEntity(packageModel);
                packageModel = PackageMapper.Map(package, Shipments.ToList());
                targetShipment?.Packages.Add(packageModel);

                OnPropertyChanged(nameof(Shipments));
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageDialogAsync(e.Message);
            }
        }
    }
}
