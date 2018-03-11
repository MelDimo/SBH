using com.sbh.dll;
using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace com.sbh.gui.invoices.ViewModel
{
    public class DocumentType1ViewModel : INotifyPropertyChanged
    {
        private dll.resdictionary.View.DialogView dialogView;
        private references.counterparty.View.CounterpartyExternalView counterpartyExternalView;
        private references.orgmodel.View.UnitExternalView unitExternalView;

        private UserControl _itemsView;
        public UserControl ItemsView
        {
            get { return _itemsView; }
            private set { _itemsView = value; OnPropertyChanged("ItemsView"); }
        }

        private Model.DocumentType1 _doc;
        public Model.DocumentType1 Doc
        {
            get { return _doc; }
            set { _doc = value; OnPropertyChanged("Doc"); }
        }

        public string CounterpartyName { get { return Doc.counterpaty.name; } }
        public string RecipientName { get { return Doc.recipient.name; } }

        public DocumentType1ViewModel(Model.DocumentType1 pDocument)
        {
            Doc = pDocument;

            ItemsView = new View.DocumentItemsView();
            ItemsView.DataContext = this;

            counterpartyExternalView = new references.counterparty.View.CounterpartyExternalView();
            unitExternalView = new references.orgmodel.View.UnitExternalView();

            SetCountertypeOnClickCommand = new DelegateCommand(SetCountertypeOnClick);
            SetRecipientOnClickCommand = new DelegateCommand(SetRecipientOnClick);

            //AddItemOnClickCommand = new DelegateCommand(AddItemOnClick);

            BackOnClickCommand = new DelegateCommand(BackOnClick);

        }

        private void createWhenNew()
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " INSERT INTO document(ref_docType, dateCreate, dateDoc, ref_status) " +
                                            "VALUES ()";

                    //command.Parameters.Add("dateStart", SqlDbType.DateTime).Value = Filter.dateStart;
                    //command.Parameters.Add("dateEnd", SqlDbType.DateTime).Value = Filter.dateEnd;

                    Doc.id = (int)command.ExecuteScalar();
                }
            }
        }

        #region Command

        public ICommand SetCountertypeOnClickCommand { get; private set; }
        void SetCountertypeOnClick(object obj)
        {

            dialogView = new dll.resdictionary.View.DialogView(counterpartyExternalView);
            if (dialogView.ShowDialog() == true)
            {
                foreach (dll.utilites.OReferences.RefCounterParty.Counterparty items
                    in ((references.counterparty.ViewModel.CounterpartyExternalViewModel)(counterpartyExternalView.DataContext)).GetSelectedItems())
                {
                    Doc.counterpaty = items;
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
                    Doc.recipient = items;
                    OnPropertyChanged("RecipientName");
                }
            }
        }

        public ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            SurfaceControlViewModel.BackOnClickCommand.Execute(null);
        }

        public ICommand AddItemOnClickCommand { get; private set; }
        void AddItemOnClick(object obj)
        {
            return;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
