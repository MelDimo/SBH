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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace com.sbh.gui.references.orgmodel.Model
{
    public class Organization : INotifyPropertyChanged
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
            get {
                return _refStatus;
                
            }
            set
            {
                _refStatus = value;
                OnPropertyChanged("refStatus");
            }
        }

        [XmlElement("ArrayOfBranch", typeof(ObservableCollection<Branch>))]
        public ObservableCollection<Branch> Branch { get; set; }

        public Organization()
        {
            Branch = new ObservableCollection<Branch>();
            AddBranchCommand = new DelegateCommand(AddBranch);
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
        public ICommand AddBranchCommand { get; private set; }
        void AddBranch(object obj)
        {
            decimal recId;

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " INSERT INTO branch(organization, name, ref_status) VALUES(@organization, @name, @ref_status); " +
                                            " SELECT SCOPE_IDENTITY();";

                    command.Parameters.Add("organization", SqlDbType.Int).Value = id;
                    command.Parameters.Add("name", SqlDbType.NVarChar).Value = "Новая группа";
                    command.Parameters.Add("ref_status", SqlDbType.Int).Value = 1;

                    recId = (decimal)command.ExecuteScalar();
                }
            }

            Branch.Add(new Branch() { id = (int)recId, name = "Новая группа", refStatus = 1 });
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
                    command.CommandText = " UPDATE organization SET ref_status = @refStatus " +
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
                    command.CommandText = " UPDATE organization SET name = @name" +
                                            " WHERE id = @id;";

                    command.Parameters.Add("id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("name", SqlDbType.NVarChar).Value = obj;

                    command.ExecuteNonQuery();
                }
            }

            name = obj as string;
        }
        

        public static ObservableCollection<Organization> CollectOrganization()
        {
            ObservableCollection<Organization> result = new ObservableCollection<Organization>();

            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT org.id, org.name, org.ref_status AS refStatus, " +
                                        " 	CONVERT(XML, " +
                                        " 		(SELECT b.id, b.name, b.ref_status AS refStatus, " +
                                        " 			CONVERT(XML, " +
                                        " 				(SELECT u.id, u.name, u.ref_status AS refStatus, isPOS, isDepot " +
                                        " 				FROM unit u " +
                                        " 				WHERE u.branch = b.id " +
                                        " 				FOR XML RAW('Unit'), ROOT('ArrayOfUnit'), ELEMENTS)) " +
                                        " 		FROM branch b " +
                                        " 		WHERE b.organization = org.id " +
                                        " 		FOR XML RAW('Branch'), ROOT('ArrayOfBranch'), ELEMENTS)) " +
                                        " FROM organization org " +
                                        " FOR XML RAW('Organization'), ROOT('ArrayOfOrganization'), ELEMENTS ";

                    XmlReader reader = command.ExecuteXmlReader();
                    while (reader.Read())
                    {
                        result = Support.XMLToObject<ObservableCollection<Organization>>(reader.ReadOuterXml());
                    }
                    //OnPropertyChanged("Organization");
                }
            }
            return result;
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
