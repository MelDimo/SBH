using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {

        public string CurrentViewHeader { get; set; }

        private UserControl priviosView;
        private UserControl currentView;
        public UserControl CurrentView
        {
            get { return currentView; }
            set
            {
                priviosView = currentView == null ? value : currentView;
                currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        public void BackClick(object obj)
        {
            CurrentView = priviosView;
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
