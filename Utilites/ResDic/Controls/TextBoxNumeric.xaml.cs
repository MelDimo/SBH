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

namespace com.sbh.dll.resdictionary.Controls
{
    /// <summary>
    /// Interaction logic for TextBoxNumeric.xaml
    /// </summary>
    public partial class TextBoxNumeric : TextBox
    {

        #region MaxValue property

        /// <summary>
        /// Maximum value to input. By default - decimal.MaxValue
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(decimal), typeof(TextBoxNumeric),
                new PropertyMetadata(decimal.MaxValue));

        /// <summary>
        /// Minimum value to input. By default - decimal.MinValue
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(decimal), typeof(TextBoxNumeric),
                new PropertyMetadata(decimal.MinValue));

        public decimal MaxValue
        {
            get { return (decimal)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public decimal MinValue
        {
            get { return (decimal)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        #endregion

        public TextBoxNumeric()
        {

            InitializeComponent();

            PreviewTextInput += TextBoxNumeric_PreviewTextInput;
        }

        private bool IsValid(string str)
        {
            decimal i;

            return decimal.TryParse(str, out i) && i >= MinValue && i <= MaxValue;
        }

        private void TextBoxNumeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((TextBox)sender).SelectedText.Length > 0) ((TextBox)sender).Text = string.Empty;

            e.Handled = !IsValid(((TextBox)sender).Text + e.Text);
        }
    }
}
