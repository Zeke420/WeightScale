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
            private set
            {
                _isEmptyScaleConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsFullScaleConnected
        {
            get => _isFullScaleConnected;
            private set
            {
                _isFullScaleConnected = value;
                OnPropertyChanged();
            }
        }

        public override void OnNavigatedTo(object parameter)
        {
            _messenger.Subscribe<EmptyConnectionStatus>(this, UpdateEmptyConnectionStatus);
            _messenger.Subscribe<FullConnectionStatus>(this, UpdateFullConnectionStatus);
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
    }
}
