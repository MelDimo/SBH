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

        private Model.DocumentTree _curDocTree;
        public Model.DocumentTree CurDocTree
        {
            get { return _curDocTree; }
            set
            {
                _curDocTree = value;
                OnPropertyChanged("CurDocTree");
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
            CurDocTree = (pSelectedItem as Model.DocumentTree);
            ShowDocDetails(null);
        }

        BackgroundWorker bgwTouchReportDocument = new BackgroundWorker();

        public SurfaceControlViewModel()
        {
            Filter = new Model.Filter();

            DocTypes = RefDocType.GetInstance.refDocType;

            FilterActionCommand = new DelegateCommand(FilterAction);
            FilterApplyCommand = new DelegateCommand(FilterApply);
            BackOnClickCommand = new DelegateCommand(BackOnClick);
            ShowDocDetailsCommand = new DelegateCommand(ShowDocDetails);

            DeleteCommand = new DelegateCommand(DeleteOnClick, DeleteCommand_CanExecute);

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
            switch (CurDocTree.docType)
            {
                case 1:
                    DocumentType1ViewModel docType1Model = new DocumentType1ViewModel(new Model.DocumentType1()
                    {
                        id = CurDocTree.id,
                        docType = CurDocTree.docType,
                        dateCreate = CurDocTree.dateCreate,
                        dateDoc = CurDocTree.dateDoc,
                        refStatus = CurDocTree.refStatus,
                        counterpaty = RefCounterParty.GetInstance.CounterPartys.SingleOrDefault(x => x.id == CurDocTree.xfrom),
                        recipient = RefRecipient.GetInstance.Recipients.SingleOrDefault(x => x.id == CurDocTree.xto)
                    });
                    View.DocumentType1View documentType1View = new View.DocumentType1View();
                    documentType1View.DataContext = docType1Model;
                    CurUserControl = documentType1View;
                    break;

                case 2:
                    DocumentType2ViewModel docType2Model = new DocumentType2ViewModel(new Model.DocumentType2()
                    {
                        id = CurDocTree.id,
                        docType = CurDocTree.docType,
                        dateCreate = CurDocTree.dateCreate,
                        dateDoc = CurDocTree.dateDoc,
                        refStatus = CurDocTree.refStatus,
                        counterpaty = RefRecipient.GetInstance.Recipients.SingleOrDefault(x => x.id == CurDocTree.xfrom),
                        recipient = RefRecipient.GetInstance.Recipients.SingleOrDefault(x => x.id == CurDocTree.xto)
                    });
                    View.DocumentType2View documentType2View = new View.DocumentType2View();
                    documentType2View.DataContext = docType2Model;
                    CurUserControl = documentType2View;
                    break;

                case 5:
                    DocumentType5ViewModel docType5Model = new DocumentType5ViewModel(new Model.DocumentType5()
                    {
                        id = CurDocTree.id,
                        docType = CurDocTree.docType,
                        dateCreate = CurDocTree.dateCreate,
                        dateDoc = CurDocTree.dateDoc,
                        refStatus = CurDocTree.refStatus,
                        counterpaty = RefCounterParty.GetInstance.CounterPartys.SingleOrDefault(x => x.id == CurDocTree.xfrom),
                        recipient = RefRecipient.GetInstance.Recipients.SingleOrDefault(x => x.id == CurDocTree.xto)
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
                    CurDocTree.dateCreate = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.dateCreate;
                    CurDocTree.dateDoc = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.dateDoc;
                    CurDocTree.docType = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.docType;
                    CurDocTree.xfrom = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.counterpaty.id;
                    CurDocTree.xto = ((CurUserControl as View.DocumentType1View).DataContext as DocumentType1ViewModel).Doc.recipient.id;
                    break;

                case "DocumentType2View":
                    CurDocTree.dateCreate = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.dateCreate;
                    CurDocTree.dateDoc = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.dateDoc;
                    CurDocTree.docType = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.docType;
                    CurDocTree.xfrom = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.counterpaty.id;
                    CurDocTree.xto = ((CurUserControl as View.DocumentType2View).DataContext as DocumentType2ViewModel).Doc.recipient.id;
                    break;

                case "DocumentType5View":
                    CurDocTree.dateCreate = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.dateCreate;
                    CurDocTree.dateDoc = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.dateDoc;
                    CurDocTree.docType = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.docType;
                    CurDocTree.xfrom = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.counterpaty.id;
                    CurDocTree.xto = ((CurUserControl as View.DocumentType5View).DataContext as DocumentType5ViewModel).Doc.recipient.id;
                    break;
            }

            CurUserControl = mDocumentJournalView;
        }

        public ICommand DeleteCommand { get; private set; }
        void DeleteOnClick(object obj)
        {
            if (MessageBox.Show(string.Format("Вы уверены что хотите удалить документ?\n Тип документа '{0}'\n № {1}\n дата документа: {2}", CurDocTree.TypeName, CurDocTree.id, CurDocTree.dateDoc),
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

                        command.Parameters.Add("id", SqlDbType.Decimal).Value = CurDocTree.id;

                        command.ExecuteNonQuery();
                    }
                }

                DocsTree.Remove(CurDocTree);
                CurDocTree = null;
                OnPropertyChanged("CurDocTree");
            }
                
        }

        public ICommand ResponseCommand { get; private set; }
        void ResponseOnClick(object obj)
        {

        }

        #endregion

        public bool DeleteCommand_CanExecute(object obj)
        {
            return CurDocTree != null;
        }

        void MenuItemOnClick(object obj)
        {
            Model.DocumentTree document = new Model.DocumentTree()
            {
                dateCreate = DateTime.Now,
                dateDoc = DateTime.Now,
                docType = (int)obj,
                refStatus = 1,
                xfrom = 1,
                xto = 1
            };

            document.id = createNewDoc(document);
            DocsTree.Add(document);
            CurDocTree = document;

            ShowDocDetails(null);
        }

        private decimal createNewDoc(Model.DocumentTree pDoc)
        {
            decimal result = 0;

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " INSERT INTO document(ref_docType, dateCreate, dateDoc, ref_status, xfrom, xto) " +
                                            " VALUES (@ref_docType, @dateCreate, @dateDoc, @ref_status, @xfrom, @xto);"+
                                            " SELECT SCOPE_IDENTITY(); ";

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

        //void CollectDocsByFilter()
        //{
        //    DocsTree = new ObservableCollection<Model.DocumentTree>();

        //    string docTypes = string.Join(",", Filter.docTypes.Where(x => x.isSelected == true).Select(x => x.id));

        //    using (SqlConnection con = new SqlConnection(GValues.connString))
        //    {
        //        con.Open();
        //        using (SqlCommand command = new SqlCommand())
        //        {
        //            command.Connection = con;
        //            command.CommandText = " SELECT id, parentId, ref_docType AS docType, dateCreate, dateDoc, ref_status AS refStatus, xfrom, xto " +
        //                                    " FROM document " +
        //                                    " WHERE ref_status = 1 AND dateDoc BETWEEN @dateStart AND @dateEnd " +
        //                                    (docTypes.Equals(string.Empty) ? string.Empty : " AND ref_docType in (" + docTypes + ")") +
        //                                    " FOR XML RAW('Document'), ROOT('ArrayOfDocument'), ELEMENTS ";

        //            command.Parameters.Add("dateStart", SqlDbType.DateTime).Value = Filter.dateStart;
        //            command.Parameters.Add("dateEnd", SqlDbType.DateTime).Value = Filter.dateEnd;

        //            XmlReader reader = command.ExecuteXmlReader();
        //            while (reader.Read())
        //            {
        //                DocsTree = Support.XMLToObject<ObservableCollection<Model.DocumentTree>>(reader.ReadOuterXml());
        //            }
        //        }
        //    }
        //}

        void CollectDocsByFilterTree()
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
