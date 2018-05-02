using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public class UnitViewModel : IViewModel
    {
        
        public UnitViewModel()
        {
            IsBackBtnEnable = true;
            ViewHeader = string.Format("Подразделение {0}", "...");
        }

        #region IViewModel Members

        public bool IsBackBtnEnable { get; set; }
        public string ViewHeader { get; set; }

        #endregion
    }
}
