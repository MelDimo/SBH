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
    public class SurfaceControlViewModel : BaseViewModel
    {
        private MainViewModel mainViewModel;
        private UnitViewModel unitViewModel;

        public SurfaceControlViewModel()
        {
            BackOnClickCommand = new DelegateCommand(OnBackClick);

            mainViewModel = new MainViewModel();
            mainViewModel.OnUnitClick += MainViewModel_OnUnitClick;

            unitViewModel = new UnitViewModel();

            CurrentView = new View.MainView() { DataContext = mainViewModel };
        }

        private void MainViewModel_OnUnitClick(object obj)
        {
            CurrentView = new View.UnitView() { DataContext = unitViewModel };
        }

        private void ChangeCurrentView()
        {

        }

        #region Command

        /// <summary>
        /// Нажатие кнопки назад
        /// </summary>
        public ICommand BackOnClickCommand { get; private set; }

        #endregion
    }
}
