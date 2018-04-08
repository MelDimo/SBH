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
        private dll.resdictionary.View.DialogView dialogView;
        private dll.resdictionary.View.DialogPrint dialogPrint;
        private references.counterparty.View.CounterpartyExternalView counterpartyExternalView;
        private references.orgmodel.View.UnitExternalView unitExternalView;

        private View.DocumentItemsView _itemsView;
        public View.DocumentItemsView ItemsView
        {
            get { return _itemsView; }
            private set { _itemsView = value; OnPropertyChanged("ItemsView"); }
        }

        public Model.Document Doc { get; }

        public DocumentViewModel(SurfaceControlViewModel.DgtGetCurrentDocument dgtGetCurrentDocument)
        {
            Doc = dgtGetCurrentDocument.Invoke();

            ItemsView = new View.DocumentItemsView
            {
                DataContext = new DocumentItemsViewModel()
                {
                    Doc = Doc,
                    IsAvailForAdding = Doc.ParentId == 0,
                    IsDocContainChild = Doc.DocumentPositions.Count > 0
                }
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
            selectedCounterparty.Add(Doc.XFrom);

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
                        command.Parameters.Add("id", SqlDbType.Decimal).Value = Doc.Id;

                        command.ExecuteNonQuery();
                    }
                }

                Doc.XFrom = counterparty.id;
            }
        }

        public ICommand SetRecipientOnClickCommand { get; private set; }
        void SetRecipientOnClick(object obj)
        {
            List<decimal> selectedRecipient = new List<decimal>();
            selectedRecipient.Add(Doc.XTo);

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
                        command.Parameters.Add("id", SqlDbType.Decimal).Value = Doc.Id;

                        command.ExecuteNonQuery();
                    }
                }

                Doc.XTo = recipient.id;
            }
        }

        public ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
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

            SurfaceControlViewModel.BackOnClickCommand.Execute(new MSG { IsSuccess = true });
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
