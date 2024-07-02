namespace WeightScale.Presentation.Services.Interfaces
{
    public interface INavigationAware
    {
        void OnNavigatedTo(object parameter);
        void OnNavigatedFrom();
    }
}