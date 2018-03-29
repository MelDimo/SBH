using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace com.sbh.dll.utilites
{
    // http://www.codewrecks.com/blog/index.php/2010/12/22/mvvm-and-focus-listview-element-with-complex-template/

    public class SelectListViewItemOnFocusBehaviour
    {

        public static readonly DependencyProperty SelectListViewItemOnFocusProperty =
        DependencyProperty.RegisterAttached
        (
        "SelectListViewItemOnFocus",
        typeof(bool),
        typeof(SelectListViewItemOnFocusBehaviour),
        new UIPropertyMetadata(false, OnSelectListViewItemOnFocusPropertyChanged)
        );

        public static bool GetSelectListViewItemOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectListViewItemOnFocusProperty);
        }

        public static void SetSelectListViewItemOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectListViewItemOnFocusProperty, value);
        }

        private static void OnSelectListViewItemOnFocusPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = dpo as FrameworkElement;
            if (element != null)
            {
                if ((bool)args.NewValue)
                {
                    element.PreviewGotKeyboardFocus += OnControlPreviewGotKeyboardFocus;
                }
                else
                {
                    element.PreviewGotKeyboardFocus -= OnControlPreviewGotKeyboardFocus;
                }
            }
        }

        private static void OnControlPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DependencyObject p = sender as DependencyObject;
            while (p != null && !(p is ListBoxItem))
            {
                p = VisualTreeHelper.GetParent(p);
            }

            if (p == null)
                return;

            ((ListBoxItem)p).IsSelected = true;
        }

    }
}
