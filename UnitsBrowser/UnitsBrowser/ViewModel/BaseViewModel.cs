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
        // Заголовок текущей View
        public string CurrentViewHeader { get; private set; }
        public bool IsBackButtonEnable { get; private set; }

        private UserControl priviosView;
        private UserControl currentView;
        public UserControl CurrentView
        {
            get { return currentView; }
            set
            {
                priviosView = currentView == null ? value : currentView;
                currentView = value;

                CurrentViewHeader = ((IViewModel)currentView.DataContext).ViewHeader;
                IsBackButtonEnable = ((IViewModel)currentView.DataContext).IsBackBtnEnable;
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
