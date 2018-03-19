using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace com.sbh.dll.utilites.OReferences
{
    public sealed class RefCurrency
    {
        private RefCurrency()
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
                    command.CommandText = " SELECT cg.id, cg.name, " +
                                          "         CONVERT(XML, " +
                                          "             (SELECT c.id, c.nameshort, c.namefull, ref_status AS refStatus, " +
                                          "                  CONVERT(XML, " +
                                          "                     (SELECT ce.id, ce.xdate, ce.buy, ce.sale, ce.tarif, ce.isCurrent " +
                                          "                         FROM currency_exchange ce " +
                                          "                         WHERE ce.currency_group = cg.id AND ce.currency = c.id AND ce.isCurrent = 1" +
                                          "                         FOR XML RAW('CurrencyExchange'), ROOT('ArrayOfCurrencyExchange'), ELEMENTS)) " +
                                          "                 FROM currency c " +
                                          "                 FOR XML RAW('Currency'), ROOT('ArrayOfCurrency'), ELEMENTS)) " +
                                          " FROM currency_group cg " +
                                          " FOR XML RAW('CurrencyGroup'), ROOT('ArrayOfCurrencyGroup'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        Currencies = Support.XMLToObject<List<Currency>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        private static readonly Lazy<RefCurrency> lazy = new Lazy<RefCurrency>(() => new RefCurrency());

        public static RefCurrency GetInstance { get { return lazy.Value; } }

        private List<Currency> _currencies;
        public List<Currency> Currencies
        {
            get { return _currencies; }
            private set
            {
                _currencies = value;
            }
        }

        public class CurrencyGroup
        {
            public decimal id { get; set; }
            public string name { get; set; }

            public string Name { get { return name;} }

            [XmlElement("ArrayOfCurrency", typeof(ObservableCollection<Currency>))]
            public ObservableCollection<Currency> currency { get; set; }

            public CurrencyGroup()
            {
                currency = new ObservableCollection<Currency>();
            }

        }

        public class Currency
        {
            public decimal id { get; set; }
            public string nameshort { get; set; }
            public string namefull { get; set; }
            public decimal ref_status { get; set; }

            [XmlElement("ArrayOfCurrencyExchange", typeof(ObservableCollection<CurrencyExchange>))]
            public ObservableCollection<CurrencyExchange> exchange { get; set; }

            public Currency()
            {
                exchange = new ObservableCollection<CurrencyExchange>();
            }
        }

        public class CurrencyExchange
        {
            public decimal id { get; set; }
            public DateTime xdate { get; set; }
            public decimal buy { get; set; }
            public decimal sale { get; set; }
            public decimal tarif { get; set; }
            public decimal isCurrent { get; set; }
        }

        

    }
}
