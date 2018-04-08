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
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace com.sbh.gui.invoices.ViewModel
{
    public class SurfaceControlViewModel : INotifyPropertyChanged
    {
        public delegate Model.Document DgtGetCurrentDocument();
        private DgtGetCurrentDocument GetCurrentDocument;

        private UserControl mDocumentJournalView;

        private View.DocumentView documentView;

        private UserControl _curUserControl;
        public UserControl CurUserControl
        {
            get { return _curUserControl; }
            private set
            {
                _curUserControl = value;
                OnPropertyChanged("CurUserControl");
            }
        }

        public Model.Filter Filter { get; set; }

        public List<RefDocType.DocType> DocTypes { get; private set; }

        public ObservableCollection<RefDocMap.DocMap> _docResponseType;
        public ObservableCollection<RefDocMap.DocMap> DocResponseType
        {
            get
            {
                return new ObservableCollection<RefDocMap.DocMap>(_docResponseType.Where(x => x.refDocTypeSource == CurSelectedDoc?.DocType).ToList());
            }

            private set { _docResponseType = value; }
        }

        private Model.Document _curSelectedDoc;
        public Model.Document CurSelectedDoc
        {
            get { return _curSelectedDoc; }
            set
            {
                _curSelectedDoc = value;
                OnPropertyChanged("DocResponseType");
            }
        }

        private ObservableCollection<Model.Document> _docs;
        public ObservableCollection<Model.Document> Docs
        {
            get { return _docs; }
            set { _docs = value; OnPropertyChanged("Docs"); }
        }

        public void SelectObject(object pSelectedItem)
        {
            CurSelectedDoc = (pSelectedItem as Model.Document);
            ShowDocDetails(null);
        }

        BackgroundWorker bgwTouchReportDocument = new BackgroundWorker();

        public SurfaceControlViewModel()
        {
            GetCurrentDocument = getCurrentDocument;

            Filter = new Model.Filter();

            DocTypes = RefDocType.GetInstance.refDocType;

            foreach (RefDocMap.DocMap docMap in RefDocMap.GetInstance.refDocMap)
            {
                docMap.OnClickCommand = new DelegateCommand(MenuItemResponseOnClick);
            }
            DocResponseType = new ObservableCollection<RefDocMap.DocMap>(RefDocMap.GetInstance.refDocMap);

            FilterActionCommand = new DelegateCommand(FilterAction);
            FilterApplyCommand = new DelegateCommand(FilterApply);
            BackOnClickCommand = new DelegateCommand(BackOnClick);
            ShowDocDetailsCommand = new DelegateCommand(ShowDocDetails);

            DeleteCommand = new DelegateCommand(DeleteOnClick, DeleteCommand_CanExecute);
            //ResponseCommand = new DelegateCommand(ResponseOnClick/*, ResponseCommand_CanExecute*/);

            foreach (RefDocType.DocType docType in DocTypes)
            {
                docType.OnClickCommand = new DelegateCommand(MenuItemOnClick);
            }

            _filterVisibility = false;

            mDocumentJournalView = new View.DocumentJournalView();

            CurUserControl = mDocumentJournalView;

            CollectDocsByFilter();

            // Дергаю Crystal для последующего быстрого вызова
            bgwTouchReportDocument.DoWork += Bgw_DoWork;
            bgwTouchReportDocument.RunWorkerAsync();
        }

        private Model.Document getCurrentDocument()
        {
            return CurSelectedDoc;
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            CrystalDecisions.CrystalReports.Engine.ReportDocument doc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            doc.Dispose();
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

        #region ICommand

        public ICommand ShowDocDetailsCommand { get; private set; }
        void ShowDocDetails(object obj)
        {
            documentView = new View.DocumentView();
            documentView.DataContext = new DocumentViewModel(GetCurrentDocument);
            CurUserControl = documentView;
        }

        public ICommand FilterActionCommand { get; private set; }
        void FilterAction(object obj)
        {
            FilterVisibility = !FilterVisibility;
        }

        public ICommand FilterApplyCommand { get; private set; }
        void FilterApply(object obj)
        {
            CollectDocsByFilter();
        }

        public static ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            CurUserControl = mDocumentJournalView;
        }

        public ICommand DeleteCommand { get; private set; }
        void DeleteOnClick(object obj)
        {
            if (MessageBox.Show(string.Format("Вы уверены что хотите удалить документ?\n Тип документа '{0}'\n № {1}\n дата документа: {2}", 
                CurSelectedDoc.DocTypeName, CurSelectedDoc.Id, CurSelectedDoc.DateDoc),
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

                        command.Parameters.Add("id", SqlDbType.Decimal).Value = CurSelectedDoc.Id;

                        command.ExecuteNonQuery();
                    }
                }

                removeDocFromTree(CurSelectedDoc.Id);
            }
        }

        public bool DeleteCommand_CanExecute(object obj)
        {
            return CurSelectedDoc?.DocumentChilds.Count == 0;
        }

        void MenuItemResponseOnClick(object obj)
        {
            Model.Document document = new Model.Document()
            {
                ParentId = CurSelectedDoc.Id,
                DateCreate = DateTime.Now,
                DateDoc = DateTime.Now,
                DocType = (decimal)obj,
                RefStatus = 1,
                XFrom = CurSelectedDoc.XFrom,
                XTo = CurSelectedDoc.XTo
            };

            document.Id = createDoc(document);

            CurSelectedDoc.DocumentChilds.Add(document);

            CurSelectedDoc = document;

            document = null;

            MSG msg = fillResponseDocumentPosition();
            if (!msg.IsSuccess)
            { 
                MessageBox.Show(msg.Message, GValues.AppNameFull, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ShowDocDetails(null);
        }

        private MSG fillResponseDocumentPosition()
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
                        command.CommandText = "Position_ResponseDublicate";

                        command.Parameters.Add("pDocId", SqlDbType.Int).Value = CurSelectedDoc.Id;

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

        void MenuItemOnClick(object obj)
        {
            Model.Document document = new Model.Document()
            {
                ParentId = 0,
                DateCreate = DateTime.Now,
                DateDoc = DateTime.Now,
                DocType = (int)obj,
                RefStatus = 1,
                XFrom = 1,
                XTo = 1
            };

            document.Id = createDoc(document);
            Docs.Add(document);
            CurSelectedDoc = document;

            ShowDocDetails(null);
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

        private void removeDocFromTree(decimal pDocId)

        {
            foreach (Model.Document doc in Docs)
            {
                if (doc.Id == pDocId) { Docs.Remove(doc); CurSelectedDoc = null; break; }
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
                    CurSelectedDoc = pDoc;
                    break;
                }
                else removeDocFromTreeRecursion(doc, pDocId);
            }
        }

        private void CollectDocsByFilter()
        {
            Docs = new ObservableCollection<Model.Document>();

            string docTypes = string.Join(",", Filter.docTypes.Where(x => x.isSelected == true).Select(x => x.id));

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

                    command.Parameters.Add("dateStart", SqlDbType.DateTime).Value = Filter.dateStart;
                    command.Parameters.Add("dateEnd", SqlDbType.DateTime).Value = Filter.dateEnd;

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        Docs = Support.XMLToObject<ObservableCollection<Model.Document>>(reader.ReadOuterXml());
                    }
                }
            }
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
