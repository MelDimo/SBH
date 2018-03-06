using com.sbh.dll.utilites;
using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

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

        public List<RefDocType.DocType> DocType { get; private set; }

        private ObservableCollection<Model.Document> _docs;
        public ObservableCollection<Model.Document> Docs
        {
            get { return _docs; }
            set { _docs = value; OnPropertyChanged("Docs"); }
        }

        public SurfaceControlViewModel()
        {
            Filter = new Model.Filter();

            DocType = RefDocType.GetInstance.refDocType;

            FilterActionCommand = new DelegateCommand(FilterAction);
            FilterApplyCommand = new DelegateCommand(FilterApply);
            BackOnClickCommand = new DelegateCommand(BackOnClick);

            foreach (RefDocType.DocType doc in DocType)
            {
                doc.OnClickCommand = new DelegateCommand(MenuItemOnClick);
            }

            _filterVisibility = false;

            mDocumentJournalView = new View.DocumentJournalView();

            CurUserControl = mDocumentJournalView;
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

        public ICommand FilterApplyCommand { get; private set; }
        void FilterApply(object obj)
        {
            FilterVisibility = !FilterVisibility;

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

                    CurUserControl = new View.DocumentView();
                    break;

                case 2:             // Перемещение
                    break;

                case 3:             // Списание
                    break;

                case 4:             // Реализация
                    break;
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
