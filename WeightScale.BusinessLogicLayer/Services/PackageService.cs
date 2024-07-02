using System;
using WeightScale.DataAccessLayer.DTOs;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.DataAccessLayer.Repository;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IPackageService
    {
        void ConnectDevices(string fullWeightDeviceIp, string emptyWeightDeviceIp);
        event Action<Package> PackageAdded;
    }

    public class PackageService : IPackageService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IDeviceManager _deviceManager;

        public event Action<Package> PackageAdded;

        public PackageService(
            IDeviceManager deviceManager,
            IShipmentRepository shipmentRepository,
            IPackageRepository packageRepository)
        {
            _shipmentRepository = shipmentRepository;
            _packageRepository = packageRepository;
            _deviceManager = deviceManager;

            _deviceManager.PackageWeightsFilledOut += OnPackageWeightsFilledOut;
        }

        public void ConnectDevices(string fullWeightDeviceIp, string emptyWeightDeviceIp)
        {
            _deviceManager.ConnectDevicesAsync(fullWeightDeviceIp, emptyWeightDeviceIp);
        }

        private void OnPackageWeightsFilledOut(PackageWeights obj)
        {
            var shipment = _shipmentRepository.GetFirstUnFinishedShipment();
            var package = new Package
                          {
                              FullWeight = obj.FullWeight,
                              EmptyWeight = obj.EmptyWeight,
                              ShipmentId = shipment.Id
                          };

            _packageRepository.Add(package);
            PackageAdded?.Invoke(package);
        }
    }
}