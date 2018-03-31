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
        public DateTime dateDoc { get; set; }
        public decimal refStatus { get; set; }
        public decimal xfrom { get; set; }
        public decimal xto { get; set; }

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
                    case 1:
                        result = RefCounterParty.GetInstance.CounterPartys.Single(x => x.id == xfrom).name;
                        break;
                    case 2:
                        result = RefRecipient.GetInstance.Recipients.Single(x => x.id == xfrom).name;
                        break;
                    case 5:
                        result = RefCounterParty.GetInstance.CounterPartys.Single(x => x.id == xfrom).name;
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
                    case 1:
                        result = RefRecipient.GetInstance.Recipients.Single(x => x.id == xto).name;
                        break;
                    case 2:
                        result = RefRecipient.GetInstance.Recipients.Single(x => x.id == xto).name;
                        break;
                    case 5:
                        result = RefRecipient.GetInstance.Recipients.Single(x => x.id == xto).name;
                        break;
                }

                return result;
            }
        }

        public Document()
        {

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
