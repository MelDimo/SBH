using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace com.sbh.dll.utilites.OReferences
{
    public sealed class RefRecipient
    {
        private RefRecipient()
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
                    command.CommandText =   " SELECT u.id, o.name + ' / ' + b.name as Name, u.name, u.ref_status AS refStatus, u.isPOS, u.isDepot " +
                                            " FROM unit u " +
                                            " INNER JOIN branch b ON b.id = u.branch " +
                                            " INNER JOIN dbo.organization o ON o.id = b.organization " +
                                            " FOR XML RAW('Recipient'), ROOT('ArrayOfRecipient'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        Recipients = Support.XMLToObject<List<Recipient>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private static readonly Lazy<RefRecipient> lazy = new Lazy<RefRecipient>(() => new RefRecipient());

        public static RefRecipient GetInstance { get { return lazy.Value; } }

        private List<Recipient> _recipients;
        public List<Recipient> Recipients
        {
            get { return _recipients; }
            private set
            {
                _recipients = value;
            }
        }

        public class Recipient
        {
            public decimal id { get; set; }
            public string Name { get; set; }
            public string name { get; set; }
            public decimal refStatus { get; set; }
            public decimal isPOS { get; set; }
            public decimal isDepot { get; set; }
        }
    }
}
