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
using com.sbh.gui.invoices.Model.DocAbstractFactory;

namespace com.sbh.gui.invoices.View
{
    /// <summary>
    /// Interaction logic for DocumentType1.xaml
    /// </summary>
    public partial class DocumentView : UserControl
    {
        private Document document;

        public DocumentView()
        {
            InitializeComponent();

            Loaded += DocumentType1View_Loaded;
        }

        private void DocumentType1View_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ViewModel.DocumentViewModel(null);
        }
    }
}
