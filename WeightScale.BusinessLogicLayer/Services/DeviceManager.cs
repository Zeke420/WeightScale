using System;
using WeightScale.DataAccessLayer.DTOs;
using WeightScale.Integration.Fixtures.Scale;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IDeviceManager
    {
        event Action<PackageWeights> PackageWeightsFilledOut;
        void ConnectDevicesAsync(string fullWeightDeviceIp, string emptyWeightDeviceIp);
    }

    public class DeviceManager : IDeviceManager
    {
        private readonly IScaleDevice _fullWeightDevice;
        private readonly IScaleDevice _emptyWeightDevice;
        private PackageWeights _packageWeights;
        public event Action<PackageWeights> PackageWeightsFilledOut;
        private bool _isWeightsProcessed;
        private bool isUpdating;

        public DeviceManager(IScaleDevice fullWeightDevice, IScaleDevice emptyWeightDevice)
        {
            _fullWeightDevice = fullWeightDevice;
            _emptyWeightDevice = emptyWeightDevice;

            _fullWeightDevice.WeightDataReceived += OnFullWeightDataReceived;
            _emptyWeightDevice.WeightDataReceived += OnEmptyWeightDataReceived;

            _packageWeights = new PackageWeights();
            isUpdating = false;
        }

        public void ConnectDevicesAsync(string fullWeightDeviceIp, string emptyWeightDeviceIp)
        {
            _fullWeightDevice.Connect(fullWeightDeviceIp);
            _emptyWeightDevice.Connect(emptyWeightDeviceIp);
        }

        private void OnFullWeightDataReceived(double weight)
        {
            if(_isWeightsProcessed)
            {
                return;
            }
            
            _packageWeights.FullWeight = weight;
        }

        private void OnEmptyWeightDataReceived(double weight)
        {
            if (_isWeightsProcessed)
            {
                return;
            }
            
            _packageWeights.EmptyWeight = weight;
            CheckIfPackageWeightsAreFilledOut();
        }

        private void CheckIfPackageWeightsAreFilledOut()
        {
            if(isUpdating)
            {
                return;
            }

            try
            {
                isUpdating = true;
                if (!_packageWeights.IsFilledOut || _isWeightsProcessed)
                {
                    return;
                }

                _isWeightsProcessed = true;
                PackageWeightsFilledOut?.Invoke(_packageWeights);
                ResetWeights();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                isUpdating = false;
                
            }
        }

        private void ResetWeights()
        {
            _packageWeights = new PackageWeights();
            _isWeightsProcessed = false;
        }
    }
}