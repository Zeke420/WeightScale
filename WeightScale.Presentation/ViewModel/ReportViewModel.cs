﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.Presentation.Command;

namespace WeightScale.Presentation.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly ICourierService _courierService;
        private readonly IFileExportService _fileExportService;
        private readonly IShipmentService _shipmentService;
        private List<Courier> _couriers;
        private DateTime _endDate;
        private List<Shipment> _shipments;

        private DateTime _startDate;

        public ReportViewModel(IShipmentService shipmentService,
                               ICourierService courierService,
                               IFileExportService fileExportService)
        {
            _shipmentService = shipmentService;
            _courierService = courierService;
            _fileExportService = fileExportService;

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
            _fileExportService.ExportToCsv(shipments);
        }
    }
}