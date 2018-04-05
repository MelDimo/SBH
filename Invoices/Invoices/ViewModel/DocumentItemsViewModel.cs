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
using System.Xml;

namespace com.sbh.gui.invoices.ViewModel
{
    class DocumentItemsViewModel : INotifyPropertyChanged
    {
        private decimal mDocId;

        private BackgroundWorker bgwItems;

        private ObservableCollection<Model.Position> _positions;
        public ObservableCollection<Model.Position> Positions
        {
            get { return _positions; }
            set { _positions = value; OnPropertyChanged("Positions"); }
        }

        private Model.Position _curPosition;
        public Model.Position CurPosition
        {
            get { return _curPosition; }
            set { _curPosition = value; OnPropertyChanged("CurPosition"); }
        }

        public DocumentItemsViewModel(decimal pDocId)
        {
            mDocId = pDocId;

            bgwItems = new BackgroundWorker();
            bgwItems.DoWork += BgwItems_DoWork;
            bgwItems.RunWorkerCompleted += BgwItems_RunWorkerCompleted;

            bgwItems.RunWorkerAsync();

            AddItemOnClickCommand = new DelegateCommand(AddItemOnClick);
            DeleteItemOnClickCommand = new DelegateCommand(DeleteItemOnClick, DeleteCommand_CanExecute);
        }

        private void BgwItems_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void BgwItems_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    Positions = new ObservableCollection<Model.Position>();

                    command.Connection = con;
                    command.CommandText = " SELECT di.id, di.item As itemId, di.ref_dimensions AS dimensionId, " +
                                            "    di.xcount, di.currency AS currencyId, di.xprice " +
                                            " FROM document_items di " +
                                            " WHERE di.ref_status = 1 AND di.xdocument = @document " +
                                            " FOR XML RAW('Position'), ROOT('ArrayOfPosition'), ELEMENTS ";

                    command.Parameters.Add("document", SqlDbType.Int).Value = mDocId;

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        Positions = Support.XMLToObject<ObservableCollection<Model.Position>>(reader.ReadOuterXml());
                    }
                }
            }

            // Перевожу полученные объекты в режим редактирования
            foreach (Model.Position mpos in Positions) mpos.isAvalForEdit = true;
        }

        #region Checking Data

        /*
         * Проверяю позиции например на дублирование 
         */
        public MSG checkData()
        {
            StringBuilder sbPosition = new StringBuilder();

            MSG result = new MSG
            {
                IsSuccess = true,
            };

            foreach (Model.Position pos in Positions)
            {
                if (Positions.Where(x => x.itemId == pos.itemId
                                     && x.currencyId == pos.currencyId
                                     && x.dimensionId == pos.dimensionId
                                     && x.xprice == pos.xprice).Count() > 1)
                {
                    result.IsSuccess = false;
                    sbPosition.Append(string.Format("{0} | {1} | {2} | {3};\n", pos.itemName, pos.currencyName, pos.dimensionName, pos.xprice));
                }
            }

            if (!result.IsSuccess)
            {
                result.Message = string.Format("{0}\n\n{1}\n{2}", "Найдены следующие дублированные позиции:", sbPosition.ToString(), "Позиции будут объеденены.");
            }

            return result;
        }

        public MSG groupingData()
        {
            MSG result = new MSG
            {
                IsSuccess = true
            };

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    SqlTransaction transaction;

                    try
                    {
                        con.Open();

                        transaction = con.BeginTransaction();

                        command.Connection = con;
                        command.Transaction = transaction;

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Position_CorrectDublicate";

                        command.Parameters.Add("pDocId", SqlDbType.Int).Value = mDocId;

                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception exc)
                    {
                        result.IsSuccess = false;
                        result.Message = exc.Message;
                    }

                }
            }

            return result;
        }
        #endregion


        #region Command

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

                    command.Parameters.Add("xdocument", SqlDbType.Int).Value = mDocId;
                    command.Parameters.Add("item", SqlDbType.Int).Value = newPositions.itemId;
                    command.Parameters.Add("ref_dimensions", SqlDbType.Int).Value = newPositions.dimensionId;
                    command.Parameters.Add("xcount", SqlDbType.Decimal).Value = newPositions.xcount;
                    command.Parameters.Add("currency", SqlDbType.Decimal).Value = newPositions.currencyId;
                    command.Parameters.Add("xprice", SqlDbType.Decimal).Value = newPositions.xprice;
                    command.Parameters.Add("ref_status", SqlDbType.Int).Value = 1;

                    newPositions.id = (decimal)command.ExecuteScalar();

                }
            }
            Positions.Add(newPositions);
            newPositions = null;
        }

        public bool DeleteCommand_CanExecute(object obj)
        {
            return CurPosition != null;
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
            Positions.Remove(CurPosition);
            CurPosition = null;
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
