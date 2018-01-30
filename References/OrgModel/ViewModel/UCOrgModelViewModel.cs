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
using System.Windows.Controls;
using System.Xml;

namespace com.sbh.gui.references.orgmodel.ViewModel
{
    public class UCOrgModelViewModel : INotifyPropertyChanged
    {
        private Model.Organization _currOrganization;
        public Model.Organization CurrOrganization
        {
            get { return _currOrganization; }
            set { _currOrganization = value; }
        }

        private Model.Branch _currBranch;
        public Model.Branch CurrBranch
        {
            get { return _currBranch; }
            set { _currBranch = value; }
        }

        private Model.Unit _currUnit;
        public Model.Unit CurrUnit
        {
            get { return _currUnit; }
            set { _currUnit = value; }
        }

        private ObservableCollection<Model.Organization> _organization { get; set; }

        public ObservableCollection<Model.Organization> Organization
        {
            get { return _organization; }
            set { _organization = value; }
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
                    command.CommandText = " SELECT org.id, org.name, org.ref_status AS refStatus, " +
                                        " 	CONVERT(XML, " +
                                        " 		(SELECT b.id, b.name, b.ref_status AS refStatus, " +
                                        " 			CONVERT(XML, " +
                                        " 				(SELECT u.id, u.name, u.ref_status AS refStatus " +
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
                        _organization = Suppurt.XMLToObject<ObservableCollection<Model.Organization>>(reader.ReadOuterXml());
                    }

                    OnPropertyChanged("Organization");
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
