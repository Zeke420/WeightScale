using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeightScale.Presentation.Enums;
using WeightScale.Presentation.Services.Interfaces;

namespace WeightScale.Presentation.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual NavigationState CanNavigate => NavigationState.Allowed;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual void OnNavigatedTo(object parameter)
        {
        }

        public virtual void OnNavigatedFrom()
        {
        }
    }
}
