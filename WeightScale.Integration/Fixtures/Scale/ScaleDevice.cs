using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Hbm.Automation.Api.Data;
using Hbm.Automation.Api.Weighing.WTX;
using Hbm.Automation.Api.Weighing.WTX.Jet;
using WeightScale.Integration.Services;

namespace WeightScale.Integration.Fixtures.Scale
{
    public interface IScaleDevice
    {
        bool IsConnected { get; }
        void Connect(string ipAddress);
        void SwitchOutput2(bool state);
        event Action<double> WeightDataReceived;
        event Action<bool> WeightStable;
        event Action<bool> ConnectionStatusChanged;
    }

    public class ScaleDevice : IScaleDevice
    {
        private const int StabilityDurationRequired = 300;

        private bool _is1DigitalInput1ActiveLastState;
        private bool _isConnected;
        private bool _isUpdating;
        private bool _isWeightDataProcessed;
        private DateTime? _weightStableSince;
        private WTXJet _wtxDevice;

        private readonly ILogger _logger;

        public ScaleDevice(ILogger logger)
        {
            _logger = logger;
        }

        public event Action<double> WeightDataReceived;
        public event Action<bool> WeightStable;
        public event Action<bool> ConnectionStatusChanged;

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                _isConnected = value;
                ConnectionStatusChanged?.Invoke(IsConnected);
            }
        }

        public void Connect(string ipAddress)
        {
            _logger.LogInfo("Connecting to WTX device IP: " + ipAddress);
            _isWeightDataProcessed = false;
            var jetConnection = new JetBusConnection(ipAddress, "Administrator", "wtx");
            _wtxDevice = new WTXJet(jetConnection, 500, Update);
            try
            {
                _wtxDevice.Connect(2000);
                IsConnected = _wtxDevice.IsConnected;
                _logger.LogInfo("Connected to WTX device IP: " + ipAddress);
            }
            catch (Exception)
            {
                IsConnected = false;
            }
        }

        public void SwitchOutput2(bool state)
        {
            var output2Mode = state ? 1 : 0;
            _wtxDevice.Connection.WriteInteger(JetBusCommands.OM2DigitalOutput2Mode, output2Mode);
            _wtxDevice.Connection.WriteInteger(JetBusCommands.OS2DigitalOutput2, output2Mode);

            if(!state)
            {
                _isUpdating = false;
            }

            _logger.LogInfo($"Switched output 2 to {(state ? "On" : "Off")}");
        }

        private void Update(object sender, ProcessDataReceivedEventArgs e)
        {
            try
            {
                if (e.ProcessData.Is1DigitalInput1Active == null || e.ProcessData.Weight == null)
                {
                    _logger.LogWarning("Received null data from WTX device");
                    return;
                }

                var isWeightStable = e.ProcessData.WeightStable;
                var netWeight = e.ProcessData.Weight.Net;

                var isInput1Active = IsInput1Active();
                if (isInput1Active)
                {
                    HandleWeightStability(isWeightStable, netWeight);
                }
                else
                {
                    _weightStableSince = null;
                }
            }
            catch (IOException ioException)
            {
                Console.WriteLine($"IOException in Update: {ioException.Message}");
                IsConnected = false;
                _wtxDevice.Disconnect();
                Connect(_wtxDevice.Connection.IpAddress);
            }
            catch (SocketException socketException)
            {
                Console.WriteLine($"SocketException in Update: {socketException.Message}");
                IsConnected = false;
                _wtxDevice.Disconnect();
                Connect(_wtxDevice.Connection.IpAddress);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception in Update: {exception.Message}");
                IsConnected = false;
                _wtxDevice.Disconnect();
                Connect(_wtxDevice.Connection.IpAddress);
            }
        }

        private void HandleWeightStability(bool isStable, double weight)
        {
            if (isStable)
            {
                if (!_weightStableSince.HasValue)
                {
                    _weightStableSince = DateTime.Now;
                }
                else if ((DateTime.Now - _weightStableSince.Value).TotalMilliseconds
                         >= StabilityDurationRequired)
                {
                    _logger.LogInfo("Weight is stable for: " + StabilityDurationRequired + "ms");
                    _wtxDevice.Connection.WriteInteger(JetBusCommands.OM1DigitalOutput1Mode, 1);
                    _wtxDevice.Connection.WriteInteger(JetBusCommands.OS1DigitalOutput1, 1);
                    _logger.LogInfo("Output 1 is switched on");
                    OnWeightStable(true);
                    _logger.LogInfo("Stable Weight: " + weight);

                    if(_isUpdating)
                    {
                        return;
                    }

                    _isUpdating = true;
                    OnWeightDataReceived(weight);
                }
            }
            else
            {
                _weightStableSince = null;
                _wtxDevice.Connection.WriteInteger(JetBusCommands.OM1DigitalOutput1Mode, 0);
                _wtxDevice.Connection.WriteInteger(JetBusCommands.OS1DigitalOutput1, 0);
                _logger.LogInfo("Output 1 is switched off");
                OnWeightStable(false);
            }
        }

        private bool IsInput1Active()
        {
            var isInput1 = _wtxDevice.DigitalIO.Input1;
            _logger.LogInfo("Input 1 is active: " + isInput1);
            switch (isInput1)
            {
                case true when !_is1DigitalInput1ActiveLastState && !_isWeightDataProcessed:
                    _is1DigitalInput1ActiveLastState = true;
                    _isWeightDataProcessed = true;
                    _wtxDevice.DigitalIO.Output1 = false;
                    break;
                case false:
                    _is1DigitalInput1ActiveLastState = false;
                    _isWeightDataProcessed = false;
                    break;
            }

            return isInput1;
        }

        private void OnWeightDataReceived(double weight)
        {
            Task.Run(() =>
                     {
                         WeightDataReceived?.Invoke(weight);
                         Console.WriteLine(weight);
                     });
        }

        private void OnWeightStable(bool isStable)
        {
            WeightStable?.Invoke(isStable);
        }
    }
}
