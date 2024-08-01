using WeightScale.BusinessLogicLayer.Models.Messages;
using WeightScale.BusinessLogicLayer.Utils;

namespace WeightScale.Presentation.ViewModel
{
    public class FooterViewModel : ViewModelBase
    {
        private readonly IMessenger _messenger;
        private bool _isEmptyScaleConnected;
        private bool _isEmptyWeightStable;
        private bool _isFullScaleConnected;
        private bool _isFullWeightStable;

        public FooterViewModel(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public bool IsEmptyScaleConnected
        {
            get => _isEmptyScaleConnected;
            set
            {
                _isEmptyScaleConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsFullScaleConnected
        {
            get => _isFullScaleConnected;
            set
            {
                _isFullScaleConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmptyWeightStable
        {
            get => _isEmptyWeightStable;
            set => SetField(ref _isEmptyWeightStable, value);
        }

        public bool IsFullWeightStable
        {
            get => _isFullWeightStable;
            set => SetField(ref _isFullWeightStable, value);
        }

        public override void OnNavigatedTo(object parameter)
        {
            _messenger.Subscribe<EmptyConnectionStatus>(this, UpdateEmptyConnectionStatus);
            _messenger.Subscribe<EmptyScaleWeightStable>(this, UpdateEmptyWeightStable);

            _messenger.Subscribe<FullConnectionStatus>(this, UpdateFullConnectionStatus);
            _messenger.Subscribe<FullScaleWeightStable>(this, UpdateFullWeightStable);
        }

        private void UpdateEmptyConnectionStatus(object obj)
        {
            if (obj is EmptyConnectionStatus emptyConnectionStatus)
            {
                IsEmptyScaleConnected = emptyConnectionStatus.IsConnected;
            }
        }

        private void UpdateFullConnectionStatus(object obj)
        {
            if (obj is FullConnectionStatus fullConnectionStatus)
            {
                IsFullScaleConnected = fullConnectionStatus.IsConnected;
            }
        }

        private void UpdateEmptyWeightStable(object obj)
        {
            if (obj is EmptyScaleWeightStable scaleWeightStable)
            {
                IsEmptyWeightStable = scaleWeightStable.IsStable;
            }
        }

        private void UpdateFullWeightStable(object obj)
        {
            if (obj is FullScaleWeightStable scaleWeightStable)
            {
                IsFullWeightStable = scaleWeightStable.IsStable;
                OnPropertyChanged(nameof(IsFullWeightStable));
            }
        }
    }
}