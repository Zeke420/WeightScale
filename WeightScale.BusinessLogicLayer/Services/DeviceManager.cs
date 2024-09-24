using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using WeightScale.BusinessLogicLayer.Models.Messages;
using WeightScale.BusinessLogicLayer.Utils;
using WeightScale.DataAccessLayer.DTOs;
using WeightScale.Integration.Fixtures.Scale;
using WeightScale.Integration.Services;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IDeviceManager
    {
        event Action<PackageWeights> PackageWeightsFilledOut;
        void ConnectDevicesAsync(string fullWeightDeviceIp, string emptyWeightDeviceIp);
    }

    public class DeviceManager : IDeviceManager
    {
        private readonly IScaleDevice _emptyWeightDevice;
        private readonly IScaleDevice _fullWeightDevice;
        private readonly IMessenger _messenger;
        private readonly ILogger _logger;
        private readonly Dispatcher _uiDispatcher;
        private bool _isFullUpdating;
        private bool _isEmptyUpdating;

        public DeviceManager(IScaleDevice fullWeightDevice,
                             IScaleDevice emptyWeightDevice,
                             IMessenger messenger,
                             ILogger logger)
        {
            _fullWeightDevice = fullWeightDevice;
            _emptyWeightDevice = emptyWeightDevice;
            _messenger = messenger;
            _logger = logger;
            _uiDispatcher = Dispatcher.CurrentDispatcher;

            _fullWeightDevice.WeightDataReceived += OnFullWeightDataReceived;
            _fullWeightDevice.WeightStable += OnFullWeightStabilized;
            _fullWeightDevice.ConnectionStatusChanged += OnFullScaleConnectionStatusChanged;

            _emptyWeightDevice.WeightDataReceived += OnEmptyWeightDataReceived;
            _emptyWeightDevice.WeightStable += OnEmptyWeightStabilized;
            _emptyWeightDevice.ConnectionStatusChanged += OnEmptyScaleConnectionStatusChange;

            _isFullUpdating = false;
            _isEmptyUpdating = false;
        }

        public event Action<PackageWeights> PackageWeightsFilledOut;

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
            Task.Run(() => { CreateNewPackage(weight); });
        }

        private void OnEmptyWeightDataReceived(double weight)
        {
            Task.Run(() => { UpdatePackage(weight); });
        }

        private void CreateNewPackage(double fullWeight)
        {
            Task.Run(() =>
                     {
                         if (_isFullUpdating)
                         {
                             return;
                         }

                         try
                         {
                             _isFullUpdating = true;
                             var packageWeight = new PackageWeights
                                                 {
                                                     FullWeight = fullWeight
                                                 };

                             _logger.LogInfo("Full weight received: " + fullWeight);
                             _logger.LogInfo("Updating UI with full weight");
                             _uiDispatcher.Invoke(() => { PackageWeightsFilledOut?.Invoke(packageWeight); });
                             _logger.LogInfo("UI updated with full weight");
                         }
                         catch (Exception e)
                         {
                             Console.WriteLine(e);
                             _logger.LogError("Error creating new package", e);
                             throw;
                         }
                         finally
                         {
                             _isFullUpdating = false;
                             _ = SignalWeightDataReceived(true);
                         }
                     });
        }

        private void UpdatePackage(double emptyWeight)
        {
            Task.Run(() =>
                     {
                         if (_isEmptyUpdating)
                         {
                             return;
                         }

                         try
                         {
                             _isEmptyUpdating = true;

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
                             _logger.LogError("Error updating package weight", e);
                             throw;
                         }
                         finally
                         {
                             _isEmptyUpdating = false;
                             _ = SignalWeightDataReceived(false);
                         }
                     });
        }

        private async Task SignalWeightDataReceived(bool isFull)
        {
            try
            {
                if (isFull)
                {
                    _logger.LogInfo("Device Manager Signaling full weight data received");
                    _fullWeightDevice.SwitchOutput2(true);
                    await Task.Delay(1000);
                    _fullWeightDevice.SwitchOutput2(false);
                }
                else
                {
                    _logger.LogInfo("Device Manager Signaling empty weight data received");
                    _emptyWeightDevice.SwitchOutput2(true);
                    await Task.Delay(1000);
                    _emptyWeightDevice.SwitchOutput2(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Error in SignalWeightDataReceived: {ex.Message}");
                _logger.LogError("Error in SignalWeightDataReceived", ex);
            }
        }
    }
}
