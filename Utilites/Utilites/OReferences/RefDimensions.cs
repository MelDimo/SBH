using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace com.sbh.dll.utilites.OReferences
{
    public sealed class RefDimensions
    {
        private RefDimensions()
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
                    command.CommandText = " SELECT id, parent_id AS parentId, name, namefull, value, ref_status AS refStatus " +
                                            " FROM ref_dimensions " +
                                            " FOR XML RAW('Dimension'), ROOT('ArrayOfDimension'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        refDimension = Support.XMLToObject<List<Dimension>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private static readonly Lazy<RefDimensions> lazy = new Lazy<RefDimensions>(() => new RefDimensions());

        public static RefDimensions GetInstance { get { return lazy.Value; } }

        private List<Dimension> _refDimension;
        public List<Dimension> refDimension
        {
            get { return _refDimension; }
            private set { _refDimension = value; }
        }

        public class Dimension
        {
            public int id { get; set; }
            public int parentId { get; set; }
            public string name { get; set; }
            public string namefull { get; set; }
            public decimal value { get; set; }
            public int refStatus { get; set; }
        }
    }
}
