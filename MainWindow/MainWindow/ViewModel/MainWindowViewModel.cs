using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace com.sbh.gui.mainwindow.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private UserControl _itemsView;
        public UserControl ItemsView
        {
            get { return _itemsView; }
            set
            {

                _itemsView = value;
                OnPropertyChanged("ItemsView");
            }
        }

        UserControl UCReferences;
        UserControl UCInvoices;

        public MainWindowViewModel()
        {
            ReferencesOnClickCommand = new DelegateCommand(ReferencesOnClick);
            JournalOnClickCommand = new DelegateCommand(JournalOnClick);

            UCReferences = new references.View.SurfaceControlView();
            UCInvoices = new invoices.View.SurfaceControlView();


        }

        #region Command

        public ICommand ReferencesOnClickCommand { get; private set; }
        void ReferencesOnClick(object obj)
        {
            ItemsView = UCReferences;
        }

        public ICommand JournalOnClickCommand { get; private set; }
        void JournalOnClick(object obj)
        {
            ItemsView = UCInvoices;
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
