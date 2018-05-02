using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public class UnitViewModel : BaseViewModel, IViewModel
    {

        public UnitViewModel()
        {
            IsBackBtnEnable = true;
            ViewHeader = string.Format("Подразделение {0}", "...");
        }

        public void InitCollection()
        {

        }


        #region IViewModel Members

        public bool IsBackBtnEnable { get; set; }
        public string ViewHeader { get; set; }

        #endregion
    }
}
