using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.sbh.gui.invoices.Model
{
    public class DocumentTree : INotifyPropertyChanged
    {
        public decimal id { get; set; }
        public decimal perentId { get; set; }
        public decimal docType { get; set; }
        public DateTime dateCreate { get; set; }
        public DateTime dateDoc { get; set; }
        public decimal refStatus { get; set; }
        public decimal xfrom { get; set; }
        public decimal xto { get; set; }

        [XmlElement("ArrayOfDocumentTree", typeof(ObservableCollection<DocumentTree>))]
        public ObservableCollection<DocumentTree> DocumentTreeChild { get; set; }

        public string TypeName
        {
            get
            {
                string result = string.Empty;

                switch (docType)
                {
                    default:
                        result = RefDocType.GetInstance.refDocType.Single(x => x.id == docType).name;
                        break;
                }

                return result;
            }
        }

        public string XFromName
        {
            get
            {
                string result = string.Empty;

                switch (docType)
                {
                    
                    case 2:
                        result = String.Format("{0} / {1}", 
                            RefRecipient.GetInstance.Recipients.Single(x => x.id == xfrom).Name, RefRecipient.GetInstance.Recipients.Single(x => x.id == xfrom).name);
                        break;

                    default:
                        result = String.Format("{0} / {1}",
                            RefCounterParty.GetInstance.CounterPartys.Single(x => x.id == xfrom).groupname, RefCounterParty.GetInstance.CounterPartys.Single(x => x.id == xfrom).name);
                        break;
                }

                return result;
            }
        }

        public string XToName
        {
            get
            {
                string result = string.Empty;

                switch (docType)
                {
                    default:
                        result = String.Format("{0} / {1}", 
                            RefRecipient.GetInstance.Recipients.Single(x => x.id == xfrom).Name, RefRecipient.GetInstance.Recipients.Single(x => x.id == xto).name);
                        break;
                }

                return result;
            }
        }

        public DocumentTree()
        {
            DocumentTreeChild = new ObservableCollection<DocumentTree>();
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
