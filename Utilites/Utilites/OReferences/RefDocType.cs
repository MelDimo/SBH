using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace com.sbh.dll.utilites.OReferences
{
    public sealed class RefDocType
    {
        private RefDocType()
        {
            reload();
        }

        public ICommand MenuItemOnClickCommand { get; set; }

        public void reload()
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT id, name " +
                                            " FROM ref_docType " +
                                            " FOR XML RAW('DocType'), ROOT('ArrayOfDocType'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        refDocType = Support.XMLToObject<List<DocType>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private static readonly Lazy<RefDocType> lazy = new Lazy<RefDocType>(() => new RefDocType());

        public static RefDocType GetInstance { get { return lazy.Value; } }

        private List<DocType> _refDocType;
        public List<DocType> refDocType
        {
            get { return _refDocType; }
            private set { _refDocType = value; }
        }

        public class DocType
        {
            public int id { get; set; }
            public string name { get; set; }
            [XmlIgnore]
            public ICommand OnClickCommand { get; set; }
        }

    }
}
