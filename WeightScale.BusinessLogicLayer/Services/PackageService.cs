using System;
using WeightScale.BusinessLogicLayer.Mappers;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.DataAccessLayer.DTOs;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.DataAccessLayer.Repository;
using WeightScale.Integration.Services;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IPackageService
    {
        event Action<Package> PackageAdded;
        void MovePackage(PackageModel package);
        void ManualMeasure(PackageModel packageModel);
        void SaveFullWeight(PackageWeights packageWeights);
        void SaveEmptyWeight(PackageWeights packageWeights);
    }

    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly ILogger _logger;

        public PackageService(
                IShipmentRepository shipmentRepository,
                IPackageRepository packageRepository,
                ILogger logger)
        {
            _shipmentRepository = shipmentRepository;
            _packageRepository = packageRepository;
            _logger = logger;
        }

        public event Action<Package> PackageAdded;

        public void MovePackage(PackageModel package)
        {
            var packageEntity = PackageMapper.MapToEntity(package);
            packageEntity.ShipmentId = package.SelectedPackageMoveModel.ShipmentId;
            _packageRepository.Update(packageEntity);
        }

        public void ManualMeasure(PackageModel packageModel)
        {
            packageModel.EmptyWeight = "0";
            packageModel.EmptyPackageDate = DateTime.Now;
            var package = PackageMapper.MapToEntity(packageModel);
            _packageRepository.Update(package);
        }

        public void SaveFullWeight(PackageWeights packageWeights)
        {
            var shipment = _shipmentRepository.GetFirstUnFinishedShipment();
            if (shipment == null)
            {
                _logger.LogWarning("No shipment found for package weights.");
                return;
            }

            var package = new Package
                          {
                                  FullWeight = packageWeights.FullWeight,
                                  EmptyWeight = null,
                                  ShipmentId = shipment.Id,
                                  FullPackageDate = DateTime.Now
                          };

            _packageRepository.Add(package);
            _logger.LogInfo("Db: New package added to shipment " + shipment.Id);
            PackageAdded?.Invoke(package);
        }

        public void SaveEmptyWeight(PackageWeights packageWeights)
        {
            var shipment = _shipmentRepository.GetFirstUnFinishedShipment();
            if (shipment == null)
            {
                _logger.LogWarning("No shipment found for package weights.");
                return;
            }

            var updatePackage = shipment.Packages.Find(x => x.EmptyWeight == null);
            if (updatePackage == null)
            {
                return;
            }

            updatePackage.EmptyWeight = packageWeights.EmptyWeight;
            updatePackage.EmptyPackageDate = DateTime.Now;

            _packageRepository.Update(updatePackage);
            _logger.LogInfo($"Db: Empty weight for package {updatePackage.Id} updated to {updatePackage.EmptyWeight}");
            PackageAdded?.Invoke(updatePackage);
        }

        private void OnPackageWeightsFilledOut(PackageWeights obj)
        {
            var shipment = _shipmentRepository.GetFirstUnFinishedShipment();
            if (shipment == null)
            {
                _logger.LogWarning("No shipment found for package weights.");
                return;
            }

            if (obj.FullWeight == null && obj.EmptyWeight.HasValue)
            {
                var updatePackage = shipment.Packages.Find(x => x.EmptyWeight == null);
                if (updatePackage == null)
                {
                    return;
                }

                updatePackage.EmptyWeight = obj.EmptyWeight;
                updatePackage.EmptyPackageDate = DateTime.Now;

                _packageRepository.Update(updatePackage);
                _logger.LogInfo($"Db: Empty weight for package {updatePackage.Id} updated to {obj.EmptyWeight}");
                PackageAdded?.Invoke(updatePackage);

                return;
            }

            var package = new Package
                          {
                                  FullWeight = obj.FullWeight,
                                  EmptyWeight = obj.EmptyWeight,
                                  ShipmentId = shipment.Id,
                                  FullPackageDate = DateTime.Now
                          };

            _packageRepository.Add(package);
            _logger.LogInfo("Db: New package added to shipment " + shipment.Id);
            PackageAdded?.Invoke(package);
        }
    }
}
