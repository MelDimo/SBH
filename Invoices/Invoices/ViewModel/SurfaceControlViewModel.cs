using com.sbh.dll;
using com.sbh.dll.utilites;
using com.sbh.dll.utilites.OReferences;
//using com.sbh.gui.invoices.Model;
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
        public DTO.DataModel dataModel = new DTO.DataModel();

        private View.DocumentView documentView;
        private View.DocumentJournalView documentJournalView;

        private DocumentJournalViewModel documentJournalViewModel;
        private DocumentViewModel documentViewModel;

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

        BackgroundWorker bgwTouchReportDocument = new BackgroundWorker();

        public SurfaceControlViewModel()
        {
            documentJournalView = new View.DocumentJournalView();
            documentView = new View.DocumentView();

            documentJournalViewModel = new DocumentJournalViewModel(dataModel);
            documentJournalViewModel.DocumentShow += DocumentJournalViewModel_DocumentShow;
            documentJournalView.DataContext = documentJournalViewModel;

            documentViewModel = new DocumentViewModel(dataModel);
            documentViewModel.JournalShow += DocumentViewModel_JournalShow;
            documentView.DataContext = documentViewModel;

            CurUserControl = documentJournalView;

            // Дергаю Crystal для последующего быстрого вызова
            bgwTouchReportDocument.DoWork += Bgw_DoWork;
            bgwTouchReportDocument.RunWorkerAsync();
        }

        private void DocumentViewModel_JournalShow()
        {
            CurUserControl = documentJournalView;
        }

        private void DocumentJournalViewModel_DocumentShow()
        {
            CurUserControl = documentView;
//            documentViewModel.Refresh();
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            //CrystalDecisions.CrystalReports.Engine.ReportDocument doc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //doc.Dispose();
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

        public ICommand FilterActionCommand { get; private set; }
        void FilterAction(object obj)
        {
            FilterVisibility = !FilterVisibility;
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
