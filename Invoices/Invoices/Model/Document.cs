using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.invoices.Model
{
    public class Document : INotifyPropertyChanged
    {
        public decimal id { get; set; }
        public decimal docType { get; set; }
        public DateTime dateCreate { get; set; }
        public decimal refStatus { get; set; }

        public RefCounterParty.Counterparty counterpaty;
        public RefRecipient.Recipient recipient;

        public Document()
        {
            counterpaty = new RefCounterParty.Counterparty();
            recipient = new RefRecipient.Recipient();
        }

        private Item _curItem;
        private Item CurItem
        {
            get { return _curItem; }
            set { _curItem = value; OnPropertyChanged("CurItem"); }
        }

        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
