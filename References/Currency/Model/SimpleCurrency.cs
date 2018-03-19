using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbh.gui.references.currency.Model
{
    public class SimpleCurrency
    {
        public int groupId { get; set; }

        public string groupName { get; set; }
        public string Name { get { return groupName; } }

        public int currencyId { get; set; }
        public string currencyNameShort { get; set; }
        public string currencyNameFull { get; set; }

        public decimal buy { get; set; }
        public decimal sale { get; set; }

        public string CoursesExpanderHeader {
            get { return string.Format("Текущий курс : покупка {0}, продажа {1}", buy, sale); }
        }

        public ObservableCollection<Courses> CoursesHistory { get; set; }

        public SimpleCurrency()
        {
            CoursesHistory = new ObservableCollection<Courses>();
        }

        public class Courses : INotifyPropertyChanged
        {
            public decimal id { get; set; }
            public DateTime xdate { get; set; }

            private decimal _buy;
            public decimal buy
            {
                get { return _buy; }
                set
                {
                    if (value == _buy) return;
                    _buy = value;
                    OnPropertyChanged("buy");
                }
            }
            public decimal sale { get; set; }
            public decimal tarif { get; set; }

            public Courses()
            {
                xdate = DateTime.Now;
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
}
