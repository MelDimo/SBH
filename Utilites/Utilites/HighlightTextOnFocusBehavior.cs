using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace com.sbh.dll.utilites
{
    //https://rachel53461.wordpress.com/2011/11/05/automatically-selecting-textbox-text-when-focused/

    public class HighlightTextOnFocusBehavior
    {
        public static readonly DependencyProperty HighlightTextOnFocusProperty =
            DependencyProperty.RegisterAttached
            (
                "HighlightTextOnFocus",
                typeof(bool),
                typeof(HighlightTextOnFocusBehavior),
                new PropertyMetadata(false, HighlightTextOnFocusPropertyChanged)
            );

        public static bool GetHighlightTextOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(HighlightTextOnFocusProperty);
        }

        public static void SetHighlightTextOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(HighlightTextOnFocusProperty, value);
        }

        private static void HighlightTextOnFocusPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var sender = dpo as UIElement;
            if (dpo != null)
            {
                if ((bool)args.NewValue)
                {
                    sender.GotKeyboardFocus += Sender_GotKeyboardFocus;
                    sender.PreviewMouseLeftButtonDown += Sender_PreviewMouseLeftButtonDown;
                }
                else
                {
                    sender.GotKeyboardFocus -= Sender_GotKeyboardFocus;
                    sender.PreviewMouseLeftButtonDown -= Sender_PreviewMouseLeftButtonDown;
                }
            }
        }

        private static void Sender_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox tb = VisualTreeHelpers.FindAncestor<TextBox>((DependencyObject)e.OriginalSource);

            if (tb == null)
                return;

            if (!tb.IsKeyboardFocusWithin)
            {
                tb.Focus();
                e.Handled = true;
            }
        }

        private static void Sender_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }
    }
}
