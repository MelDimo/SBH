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
    /// Interaction logic for DocumentType5View.xaml
    /// </summary>
    public partial class DocumentType5View : UserControl
    {
        public DocumentType5View()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
                // sizing of the element.
                this.Measure(pageSize);
                this.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                printDialog.PrintVisual(this, "Распечатываем элемент Canvas");
            }
        }
    }
}
