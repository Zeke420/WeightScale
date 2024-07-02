using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WeightScale.Presentation.Helpers
{
    public class ObservableCollectionWithItemNotify<T> : ObservableCollection<T> where T: INotifyPropertyChanged
    {
        public ObservableCollectionWithItemNotify()
        {
            CollectionChanged += ItemsCollectionChanged;
        }

        public ObservableCollectionWithItemNotify(IEnumerable<T> collection) : base(collection)
        {
            CollectionChanged += ItemsCollectionChanged;

            foreach (var item in collection)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e is null)
            {
                return;
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= ItemPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var reset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(reset);
        }
    }
}