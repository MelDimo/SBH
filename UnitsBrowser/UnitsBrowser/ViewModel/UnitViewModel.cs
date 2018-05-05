using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using com.sbh.dll.utilites;

namespace com.sbh.gui.unitsbrowser.ViewModel
{
    public class UnitViewModel : IViewModel, INotifyPropertyChanged
    {
        private string filterText;
        public string FilterText
        {
            get { return filterText; }
            set { filterText = value; OnPropertyChanged(); }
        }

        public UnitViewModel()
        {
            BaseViewModel = BaseViewModel.GetInstance;
            IsBackBtnEnable = true;
            ViewHeader = string.Format("{0} / {1} / {2}", BaseViewModel.CurrentUnitEx.OrgName, BaseViewModel.CurrentUnitEx.BranchName, BaseViewModel.CurrentUnitEx.UnitName);
        }

        public void InitCollection()
        {

        }

        #region IViewModel Members

        public bool IsBackBtnEnable { get; set; }
        public string ViewHeader { get; set; }
        public BaseViewModel BaseViewModel { get; set; }
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
