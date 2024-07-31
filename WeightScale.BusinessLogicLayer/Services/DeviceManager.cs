using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using WeightScale.BusinessLogicLayer.Models.Messages;
using WeightScale.BusinessLogicLayer.Utils;
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
        private readonly IMessenger _messenger;
        private readonly Dispatcher _uiDispatcher;
        public event Action<PackageWeights> PackageWeightsFilledOut;
        private bool _isUpdating;

        public DeviceManager(IScaleDevice fullWeightDevice,
                             IScaleDevice emptyWeightDevice,
                             IMessenger messenger)
        {
            _fullWeightDevice = fullWeightDevice;
            _emptyWeightDevice = emptyWeightDevice;
            _messenger = messenger;
            _uiDispatcher = Dispatcher.CurrentDispatcher;

            _fullWeightDevice.WeightDataReceived += OnFullWeightDataReceived;
            _fullWeightDevice.WeightStable += OnFullWeightStabilized;
            _fullWeightDevice.ConnectionStatusChanged += OnFullScaleConnectionStatusChanged;

            _emptyWeightDevice.WeightDataReceived += OnEmptyWeightDataReceived;
            _emptyWeightDevice.WeightStable += OnEmptyWeightStabilized;
            _emptyWeightDevice.ConnectionStatusChanged += OnEmptyScaleConnectionStatusChange;

            _isUpdating = false;
        }

        public void ConnectDevicesAsync(string fullWeightDeviceIp, string emptyWeightDeviceIp)
        {
            _fullWeightDevice.Connect(fullWeightDeviceIp);
            _emptyWeightDevice.Connect(emptyWeightDeviceIp);
        }

        private void OnFullScaleConnectionStatusChanged(bool isConnected)
        {
            var message = new FullConnectionStatus
                          {
                                  IsConnected = isConnected
                          };

            _messenger.Send(message);
        }

        private void OnEmptyScaleConnectionStatusChange(bool isConnected)
        {
            var message = new EmptyConnectionStatus
                          {
                                  IsConnected = isConnected
                          };

            _messenger.Send(message);
        }

        private void OnFullWeightStabilized(bool isStable)
        {
            var message = new FullScaleWeightStable
                          {
                                  IsStable = isStable
                          };

            _messenger.Send(message);
        }

        private void OnEmptyWeightStabilized(bool isStable)
        {
            var message = new EmptyScaleWeightStable
                          {
                                  IsStable = isStable
                          };

            _messenger.Send(message);
        }

        private void OnFullWeightDataReceived(double weight)
        {
            Task.Run(() =>
                     {
                         CreateNewPackage(weight);
                     });
        }

        private void OnEmptyWeightDataReceived(double weight)
        {
            Task.Run(() =>
                     {
                         UpdatePackage(weight);
                     });
        }

        private void CreateNewPackage(double fullWeight)
        {
            Task.Run(() =>
                     {
                         if (_isUpdating)
                         {
                             return;
                         }

                         try
                         {
                             var packageWeight = new PackageWeights
                                                 {
                                                         FullWeight = fullWeight
                                                 };

                             _uiDispatcher.Invoke(() => { PackageWeightsFilledOut?.Invoke(packageWeight); });
                         }
                         catch (Exception e)
                         {
                             Console.WriteLine(e);
                             throw;
                         }
                         finally
                         {
                             _isUpdating = false;
                         }
                     });
        }

        private void UpdatePackage(double emptyWeight)
        {
            Task.Run(() =>
                     {
                         if (_isUpdating)
                         {
                             return;
                         }

                         try
                         {
                             _isUpdating = true;

                             // Use the captured UI dispatcher
                             var packageWeight = new PackageWeights
                                                 {
                                                         EmptyWeight = emptyWeight
                                                 };

                             _uiDispatcher.Invoke(() => { PackageWeightsFilledOut?.Invoke(packageWeight); });
                         }
                         catch (Exception e)
                         {
                             Console.WriteLine(e);
                             throw;
                         }
                         finally
                         {
                             _isUpdating = false;
                         }
                     });
        }
    }
}
