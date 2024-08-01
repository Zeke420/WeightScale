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
        private readonly IDeviceManager _deviceManager;
        private readonly IPackageRepository _packageRepository;
        private readonly IShipmentRepository _shipmentRepository;

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

        public event Action<Package> PackageAdded;

        public void ConnectDevices(string fullWeightDeviceIp, string emptyWeightDeviceIp)
        {
            _deviceManager.ConnectDevicesAsync(fullWeightDeviceIp, emptyWeightDeviceIp);
        }

        public void MovePackage(PackageModel package, ShipmentModel newShipment)
        {
            var packageEntity = PackageMapper.MapToEntity(package);
            packageEntity.ShipmentId = newShipment.Id;
            _packageRepository.Update(packageEntity);
        }

        private void OnPackageWeightsFilledOut(PackageWeights obj)
        {
            var shipment = _shipmentRepository.GetFirstUnFinishedShipment();

            if (obj.FullWeight == null && obj.EmptyWeight.HasValue)
            {
                var updatePackage = shipment.Packages.Find(x => x.EmptyWeight == null);
                if (updatePackage == null)
                {
                    return;
                }

                updatePackage.EmptyWeight = obj.EmptyWeight;
                _packageRepository.Update(updatePackage);
                PackageAdded?.Invoke(updatePackage);

                return;
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
    }
}