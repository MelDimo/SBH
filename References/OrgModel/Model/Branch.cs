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
using System.Xml.Serialization;

namespace com.sbh.gui.references.orgmodel.Model
{

    public class Branch : INotifyPropertyChanged
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

        [XmlElement("ArrayOfUnit", typeof(ObservableCollection<Unit>))]
        public ObservableCollection<Unit> Unit { get; set; }

        public Branch()
        {
            Unit = new ObservableCollection<Unit>();
            AddUnitCommand = new DelegateCommand(AddUnit);
            ChangeStatusCommand = new DelegateCommand(ChangeStatus);
            SaveCommand = new DelegateCommand(Save);
            _isSelected = false;
        }

        private bool _isSelected;
        public bool isSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("isSelected");
            }
        }

        [XmlIgnore]
        public ICommand AddUnitCommand { get; private set; }
        void AddUnit(object obj)
        {
            decimal recId;

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " INSERT INTO unit(branch, name, ref_status) VALUES(@branch, @name, @ref_status); " +
                                            " SELECT SCOPE_IDENTITY();";

                    command.Parameters.Add("branch", SqlDbType.Int).Value = id;
                    command.Parameters.Add("name", SqlDbType.NVarChar).Value = "Новая торговая точка";
                    command.Parameters.Add("ref_status", SqlDbType.Int).Value = 1;

                    recId = (decimal)command.ExecuteScalar();
                }
            }

            Unit.Add(new Unit() { id = (int)recId, name = "Новая торговая точка", refStatus = 1 });
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
                    command.CommandText = " UPDATE branch SET ref_status = @refStatus " +
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
                    command.CommandText = " UPDATE branch SET name = @name" +
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
