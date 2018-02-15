using com.sbh.dll;
using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace com.sbh.gui.invoices.Model
{
    public class Filter
    {
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public List<DocType> docType { get; set; }

        public Filter()
        {
            dateStart = DateTime.Now.AddDays(-7);
            dateEnd = DateTime.Now;
            docType = docTypeCollect();
        }

        public class DocType
        {
            public int id { get; set; }
            public string name { get; set; }
            public bool isSelected { get; set; }
        }

        private List<DocType> docTypeCollect()
        {
            List<DocType> result = new List<DocType>();

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT id, name, 1 As isSelected " +
                                            " FROM ref_docType " +
                                            " FOR XML RAW('DocType'), ROOT('ArrayOfDocType'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        result = Support.XMLToObject<List<DocType>>(reader.ReadOuterXml());
                    }
                }
            }

            return result;
        }
    }
}
