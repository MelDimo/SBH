using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public class SurfaceControlViewModel
    {
        private MainViewModel mainViewModel;
        private UnitViewModel unitViewModel;

        public BaseViewModel BaseViewModel { get; set; }

        public SurfaceControlViewModel()
        {
            BaseViewModel = BaseViewModel.GetInstance;

            BackOnClickCommand = new DelegateCommand(BaseViewModel.OnBackClick);

            mainViewModel = new MainViewModel();
            mainViewModel.OnUnitClick += MainViewModel_OnUnitClick;
            mainViewModel.CollectUnitExAsync();

            BaseViewModel.CurrentView = new View.MainView() { DataContext = mainViewModel };
        }

        private void MainViewModel_OnUnitClick(object obj)
        {
            unitViewModel = new UnitViewModel();
            BaseViewModel.CurrentView = new View.UnitView() { DataContext = unitViewModel };
        }

        #region Command

        /// <summary>
        /// Нажатие кнопки назад
        /// </summary>
        public ICommand BackOnClickCommand { get; private set; }

        #endregion
    }
}
