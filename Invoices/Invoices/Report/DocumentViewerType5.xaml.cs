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

namespace com.sbh.gui.invoices.Report
{
    /// <summary>
    /// Interaction logic for DocumentViewer.xaml
    /// </summary>
    public partial class DocumentViewerType5 : UserControl
    {
        public DocumentViewerType5(object[] pParam)
        {
            InitializeComponent();

            fromName.Text = string.Format("Поставщик: {0}", pParam[0].ToString());
            toName.Text = string.Format("Получатель: {0}", pParam[1].ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = flowDocumentScrollViewer.Document;
                doc.PageHeight = printDialog.PrintableAreaHeight;
                doc.PageWidth = printDialog.PrintableAreaWidth;
                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)doc).DocumentPaginator,
                    "A Flow Document");
            }
        }
    }
}
