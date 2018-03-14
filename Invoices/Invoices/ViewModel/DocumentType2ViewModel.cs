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
    class DocumentType2ViewModel : INotifyPropertyChanged
    {
        private dll.resdictionary.View.DialogView dialogView;
        private references.orgmodel.View.UnitExternalView counterpartyExternalView;
        private references.orgmodel.View.UnitExternalView unitExternalView;

        private UserControl _itemsView;
        public UserControl ItemsView
        {
            get { return _itemsView; }
            private set { _itemsView = value; OnPropertyChanged("ItemsView"); }
        }

        private Model.DocumentType2 _doc;
        public Model.DocumentType2 Doc
        {
            get { return _doc; }
            set { _doc = value; OnPropertyChanged("Doc"); }
        }

        public string CounterpartyName { get { return Doc.counterpaty.name; } }
        public string RecipientName { get { return Doc.recipient.name; } }

        public DocumentType2ViewModel(Model.DocumentType2 pDocument)
        {
            Doc = pDocument;

            ItemsView = new View.DocumentItemsView();
            ItemsView.DataContext = new DocumentItemsViewModel(Doc.id);

            counterpartyExternalView = new references.orgmodel.View.UnitExternalView();
            unitExternalView = new references.orgmodel.View.UnitExternalView();

            SetCountertypeOnClickCommand = new DelegateCommand(SetCountertypeOnClick);
            SetRecipientOnClickCommand = new DelegateCommand(SetRecipientOnClick);

            BackOnClickCommand = new DelegateCommand(BackOnClick);

        }

        #region Command

        public ICommand SetCountertypeOnClickCommand { get; private set; }
        void SetCountertypeOnClick(object obj)
        {
            List<decimal> selectedCounterparty = new List<decimal>();
            selectedCounterparty.Add(Doc.counterpaty.id);
            (unitExternalView.DataContext as references.orgmodel.ViewModel.UnitExternalViewModel)
                .PresetData(selectedCounterparty, false);

            dialogView = new dll.resdictionary.View.DialogView(counterpartyExternalView);
            if (dialogView.ShowDialog() == true)
            {
                dll.utilites.OReferences.RefRecipient.Recipient counterparty =
                     ((references.orgmodel.ViewModel.UnitExternalViewModel)(unitExternalView.DataContext)).CurRecipient.Item;

                using (SqlConnection con = new SqlConnection(GValues.connString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = con;
                        command.CommandText = " UPDATE document SET xfrom = @xfrom" +
                                                " WHERE id = @id";

                        command.Parameters.Add("xfrom", SqlDbType.Decimal).Value = counterparty.id;
                        command.Parameters.Add("id", SqlDbType.Decimal).Value = Doc.id;

                        command.ExecuteNonQuery();
                    }
                }

                Doc.counterpaty = counterparty;
                counterparty = null;
                OnPropertyChanged("CounterpartyName");
            }
        }

        public ICommand SetRecipientOnClickCommand { get; private set; }
        void SetRecipientOnClick(object obj)
        {
            List<decimal> selectedRecipient = new List<decimal>();
            selectedRecipient.Add(Doc.recipient.id);
            (unitExternalView.DataContext as references.orgmodel.ViewModel.UnitExternalViewModel)
                .PresetData(selectedRecipient, false);

            dialogView = new dll.resdictionary.View.DialogView(unitExternalView);
            if (dialogView.ShowDialog() == true)
            {
                dll.utilites.OReferences.RefRecipient.Recipient recipient =
                    ((references.orgmodel.ViewModel.UnitExternalViewModel)(unitExternalView.DataContext)).CurRecipient.Item;

                using (SqlConnection con = new SqlConnection(GValues.connString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = con;
                        command.CommandText = " UPDATE document SET xto = @xto" +
                                                " WHERE id = @id";

                        command.Parameters.Add("xto", SqlDbType.Decimal).Value = recipient.id;
                        command.Parameters.Add("id", SqlDbType.Decimal).Value = Doc.id;

                        command.ExecuteNonQuery();
                    }
                }

                Doc.recipient = recipient;
                recipient = null;
                OnPropertyChanged("RecipientName");
            }
        }

        public ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            SurfaceControlViewModel.BackOnClickCommand.Execute(null);
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

