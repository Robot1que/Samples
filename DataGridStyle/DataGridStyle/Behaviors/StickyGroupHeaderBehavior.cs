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

    public interface IFloatingGroupHeaderInfo
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

            var element = (FrameworkElement)this.AdornedElement;
            var renderInfos = this.FloatingGroupHeaderRenderInfosGet();

            foreach(var renderInfo in renderInfos)
            {
                this.HeaderDraw(
                    drawingContext,
                    new Rect(renderInfo.Location, new Size(element.ActualWidth, 54)),
                    renderInfo.HeaderText
                );
            }
        }

        private Point LocationGet(AdornedItem item)
        {
            return item.Element.TranslatePoint(new Point(0, 0), this.AdornedElement);
        }

        private FloatingGroupHeaderRenderInfo[] FloatingGroupHeaderRenderInfosGet()
        {
            var renderInfoList = new List<FloatingGroupHeaderRenderInfo>();
            var panel = (Panel)this.AdornedElement;

            var adornedItems = 
                panel.Children.Cast<FrameworkElement>()
                    .Select(item => new AdornedItem(item))
                    .ToArray();

            var headers = 
                adornedItems
                    .Where(item => item.GroupHeaderInfo.IsHeader)
                    .Select(item => new { Item = item, Location = this.LocationGet(item) })
                    .Where(item => item.Location.Y >= 0)
                    .ToArray();

            if (!headers.Any())
            {
                renderInfoList.Add(
                    new FloatingGroupHeaderRenderInfo(
                        new Point(0, 0), 
                        adornedItems.Last().GroupHeaderInfo.HeaderText
                    )
                );
            }
            else
            {
                var firstHeader = headers.First();

                if (firstHeader.Location.Y > 0)
                {
                    var idx = Array.IndexOf(adornedItems, firstHeader.Item);
                    if (idx > 0)
                    {
                        var adornedItem = adornedItems[idx-1];
                        renderInfoList.Add(
                            new FloatingGroupHeaderRenderInfo(
                                new Point(0, Math.Min(0, firstHeader.Location.Y - 54)),
                                adornedItem.GroupHeaderInfo.HeaderText
                            )
                        );
                    }
                }

                foreach (var header in headers)
                {
                    renderInfoList.Add(
                        new FloatingGroupHeaderRenderInfo(
                            header.Location,
                            header.Item.GroupHeaderInfo.HeaderText
                        )
                    );
                }
            }

            return renderInfoList.ToArray();
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

        private class AdornedItem
        {
            public UIElement Element { get; }

            public IFloatingGroupHeaderInfo GroupHeaderInfo { get; }

            public AdornedItem(FrameworkElement element)
            {
                this.Element = element;
                this.GroupHeaderInfo = element.DataContext as IFloatingGroupHeaderInfo;
            }
        }

    }

    public class FloatingGroupHeaderRenderInfo
    {
        public Point Location { get; }

        public string HeaderText { get; }

        public FloatingGroupHeaderRenderInfo(Point location, string headerText)
        {
            if (headerText == null)
            {
                throw new ArgumentNullException(nameof(headerText));
            }

            this.Location = location;
            this.HeaderText = headerText;
        }
    }
}
