using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace com.sbh.gui.unitsbrowser.Model
{
    public class Document
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int DocType { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateDoc { get; set; }
        public int XFrom { get; set; }
        public int XTo { get; set; }

        [XmlIgnore]
        public string DocTypeName
        {
            get { return RefDocType.GetInstance.refDocType.Single(x => x.id == DocType).name; }
        }
        [XmlIgnore]
        public string XToName
        {
            get
            {
                string result = string.Empty;

                switch (DocType)
                {
                    default:
                        result = String.Format("{0} / {1}",
                            RefRecipient.GetInstance.Recipients.Single(x => x.id == XTo).Name, RefRecipient.GetInstance.Recipients.Single(x => x.id == XTo).name);
                        break;
                }

                return result;
            }
        }
        [XmlIgnore]
        public string XFromName
        {
            get
            {
                string result = string.Empty;

                switch (DocType)
                {

                    case 2:
                        result = String.Format("{0} / {1}",
                            RefRecipient.GetInstance.Recipients.Single(x => x.id == XFrom).Name, RefRecipient.GetInstance.Recipients.Single(x => x.id == XFrom).name);
                        break;

                    default:
                        result = String.Format("{0} / {1}",
                            RefCounterParty.GetInstance.CounterPartys.Single(x => x.id == XFrom).groupname, RefCounterParty.GetInstance.CounterPartys.Single(x => x.id == XFrom).name);
                        break;
                }

                return result;
            }
        }
    }
}
