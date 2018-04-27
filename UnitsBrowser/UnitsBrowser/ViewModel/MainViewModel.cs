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
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Button_Click = new DelegateCommand(BackClick);
        }

        #region Command

        public ICommand Button_Click { get; private set; }

        #endregion
    }
}
