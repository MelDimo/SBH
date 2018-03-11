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
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace com.sbh.gui.invoices.ViewModel
{
    public class SurfaceControlViewModel : INotifyPropertyChanged
    {
        //В дальнейшем добавляю в коллекцию
        private DocumentType1ViewModel docType1Model;

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

        private Model.Document _curDoc;
        public Model.Document CurDoc {
            get { return _curDoc; }
            set
            {
                _curDoc = value;
                OnPropertyChanged("CurDoc");
            }
        }

        private ObservableCollection<Model.Document> _docs;
        public ObservableCollection<Model.Document> Docs
        {
            get { return _docs; }
            set { _docs = value; OnPropertyChanged("Docs"); }
        }

        public SurfaceControlViewModel()
        {
            Filter = new Model.Filter();

            DocTypes = RefDocType.GetInstance.refDocType;

            FilterActionCommand = new DelegateCommand(FilterAction);
            FilterApplyCommand = new DelegateCommand(FilterApply);
            BackOnClickCommand = new DelegateCommand(BackOnClick);
            ShowDocDetailsCommand = new DelegateCommand(ShowDocDetails);

            foreach (RefDocType.DocType docType in DocTypes)
            {
                docType.OnClickCommand = new DelegateCommand(MenuItemOnClick);
            }

            _filterVisibility = false;

            mDocumentJournalView = new View.DocumentJournalView();

            CurUserControl = mDocumentJournalView;

            CollectDocsByFilter();
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
            switch (CurDoc.docType)
            {
                case 1:
                    DocumentType1ViewModel docType1Model = new DocumentType1ViewModel(new Model.DocumentType1()
                    {
                        id = CurDoc.id,
                        docType = CurDoc.docType,
                        dateCreate = CurDoc.dateCreate,
                        dateDoc = CurDoc.dateDoc,
                        refStatus = CurDoc.refStatus
                    });
                    View.DocumentType1View documentType1View = new View.DocumentType1View();
                    documentType1View.DataContext = docType1Model;
                    CurUserControl = documentType1View;
                    break;

                case 2:
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
            CollectDocsByFilter();
        }

        public static ICommand BackOnClickCommand { get; private set; }
        void BackOnClick(object obj)
        {
            CurUserControl = mDocumentJournalView;
        }

        #endregion

        void MenuItemOnClick(object obj)
        {
            
            switch ((int)obj)
            {
                case 1:             // Приход
                    DocumentType1ViewModel docType1Model = new DocumentType1ViewModel(new Model.DocumentType1());
                    View.DocumentType1View documentType1View = new View.DocumentType1View();
                    documentType1View.DataContext = docType1Model;
                    CurUserControl = documentType1View;
                    break;

                case 2:             // Перемещение
                    break;

                case 3:             // Списание
                    break;

                case 4:             // Реализация
                    break;
            }
        }

        void CollectDocsByFilter()
        {
            Docs = new ObservableCollection<Model.Document>();

            string docTypes = string.Join(",", Filter.docTypes.Where(x => x.isSelected == true).Select(x => x.id));

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT id, ref_docType AS docType, dateCreate, dateDoc, ref_status AS refStatus " +
                                            " FROM document " +
                                            " WHERE dateDoc BETWEEN @dateStart AND @dateEnd " +
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
