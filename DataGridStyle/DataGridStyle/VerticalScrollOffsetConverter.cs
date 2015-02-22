using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace DataGridStyle
{
    public class VerticalScrollOffsetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var hostPanel = (FrameworkElement)values[0];
            var floatingElementHost = (FrameworkElement)values[1];
            var contentPanelHeight = (double)values[2];
            var scrollViewerOffset = (double)values[3];
            var floatingElementHeight = (double)values[4];

            var value = 0d;

            if (floatingElementHost.IsLoaded)
            {
                var transform = (MatrixTransform)floatingElementHost.TransformToVisual(hostPanel);

                value = scrollViewerOffset - transform.Matrix.OffsetY;

                var offsetMin = 0d;
                var offsetMax = contentPanelHeight - floatingElementHeight;

                value = Math.Max(value, offsetMin);
                value = Math.Min(value, offsetMax);
            }

            return value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
