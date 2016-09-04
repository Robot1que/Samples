using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace ToggleSwitch.Behaviors
{
    public class SwitchToggleThumbBehavior : Behavior<FrameworkElement>
    {

        private double? _dragProgress = null;
        private Point? _initialPosition = null;
        private bool _isThumbDragingThresholdPassed = false;

#region Properties

        public static readonly DependencyProperty IsCheckedProperty;
        public bool IsChecked
        {
            get
            {
                return (bool)this.GetValue(SwitchToggleThumbBehavior.IsCheckedProperty);
            }
            set
            {
                this.SetValue(SwitchToggleThumbBehavior.IsCheckedProperty, value);
            }
        }

        public static readonly DependencyProperty ThumbProperty;
        public FrameworkElement Thumb
        {
            get
            {
                return (FrameworkElement)this.GetValue(SwitchToggleThumbBehavior.ThumbProperty);
            }
            set
            {
                this.SetValue(SwitchToggleThumbBehavior.ThumbProperty, value);
            }
        }

        public double ThumbLaneWidth { get; set; }

        public double ThumbDragingThreshold { get; set; } = 4;

        public Storyboard SwitchStoryboard { get; set; }

        public Storyboard SwitchOnStoryboard { get; set; }

        public Storyboard SwitchOffStoryboard { get; set; }

        public Storyboard SwitchOnFullStoryboard { get; set; }

        public Storyboard SwitchOffFullStoryboard { get; set; }

        public Storyboard SwitchOnHoldStoryboard { get; set; }

#endregion

        static SwitchToggleThumbBehavior()
        {
            SwitchToggleThumbBehavior.IsCheckedProperty =
                DependencyProperty.Register(
                    nameof(SwitchToggleThumbBehavior.IsChecked), 
                    typeof(bool), 
                    typeof(SwitchToggleThumbBehavior),
                    new PropertyMetadata(false)
                );

            SwitchToggleThumbBehavior.ThumbProperty = 
                DependencyProperty.Register(
                    nameof(SwitchToggleThumbBehavior.Thumb), 
                    typeof(FrameworkElement), 
                    typeof(SwitchToggleThumbBehavior),
                    new PropertyMetadata(SwitchToggleThumbBehavior.ThumbChangedCallback)
                );
        }

#region Overriden Methods

        protected override void OnAttached()
        {
            if (this.Thumb != null)
            {
                this.ThumbMouseEventSubscribe(true);
            }

            this.SwitchOffStoryboard.Completed += this.SwitchOffStoryboard_Completed;
            this.SwitchOffFullStoryboard.Completed += this.SwitchOffStoryboard_Completed;

            this.SwitchOnStoryboard.Completed += this.SwitchOnStoryboard_Completed;
            this.SwitchOnFullStoryboard.Completed += this.SwitchOnStoryboard_Completed;
        }

        protected override void OnDetaching()
        {
            if (this.Thumb != null)
            {
                this.ThumbMouseEventSubscribe(false);
            }

            this.SwitchOffStoryboard.Completed -= this.SwitchOffStoryboard_Completed;
            this.SwitchOffFullStoryboard.Completed -= this.SwitchOffStoryboard_Completed;

            this.SwitchOnStoryboard.Completed -= this.SwitchOnStoryboard_Completed;
            this.SwitchOnFullStoryboard.Completed -= this.SwitchOnStoryboard_Completed;
        }

#endregion

#region Private Methods

        private static void ThumbChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (FrameworkElement)e.OldValue;
            var newValue = (FrameworkElement)e.NewValue;
            var behavior = (SwitchToggleThumbBehavior)d;

            if (oldValue != null)
            {
                behavior.ThumbMouseEventSubscribe(false);
            }

            if (newValue != null)
            {
                behavior.ThumbMouseEventSubscribe(true);
            }
        }

        private void ThumbMouseEventSubscribe(bool value)
        {
            if (value)
            {
                this.Thumb.MouseLeftButtonDown += this.SwitchThumb_PreviewMouseLeftButtonDown;
                this.Thumb.MouseMove += this.SwitchThumb_MouseMove;
                this.Thumb.MouseLeftButtonUp += this.SwitchThumb_PreviewMouseLeftButtonUp;
            }
            else
            {
                this.Thumb.MouseLeftButtonDown -= this.SwitchThumb_PreviewMouseLeftButtonDown;
                this.Thumb.MouseMove -= this.SwitchThumb_MouseMove;
                this.Thumb.MouseLeftButtonUp -= this.SwitchThumb_PreviewMouseLeftButtonUp;
            }
        }

        private void SwitchThumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = (Border)sender;
            Mouse.Capture(element);
            this._initialPosition = e.GetPosition(this.AssociatedObject);

            this._isThumbDragingThresholdPassed = false;
            var offsetTime = TimeSpan.FromSeconds(this.IsChecked ? 1 : 0);

            this.SwitchStoryboard.Begin(this.AssociatedObject, true);
            this.SwitchStoryboard.Seek(this.AssociatedObject, offsetTime, TimeSeekOrigin.BeginTime);
            this.SwitchStoryboard.Pause(this.AssociatedObject);
        }

        private void SwitchThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._initialPosition.HasValue)
            {
                var pos = e.GetPosition(this.AssociatedObject);
                var mouseMoveDelta = pos.X - this._initialPosition.Value.X;

                if (!this._isThumbDragingThresholdPassed)
                {
                    this._isThumbDragingThresholdPassed = 
                        Math.Abs(mouseMoveDelta) > this.ThumbDragingThreshold;
                }

                var xMax = this.ThumbLaneWidth;

                if (this.IsChecked)
                {
                    mouseMoveDelta += xMax;
                }

                mouseMoveDelta = mouseMoveDelta < 0 ? 0 : mouseMoveDelta > xMax ? xMax : mouseMoveDelta;

                this._dragProgress = mouseMoveDelta / xMax;

                var timeOffset = TimeSpan.FromSeconds(this._dragProgress.Value);
                this.SwitchStoryboard.SeekAlignedToLastTick(
                    this.AssociatedObject, 
                    timeOffset, 
                    TimeSeekOrigin.BeginTime
                );
            }
        }

        private void SwitchThumb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = (Border)sender;
            Mouse.Capture(null);
            this._initialPosition = null;

            this.SwitchStoryboard.Remove(this.AssociatedObject);

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
                    this.SwitchOnStoryboard.Begin(this.AssociatedObject);
                }
                else
                {
                    this.SwitchOffStoryboard.Begin(this.AssociatedObject);
                }

                this._dragProgress = null;
            }
            else
            {
                if (this.IsChecked)
                {
                    this.SwitchOffFullStoryboard.Begin(this.AssociatedObject);
                }
                else
                {
                    this.SwitchOnFullStoryboard.Begin(this.AssociatedObject);
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
                this.SwitchOnHoldStoryboard.Begin(this.AssociatedObject);
            }
        }

#endregion

    }
}
