using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WeightScale.Presentation.ViewModel;

namespace WeightScale.Presentation.Helpers
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ViewModelLocator()
        {
            _serviceProvider = ((App)Application.Current).ServiceProvider;
        }

        public HeaderViewModel HeaderViewModel => _serviceProvider.GetService<HeaderViewModel>();
        public CourierViewModel CourierViewModel => _serviceProvider.GetService<CourierViewModel>();
        public ShipmentViewModel ShipmentViewModel => _serviceProvider.GetService<ShipmentViewModel>();
        public WeightViewModel WeightViewModel => _serviceProvider.GetService<WeightViewModel>();
        public ReportViewModel ReportViewModel => _serviceProvider.GetService<ReportViewModel>();
    }
}