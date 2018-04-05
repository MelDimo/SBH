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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace com.sbh.gui.invoices.ViewModel
{
    class DocumentType5ViewModel : INotifyPropertyChanged
    {

        private dll.resdictionary.View.DialogView dialogView;
        private references.counterparty.View.CounterpartyExternalView counterpartyExternalView;
        private references.orgmodel.View.UnitExternalView unitExternalView;

        private dll.resdictionary.View.DialogPrint dialogPrint;

        private View.DocumentItemsView _itemsView;
        public View.DocumentItemsView ItemsView
        {
            get { return _itemsView; }
            private set { _itemsView = value; OnPropertyChanged("ItemsView"); }
        }

        private Model.DocumentType5 _doc;
        public Model.DocumentType5 Doc
        {
            get { return _doc; }
            set { _doc = value; OnPropertyChanged("Doc"); }
        }

        public string CounterpartyName { get { return string.Format("{0} / {1}", Doc.counterpaty.groupname, Doc.counterpaty.name); } }
        public string RecipientName { get { return string.Format("{0} / {1}", Doc.recipient.Name, Doc.recipient.name); } }

        public DocumentType5ViewModel(Model.DocumentType5 pDocument)
        {
            Doc = pDocument;

            ItemsView = new View.DocumentItemsView();
            ItemsView.DataContext = new DocumentItemsViewModel(Doc.id);

            counterpartyExternalView = new references.counterparty.View.CounterpartyExternalView();
            unitExternalView = new references.orgmodel.View.UnitExternalView();

            SetCountertypeOnClickCommand = new DelegateCommand(SetCountertypeOnClick);
            SetRecipientOnClickCommand = new DelegateCommand(SetRecipientOnClick);

            PrintOnClickCommand = new DelegateCommand(PrintOnClick);

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
            //(ItemsView as View.DocumentItemsView).DocumentItemsView_UnsetPreviewKeyDown();

            MSG oMsg = (ItemsView.DataContext as ViewModel.DocumentItemsViewModel).checkData();

            if (!oMsg.IsSuccess)
                MessageBox.Show(oMsg.Message, GValues.AppNameFull, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                //SurfaceControlViewModel.BackOnClickCommand.Execute(null);
        }

        public ICommand PrintOnClickCommand { get; private set; }
        void PrintOnClick(object obj)
        {
            Report.DocumentType5 report = new Report.DocumentType5();
            

            var items = (from p in (ItemsView.DataContext as DocumentItemsViewModel).Positions
                         select new { p.itemName, p.xcount, p.currencyName, p.dimensionName }).ToList();

            report.SetDataSource(items);
            report.SetParameterValue("counterpatyName", CounterpartyName);
            report.SetParameterValue("recipientName", RecipientName);
            report.SetParameterValue("dateDoc", Doc.dateDoc);


            dialogPrint = new dll.resdictionary.View.DialogPrint();
            dialogPrint.reportViewer.ViewerCore.ReportSource = report;
            dialogPrint.Show();
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
