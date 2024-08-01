using System;
using System.Collections.Generic;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.DataAccessLayer.Repository;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IShipmentService
    {
        List<Shipment> GetShipmentsByDate(DateTime date);
        void AddShipment(Courier selectedCourier, DateTime selectedDate);
        List<Shipment> GetShipmentWeightByDate(DateTime date);
        void CompleteShipment(Shipment shipment);
        event Action<Package> PackageAdded;
        void DeleteShipment(Shipment shipment);
        List<Shipment> GetShipmentsInRange(DateTime startDate, DateTime endDate, List<Courier> couriers);
    }

    public class ShipmentService : IShipmentService
    {
        private readonly IPackageService _packageService;
        private readonly IShipmentRepository _shipmentRepository;

        public ShipmentService(IShipmentRepository shipmentRepository,
                               IPackageService packageService)
        {
            _shipmentRepository = shipmentRepository;
            _packageService = packageService;
            _packageService.PackageAdded += OnPackageAdded;
        }

        public event Action<Package> PackageAdded;

        public List<Shipment> GetShipmentsByDate(DateTime date)
        {
            var shipments = _shipmentRepository.GetShipmentsByDate(date);
            return shipments ?? new List<Shipment>();
        }

        public void AddShipment(Courier selectedCourier, DateTime selectedDate)
        {
            var shipment = new Shipment(selectedDate, selectedCourier.Id);
            _shipmentRepository.AddShipment(shipment);
        }

        public List<Shipment> GetShipmentWeightByDate(DateTime date)
        {
            var shipments = _shipmentRepository.GetShipmentsWeightsByDate(date);
            return shipments ?? new List<Shipment>();
        }

        public void CompleteShipment(Shipment shipment)
        {
            _shipmentRepository.CompleteShipment(shipment);
        }

        public void DeleteShipment(Shipment shipment)
        {
            _shipmentRepository.Delete(shipment);
        }

        public List<Shipment> GetShipmentsInRange(DateTime startDate, DateTime endDate, List<Courier> couriers)
        {
            return _shipmentRepository.GetShipmentsInRange(startDate, endDate, couriers);
        }

        private void OnPackageAdded(Package obj)
        {
            PackageAdded?.Invoke(obj);
        }
    }
}
