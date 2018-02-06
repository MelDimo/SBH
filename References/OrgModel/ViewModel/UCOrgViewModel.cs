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
using System.Windows.Input;
using System.Xml;

namespace com.sbh.gui.references.orgmodel.ViewModel
{
    public class UCOrgViewModel : INotifyPropertyChanged
    {
        private UserControl organizationView;
        private UserControl branchView;
        private UserControl unitView;

        private Model.Organization _currOrganization;
        public Model.Organization CurrOrganization
        {
            get { return _currOrganization; }
            set {
                _currOrganization = value;
                OnPropertyChanged("CurrOrganization");
            }
        }

        private Model.Branch _currBranch;
        public Model.Branch CurrBranch
        {
            get { return _currBranch; }
            set
            {
                _currBranch = value;
                OnPropertyChanged("CurrBranch");
            }
        }

        private Model.Unit _currUnit;
        public Model.Unit CurrUnit
        {
            get { return _currUnit; }
            set
            {
                _currUnit = value;
                OnPropertyChanged("CurrUnit");
            }
        }

        private UserControl _currControl;
        public UserControl CurrControl
        {
            get { return _currControl; }
            set
            {
                _currControl = value;
                OnPropertyChanged("CurrControl");
            }
        }

        public void SelectObject(object pSelectedItem)
        {
            foreach (Model.Organization org in Organizations)
            {
                org.isSelected = false;
                foreach (Model.Branch branch in org.Branch)
                {
                    branch.isSelected = false;
                }
            }

            switch (pSelectedItem.GetType().Name)
            {
                case "Unit":
                    CurrUnit = pSelectedItem as Model.Unit;
                    CurrControl = unitView;
                    break;

                case "Branch":
                    CurrBranch = pSelectedItem as Model.Branch;
                    CurrBranch.isSelected = true;
                    CurrControl = branchView;
                    break;

                case "Organization":
                    CurrOrganization = pSelectedItem as Model.Organization;
                    CurrOrganization.isSelected = true;
                    CurrControl = organizationView;
                    break;
            }
        }

        private ObservableCollection<Model.Organization> _organizations { get; set; }
        public ObservableCollection<Model.Organization> Organizations
        {
            get { return _organizations; }
            private set { _organizations = value; }
        }

        public UCOrgViewModel()
        {
            Organizations = Model.Organization.CollectOrganization();
            organizationView = new View.OrganizationView();
            branchView = new View.BranchView();
            unitView = new View.UnitView();
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
