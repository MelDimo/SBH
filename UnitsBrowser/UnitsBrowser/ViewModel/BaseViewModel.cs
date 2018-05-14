using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public sealed class BaseViewModel : INotifyPropertyChanged
    {
        private static readonly Lazy<BaseViewModel> lazy = new Lazy<BaseViewModel>(() => new BaseViewModel());
        public static BaseViewModel GetInstance { get { return lazy.Value; } }

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
                priviosView = currentView ?? value;
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

        #region Коллеции
        
        /// <summary>
        /// Текушая выбранная конечная точка
        /// </summary>
        public Model.Unit CurrentUnitEx { get; set; }

        /// <summary>
        /// Доступные конечные точки
        /// </summary>
        private ObservableCollection<Model.Unit> collectionUnitEx;
        public ObservableCollection<Model.Unit> CollectionUnitEx
        {
            get { return collectionUnitEx; }
            set { collectionUnitEx = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Текущая выбранная позиция балланса
        /// </summary>
        public Model.Item CurrentItemBalans { get; set; }

        /// <summary>
        /// Доступные позиции баланса
        /// </summary>
        private ObservableCollection<Model.Item> collectionItemBalans;
        public ObservableCollection<Model.Item> CollectionItemBalans
        {
            get { return collectionItemBalans; }
            set
            {
                collectionItemBalans = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
