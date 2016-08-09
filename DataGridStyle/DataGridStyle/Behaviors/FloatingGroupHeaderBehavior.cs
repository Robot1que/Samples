using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using System.Windows.Interactivity;

namespace DataGridStyle.Behaviors
{
    public class FloatingGroupHeaderBehavior : Behavior<ContentPresenter>
    {
        public static readonly DependencyProperty ScrollableElementProperty;
        public FrameworkElement ScrollableElement
        {
            get
            {
                return (FrameworkElement)this.GetValue(FloatingGroupHeaderBehavior.ScrollableElementProperty);
            }
            set
            {
                this.SetValue(FloatingGroupHeaderBehavior.ScrollableElementProperty, value);
            }
        }

        public static readonly DependencyProperty VerticalOffsetProperty;
        public double VerticalOffset
        {
            get
            {
                return (double)this.GetValue(FloatingGroupHeaderBehavior.VerticalOffsetProperty);
            }
            set
            {
                this.SetValue(FloatingGroupHeaderBehavior.VerticalOffsetProperty, value);
            }
        }

        public static readonly DependencyProperty FloatingElementHostHeightProperty;
        public double FloatingElementHostHeight
        {
            get
            {
                return (double)this.GetValue(FloatingGroupHeaderBehavior.FloatingElementHostHeightProperty);
            }
            set
            {
                this.SetValue(FloatingGroupHeaderBehavior.FloatingElementHostHeightProperty, value);
            }
        }

        static FloatingGroupHeaderBehavior()
        {
            FloatingGroupHeaderBehavior.ScrollableElementProperty = 
                DependencyProperty.Register(
                    "ScrollableElement",
                    typeof(FrameworkElement),
                    typeof(FloatingGroupHeaderBehavior)
                );

            FloatingGroupHeaderBehavior.VerticalOffsetProperty =
                DependencyProperty.Register(
                    "VerticalOffset",
                    typeof(double),
                    typeof(FloatingGroupHeaderBehavior),
                    new PropertyMetadata(FloatingGroupHeaderBehavior.VerticalOffsetProperty_Changed)
                );

            FloatingGroupHeaderBehavior.FloatingElementHostHeightProperty =
                DependencyProperty.Register(
                    "FloatingElementHostHeight",
                    typeof(double),
                    typeof(FloatingGroupHeaderBehavior)
                );
        }

        private static void VerticalOffsetProperty_Changed(
            DependencyObject sender, 
            DependencyPropertyChangedEventArgs args)
        {
            var floatingGroupHeaderBehavior = (FloatingGroupHeaderBehavior)sender;

            var floatingElement = floatingGroupHeaderBehavior.AssociatedObject;
            var scrollableElement = floatingGroupHeaderBehavior.ScrollableElement;
            var floatingElementHostHeight = floatingGroupHeaderBehavior.FloatingElementHostHeight;

            var transform = (MatrixTransform)floatingElement.TransformToVisual(scrollableElement);

            var translateTransformY = (double)args.NewValue - transform.Matrix.OffsetY;

            var translateTransform = floatingElement.RenderTransform as TranslateTransform;
            if (translateTransform == null)
            {
                translateTransform = new TranslateTransform();
                floatingElement.RenderTransform = translateTransform;
            }

            translateTransformY += translateTransform.Y;

            var offsetMin = 0d;
            var offsetMax = floatingElementHostHeight - floatingElement.ActualHeight;

            translateTransformY = Math.Max(translateTransformY, offsetMin);
            translateTransformY = Math.Min(translateTransformY, offsetMax);

            translateTransform.Y = translateTransformY;
        }
    }
}
