using com.sbh.dll;
using com.sbh.dll.utilites;
using com.sbh.dll.utilites.OReferences;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace com.sbh.gui.references.currency.ViewModel
{
    public class SurfaceCurrencyViewModel : INotifyPropertyChanged
    {
        BackgroundWorker bgwCourses;

        public View.ItemView itemView { get; private set; }

        private Model.SimpleCurrency _curSimpleCurrency;
        public Model.SimpleCurrency CurSimpleCurrency {
            get { return _curSimpleCurrency; }
            set {
                _curSimpleCurrency = value;
                bgwCourses.RunWorkerAsync();
            }
        }

        public ObservableCollection<Model.SimpleCurrency> SimpleCurrencies { get; set; }

        public SurfaceCurrencyViewModel()
        {
            bgwCourses = new BackgroundWorker();
            bgwCourses.DoWork += BgwCourses_DoWork;
            bgwCourses.RunWorkerCompleted += BgwCourses_RunWorkerCompleted;

            collectItems();
            itemView = new View.ItemView();

            AddCourseCommand = new DelegateCommand(AddCourse);
        }

        private void BgwCourses_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnPropertyChanged("CurSimpleCurrency");
        }

        private void BgwCourses_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT id, xdate, buy, sale, tarif " +
                                            " FROM currency_exchange " +
                                            " WHERE currency_group = @currency_group AND currency = @currency " +
                                            " ORDER BY xdate DESC" +
                                            " FOR XML RAW('Courses'), ROOT('ArrayOfCourses'), ELEMENTS;";

                    command.Parameters.Add("currency_group", SqlDbType.Int).Value = CurSimpleCurrency.groupId;
                    command.Parameters.Add("currency", SqlDbType.Int).Value = CurSimpleCurrency.currencyId;

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        CurSimpleCurrency.CoursesHistory = dll.utilites.Support.XMLToObject<ObservableCollection<Model.SimpleCurrency.Courses>>(reader.ReadOuterXml());
                    }

                }
            }
        }

        public ICommand AddCourseCommand { get; private set; }
        void AddCourse(object obj)
        {
            decimal recId;
            Model.SimpleCurrency.Courses newCourse = new Model.SimpleCurrency.Courses()
            { id = 0, xdate = DateTime.Now, buy = 0, sale = 0, tarif = 1 };

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " INSERT INTO currency_exchange(currency, currency_group, xdate, buy, sale, tarif) "+
                                            " VALUES(@currency, @currency_group, @xdate, @buy, @sale, @tarif); " +
                                            " SELECT SCOPE_IDENTITY();";

                    command.Parameters.Add("currency", SqlDbType.Int).Value = CurSimpleCurrency.currencyId;
                    command.Parameters.Add("currency_group", SqlDbType.Int).Value = CurSimpleCurrency.groupId;
                    command.Parameters.Add("xdate", SqlDbType.DateTime).Value = newCourse.xdate;
                    command.Parameters.Add("buy", SqlDbType.Decimal).Value = newCourse.buy;
                    command.Parameters.Add("sale", SqlDbType.Decimal).Value = newCourse.sale;
                    command.Parameters.Add("tarif", SqlDbType.Decimal).Value = newCourse.tarif;

                    recId = (decimal)command.ExecuteScalar();
                }
            }
            newCourse.id = recId;
            CurSimpleCurrency.CoursesHistory.Add(newCourse);
        }

        public List<string> GroupName
        {
            get
            {
                return SimpleCurrencies.OrderBy(x => x.groupName).Select(x => x.groupName).Distinct().ToList();
            }
        }


        private void collectItems()
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText =   " SELECT cg.id AS groupId, " +
                                            "		 cg.name AS groupName, " +
                                            "		 c.id AS currencyId, " +
                                            "		 c.nameshort AS currencyNameShort, " +
                                            "		 c.namefull AS currencyNameFull, " +
                                            "		 ce.buy, " +
                                            "		 ce.sale " +
                                            " FROM currency_exchange ce " +
                                            " LEFT JOIN currency_group cg ON cg.id = ce.currency_group " +
                                            " LEFT JOIN currency c ON c.id = ce.currency " +
                                            " WHERE ce.xdate = (SELECT max(xdate) FROM currency_exchange WHERE currency = ce.currency AND currency_group = ce.currency_group) " +
                                            " FOR XML RAW('SimpleCurrency'), ROOT('ArrayOfSimpleCurrency'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        SimpleCurrencies = dll.utilites.Support.XMLToObject<ObservableCollection<Model.SimpleCurrency>>(reader.ReadOuterXml());
                    }
                }
            }

            if (SimpleCurrencies == null) SimpleCurrencies = new ObservableCollection<Model.SimpleCurrency>();
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
