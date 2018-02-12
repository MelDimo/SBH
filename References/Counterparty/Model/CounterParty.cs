using com.sbh.dll;
using com.sbh.dll.utilites;
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
using System.Xml.Serialization;

namespace com.sbh.gui.references.counterparty.Model
{
    public class Counterparty : INotifyPropertyChanged
    {
        public decimal id { get; set; }

        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        private string _description;
        public string description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("description");
            }
        }

        private string _groupname;
        public string groupname
        {
            get { return _groupname; }
            set
            {
                _groupname = value;
                OnPropertyChanged("groupname");
            }
        }

        public string Name { get { return groupname; } }

        private int _refStatus;
        public int refStatus
        {
            get { return _refStatus; }
            set
            {
                _refStatus = value;
                OnPropertyChanged("refStatus");
            }
        }

        public Counterparty()
        {
            ChangeStatusCommand = new DelegateCommand(ChangeStatus);
            SaveCommand = new DelegateCommand(Save);
        }

        public static ObservableCollection<Counterparty> CollectCounterParty()
        {
            ObservableCollection<Counterparty> result = new ObservableCollection<Counterparty>();

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
                        result = Support.XMLToObject<ObservableCollection<Counterparty>>(reader.ReadOuterXml());
                    }
                }
            }
            return result;
        }

        [XmlIgnore]
        public ICommand ChangeStatusCommand { get; private set; }
        void ChangeStatus(object obj)
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " UPDATE counterparty SET ref_status = @refStatus " +
                                            " WHERE id = @id;";

                    command.Parameters.Add("id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("refStatus", SqlDbType.Int).Value = refStatus == 1 ? 2 : 1;

                    command.ExecuteNonQuery();
                }
            }

            refStatus = refStatus == 1 ? 2 : 1;
        }

        [XmlIgnore]
        public ICommand SaveCommand { get; private set; }
        void Save(object obj)
        {
            var values = (object[])obj;
            string xName = values[0] as string;
            string xGroupname = values[1] as string;
            string xDescription = values[2] as string;

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText =   " UPDATE counterparty " + 
                                            " SET name = @name, " +
                                            " groupname = @groupname, " +
                                            " [description] = @description " +
                                            " WHERE id = @id; ";

                    command.Parameters.Add("id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("name", SqlDbType.NVarChar).Value = xName;
                    command.Parameters.Add("groupname", SqlDbType.NVarChar).Value = xGroupname;
                    command.Parameters.Add("description", SqlDbType.NVarChar).Value = xDescription;

                    command.ExecuteNonQuery();
                }
            }

            name = xName;
            groupname = xGroupname;
            description = xDescription;
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
