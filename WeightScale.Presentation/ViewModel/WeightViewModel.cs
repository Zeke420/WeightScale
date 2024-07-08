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
        MovePackageUpCommand = new DelegateCommand(MovePackageUp);
        MovePackageDownCommand = new DelegateCommand(MovePackageDown);
        Shipments = new List<Shipment>();
    }

    private void OnPackageAdded(Package obj)
    {
        GetShipmentWeightByDate(SelectedDate);
    }

    public DelegateCommand CompleteShipmentCommand { get; set; }
    public DelegateCommand MovePackageUpCommand { get; set; }
    public DelegateCommand MovePackageDownCommand { get; set; }

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
    
    private void MovePackageDown(object obj)
    {
        if (!(obj is Package package))
        {
            return;
        }

        var shipment = Shipments.Find(s => s.Id == package.ShipmentId);
        var index = Shipments.IndexOf(shipment);
        var nextShipment = Shipments[index + 1];
        
        if(nextShipment == null)
        {
            return;
        }
        
        _shipmentService.MovePackage(package, nextShipment);
        GetShipmentWeightByDate(_selectedDate);
    }
    
    private void MovePackageUp(object obj)
    {
        if (!(obj is Package package))
        {
            return;
        }

        var shipment = Shipments.Find(s => s.Id == package.ShipmentId);
        var index = Shipments.IndexOf(shipment);
        var previousShipment = Shipments[index - 1];
        
        if(previousShipment == null)
        {
            return;
        }
        
        _shipmentService.MovePackage(package, previousShipment);
        GetShipmentWeightByDate(_selectedDate);
    }
}
}