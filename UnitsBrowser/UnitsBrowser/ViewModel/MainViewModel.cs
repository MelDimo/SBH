using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public class MainViewModel : IViewModel, INotifyPropertyChanged
    {
        public delegate void EventRaise(object obj);

        private bool showLoader;
        public bool ShowLoader
        {
            get { return showLoader; }
            set { showLoader = value; OnPropertyChanged(); }
        }

        private DBAccess dbAccess = new DBAccess();

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Запрос на отображение UnitView
        /// </summary>
        public event EventRaise OnUnitClick;

        public MainViewModel()
        {
            BaseViewModel = BaseViewModel.GetInstance;
            IsBackBtnEnable = false;
            ViewHeader = "Основная форма.";

            UnitOnClickCommand = new DelegateCommand(UnitOnClick);
        }

        public async void CollectUnitExAsync()
        {
            ShowLoader = true;
            await Task.Run(() =>
            {
                Msg = dbAccess.CollectUnitEx();
                if (Msg.IsSuccess)
                {
                    BaseViewModel.CollectionUnitEx = msg.Obj as ObservableCollection<Model.Unit>;
                    if (BaseViewModel.CollectionUnitEx.Count == 0) Msg.Message = "Нет данных!";
                }
                else { Msg.Message = String.Format("Ошибка получения данных:\n{0}", Msg.Message); return; }
            });
            ShowLoader = false;
        }

        public ICommand UnitOnClickCommand { get; private set; }
        void UnitOnClick(object obj)
        {
            OnUnitClick(obj);
        }


        #region IViewModel Members

        public BaseViewModel BaseViewModel { get; set; }
        public bool IsBackBtnEnable { get; set; }
        public string ViewHeader { get; set; }

        private MSG msg;
        public MSG Msg
        {
            get { return msg; }
            set { msg = value; OnPropertyChanged(); }
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
