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
using DataGridStyle.Behaviors;

namespace DataGridStyle.Controls
{
    public partial class ItemsControlWithoutGrouping : UserControl
    {
        public ItemsControlWithoutGrouping()
        {
            this.InitializeComponent();

            this.ItemsControl.ItemsSource =
                DataSource.CountriesGet()
                    .OrderBy(item => item)
                    .GroupBy(item => item.Substring(0, 1))
                    .SelectMany(
                        group => 
                            new GroupItem[] { new GroupItem(group.Key, group.Key, true) } 
                                .Concat(group.Select(item => new GroupItem(item, group.Key, false)))
                    )
                    .ToArray();
        }
    }

    public class GroupItem : IFloatingGroupHeaderInfo
    {
        public string Title { get; }

        public string HeaderText { get; }

        public bool IsHeader { get; }

        public GroupItem(string title, string headerText, bool isHeader)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (headerText == null)
            {
                throw new ArgumentNullException(nameof(headerText));
            }

            this.Title = title;
            this.HeaderText = headerText;
            this.IsHeader = isHeader;
        }
    }

    public class ItemsControlWithoutGroupingItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GroupItemTemplate { get; set; }

        public DataTemplate GroupHeaderItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate;
            
            if (item is GroupItem)
            {
                var groupItem = (GroupItem)item;
                dataTemplate = groupItem.IsHeader ? this.GroupHeaderItemTemplate : this.GroupItemTemplate;
            }
            else
            {
                dataTemplate = base.SelectTemplate(item, container);
            }

            return dataTemplate;
        }
    }
}
