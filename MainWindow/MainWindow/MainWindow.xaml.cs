using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace com.sbh.gui.mainwindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            tblockOutput.Text += "Starting async download\n";

            await DoDownloadAsync();

            tblockOutput.Text += "Async download started\n";
        }

        private async Task DoDownloadAsync()
        {
            var req = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            req.Method = "GET";
            var task = req.GetResponseAsync();

            var resp = (HttpWebResponse)await task;

            tblockOutput.Text += resp.Headers.ToString();
            tblockOutput.Text += "Async download completed\n";
        }

        async void DoDownloadFromAsync()
        {
            var req = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            req.Method = "GET";

            Task<WebResponse> getResponseTask = Task.Factory.FromAsync<WebResponse>(
                req.BeginGetResponse, req.EndGetResponse, null);

            var resp = (HttpWebResponse)await getResponseTask;

            string headersText = resp.Headers.ToString();
            tblockOutput.Text += headersText;
            tblockOutput.Text += "Async download completed\n";
        }
    }
}
