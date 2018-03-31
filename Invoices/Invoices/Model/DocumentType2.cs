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
    public class DocumentType2 : INotifyPropertyChanged
    {
        public decimal id { get; set; }
        public decimal parentId { get; set; }
        public decimal docType { get; set; }
        public DateTime dateCreate { get; set; }
        public DateTime dateDoc { get; set; }
        public decimal refStatus { get; set; }

        public RefRecipient.Recipient counterpaty;
        public RefRecipient.Recipient recipient;

        public DocumentType2()
        {
            counterpaty = new RefRecipient.Recipient();
            recipient = new RefRecipient.Recipient();
            dateCreate = DateTime.Now;
            dateDoc = DateTime.Now;
        }

        private Position _curItem;
        private Position CurItem
        {
            get { return _curItem; }
            set { _curItem = value; OnPropertyChanged("CurItem"); }
        }

        private ObservableCollection<Position> _items;
        public ObservableCollection<Position> Items
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
