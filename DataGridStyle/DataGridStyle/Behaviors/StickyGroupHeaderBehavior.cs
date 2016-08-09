using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace DataGridStyle.Behaviors
{
    public class StickyGroupHeaderBehavior : Behavior<VirtualizingStackPanel>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var scrollViewer = this.VisualTreeParentGet<ScrollViewer>(this.AssociatedObject);

            var adorner = new FloatingGroupHeaderAdorner(this.AssociatedObject, scrollViewer);
            AdornerLayer.GetAdornerLayer(this.AssociatedObject).Add(adorner);
        }

        private T VisualTreeParentGet<T>(DependencyObject child) where T: DependencyObject
        {
            T parent = null;

            while(parent == null && child != null)
            {
                child = VisualTreeHelper.GetParent(child);
                parent = child as T;
            }

            return parent;
        }
    }

    public interface IFloatingGroupHeaderTextProvider
    {
        string HeaderText { get; }

        bool IsHeader { get; }
    }

    public class FloatingGroupHeaderAdorner : Adorner
    {
        private readonly ScrollViewer _scrollViewer;

        public FloatingGroupHeaderAdorner(UIElement adornedElement, ScrollViewer scrollViewer)
            : base(adornedElement)
        {
            if (scrollViewer == null)
            {
                throw new ArgumentNullException(nameof(scrollViewer));
            }

            this._scrollViewer = scrollViewer;

            this._scrollViewer.ScrollChanged += this.ScrollViewer_ScrollChanged;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            
            var isFirstItem = true;
            var adornedItems = this.AdornedItemsGet();
            var headers = adornedItems.Where(item => item.IsHeader);

            foreach(var header in headers)
            {
                var point = header.Element.TranslatePoint(new Point(0, 0), this.AdornedElement);
                if (point.Y > 0)
                {
                    if (isFirstItem)
                    {
                        isFirstItem = false;

                        var idx = Array.IndexOf(adornedItems, header);
                        if (idx > 0)
                        {
                            this.HeaderDraw(
                                drawingContext, 
                                new Rect(
                                    new Point(0, Math.Min(0, point.Y - header.Element.ActualHeight)), 
                                    new Size(header.Element.ActualWidth, header.Element.ActualHeight)
                                ), 
                                adornedItems[idx - 1].HeaderText
                            );
                        }
                    }
                    
                    var bounds = 
                        new Rect(point, new Size(header.Element.ActualWidth, header.Element.ActualHeight));
                    this.HeaderDraw(drawingContext, bounds, header.HeaderText);
                }
            }
        }

        private void HeaderDraw(DrawingContext drawingContext, Rect bounds, string text)
        {
            drawingContext.DrawRectangle(
                new SolidColorBrush(Colors.AntiqueWhite) { Opacity = 0.7d },
                new Pen(Brushes.AntiqueWhite, 0),
                bounds
            );

            var formattedText =
                new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(
                        new FontFamily("Segoe UI"),
                        FontStyles.Normal,
                        FontWeights.Bold,
                        FontStretches.Normal
                    ),
                    16d,
                    Brushes.Black
                );
            drawingContext.DrawText(
                formattedText, 
                new Point(
                    bounds.X + (bounds.Width - formattedText.Width) / 2, 
                    bounds.Y + (bounds.Height - formattedText.Height) / 2
                )
            );
        }

        private AdornedItem[] AdornedItemsGet()
        {
            var panel = (Panel)this.AdornedElement;
            return 
                panel.Children.Cast<FrameworkElement>()
                    .Select(item => new AdornedItem(item))
                    .ToArray();
        }

        private class AdornedItem
        {
            public FrameworkElement Element { get; }

            public bool IsHeader { get; } = false;

            public string HeaderText { get; } = null;

            public AdornedItem(FrameworkElement element)
            {
                this.Element = element;

                var headerProvider = element.DataContext as IFloatingGroupHeaderTextProvider;
                if (headerProvider != null)
                {
                    this.IsHeader = headerProvider.IsHeader;
                    this.HeaderText = headerProvider.HeaderText;
                }
            }
        }

    }
}
