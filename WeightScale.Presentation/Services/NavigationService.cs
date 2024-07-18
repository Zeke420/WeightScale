using System;
using System.Windows;
using WeightScale.BusinessLogicLayer.Utils;
using WeightScale.Presentation.Enums;
using WeightScale.Presentation.Resources.Constants;
using WeightScale.Presentation.Services.Interfaces;
using WeightScale.Presentation.ViewModel;

namespace WeightScale.Presentation.Services
{
    public class NavigationService : ViewModelBase, INavigationService
    {
        private readonly IDialogService _dialogService;
        private readonly IMessenger _messenger;
        private readonly Func<Type, ViewModelBase> _viewModelFactory;
        private ViewModelBase _footerRegion;
        private ViewModelBase _headerRegion;
        private ViewModelBase _mainRegion;

        public NavigationService(
            IDialogService dialogService,
            IMessenger messenger,
            Func<Type, ViewModelBase> viewModelFactory)
        {
            _dialogService = dialogService;
            _messenger = messenger;
            _viewModelFactory = viewModelFactory;
        }

        public ViewModelBase HeaderRegion
        {
            get => _headerRegion;
            set
            {
                _headerRegion = value;
                OnPropertyChanged();
            }
        }

        public ViewModelBase MainRegion
        {
            get => _mainRegion;
            set
            {
                _mainRegion = value;
                OnPropertyChanged();
            }
        }

        public ViewModelBase FooterRegion
        {
            get => _footerRegion;
            set
            {
                _footerRegion = value;
                OnPropertyChanged();
            }
        }

        public void NavigateTo<TViewModel>(NavigationRegion region, object parameter = null)
            where TViewModel : ViewModelBase
        {
            if (!CanNavigate())
            {
                return;
            }

            UpdateRegion<TViewModel>(region, parameter);
        }

        public bool CanNavigate(string  message = null)
        {
            if (MainRegion is null)
            {
                return true;
            }

            if (MainRegion.CanNavigate == NavigationState.WithConfirmation &&
                ShowConfirmNavigation(message) == MessageBoxResult.No)
            {
                return false;
            }

            if (MainRegion.CanNavigate != NavigationState.Forbidden)
            {
                return true;
            }

            ShowForbiddenNavigationDialog();
            return false;
        }

        private MessageBoxResult ShowConfirmNavigation(string message = null)
        {
            var dialogMessage = message ?? DialogMessages.ConfirmMessage;
            return _dialogService.ShowConfirmationDialog(dialogMessage);
        }

        private async void ShowForbiddenNavigationDialog()
        {
            await _dialogService.ShowMessageDialogAsync(DialogMessages.ForbiddenMessage);
        }

        private void UpdateRegion<TViewModel>(NavigationRegion region, object parameter) where TViewModel : ViewModelBase
        {
            var viewModel = CreateViewModel<TViewModel>();

            switch (region)
            {
                case NavigationRegion.Main:
                    MainRegion = OnNavigate(MainRegion, viewModel, parameter);
                    break;
                case NavigationRegion.Header:
                    HeaderRegion = OnNavigate(HeaderRegion, viewModel, parameter);
                    break;
                case NavigationRegion.Footer:
                    FooterRegion = OnNavigate(FooterRegion, viewModel, parameter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(region), region, null);
            }

            _messenger.Send(this);
        }

        private ViewModelBase CreateViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            return _viewModelFactory.Invoke(typeof(TViewModel));
        }

        private ViewModelBase OnNavigate(
            ViewModelBase oldViewModel,
            ViewModelBase newViewModel,
            object parameter)
        {
            if (oldViewModel is null)
            {
                newViewModel.OnNavigatedTo(parameter);
                return newViewModel;
            }

            oldViewModel.OnNavigatedFrom();
            newViewModel.OnNavigatedTo(parameter);

            return newViewModel;
        }
    }
}
