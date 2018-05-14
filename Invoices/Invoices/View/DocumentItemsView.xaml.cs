using com.sbh.dll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for DocumentItemsView.xaml
    /// </summary>
    public partial class DocumentItemsView : UserControl
    {
        
        public DocumentItemsView()
        {
            InitializeComponent();

            IsVisibleChanged += DocumentItemsView_IsVisibleChanged;
        }

        private void DocumentItemsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                GValues.MainWindow.PreviewKeyDown += DocumentItemsView_PreviewKeyDown;
            }
            else
            {
                GValues.MainWindow.PreviewKeyDown -= DocumentItemsView_PreviewKeyDown;
            }
        }

        private void DocumentItemsView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert)
                (this.DataContext as com.sbh.gui.invoices.ViewModel.DocumentViewModel).AddItemOnClickCommand.Execute(null);
        }
    }
}
