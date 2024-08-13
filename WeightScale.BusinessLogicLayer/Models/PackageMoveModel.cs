using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WeightScale.BusinessLogicLayer.Models
{
    public class PackageMoveModel : INotifyPropertyChanged
    {
        private string _courierName;
        private int _shipmentId;

        public string CourierName
        {
            get => _courierName;
            set
            {
                _courierName = value;
                OnPropertyChanged();
            }
        }

        public int ShipmentId
        {
            get => _shipmentId;
            set
            {
                _shipmentId = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
