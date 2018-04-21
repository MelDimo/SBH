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
    /// Interaction logic for DocumentJournalView.xaml
    /// </summary>
    public partial class DocumentJournalView : UserControl
    {
        public DocumentJournalView()
        {
            InitializeComponent();
        }

        private void treeview_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (DataContext != null)
            {
                ViewModel.DocumentJournalViewModel viewModel = DataContext as ViewModel.DocumentJournalViewModel;
                viewModel.DataModel.CurDocument = (Model.Document)((TreeView)sender).SelectedItem;
                viewModel.showDoc();
            }
        }

        private void treeview_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Model.Document doc = ((TreeView)sender).SelectedItem as Model.Document;

            if (doc != null) (DataContext as ViewModel.DocumentJournalViewModel).DataModel.CurDocument = doc;
        }
    }
}
