using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using com.sbh.dll.utilites.OReferences;
using System.ComponentModel;

namespace com.sbh.gui.invoices.Model
{
    public class Document : INotifyPropertyChanged
    {
        public decimal Id { get; set; }
        public decimal ParentId { get; set; }
        public decimal DocType { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateDoc { get; set; }
        public decimal RefStatus { get; set; }
        private decimal _XFrom;
        public decimal XFrom
        {
            get { return _XFrom; }
            set { _XFrom = value; OnPropertyChanged("XFromName"); }
        }
        private decimal _XTo;
        public decimal XTo
        {
            get { return _XTo; }
            set { _XTo = value; OnPropertyChanged("XToName"); }
        }

        public string DocTypeName
        {
            get { return RefDocType.GetInstance.refDocType.Single(x => x.id == DocType).name; }
        }

        public decimal PositionCount()
        {
            return DocumentPositions.Sum(x => x.xcount);
        }

        public string PositionCountNote
        {
            get
            {
                decimal docPositionCount = 0;

                foreach (Document doc in DocumentChilds)
                {
                    docPositionCount = docPositionCount + doc.PositionCount();
                }

                return string.Format("Респределенно {0} из {1}", docPositionCount, PositionCount());
            }
        }

        public void RefreshData()
        {
            OnPropertyChanged("PositionCountNote");
        }

        [XmlIgnore]
        public string XToName
        {
            get
            {
                string result = string.Empty;

                switch (DocType)
                {
                    default:
                        result = String.Format("{0} / {1}",
                            RefRecipient.GetInstance.Recipients.Single(x => x.id == XTo).Name, RefRecipient.GetInstance.Recipients.Single(x => x.id == XTo).name);
                        break;
                }

                return result;
            }
        }
        [XmlIgnore]
        public string XFromName
        {
            get
            {
                string result = string.Empty;

                switch (DocType)
                {

                    case 2:
                        result = String.Format("{0} / {1}",
                            RefRecipient.GetInstance.Recipients.Single(x => x.id == XFrom).Name, RefRecipient.GetInstance.Recipients.Single(x => x.id == XFrom).name);
                        break;

                    default:
                        result = String.Format("{0} / {1}",
                            RefCounterParty.GetInstance.CounterPartys.Single(x => x.id == XFrom).groupname, RefCounterParty.GetInstance.CounterPartys.Single(x => x.id == XFrom).name);
                        break;
                }

                return result;
            }
        }

        public Document()
        {
            DateCreate = DateTime.Now;
            DateDoc = DateTime.Now;
            ParentId = 0;
            XFrom = 1;
            XTo = 1;
            RefStatus = 1;
            DocumentChilds = new ObservableCollection<Document>();
            DocumentPositions = new ObservableCollection<Position>();
        }

        [XmlElement("ArrayOfDocument", typeof(ObservableCollection<Document>))]
        public ObservableCollection<Document> DocumentChilds { get; set; }

        [XmlElement("ArrayOfPosition", typeof(ObservableCollection<Position>))]
        public ObservableCollection<Position> DocumentPositions { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
