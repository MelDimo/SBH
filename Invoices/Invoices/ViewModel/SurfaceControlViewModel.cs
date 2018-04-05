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
        private UserControl mDocumentJournalView;

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
                return new ObservableCollection<RefDocMap.DocMap>(_docResponseType.Where(x => x.refDocTypeSource == CurSelectedDocTree?.docType).ToList());
            }

            private set { _docResponseType = value; }
        }

        private Model.DocumentTree _curSelectedDocTree;
        public Model.DocumentTree CurSelectedDocTree
        {
            get { return _curSelectedDocTree; }
            set
            {
                _curSelectedDocTree = value;
                OnPropertyChanged("DocResponseType");
            }
        }

        private ObservableCollection<Model.DocumentTree> _docsTree;
        public ObservableCollection<Model.DocumentTree> DocsTree
        {
            get { return _docsTree; }
            set { _docsTree = value; OnPropertyChanged("DocsTree"); }
        }

        public void SelectObject(object pSelectedItem)
        {
            CurSelectedDocTree = (pSelectedItem as Model.DocumentTree);
            ShowDocDetails(null);
        }

        BackgroundWorker bgwTouchReportDocument = new BackgroundWorker();

        public SurfaceControlViewModel()
        {
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

            CollectDocsByFilterTree();

            // Дергаю документ для последующего быстрого вызова
            bgwTouchReportDocument.DoWork += Bgw_DoWork;
            bgwTouchReportDocument.RunWorkerAsync();
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
            switch (CurSelectedDocTree.docType)
            {
                case 1:
                    DocumentType1ViewModel docType1Model = new DocumentType1ViewModel(new Model.DocumentType1()
                    {
                        id = CurSelectedDocTree.id,
                        parentId = CurSelectedDocTree.perentId,
                        docType = CurSelectedDocTree.docType,
                        dateCreate = CurSelectedDocTree.dateCreate,
                        dateDoc = CurSelectedDocTree.dateDoc,
                        refStatus = CurSelectedDocTree.refStatus,
                        counterpaty = RefCounterParty.GetInstance.CounterPartys.SingleOrDefault(x => x.id == CurSelectedDocTree.xfrom),
                        recipient = RefRecipient.GetInstance.Recipients.SingleOrDefault(x => x.id == CurSelectedDocTree.xto)
                    });
                    View.DocumentType1View documentType1View = new View.DocumentType1View();
                    documentType1View.DataContext = docType1Model;
                    CurUserControl = documentType1View;
                    break;

                case 2:
                    DocumentType2ViewModel docType2Model = new DocumentType2ViewModel(new Model.DocumentType2()
                    {
                        id = CurSelectedDocTree.id,
                        parentId = CurSelectedDocTree.perentId,
                        docType = CurSelectedDocTree.docType,
                        dateCreate = CurSelectedDocTree.dateCreate,
                        dateDoc = CurSelectedDocTree.dateDoc,
                        refStatus = CurSelectedDocTree.refStatus,
                        counterpaty = RefRecipient.GetInstance.Recipients.SingleOrDefault(x => x.id == CurSelectedDocTree.xfrom),
                        recipient = RefRecipient.GetInstance.Recipients.SingleOrDefault(x => x.id == CurSelectedDocTree.xto)
                    });
                    View.DocumentType2View documentType2View = new View.DocumentType2View();
                    documentType2View.DataContext = docType2Model;
                    CurUserControl = documentType2View;
                    break;

                case 5:
                    DocumentType5ViewModel docType5Model = new DocumentType5ViewModel(new Model.DocumentType5()
                    {
                        id = CurSelectedDocTree.id,
                        parentId = CurSelectedDocTree.perentId,
                        docType = CurSelectedDocTree.docType,
                        dateCreate = CurSelectedDocTree.dateCreate,
                        dateDoc = CurSelectedDocTree.dateDoc,
                        refStatus = CurSelectedDocTree.refStatus,
                        counterpaty = RefCounterParty.GetInstance.CounterPartys.SingleOrDefault(x => x.id == CurSelectedDocTree.xfrom),
                        recipient = RefRecipient.GetInstance.Recipients.SingleOrDefault(x => x.id == CurSelectedDocTree.xto)
                    });
                    View.DocumentType5View documentType5View = new View.DocumentType5View();
                    documentType5View.DataContext = docType5Model;
                    CurUserControl = documentType5View;
                    break;
            }
        }

        public ICommand FilterActionCommand { get; private set; }
        void FilterAction(object obj)
        {
            FilterVisibility = !FilterVisibility;
        }

        public ICommand FilterApplyCommand { get; private set; }
        void FilterApply(object obj)
        {
            CollectDocsByFilterTree();
        }

        public static ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            switch (CurUserControl.GetType().Name)
            {
                case "DocumentType1View":
                    CurSelectedDocTree.dateCreate = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.dateCreate;
                    CurSelectedDocTree.perentId = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.parentId;
                    CurSelectedDocTree.dateDoc = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.dateDoc;
                    CurSelectedDocTree.docType = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.docType;
                    CurSelectedDocTree.xfrom = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.counterpaty.id;
                    CurSelectedDocTree.xto = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.recipient.id;
                    break;

                case "DocumentType2View":
                    CurSelectedDocTree.dateCreate = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.dateCreate;
                    CurSelectedDocTree.perentId = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.parentId;
                    CurSelectedDocTree.dateDoc = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.dateDoc;
                    CurSelectedDocTree.docType = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.docType;
                    CurSelectedDocTree.xfrom = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.counterpaty.id;
                    CurSelectedDocTree.xto = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.recipient.id;
                    break;

                case "DocumentType5View":
                    CurSelectedDocTree.dateCreate = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.dateCreate;
                    CurSelectedDocTree.perentId = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.parentId;
                    CurSelectedDocTree.dateDoc = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.dateDoc;
                    CurSelectedDocTree.docType = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.docType;
                    CurSelectedDocTree.xfrom = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.counterpaty.id;
                    CurSelectedDocTree.xto = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.recipient.id;
                    break;
            }

            CurUserControl = mDocumentJournalView;
        }

        public ICommand DeleteCommand { get; private set; }
        void DeleteOnClick(object obj)
        {
            if (MessageBox.Show(string.Format("Вы уверены что хотите удалить документ?\n Тип документа '{0}'\n № {1}\n дата документа: {2}", CurSelectedDocTree.TypeName, CurSelectedDocTree.id, CurSelectedDocTree.dateDoc),
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

                        command.Parameters.Add("id", SqlDbType.Decimal).Value = CurSelectedDocTree.id;

                        command.ExecuteNonQuery();
                    }
                }
                //OnPropertyChanged("CurDocTree");
            }

            removeDocFromTree(CurSelectedDocTree.id);
        }

        public bool DeleteCommand_CanExecute(object obj)
        {
            return CurSelectedDocTree?.DocumentTreeChild.Count == 0;
        }

        void MenuItemResponseOnClick(object obj)
        {
            Model.DocumentTree document = new Model.DocumentTree()
            {
                perentId = CurSelectedDocTree.id,
                dateCreate = DateTime.Now,
                dateDoc = DateTime.Now,
                docType = (decimal)obj,
                refStatus = 1,
                xfrom = CurSelectedDocTree.xfrom,
                xto = CurSelectedDocTree.xto
            };

            document.id = createDoc(document);

            CurSelectedDocTree.DocumentTreeChild.Add(document);

            CurSelectedDocTree = document;

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

                        command.Parameters.Add("pDocId", SqlDbType.Int).Value = CurSelectedDocTree.id;

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
            Model.DocumentTree document = new Model.DocumentTree()
            {
                perentId = 0,
                dateCreate = DateTime.Now,
                dateDoc = DateTime.Now,
                docType = (int)obj,
                refStatus = 1,
                xfrom = 1,
                xto = 1
            };

            document.id = createDoc(document);
            DocsTree.Add(document);
            CurSelectedDocTree = document;

            ShowDocDetails(null);
        }

        private decimal createDoc(Model.DocumentTree pDoc)
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

                    command.Parameters.Add("parentId", SqlDbType.Int).Value = pDoc.perentId;
                    command.Parameters.Add("ref_docType", SqlDbType.Int).Value = pDoc.docType;
                    command.Parameters.Add("dateCreate", SqlDbType.DateTime).Value = pDoc.dateCreate;
                    command.Parameters.Add("dateDoc", SqlDbType.DateTime).Value = pDoc.dateDoc;
                    command.Parameters.Add("ref_status", SqlDbType.Int).Value = pDoc.refStatus;
                    command.Parameters.Add("xfrom", SqlDbType.Int).Value = pDoc.xfrom;
                    command.Parameters.Add("xto", SqlDbType.Int).Value = pDoc.xto;

                    result = (decimal)command.ExecuteScalar();
                }
            }

            return result;
        }

        private void removeDocFromTree(decimal pDocId)

        {
            foreach (Model.DocumentTree doc in DocsTree)
            {
                if (doc.id == pDocId) { DocsTree.Remove(doc); CurSelectedDocTree = null; break; }
                else removeDocFromTreeRecursion(doc, pDocId);
            }
        }

        private void removeDocFromTreeRecursion(Model.DocumentTree pDoc, decimal pDocId)
        {
            foreach (Model.DocumentTree doc in pDoc.DocumentTreeChild)
            {
                if (doc.id == pDocId)
                {
                    pDoc.DocumentTreeChild.Remove(doc);
                    CurSelectedDocTree = pDoc;
                    break;
                }
                else removeDocFromTreeRecursion(doc, pDocId);
            }
        }


        private void CollectDocsByFilterTree()
        {
            DocsTree = new ObservableCollection<Model.DocumentTree>();

            string docTypes = string.Join(",", Filter.docTypes.Where(x => x.isSelected == true).Select(x => x.id));

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT	[id], " +
                                            "		[parentId], " +
                                            "		[ref_docType] AS docType, " +
                                            "		[dateCreate], " +
                                            "		[dateDoc], " +
                                            "		[ref_status] AS refStatus, " +
                                            "		[xfrom], " +
                                            "		[xto], " +
                                            "		dbo.SelectDocumentChild_xml([id]) " +
                                            " FROM document " +
                                            " WHERE ref_status = 1 AND parentId = 0 AND dateDoc BETWEEN @dateStart AND @dateEnd " +
                                            (docTypes.Equals(string.Empty) ? string.Empty : " AND ref_docType in (" + docTypes + ")") +
                                            " FOR XML RAW('DocumentTree'), ROOT('ArrayOfDocumentTree'), ELEMENTS ";

                    command.Parameters.Add("dateStart", SqlDbType.DateTime).Value = Filter.dateStart;
                    command.Parameters.Add("dateEnd", SqlDbType.DateTime).Value = Filter.dateEnd;

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        DocsTree = Support.XMLToObject<ObservableCollection<Model.DocumentTree>>(reader.ReadOuterXml());
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
