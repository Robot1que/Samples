using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ToggleSwitch.Behaviors
{
    public class CommonVisualStateGroupBehavior : Behavior<FrameworkElement>
    {
        private const string DefaultNormalStateName = "Normal";
        private const string DefaultMouseOverStateName = "MouseOver";
        private const string DefaultPressedStateName = "Pressed";

        private bool _isMouseOver = false;
        private bool _isPressed = false;

#region Overriden Methods

        protected override void OnAttached()
        {
            this.AssociatedObject.MouseEnter += this.AssociatedObject_MouseEnter;
            this.AssociatedObject.MouseLeave += this.AssociatedObject_MouseLeave;
            this.AssociatedObject.MouseLeftButtonDown += this.AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseLeftButtonUp += this.AssociatedObject_MouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.MouseEnter -= this.AssociatedObject_MouseEnter;
            this.AssociatedObject.MouseLeave -= this.AssociatedObject_MouseLeave;
            this.AssociatedObject.MouseLeftButtonDown -= this.AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseLeftButtonUp -= this.AssociatedObject_MouseLeftButtonUp;
        }

#endregion

#region Private Methods

        private void VisualStateUpdate()
        {
            string stateName;

            if (this._isPressed)
            {
                stateName = CommonVisualStateGroupBehavior.DefaultPressedStateName;
            }
            else if (this._isMouseOver)
            {
                stateName = CommonVisualStateGroupBehavior.DefaultMouseOverStateName;
            }
            else
            {
                stateName = CommonVisualStateGroupBehavior.DefaultNormalStateName;
            }

            VisualStateManager.GoToElementState(this.AssociatedObject, stateName, true);
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this._isMouseOver = true;
            this.VisualStateUpdate();
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this._isMouseOver = false;
            this.VisualStateUpdate();
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this._isPressed = true;
            this.VisualStateUpdate();
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this._isPressed = false;
            this.VisualStateUpdate();
        }

#endregion

    }
}
