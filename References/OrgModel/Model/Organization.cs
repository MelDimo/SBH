using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.sbh.gui.references.orgmodel.Model
{
    public class Organization 
    {
        public int id { get; set; }
        public string name { get; set; }
        public int refStatus { get; set; }
        [XmlElement("ArrayOfBranch", typeof(ObservableCollection<Branch>))]
        public ObservableCollection<Branch> Branch { get; set; }

    }
}
