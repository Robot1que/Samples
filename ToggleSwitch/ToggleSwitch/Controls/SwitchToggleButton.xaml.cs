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

        private const double ThumbLaneWidthMin = 2;
        private const double ThumbLaneWidthMax = 178;
        private const double ThumbDragingThreshold = 4;
        
        private double? _dragProgress = null;
        private Point? _initialPosition = null;
        private bool _isThumbDragingThresholdPassed = false;
        private Storyboard _switchStoryboard = null;
        private Storyboard _switchOnStoryboard = null;
        private Storyboard _switchOffStoryboard = null;
        private Storyboard _switchOnFullStoryboard = null;
        private Storyboard _switchOffFullStoryboard = null;
        private Storyboard _switchOnHoldStoryboard = null;

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
                    typeof(MainWindow),
                    new PropertyMetadata(false)
                );
        }

        public SwitchToggleButton()
        {
            this.InitializeComponent();
        }

#region Private Methods

        private void SwitchToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            this._switchStoryboard = (Storyboard)this.FindResource("SwitchStoryboard");
            this._switchOffStoryboard = (Storyboard)this.FindResource("SwitchOffStoryboard");
            this._switchOnStoryboard = (Storyboard)this.FindResource("SwitchOnStoryboard");
            this._switchOffFullStoryboard = (Storyboard)this.FindResource("SwitchOffFullStoryboard");
            this._switchOnFullStoryboard = (Storyboard)this.FindResource("SwitchOnFullStoryboard");
            this._switchOnHoldStoryboard = (Storyboard)this.FindResource("SwitchOnHoldStoryboard");
        }

        private void SwitchThumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = (Border)sender;
            Mouse.Capture(element);
            this._initialPosition = e.GetPosition(this);

            this._isThumbDragingThresholdPassed = false;
            var offsetTime = TimeSpan.FromSeconds(this.IsChecked ? 1 : 0);

            this._switchStoryboard.Begin(this, true);
            this._switchStoryboard.Seek(this, offsetTime, TimeSeekOrigin.BeginTime);
            this._switchStoryboard.Pause(this);
        }

        private void SwitchThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._initialPosition.HasValue)
            {
                var pos = e.GetPosition(this);
                var mouseMoveDelta = pos.X - this._initialPosition.Value.X;

                if (!this._isThumbDragingThresholdPassed)
                {
                    this._isThumbDragingThresholdPassed = 
                        Math.Abs(mouseMoveDelta) > SwitchToggleButton.ThumbDragingThreshold;
                }

                var xMax = SwitchToggleButton.ThumbLaneWidthMax - SwitchToggleButton.ThumbLaneWidthMin;

                if (this.IsChecked)
                {
                    mouseMoveDelta += xMax;
                }

                mouseMoveDelta = mouseMoveDelta < 0 ? 0 : mouseMoveDelta > xMax ? xMax : mouseMoveDelta;

                this._dragProgress = mouseMoveDelta / xMax;

                var timeOffset = TimeSpan.FromSeconds(this._dragProgress.Value);
                this._switchStoryboard.SeekAlignedToLastTick(this, timeOffset, TimeSeekOrigin.BeginTime);
            }
        }

        private void SwitchThumb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = (Border)sender;
            Mouse.Capture(null);
            this._initialPosition = null;

            this._switchStoryboard.Remove(this);

            if (this._isThumbDragingThresholdPassed)
            {
                if (this._dragProgress.Value == 0)
                {
                    this.SwitchValueChange(false);
                }
                else if (this._dragProgress.Value == 1)
                {
                    this.SwitchValueChange(true);
                }
                else if (this._dragProgress.Value > 0.5)
                {
                    this._switchOnStoryboard.Begin(this);
                }
                else
                {
                    this._switchOffStoryboard.Begin(this);
                }

                this._dragProgress = null;
            }
            else
            {
                if (this.IsChecked)
                {
                    this._switchOffFullStoryboard.Begin(this);
                }
                else
                {
                    this._switchOnFullStoryboard.Begin(this);
                }
            }
        }

        private void SwitchOffStoryboard_Completed(object sender, EventArgs e)
        {
            this.SwitchValueChange(false);
        }

        private void SwitchOnStoryboard_Completed(object sender, EventArgs e)
        {
            this.SwitchValueChange(true);
        }

        private void SwitchValueChange(bool newValue)
        {
            this.IsChecked = newValue;

            if (this.IsChecked)
            {
                this._switchOnHoldStoryboard.Begin(this);
            }
        }

#endregion

    }
}
