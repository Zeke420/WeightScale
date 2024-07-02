using System;
using System.Linq;
using Hbm.Automation.Api.Data;
using Hbm.Automation.Api.Weighing.WTX;
using Hbm.Automation.Api.Weighing.WTX.Jet;

namespace WeightScale.Integration.Fixtures.Scale
{
    public interface IScaleDevice
    {
        void Connect(string ipAddress);
        event Action<double> WeightDataReceived;
        bool IsConnected { get; }
    }

    public class ScaleDevice : IScaleDevice
    {
        public event Action<double> WeightDataReceived;
        public bool IsConnected { get; private set; }
        
        private bool _is1DigitalInput1ActiveLastState;
        private WTXJet _wtxDevice;
        private bool _isInput1ActiveLastState;
        private bool _isWeightDataProcessed;
        private bool isUpdating = false;
    
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
            if(isUpdating)
            {
                return;
            }

            try
            {
                isUpdating = true;
                if (e.ProcessData.Is1DigitalInput1Active == null)
                {
                    return;
                }

                var isInput1 = _wtxDevice.DigitalIO.Input1;

                if (isInput1 && !_is1DigitalInput1ActiveLastState && !_isWeightDataProcessed)
                {
                    OnWeightDataReceived(e.ProcessData.Weight.Net);
                    _isInput1ActiveLastState = true;
                    _isWeightDataProcessed = true;
                }
                else if (!isInput1)
                {
                    _isInput1ActiveLastState = false;
                    _isWeightDataProcessed = false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void OnWeightDataReceived(double weight)
        {
            WeightDataReceived?.Invoke(weight);
            Console.WriteLine(weight);
        }
    }
}