using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace com.sbh.dll.utilites.OReferences
{
    public sealed class RefItem
    {
        private RefItem()
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
                    command.CommandText = " SELECT id, name, ref_dimensions AS refDimensions " +
                                            " FROM item " +
                                            " FOR XML RAW('Item'), ROOT('ArrayOfItem'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        refItem = Support.XMLToObject<List<Item>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private static readonly Lazy<RefItem> lazy = new Lazy<RefItem>(() => new RefItem());

        public static RefItem GetInstance { get { return lazy.Value; } }

        private List<Item> _refItem;
        public List<Item> refItem
        {
            get { return _refItem; }
            private set { _refItem = value; }
        }

        public class Item
        {
            public int id { get; set; }
            public string name { get; set; }
            public int refDimensions { get; set; }
        }
    }
}
