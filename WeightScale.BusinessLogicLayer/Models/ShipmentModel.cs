using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Models
{
    public class ShipmentModel : INotifyPropertyChanged
    {
        private int _id;
        private DateTime _shipmentDate;
        private int _courierId;
        private Courier _courier;
        private bool _isFinished;
        private ObservableCollection<PackageModel> _packages;

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

        public DateTime ShipmentDate
        {
            get => _shipmentDate;
            set
            {
                if (_shipmentDate != value)
                {
                    _shipmentDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CourierId
        {
            get => _courierId;
            set
            {
                if (_courierId != value)
                {
                    _courierId = value;
                    OnPropertyChanged();
                }
            }
        }

        public Courier Courier
        {
            get => _courier;
            set
            {
                if (_courier != value)
                {
                    _courier = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsFinished
        {
            get => _isFinished;
            set
            {
                if (_isFinished != value)
                {
                    _isFinished = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<PackageModel> Packages
        {
            get => _packages;
            set
            {
                if (_packages != value)
                {
                    _packages = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
