using com.sbh.dll;
using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace com.sbh.gui.invoices.ViewModel
{
    public class DocumentType1ViewModel : INotifyPropertyChanged
    {
        private dll.resdictionary.View.DialogView dialogView;
        private com.sbh.gui.references.counterparty.View.CounterpartyExternalView counterpartyExternalView;
        private com.sbh.gui.references.orgmodel.View.UnitExternalView unitExternalView;

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

        public string CounterpartyName { get { return string.Format("{0} / {1}", Doc.counterpaty.groupname, Doc.counterpaty.name); } }
        public string RecipientName { get { return string.Format("{0} / {1}", Doc.recipient.Name, Doc.recipient.name); } }

        public DocumentType1ViewModel(Model.DocumentType1 pDocument)
        {
            Doc = pDocument;

            ItemsView = new View.DocumentItemsView();
            ItemsView.DataContext = new DocumentItemsViewModel(Doc.id)
            {
                IsAvailForAdding = Doc.parentId == 0,
                IsDocContainChild = Doc.IsContainsChild
            };

            counterpartyExternalView = new references.counterparty.View.CounterpartyExternalView();
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

            (counterpartyExternalView.DataContext as references.counterparty.ViewModel.CounterpartyExternalViewModel)
                .PresetData(selectedCounterparty, false);

            dialogView = new dll.resdictionary.View.DialogView(counterpartyExternalView);
            dialogView.Owner = Application.Current.MainWindow;
            dialogView.Header = "Поставщик";
            if (dialogView.ShowDialog() == true)
            {
                dll.utilites.OReferences.RefCounterParty.Counterparty counterparty =
                    ((references.counterparty.ViewModel.CounterpartyExternalViewModel)(counterpartyExternalView.DataContext)).CurCounterparty.Item;

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
            dialogView.Owner = Application.Current.MainWindow;
            dialogView.Header = "Получатель";
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
            DocumentItemsViewModel documentItemsViewModel = (ItemsView.DataContext as DocumentItemsViewModel);

            MSG oMsg = documentItemsViewModel.checkData();

            if (!oMsg.IsSuccess)
            {
                if (MessageBox.Show(oMsg.Message, GValues.AppNameFull, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    oMsg = documentItemsViewModel.groupingData();
                }
            }

            if (!oMsg.IsSuccess)
            {
                MessageBox.Show(oMsg.Message, GValues.AppNameFull, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            oMsg = new MSG
            {
                Obj = documentItemsViewModel.Positions.Sum(x => x.xcount)
            };
            

            SurfaceControlViewModel.BackOnClickCommand.Execute(oMsg);
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
