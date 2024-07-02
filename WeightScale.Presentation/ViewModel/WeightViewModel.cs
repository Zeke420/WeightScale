using System;
using System.Collections.Generic;
using WeightScale.BusinessLogicLayer.Services;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.Presentation.Command;

namespace WeightScale.Presentation.ViewModel
{
public class WeightViewModel : ViewModelBase
{
    private readonly IWeightService _weightService;
    private readonly IShipmentService _shipmentService;
    private DateTime _selectedDate;
    private List<Shipment> _shipments;

    public WeightViewModel(IWeightService weightService,
                           IShipmentService shipmentService)
    {
        _weightService = weightService;
        _shipmentService = shipmentService;
        _shipmentService.PackageAdded += OnPackageAdded;
        
        SelectedDate = DateTime.Today;
        CompleteShipmentCommand = new DelegateCommand(CompleteShipment);
        Shipments = new List<Shipment>();
    }

    private void OnPackageAdded(Package obj)
    {
        GetShipmentWeightByDate(SelectedDate);
    }

    public DelegateCommand CompleteShipmentCommand { get; set; }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            _selectedDate = value;
            OnPropertyChanged();
            GetShipmentWeightByDate(_selectedDate);
        }
    }

    public List<Shipment> Shipments
    {
        get => _shipments;
        set
        {
            _shipments = value;
            OnPropertyChanged();
        }
    }

    private void CompleteShipment(object obj)
    {
        if (!(obj is Shipment shipment))
        {
            return;
        }

        shipment.IsFinished = true;
        _weightService.CompleteShipment(shipment);
        OnPropertyChanged(nameof(Shipments));
        GetShipmentWeightByDate(_selectedDate);
    }
    private void GetShipmentWeightByDate(DateTime date)
    {
        Shipments?.Clear();
        Shipments = _weightService.GetShipmentWeightByDate(date);
    }
}
}