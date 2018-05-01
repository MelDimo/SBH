using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    interface IViewModel
    {
        bool IsBackBtnEnable { get; set; }
        string ViewHeader { get; set; }
    }
}
