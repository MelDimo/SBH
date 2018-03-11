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
    public class DocumentType1 : INotifyPropertyChanged
    {
        public decimal id { get; set; }
        public decimal docType { get; set; }
        public DateTime dateCreate { get; set; }
        public DateTime dateDoc { get; set; }
        public decimal refStatus { get; set; }

        public RefCounterParty.Counterparty counterpaty;
        public RefRecipient.Recipient recipient;

        public DocumentType1()
        {
            counterpaty = new RefCounterParty.Counterparty();
            recipient = new RefRecipient.Recipient();
            dateCreate = DateTime.Now;
            dateDoc = DateTime.Now;
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
