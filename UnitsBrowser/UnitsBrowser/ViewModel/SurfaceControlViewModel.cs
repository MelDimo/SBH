using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public class SurfaceControlViewModel : BaseViewModel
    {

        public string CurrentViewHeader { get { return Model.BaseValues.ViewHeader; } }

        public SurfaceControlViewModel()
        {
            CurrentView = new View.MainView();
        }
    }
}
