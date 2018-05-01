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
        private MainViewModel maintViewModel;

        public SurfaceControlViewModel()
        {
            BackOnClickCommand = new DelegateCommand(BackClick);

            maintViewModel = new MainViewModel();

            CurrentView = new View.MainView() { DataContext = maintViewModel };
        }


        #region Command

        public ICommand BackOnClickCommand { get; private set; }

        #endregion
    }
}
