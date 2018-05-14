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

namespace com.sbh.gui.invoices.View
{
    /// <summary>
    /// Interaction logic for SurfaceControlView.xaml
    /// </summary>
    public partial class SurfaceControlView : UserControl
    {
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(SurfaceControlView), new PropertyMetadata(null));

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        public SurfaceControlView()
        {
            InitializeComponent();
            Header.Visibility = ShowHeader ? Visibility.Collapsed : Visibility.Collapsed;
        }
    }
}
