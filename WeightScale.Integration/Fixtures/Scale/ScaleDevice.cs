using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Hbm.Automation.Api.Data;
using Hbm.Automation.Api.Weighing.WTX;
using Hbm.Automation.Api.Weighing.WTX.Jet;

namespace WeightScale.Integration.Fixtures.Scale
{
    public interface IScaleDevice
    {
        bool IsConnected { get; }
        void Connect(string ipAddress);
        event Action<double> WeightDataReceived;
        event Action<bool> WeightStable;
        event Action<bool> ConnectionStatusChanged;
    }

    public class ScaleDevice : IScaleDevice
    {
        private const int StabilityDurationRequired = 3000;

        private bool _is1DigitalInput1ActiveLastState;
        private bool _isConnected;
        private bool _isUpdating;
        private bool _isWeightDataProcessed;
        private DateTime? _weightStableSince;
        private WTXJet _wtxDevice;
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
            _isWeightDataProcessed = false;
            var jetConnection = new JetBusConnection(ipAddress, "Administrator", "wtx");
            _wtxDevice = new WTXJet(jetConnection, 500, Update);
            try
            {
                _wtxDevice.Connect(2000);
                IsConnected = _wtxDevice.IsConnected;
            }
            catch (Exception)
            {
                IsConnected = false;
            }
        }

        private void Update(object sender, ProcessDataReceivedEventArgs e)
        {
            if (_isUpdating)
            {
                return;
            }

            try
            {
                _isUpdating = true;
                if (e.ProcessData.Is1DigitalInput1Active == null || e.ProcessData.Weight == null)
                {
                    return;
                }

                var isWeightStable = e.ProcessData.WeightStable;
                var netWeight = e.ProcessData.Weight.Net;

                HandleWeightStability(isWeightStable, netWeight);
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
            finally
            {
                _isUpdating = false;
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
                else if (( DateTime.Now - _weightStableSince.Value ).TotalMilliseconds >= StabilityDurationRequired)
                {
                    _wtxDevice.DigitalIO.Output1 = true;
                    OnWeightStable(true);
                    CheckDigitalInput1ActiveState(weight);
                }
            }
            else
            {
                _weightStableSince = null;
                _wtxDevice.DigitalIO.Output1 = false;
                OnWeightStable(false);
            }
        }

        private void CheckDigitalInput1ActiveState(double weight)
        {
            var isInput1 = _wtxDevice.DigitalIO.Input1;
            if (isInput1 && !_is1DigitalInput1ActiveLastState && !_isWeightDataProcessed)
            {
                OnWeightDataReceived(weight);
                _is1DigitalInput1ActiveLastState = true;
                _isWeightDataProcessed = true;
                _wtxDevice.DigitalIO.Output1 = false;
            }
            else if (!isInput1)
            {
                _is1DigitalInput1ActiveLastState = false;
                _isWeightDataProcessed = false;
            }
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
            try
            {
                WeightStable?.Invoke(isStable);

                if (!isStable)
                {
                    return;
                }

                _wtxDevice.DigitalIO.Output1 = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnWeightStable: {ex.Message}");
            }
        }
    }
}
