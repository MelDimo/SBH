using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace com.sbh.gui.references.orgmodel
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UCOrgModel : UserControl
    {
        ViewModel.UCOrgModelViewModel mUCOrgModelViewModel;

        View.ViewOrganization viewOrganization;


        public UCOrgModel()
        {
            InitializeComponent();

            mUCOrgModelViewModel = new ViewModel.UCOrgModelViewModel();

            DataContext = mUCOrgModelViewModel;

            viewOrganization = new View.ViewOrganization();

            treeViewOrg.SelectedItemChanged += TreeViewOrg_SelectedItemChanged;

        }

        private void TreeViewOrg_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            dpItemInfo.Children.Clear();

            switch (e.NewValue.GetType().Name)
            {
                case "Unit":
                    break;

                case "Branch":
                    break;

                case "Organization":
                    mUCOrgModelViewModel.CurrOrganization = e.NewValue as Model.Organization;
                    dpItemInfo.Children.Add(viewOrganization);
                    break;
            }
        }
    }
}
