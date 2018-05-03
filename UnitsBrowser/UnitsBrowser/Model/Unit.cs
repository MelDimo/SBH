using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.sbh.gui.unitsbrowser.Model
{
    public class Unit : INotifyPropertyChanged
    {
        public int UnitId { get; set; }

        private string unitName;
        public string UnitName
        {
            get { return unitName; }
            set { unitName = value; OnPropertyChanged(); }
        }

        public string BranchName { get; set; }
        public string OrgName { get; set; }

        [XmlIgnore]
        public string Header { get { return string.Format("{0} / {1}", OrgName, BranchName); } }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
