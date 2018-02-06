using com.sbh.dll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace com.sbh.gui.references.orgmodel.Model
{
    
    public class Unit : INotifyPropertyChanged
    {
        public int id { get; set; }

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

        public Unit()
        {
            SaveCommand = new DelegateCommand(Save);
            ChangeStatusCommand = new DelegateCommand(ChangeStatus);
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
                    command.CommandText = " UPDATE unit SET ref_status = @refStatus " +
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
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " UPDATE unit SET name = @name" +
                                            " WHERE id = @id;";

                    command.Parameters.Add("id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("name", SqlDbType.NVarChar).Value = obj;

                    command.ExecuteNonQuery();
                }
            }

            name = obj as string;
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
