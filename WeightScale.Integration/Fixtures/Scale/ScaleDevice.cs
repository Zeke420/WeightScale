using System;
using System.Threading.Tasks;
using Hbm.Automation.Api.Data;
using Hbm.Automation.Api.Weighing.WTX;
using Hbm.Automation.Api.Weighing.WTX.Jet;

namespace WeightScale.Integration.Fixtures.Scale
{
    public interface IScaleDevice
    {
        void Connect(string ipAddress);
        event Action<double> WeightDataReceived;
        event Action<bool> WeightStable;
        event Action<bool> ConnectionStatusChanged;
        bool IsConnected { get; }
    }

    public class ScaleDevice : IScaleDevice
    {
        public event Action<double> WeightDataReceived;
        public event Action<bool> WeightStable;
        public event Action<bool> ConnectionStatusChanged;

        private const int StabilityDurationRequired = 3000;
        private DateTime? _weightStableSince = null;
        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                _isConnected = value;
                ConnectionStatusChanged?.Invoke(IsConnected);
            }
        }

        private bool _is1DigitalInput1ActiveLastState;
        private WTXJet _wtxDevice;
        private bool _isInput1ActiveLastState;
        private bool _isWeightDataProcessed;
        private bool _isUpdating = false;

        public void Connect(string ipAddress)
        {
            _isInput1ActiveLastState = false;
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
            if(_isUpdating)
            {
                return;
            }

            try
            {
                _isUpdating = true;
                if (e.ProcessData.Is1DigitalInput1Active == null
                    || e.ProcessData.Weight == null)
                {
                    return;
                }

                var isWeightStable = e.ProcessData.WeightStable;
                var netWeight = e.ProcessData.Weight.Net;

                HandleWeightStability(isWeightStable,
                                      netWeight);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                IsConnected = false;
                throw;
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
                else if((DateTime.Now - _weightStableSince.Value).TotalMilliseconds >= StabilityDurationRequired)
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
            if (isInput1
                && !_is1DigitalInput1ActiveLastState
                && !_isWeightDataProcessed)
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
