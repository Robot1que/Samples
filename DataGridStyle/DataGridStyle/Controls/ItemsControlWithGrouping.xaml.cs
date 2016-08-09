using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace DataGridStyle.Controls
{
    public partial class ItemsControlWithGrouping : UserControl
    {
        public ItemsControlWithGrouping()
        {
            this.InitializeComponent();

            this.ItemsControl.ItemsSource = DataSource.CountriesGet();

            this.GroupingAdd();
        }

        private void GroupingAdd()
        {
            var collectionView = (CollectionView)
                CollectionViewSource.GetDefaultView(this.ItemsControl.ItemsSource);
            if (collectionView.CanGroup)
            {
                collectionView.GroupDescriptions.Add(
                    new PropertyGroupDescription("", new CountryToGroupValueConverter())
                );
            }
        }
    }

    internal class CountryToGroupValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result;

            var stringValue = value as string;
            if (stringValue != null && stringValue.Length > 0)
            {
                result = stringValue[0];
            }
            else
            {
                result = value;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
