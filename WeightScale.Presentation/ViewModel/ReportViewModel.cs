using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WeightScale.BusinessLogicLayer.Mappers;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.Presentation.Command;
using WeightScale.Presentation.Services.Interfaces;

namespace WeightScale.Presentation.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly ICourierService _courierService;
        private readonly IFileExportService _fileExportService;
        private readonly IShipmentService _shipmentService;
        private readonly IDialogService _dialogService;
        private List<Courier> _couriers;
        private DateTime _endDate;
        private List<Shipment> _shipments;

        private DateTime _startDate;

        public ReportViewModel(IShipmentService shipmentService,
                               ICourierService courierService,
                               IFileExportService fileExportService,
                               IDialogService dialogService)
        {
            _shipmentService = shipmentService;
            _courierService = courierService;
            _fileExportService = fileExportService;
            _dialogService = dialogService;

            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            Couriers = new ObservableCollection<CouriersSelectionModel>();
            Shipments = new ObservableCollection<Shipment>();

            LoadDataCommand = new DelegateCommand(LoadData);
            ExportDataCommand = new DelegateCommand(ExportData);
            LoadCouriers();
        }

        public ObservableCollection<CouriersSelectionModel> Couriers { get; set; }
        public ObservableCollection<Shipment> Shipments { get; set; }

        public DelegateCommand LoadDataCommand { get; set; }
        public DelegateCommand ExportDataCommand { get; set; }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        private void LoadData(object obj)
        {
            GetShipmentsInDateRange();
        }

        private void GetShipmentsInDateRange()
        {
            Shipments.Clear();
            var selectedCouriers = Couriers.Where(c => c.IsSelected)
                                           .Select(c => c.Courier)
                                           .ToList();

            var shipments = _shipmentService.GetShipmentsInRange(StartDate,
                                                                 EndDate,
                                                                 selectedCouriers);

            foreach (var shipment in shipments)
            {
                Shipments.Add(shipment);
            }
        }

        private void LoadCouriers()
        {
            var couriers = _courierService.GetCouriers();

            foreach (var courier in couriers)
            {
                var courierSelectionModel = new CouriersSelectionModel
                                            {
                                                    Courier = courier,
                                                    IsSelected = false
                                            };

                Couriers.Add(courierSelectionModel);
            }

            OnPropertyChanged(nameof(Couriers));
        }

        private void ExportData(object obj)
        {
            var shipments = Shipments.ToList();
            if (shipments.Count == 0)
            {
                _dialogService.ShowMessageDialogAsync("No data to export.");
                return;
            }

            try
            {
                var exportModels = ExportFileMapper.Map(shipments);
                _fileExportService.ExportToCsv(exportModels);
                _dialogService.SuccessMessage("Data exported successfully.");
            }
            catch (Exception e)
            {
                _dialogService.ShowMessageDialogAsync("An error occurred while exporting data.");
            }

        }
    }
}
