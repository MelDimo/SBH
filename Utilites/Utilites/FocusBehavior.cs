using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace com.sbh.dll.utilites
{
    public class FocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.GotFocus += (sender, args) => IsFocused = true;
            AssociatedObject.LostFocus += (sender, args) => IsFocused = false;
            AssociatedObject.TextChanged += (sender, args) =>
             {
                 if (!IsFocused)
                 {
                     AssociatedObject.Focus();
                     AssociatedObject.Select(AssociatedObject.Text.Length, 0);
                 }
             };
            base.OnAttached();
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false));

        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            set { SetValue(IsFocusedProperty, value); }
        }
    }
}
