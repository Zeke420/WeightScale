using System;
using WeightScale.BusinessLogicLayer.Mappers;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.DataAccessLayer.DTOs;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.DataAccessLayer.Repository;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IPackageService
    {
        void ConnectDevices(string fullWeightDeviceIp, string emptyWeightDeviceIp);
        event Action<Package> PackageAdded;
        void MovePackage(PackageModel package, ShipmentModel newShipment);
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

            if (obj.EmptyWeight == null
                && obj.FullWeight.HasValue)
            {
                var updatePackage = shipment.Packages.Find(x => x.FullWeight == null);
                updatePackage.FullWeight = obj.FullWeight;
                _packageRepository.Update(updatePackage);
            }

            var package = new Package
                          {
                              FullWeight = obj.FullWeight,
                              EmptyWeight = obj.EmptyWeight,
                              ShipmentId = shipment.Id
                          };

            _packageRepository.Add(package);
            PackageAdded?.Invoke(package);
        }

        public void MovePackage(PackageModel package, ShipmentModel newShipment)
        {
            var packageEntity = PackageMapper.MapToEntity(package);
            packageEntity.ShipmentId = newShipment.Id;
            _packageRepository.Update(packageEntity);
        }
    }
}
