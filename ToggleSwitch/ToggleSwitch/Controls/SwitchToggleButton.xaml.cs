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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToggleSwitch.Controls
{
    public partial class SwitchToggleButton : UserControl
    {
        

#region Properties

        public static readonly DependencyProperty IsCheckedProperty;
        public bool IsChecked
        {
            get
            {
                return (bool)this.GetValue(SwitchToggleButton.IsCheckedProperty);
            }
            set
            {
                this.SetValue(SwitchToggleButton.IsCheckedProperty, value);
            }
        }

#endregion

        static SwitchToggleButton()
        {
            SwitchToggleButton.IsCheckedProperty =
                DependencyProperty.Register(
                    nameof(SwitchToggleButton.IsChecked), 
                    typeof(bool), 
                    typeof(SwitchToggleButton),
                    new PropertyMetadata(false)
                );
        }

        public SwitchToggleButton()
        {
            this.InitializeComponent();
        }

    }
}
