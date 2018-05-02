using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        // Заголовок текущей View
        private string currentViewHeader;
        public string CurrentViewHeader
        {
            get { return currentViewHeader; }
            private set { currentViewHeader = value; OnPropertyChanged(); }
        }

        // Отображается ли кнопка "Назад" для загруженной View
        private bool isBackButtonEnable;
        public bool IsBackButtonEnable
        {
            get { return isBackButtonEnable; }
            private set { isBackButtonEnable = value; OnPropertyChanged(); }
        }

        #region Навигация OnBackClick

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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Кнопка "Назад"
        /// </summary>
        /// <param name="obj"></param>
        public void OnBackClick(object obj)
        {
            CurrentView = priviosView;
        }

        #endregion

        public Model.UnitEx CurrentUnitEx { get; set; }

        private ObservableCollection<Model.UnitEx> collectionUnitEx;
        public ObservableCollection<Model.UnitEx> CollectionUnitEx
        {
            get { return collectionUnitEx; }
            set { collectionUnitEx = value; OnPropertyChanged(); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
