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

namespace com.sbh.gui.references.orgmodel.View
{
    /// <summary>
    /// Interaction logic for ViewOrganization.xaml
    /// </summary>
    public partial class ViewOrganization : UserControl
    {

        public ViewOrganization()
        {
            InitializeComponent();

            tbRefStatus.MouseUp += TbRefStatus_MouseUp;
        }

        private void TbRefStatus_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //switch (mUCOrgModelViewModel.CurrOrganization.refStatus)
            //{
            //    case 1:
            //        mUCOrgModelViewModel.CurrOrganization.refStatus = 2;
            //        break;

            //    case 2:
            //        mUCOrgModelViewModel.CurrOrganization.refStatus = 1;
            //        break;
            //}
        }
    }
}
