using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeightScale.Integration.Fixtures.Scale
{
    public class MockScaleDevice : IScaleDevice
    {
        private bool _isConnected;
        private readonly Random _random = new Random();
        private CancellationTokenSource _cancellationTokenSource;

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
            IsConnected = true;
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => SimulateWeightDataAsync(_cancellationTokenSource.Token));
        }

        public void SwitchOutput2(bool state)
        {
            Console.WriteLine("Switching output 2 to " + state);
        }

        public void Disconnect()
        {
            IsConnected = false;
            _cancellationTokenSource?.Cancel();
        }

        private async Task SimulateWeightDataAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                double weight = _random.Next(100, 501);
                var isStable = _random.Next(0, 2) == 1;
                WeightDataReceived?.Invoke(weight);
                WeightStable?.Invoke(isStable);

                var delay = _random.Next(40000, 50001);
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
