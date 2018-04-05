using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace com.sbh.dll.utilites.OReferences
{
    public sealed class RefDocMap
    {
        private RefDocMap()
        {
            reload();
        }

        public void reload()
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT dm.ref_docType_source AS refDocTypeSource, dm.ref_docType_receiver AS refDocTypeReceiver, rdt.name AS refDocTypeReceiverName " +
                                            " FROM document_map dm " +
                                            " INNER JOIN dbo.ref_docType rdt ON rdt.id = dm.ref_docType_receiver " +
                                            " FOR XML RAW('DocMap'), ROOT('ArrayOfDocMap'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        refDocMap = Support.XMLToObject<List<DocMap>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private static readonly Lazy<RefDocMap> lazy = new Lazy<RefDocMap>(() => new RefDocMap());

        public static RefDocMap GetInstance { get { return lazy.Value; } }

        private List<DocMap> _refDocMap;
        public List<DocMap> refDocMap
        {
            get { return _refDocMap; }
            set { _refDocMap = value; }
        }

        public class DocMap
        {
            public decimal refDocTypeSource { get; set; }
            public decimal refDocTypeReceiver { get; set; }
            public string refDocTypeReceiverName { get; set; }

            [XmlIgnore]
            public ICommand OnClickCommand { get; set; }
        }
    }
}
