using com.sbh.dll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.sbh.gui.references.orgmodel.Model
{
    public class Organization : INotifyPropertyChanged
    {
        public int id { get; set; }

        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        private int _refStatus;
        public int refStatus
        {
            get {
                return _refStatus;
                
            }
            set
            {
                _refStatus = value;
            }
        }

        [XmlElement("ArrayOfBranch", typeof(ObservableCollection<Branch>))]
        public ObservableCollection<Branch> Branch { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        #endregion

    }
}
