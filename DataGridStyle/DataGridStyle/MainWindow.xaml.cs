using System;
using System.Collections.Generic;
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
using System.Globalization;

namespace DataGridStyle
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.ItemsControl.ItemsSource = MainWindow.CountriesGet();

            this.GroupingAdd();
        }

        private static string[] CountriesGet()
        {
            return
                new string[] 
                {
                    "Aland Islands",
                    "Argentina",
                    "Australia", // 3
                    "Austria", // 2
                    "Belarus", // 1
                    "Belgium", // 1
                    "Benin", // 1
                    "Botswana", // 1
                    "Brasil", // 2
                    "Bulgaria", // 1
                    "Burkina Faso", // 1
                    "Cameroon", // 1
                    "Canada", // 4
                    "Chile", // 1
                    "China", // 1
                    "Colombia", // 0
                    "Costa Rica", // 1
                    "Cote d`Ivoire", // 1
                    "Croatia", // 1
                    "Cyprus", // 1
                    "Czech Republic", // 1
                    "Denmark", // 1
                    "Djibouti", // 1
                    "Egypt", // 1
                    "Estonia", // 1
                    "Finland", // 1
                    "France", // 7
                    "Gabon", // 1
                    "Georgia", // 1
                    "Germany", // 5
                    "Ghana", // 1
                    "Greece", // 1
                    "Guinea-Bissau", // 1
                    "Hong Kong", // 2
                    "Hungary", // 1
                    "Iceland", // 1
                    "India", // 1
                    "Indonesia", // 2
                    "Ireland", // 1
                    "Israel", // 2
                    "Italy", // 2
                    "Japan", // 1
                    "Jordan", // 1
                    "Kenya", // 1
                    "Kiribati", // 1
                    "Kuwait", // 1
                    "Latvia", // 1
                    "Lithuania", // 1
                    "Luxembourg", // 1
                    "Malaysia", // 2
                    "Malta", // 1
                    "Mexico", // 1
                    "Moldova", // 1
                    "Morocco", // 1
                    "Myanmar", // 1
                    "Namibia", // 1
                    "Netherlands", // 4
                    "New Zealand", // 1
                    "Nigeria", // 1
                    "Norway", // 1
                    "Palau", // 1
                    "Panama", // 1
                    "Philippines", // 1
                    "Poland", // 1
                    "Portugal", // 2
                    "Qatar", // 1
                    "Romania", // 2
                    "Russia", // 2
                    "Rwanda", // 1
                    "Saudi Arabia", // 1
                    "Serbia", // 1
                    "Seychelles", // 1
                    "Singapore", // 1
                    "Slovakia", // 1
                    "Slovenia", // 1
                    "South Africa", // 1
                    "South Korea", // 1
                    "Spain", // 4
                    "Sweden", // 1
                    "Switzerland", // 1
                    "Taiwan", // 1
                    "Thailand", // 1
                    "Turkey", // 1
                    "Uganda", // 1
                    "Ukraine", // 1
                    "United Arab Emirates", // 1
                    "United Kingdom", // 13
                    "United States of America", // 56
                    "Uruguay", // 1
                    "Vanuatu", // 1
                    "Vietnam" // 1
                };
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
