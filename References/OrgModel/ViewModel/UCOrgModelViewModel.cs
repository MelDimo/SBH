using com.sbh.dll;
using com.sbh.dll.utilites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace com.sbh.gui.references.orgmodel.ViewModel
{
    public class UCOrgModelViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Model.Organization> _organization { get; set; }

        public ObservableCollection<Model.Organization> Organization
        {
            get { return _organization; }
            set
            {
                _organization = value;
                OnPropertyChanged("Organization");
            }
        }

        public UCOrgModelViewModel()
        {
            collectOrganization();
        }

        public void collectOrganization()
        {
            using (SqlConnection con = new SqlConnection(GValues.connString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = " SELECT org.id, org.name, org.ref_status, " +
                                        " 	CONVERT(XML, " +
                                        " 		(SELECT b.id, b.name, b.ref_status, " +
                                        " 			CONVERT(XML, " +
                                        " 				(SELECT u.id, u.name, u.ref_status " +
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
                        Organization = Suppurt.XMLToObject<ObservableCollection<Model.Organization>>(reader.ReadOuterXml());
                    }
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
