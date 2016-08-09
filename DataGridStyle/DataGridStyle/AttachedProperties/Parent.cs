using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DataGridStyle.AttachedProperties
{
    public static class Parent
    {
        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.RegisterAttached(
                "ZIndex",
                typeof(int),
                typeof(Parent),
                new PropertyMetadata(0, Parent.ZIndexChangedCallback)
            );

        public static void ZIndexChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(d);
            parent.SetValue(Panel.ZIndexProperty, e.NewValue);
        }

        public static void SetZIndexProperty(DependencyObject dependencyObject, int value)
        {
            dependencyObject.SetValue(Parent.ZIndexProperty, value);
        }

        public static int GetZIndexProperty(DependencyObject dependencyObject)
        {
            return (int)dependencyObject.GetValue(Parent.ZIndexProperty);
        }
    }
}
