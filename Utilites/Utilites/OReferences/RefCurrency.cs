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
            //reload();
            reloadLite();
        }

        #region Большая модель

        public void reload()
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;

 //   SELECT ce.id, c.nameshort AS name, cg.name AS namegroup, ce.buy, ce.sale
 //   FROM dbo.currency_exchange ce

 //   INNER JOIN dbo.currency c ON c.id = ce.currency

 //   INNER JOIN dbo.currency_group cg ON cg.id = ce.currency_group

 //   WHERE xdate = (SELECT max(xdate) FROM currency_exchange WHERE currency = ce.currency AND currency_group = ce.currency_group)
	//ORDER BY c.nameshort

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
                        CurrenciesLite = Support.XMLToObject<List<CurrencyLite>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        #endregion

        #region Простая модель для справочника, combobox-a

        public void reloadLite()
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT id, nameshort AS name " +
                                          " FROM currency " +
                                          " WHERE ref_status = 1 " +
                                          " FOR XML RAW('CurrencyLite'), ROOT('ArrayOfCurrencyLite'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        CurrenciesLite = Support.XMLToObject<List<CurrencyLite>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        #endregion


        private static readonly Lazy<RefCurrency> lazy = new Lazy<RefCurrency>(() => new RefCurrency());

        public static RefCurrency GetInstance { get { return lazy.Value; } }

        private List<CurrencyLite> _currenciesLite;
        public List<CurrencyLite> CurrenciesLite
        {
            get { return _currenciesLite; }
            private set
            {
                _currenciesLite = value;
            }
        }

        #region Простая модель для справочника, combobox-a

        public class CurrencyLite
        {
            public decimal id { get; set; }
            public string name { get; set; }
        }

        #endregion

        #region Большая модель

        //public class CurrencyGroup
        //{
        //    public decimal id { get; set; }
        //    public string name { get; set; }

        //    public string Name { get { return name;} }

        //    [XmlElement("ArrayOfCurrency", typeof(ObservableCollection<Currency>))]
        //    public ObservableCollection<Currency> currency { get; set; }

        //    public CurrencyGroup()
        //    {
        //        currency = new ObservableCollection<Currency>();
        //    }

        //}

        //public class Currency
        //{
        //    public decimal id { get; set; }
        //    public string nameshort { get; set; }
        //    public string namefull { get; set; }
        //    public decimal ref_status { get; set; }

        //    [XmlElement("ArrayOfCurrencyExchange", typeof(ObservableCollection<CurrencyExchange>))]
        //    public ObservableCollection<CurrencyExchange> exchange { get; set; }

        //    public Currency()
        //    {
        //        exchange = new ObservableCollection<CurrencyExchange>();
        //    }
        //}

        //public class CurrencyExchange
        //{
        //    public decimal id { get; set; }
        //    public DateTime xdate { get; set; }
        //    public decimal buy { get; set; }
        //    public decimal sale { get; set; }
        //    public decimal tarif { get; set; }
        //    public decimal isCurrent { get; set; }
        //}

        #endregion





    }
}
