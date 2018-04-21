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
using System.Windows.Input;

namespace com.sbh.gui.invoices.ViewModel
{
    public class DocumentViewModel : INotifyPropertyChanged
    {
        public delegate void DocumentStateHandler();
        public event DocumentStateHandler JournalShow;

        public DTO.DataModel DataModel { get; set; }

        private Model.Position _сurPosition;
        public Model.Position CurPosition { get; set; }

        public View.DocumentItemsView ItemsView { get; set; }

        public ObservableCollection<dll.utilites.OReferences.RefItem.Item> RefItems { get; set; }
        public ObservableCollection<dll.utilites.OReferences.RefCurrency.CurrencyLite> RefCurrency { get; set; }
        public ObservableCollection<dll.utilites.OReferences.RefDimensions.Dimension> RefDimensions { get; set; }
        
        private dll.resdictionary.View.DialogView dialogView;
        private dll.resdictionary.View.DialogPrint dialogPrint;
        private references.counterparty.View.CounterpartyExternalView counterpartyExternalView;
        private references.orgmodel.View.UnitExternalView unitExternalView;

        public DocumentViewModel(DTO.DataModel pDataModel)
        {
            DataModel = pDataModel;

            RefItems = new ObservableCollection<dll.utilites.OReferences.RefItem.Item>(dll.utilites.OReferences.RefItem.GetInstance.refItem);
            RefCurrency = new ObservableCollection<dll.utilites.OReferences.RefCurrency.CurrencyLite>(dll.utilites.OReferences.RefCurrency.GetInstance.CurrenciesLite);
            RefDimensions = new ObservableCollection<dll.utilites.OReferences.RefDimensions.Dimension>(dll.utilites.OReferences.RefDimensions.GetInstance.refDimension);

            ItemsView = new View.DocumentItemsView();
            ItemsView.DataContext = this;

            counterpartyExternalView = new references.counterparty.View.CounterpartyExternalView();
            unitExternalView = new references.orgmodel.View.UnitExternalView();

            SetCountertypeOnClickCommand = new DelegateCommand(SetCountertypeOnClick);
            SetRecipientOnClickCommand = new DelegateCommand(SetRecipientOnClick);

            AddItemOnClickCommand = new DelegateCommand(AddItemOnClick);
            DeleteItemOnClickCommand = new DelegateCommand(DeleteItemOnClick);

            BackOnClickCommand = new DelegateCommand(BackOnClick);
        }

        #region Command

        public ICommand SetCountertypeOnClickCommand { get; private set; }
        void SetCountertypeOnClick(object obj)
        {
            List<decimal> selectedCounterparty = new List<decimal>();
            selectedCounterparty.Add(DataModel.CurDocument.XFrom);

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
                        command.Parameters.Add("id", SqlDbType.Decimal).Value = DataModel.CurDocument.Id;

                        command.ExecuteNonQuery();
                    }
                }

                DataModel.CurDocument.XFrom = counterparty.id;
            }
        }

        public ICommand SetRecipientOnClickCommand { get; private set; }
        void SetRecipientOnClick(object obj)
        {
            List<decimal> selectedRecipient = new List<decimal>();
            selectedRecipient.Add(DataModel.CurDocument.XTo);

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
                        command.Parameters.Add("id", SqlDbType.Decimal).Value = DataModel.CurDocument.Id;

                        command.ExecuteNonQuery();
                    }
                }

                DataModel.CurDocument.XTo = recipient.id;
            }
        }

        public ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            //OnPropertyChanged("");
            //DocumentItemsViewModel documentItemsViewModel = (ItemsView.DataContext as DocumentItemsViewModel);

            //MSG oMsg = documentItemsViewModel.checkData();

            //if (!oMsg.IsSuccess)
            //{
            //    if (MessageBox.Show(oMsg.Message, GValues.AppNameFull, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            //    {
            //        oMsg = documentItemsViewModel.groupingData();
            //    }
            //}

            //if (!oMsg.IsSuccess)
            //{
            //    MessageBox.Show(oMsg.Message, GValues.AppNameFull, MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            //oMsg = new MSG
            //{
            //    Obj = documentItemsViewModel.Positions.Sum(x => x.xcount)
            //};
            DataModel.CurDocument.RefreshData();
            JournalShow();
        }

        public ICommand AddItemOnClickCommand { get; private set; }
        void AddItemOnClick(object obj)
        {
            Model.Position newPositions;
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    newPositions = new Model.Position();
                    newPositions.itemId = 0;
                    newPositions.xcount = 0;
                    newPositions.currencyId = Model.Position.lastCurrency;
                    newPositions.xprice = 0;
                    newPositions.isAvalForEdit = true;

                    command.Connection = con;
                    command.CommandText = " INSERT INTO document_items(xdocument, item, ref_dimensions, xcount, currency, xprice, ref_status) " +
                                            " VALUES(@xdocument, @item, @ref_dimensions, @xcount, @currency, @xprice, @ref_status ); " +
                                            " SELECT SCOPE_IDENTITY();";

                    command.Parameters.Add("xdocument", SqlDbType.Int).Value = DataModel.CurDocument.Id;
                    command.Parameters.Add("item", SqlDbType.Int).Value = newPositions.itemId;
                    command.Parameters.Add("ref_dimensions", SqlDbType.Int).Value = newPositions.dimensionId;
                    command.Parameters.Add("xcount", SqlDbType.Decimal).Value = newPositions.xcount;
                    command.Parameters.Add("currency", SqlDbType.Decimal).Value = newPositions.currencyId;
                    command.Parameters.Add("xprice", SqlDbType.Decimal).Value = newPositions.xprice;
                    command.Parameters.Add("ref_status", SqlDbType.Int).Value = 1;

                    newPositions.id = (decimal)command.ExecuteScalar();

                }
            }
            DataModel.CurDocument.DocumentPositions.Add(newPositions);
            newPositions = null;
        }
        public bool AddItemOnClick_CanExecute(object obj)
        {
            return true;
        }

        public ICommand DeleteItemOnClickCommand { get; private set; }
        void DeleteItemOnClick(object obj)
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " DELETE FROM document_items WHERE id = @id;";

                    command.Parameters.Add("id", SqlDbType.Int).Value = CurPosition.id;

                    command.ExecuteNonQuery();
                }
            }
            DataModel.CurDocument.DocumentPositions.Remove(CurPosition);
            CurPosition = null;
        }
        public bool DeleteCommand_CanExecute(object obj)
        {
            return true;
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
