using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.sbh.dll.utilites.OReferences;

namespace com.sbh.gui.references.counterparty.ViewModel
{
    public class CounterpartyExternalViewModel : INotifyPropertyChanged
    {
        public RefCounterParty refCounterParty { get; private set; }

        private RefCounterParty.Counterparty _curCounterParty;
        public RefCounterParty.Counterparty curCounterParty
        {
            get { return _curCounterParty; }
            set { _curCounterParty = value; }
        }

        public CounterpartyExternalViewModel()
        {
            refCounterParty = RefCounterParty.GetInstance;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
