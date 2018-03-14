using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace com.sbh.dll.utilites.OReferences
{
    public sealed class RefCounterParty
    {
        private RefCounterParty()
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
                    command.CommandText = " SELECT id, name, [description], groupname, ref_status AS refStatus " +
                                            " FROM counterparty " +
                                            " FOR XML RAW('Counterparty'), ROOT('ArrayOfCounterparty'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        CounterPartys = Support.XMLToObject<List<Counterparty>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private static readonly Lazy<RefCounterParty> lazy = new Lazy<RefCounterParty>(() => new RefCounterParty());

        public static RefCounterParty GetInstance { get { return lazy.Value; } }

        private List<Counterparty> _counterPartys;
        public List<Counterparty> CounterPartys
        {
            get { return _counterPartys; }
            private set 
            {
                _counterPartys = value;
            }
        }

        public class Counterparty
        {
            public decimal id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string groupname { get; set; }
            public int refStatus { get; set; }
            public string Name { get { return groupname; } }
        }

    }
}
