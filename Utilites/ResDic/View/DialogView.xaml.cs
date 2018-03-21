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
using System.Windows.Shapes;

namespace com.sbh.dll.resdictionary.View
{
    /// <summary>
    /// Interaction logic for DialogView.xaml
    /// </summary>
    public partial class DialogView : Window
    {
        public string Header;

        public DialogView(UserControl pContent)
        {
            InitializeComponent();

            MainContainer.Content = pContent;
            DataContext = pContent.DataContext;

            Loaded += DialogView_Loaded;
            
            this.PreviewKeyDown += DialogView_PreviewKeyDown;
        }

        private void DialogView_Loaded(object sender, RoutedEventArgs e)
        {
            tbHeader.Text = Header;
        }

        private void DialogView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.DialogResult = false;
        }
    }
}
