using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WeightScale.BusinessLogicLayer.Models
{
    public class PackageModel : INotifyPropertyChanged
    {
        private int _id;
        private string _fullWeight;
        private string _emptyWeight;
        private int _shipmentId;
        private string _weightDifference;
        private bool _canManualMeasure;
        private ObservableCollection<PackageMoveModel> _packageMoves;
        private PackageMoveModel _selectedPackageMoveModel;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FullWeight
        {
            get => _fullWeight;
            set
            {
                if (_fullWeight != value)
                {
                    _fullWeight = value;
                    CanManualMeasure = true;
                    OnPropertyChanged();
                }
            }
        }

        public string EmptyWeight
        {
            get => _emptyWeight;
            set
            {
                if (_emptyWeight != value)
                {
                    _emptyWeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ShipmentId
        {
            get => _shipmentId;
            set
            {
                if (_shipmentId != value)
                {
                    _shipmentId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WeightDifference
        {
            get => _weightDifference;
            set
            {
                if (_weightDifference != value)
                {
                    _weightDifference = value;
                    CanManualMeasure = false;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<PackageMoveModel> PackageMoves
        {
            get => _packageMoves;
            set
            {
                if (_packageMoves != value)
                {
                    _packageMoves = value;
                    OnPropertyChanged();
                }
            }
        }

        public PackageMoveModel SelectedPackageMoveModel
        {
            get => _selectedPackageMoveModel;
            set
            {
                _selectedPackageMoveModel = value;
                OnPropertyChanged();
            }
        }

        public bool CanManualMeasure
        {
            get => _canManualMeasure;
            set
            {
                _canManualMeasure = value;
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
