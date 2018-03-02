using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace com.sbh.gui.invoices.ViewModel
{
    public class DocumentType1ViewModel : INotifyPropertyChanged
    {

        dll.resdictionary.View.DialogView dialogView;
        references.counterparty.View.CounterpartyExternalView counterpartyExternalView;
        references.orgmodel.View.UnitExternalView unitExternalView;

        public Model.DocumentType1 Document;

        public string CounterpartyName {
            get { return Document.counterpaty.name; }
        }

        public string RecipientName {
            get { return Document.recipient.name; }
        }


        public DocumentType1ViewModel()
        {
            Document = new Model.DocumentType1();

            counterpartyExternalView = new references.counterparty.View.CounterpartyExternalView();
            unitExternalView = new references.orgmodel.View.UnitExternalView();

            SetCountertypeOnClickCommand = new DelegateCommand(SetCountertypeOnClick);
            SetRecipientOnClickCommand = new DelegateCommand(SetRecipientOnClick);

            BackOnClickCommand = new DelegateCommand(BackOnClick);
        }

        public ICommand SetCountertypeOnClickCommand { get; private set; }
        void SetCountertypeOnClick(object obj)
        {

            dialogView = new dll.resdictionary.View.DialogView(counterpartyExternalView);
            if (dialogView.ShowDialog() == true)
            {
                foreach (dll.utilites.OReferences.RefCounterParty.Counterparty items 
                    in ((references.counterparty.ViewModel.CounterpartyExternalViewModel)(counterpartyExternalView.DataContext)).GetSelectedItems())
                {
                    Document.counterpaty = items;
                    OnPropertyChanged("CounterpartyName");
                }
            }
        }

        public ICommand SetRecipientOnClickCommand { get; private set; }
        void SetRecipientOnClick(object obj)
        {

            dialogView = new dll.resdictionary.View.DialogView(unitExternalView);
            if (dialogView.ShowDialog() == true)
            {
                foreach (dll.utilites.OReferences.RefRecipient.Recipient items
                    in ((references.orgmodel.ViewModel.UnitExternalViewModel)(unitExternalView.DataContext)).GetSelectedItems())
                {
                    Document.recipient = items;
                    OnPropertyChanged("RecipientName");
                }
            }
        }


        public ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            SurfaceControlViewModel.BackOnClickCommand.Execute(null);
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
