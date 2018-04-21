using com.sbh.dll;
using com.sbh.dll.utilites;
using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace com.sbh.gui.invoices.ViewModel
{
    public class DocumentJournalViewModel : INotifyPropertyChanged
    {
        public delegate void DocumentStateHandler();
        public event DocumentStateHandler DocumentShow;

        public DTO.DataModel DataModel { get; set; }

        public List<RefDocType.DocType> DocTypes { get; private set; }

        public ObservableCollection<RefDocMap.DocMap> _docResponseType;
        public ObservableCollection<RefDocMap.DocMap> DocResponseType
        {
            get { return new ObservableCollection<RefDocMap.DocMap>(_docResponseType.Where(x => x.refDocTypeSource == DataModel.CurDocument?.DocType).ToList()); }
            private set { _docResponseType = value; }
        }

        private bool _filterVisibility;
        public bool FilterVisibility
        {
            get { return _filterVisibility; }
            set
            {
                _filterVisibility = value;
                OnPropertyChanged("FilterVisibility");
            }
        }

        public DocumentJournalViewModel(DTO.DataModel pDataModel)
        {
            DataModel = pDataModel;
            DocResponseType = new ObservableCollection<RefDocMap.DocMap>();

            DocTypes = RefDocType.GetInstance.refDocType;

            foreach (RefDocType.DocType docType in DocTypes) docType.OnClickCommand = new DelegateCommand(CreateDoc);

            FilterActionCommand = new DelegateCommand(FilterAction);
            FilterApplyCommand = new DelegateCommand(FilterApply);
            DeleteCommand = new DelegateCommand(DeleteOnClick, DeleteCommand_CanExecute);

            collectDocumentByFilter();
        }

        #region Commands
        
        private void CreateDoc(object obj)
        {
            Model.Document doc = new Model.Document
            {
                DocType = Convert.ToDecimal(obj)
            };

            try
            {
                doc.Id = createDoc(doc);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, GValues.AppNameFull, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DataModel.Documents.Add(doc);
            DataModel.CurDocument = doc;

            showDoc();
        }

        public ICommand DeleteCommand { get; private set; }
        void DeleteOnClick(object obj)
        {
            if (MessageBox.Show(string.Format("Вы уверены что хотите удалить документ?\n Тип документа '{0}'\n № {1}\n дата документа: {2}",
                DataModel.CurDocument.DocTypeName, DataModel.CurDocument.Id, DataModel.CurDocument.DateDoc),
                GValues.AppNameFull, MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(GValues.connString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = con;
                        command.CommandText = " UPDATE document SET ref_status = 2 WHERE id = @id";

                        command.Parameters.Add("id", SqlDbType.Decimal).Value = DataModel.CurDocument.Id;

                        command.ExecuteNonQuery();
                    }
                }

                removeDocFromTree(DataModel.CurDocument.Id);
            }
        }
        public bool DeleteCommand_CanExecute(object obj)
        {
            return DataModel.CurDocument?.DocumentChilds.Count == 0;
        }

        private void removeDocFromTree(decimal pDocId)

        {
            foreach (Model.Document doc in DataModel.Documents)
            {
                if (doc.Id == pDocId) { DataModel.Documents.Remove(doc); DataModel.CurDocument = null; break; }
                else removeDocFromTreeRecursion(doc, pDocId);
            }
        }

        private void removeDocFromTreeRecursion(Model.Document pDoc, decimal pDocId)
        {
            foreach (Model.Document doc in pDoc.DocumentChilds)
            {
                if (doc.Id == pDocId)
                {
                    pDoc.DocumentChilds.Remove(doc);
                    DataModel.CurDocument = pDoc;
                    break;
                }
                else removeDocFromTreeRecursion(doc, pDocId);
            }
        }

        public ICommand FilterApplyCommand { get; private set; }
        void FilterApply(object obj)
        {
            collectDocumentByFilter();
        }

        public ICommand FilterActionCommand { get; private set; }
        void FilterAction(object obj)
        {
            FilterVisibility = !FilterVisibility;
        }

        #endregion

        private void collectDocumentByFilter()
        {
            DataModel.Documents = new ObservableCollection<Model.Document>();

            string docTypes = string.Join(",", DataModel.Filter.docTypes.Where(x => x.isSelected == true).Select(x => x.id));

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT	[id] As Id, " +
                                            "		[parentId] AS ParentId, " +
                                            "		[ref_docType] AS DocType, " +
                                            "		[dateCreate] AS DateCreate, " +
                                            "		[dateDoc] AS DateDoc, " +
                                            "		[ref_status] AS RefStatus, " +
                                            "		[xfrom] AS XFrom, " +
                                            "		[xto] AS XTo, " +
                                            "       dbo.SelectDocumentItems_xml([id]), " +
                                            "		dbo.SelectDocumentChild_xml([id]) " +
                                            " FROM document " +
                                            " WHERE ref_status = 1 AND parentId = 0 AND dateDoc BETWEEN @dateStart AND @dateEnd " +
                                            (docTypes.Equals(string.Empty) ? string.Empty : " AND ref_docType in (" + docTypes + ")") +
                                            " FOR XML RAW('Document'), ROOT('ArrayOfDocument'), ELEMENTS ";

                    command.Parameters.Add("dateStart", SqlDbType.DateTime).Value = DataModel.Filter.dateStart;
                    command.Parameters.Add("dateEnd", SqlDbType.DateTime).Value = DataModel.Filter.dateEnd;

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        DataModel.Documents = Support.XMLToObject<ObservableCollection<Model.Document>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private decimal createDoc(Model.Document pDoc)
        {
            decimal result = 0;

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " INSERT INTO document(parentId, ref_docType, dateCreate, dateDoc, ref_status, xfrom, xto) " +
                                            " VALUES (@parentId, @ref_docType, @dateCreate, @dateDoc, @ref_status, @xfrom, @xto);" +
                                            " SELECT SCOPE_IDENTITY(); ";

                    command.Parameters.Add("parentId", SqlDbType.Int).Value = pDoc.ParentId;
                    command.Parameters.Add("ref_docType", SqlDbType.Int).Value = pDoc.DocType;
                    command.Parameters.Add("dateCreate", SqlDbType.DateTime).Value = pDoc.DateCreate;
                    command.Parameters.Add("dateDoc", SqlDbType.DateTime).Value = pDoc.DateDoc;
                    command.Parameters.Add("ref_status", SqlDbType.Int).Value = pDoc.RefStatus;
                    command.Parameters.Add("xfrom", SqlDbType.Int).Value = pDoc.XFrom;
                    command.Parameters.Add("xto", SqlDbType.Int).Value = pDoc.XTo;

                    result = (decimal)command.ExecuteScalar();
                }
            }

            return result;
        }

        public void showDoc()
        {
            DocumentShow();
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
