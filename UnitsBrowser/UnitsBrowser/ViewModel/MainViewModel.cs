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
    public class MainViewModel : IViewModel
    {
        public delegate void EventRaise(object obj);

        /// <summary>
        /// Запрос на отображение UnitView
        /// </summary>
        public event EventRaise OnUnitClick;

        public MainViewModel()
        {
            IsBackBtnEnable = false;
            ViewHeader = "Основная форма.";

            UnitOnClickCommand = new DelegateCommand(UnitOnClick);
        }

        public ICommand UnitOnClickCommand { get; private set; }
        void UnitOnClick(object obj)
        {
            OnUnitClick(obj);
        }


        #region IViewModel Members

        public bool IsBackBtnEnable { get; set; }
        public string ViewHeader { get; set; }

        #endregion
    }
}
