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
        public ObservableCollection<Counterparty> result = new ObservableCollection<Counterparty>();

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
                        refCounterParty = Support.XMLToObject<List<Counterparty>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private static readonly Lazy<RefCounterParty> lazy = new Lazy<RefCounterParty>(() => new RefCounterParty());

        public static RefCounterParty GetInstance { get { return lazy.Value; } }

        private List<Counterparty> _refCounterParty;
        public List<Counterparty> refCounterParty
        {
            get { return _refCounterParty; }
            private set 
            {
                _refCounterParty = value;
            }
        }

        public class Counterparty
        {
            public decimal id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string groupname { get; set; }
            public int refStatus { get; set; }
        }

    }
}
