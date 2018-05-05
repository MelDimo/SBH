using com.sbh.dll;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace com.sbh.gui.unitsbrowser.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            if (GValues.IsUseAnimation)
                Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            TranslateTransform tt = new TranslateTransform(-this.ActualWidth, 0);
            this.RenderTransform = tt;
            this.Opacity = 1;

            Window parentWindow = Window.GetWindow(this);
            DoubleAnimation animation = new DoubleAnimation
            {
                From = -this.ActualWidth,
                To = 0
            };
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UserControl.RenderTransform).(TranslateTransform.X)"));
            animation.Duration = TimeSpan.FromSeconds(0.3);
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Begin();

        }
    }
}
