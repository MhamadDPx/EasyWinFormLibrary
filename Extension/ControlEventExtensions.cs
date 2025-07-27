using System;
using System.Reflection;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Extension
{
    // <summary>
    /// Extension methods for programmatically triggering control events
    /// </summary>
    public static class ControlEventExtensions
    {
        /// <summary>
        /// Programmatically performs a click on any control
        /// </summary>
        /// <param name="control">The control to click</param>
        public static void PerformClick(this Control control)
        {
            if (control == null) return;

            try
            {
                // First, try the built-in PerformClick method for buttons
                if (control is Button button)
                {
                    button.PerformClick();
                    return;
                }

                // For other controls, use reflection to call OnClick
                var onClickMethod = typeof(Control).GetMethod("OnClick",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onClickMethod != null)
                {
                    onClickMethod.Invoke(control, new object[] { EventArgs.Empty });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing click on {control.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Programmatically performs a click with custom EventArgs
        /// </summary>
        /// <param name="control">The control to click</param>
        /// <param name="eventArgs">Custom event arguments</param>
        public static void PerformClick(this Control control, EventArgs eventArgs)
        {
            if (control == null) return;

            try
            {
                var onClickMethod = typeof(Control).GetMethod("OnClick",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onClickMethod != null)
                {
                    onClickMethod.Invoke(control, new object[] { eventArgs ?? EventArgs.Empty });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing click: {ex.Message}");
            }
        }

        /// <summary>
        /// Programmatically performs a double-click on a control
        /// </summary>
        /// <param name="control">The control to double-click</param>
        public static void PerformDoubleClick(this Control control)
        {
            if (control == null) return;

            try
            {
                var onDoubleClickMethod = typeof(Control).GetMethod("OnDoubleClick",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onDoubleClickMethod != null)
                {
                    onDoubleClickMethod.Invoke(control, new object[] { EventArgs.Empty });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing double-click: {ex.Message}");
            }
        }

        /// <summary>
        /// Programmatically triggers mouse enter event
        /// </summary>
        /// <param name="control">The control to trigger event on</param>
        public static void PerformMouseEnter(this Control control)
        {
            if (control == null) return;

            try
            {
                var onMouseEnterMethod = typeof(Control).GetMethod("OnMouseEnter",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onMouseEnterMethod != null)
                {
                    onMouseEnterMethod.Invoke(control, new object[] { EventArgs.Empty });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing mouse enter: {ex.Message}");
            }
        }

        /// <summary>
        /// Programmatically triggers mouse leave event
        /// </summary>
        /// <param name="control">The control to trigger event on</param>
        public static void PerformMouseLeave(this Control control)
        {
            if (control == null) return;

            try
            {
                var onMouseLeaveMethod = typeof(Control).GetMethod("OnMouseLeave",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onMouseLeaveMethod != null)
                {
                    onMouseLeaveMethod.Invoke(control, new object[] { EventArgs.Empty });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing mouse leave: {ex.Message}");
            }
        }

        /// <summary>
        /// Programmatically triggers key press event
        /// </summary>
        /// <param name="control">The control to trigger event on</param>
        /// <param name="key">The key to simulate</param>
        public static void PerformKeyPress(this Control control, char key)
        {
            if (control == null) return;

            try
            {
                var keyPressEventArgs = new KeyPressEventArgs(key);
                var onKeyPressMethod = typeof(Control).GetMethod("OnKeyPress",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onKeyPressMethod != null)
                {
                    onKeyPressMethod.Invoke(control, new object[] { keyPressEventArgs });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing key press: {ex.Message}");
            }
        }

        /// <summary>
        /// Programmatically triggers key down event
        /// </summary>
        /// <param name="control">The control to trigger event on</param>
        /// <param name="keys">The keys to simulate</param>
        public static void PerformKeyDown(this Control control, Keys keys)
        {
            if (control == null) return;

            try
            {
                var keyEventArgs = new KeyEventArgs(keys);
                var onKeyDownMethod = typeof(Control).GetMethod("OnKeyDown",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onKeyDownMethod != null)
                {
                    onKeyDownMethod.Invoke(control, new object[] { keyEventArgs });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing key down: {ex.Message}");
            }
        }

        /// <summary>
        /// Programmatically triggers focus events
        /// </summary>
        /// <param name="control">The control to focus</param>
        public static void PerformFocus(this Control control)
        {
            if (control == null) return;

            try
            {
                control.Focus();

                var onGotFocusMethod = typeof(Control).GetMethod("OnGotFocus",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onGotFocusMethod != null)
                {
                    onGotFocusMethod.Invoke(control, new object[] { EventArgs.Empty });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing focus: {ex.Message}");
            }
        }

        /// <summary>
        /// Programmatically simulates a complete mouse click sequence (down, up, click)
        /// </summary>
        /// <param name="control">The control to click</param>
        /// <param name="button">The mouse button to simulate</param>
        /// <param name="x">X coordinate relative to control</param>
        /// <param name="y">Y coordinate relative to control</param>
        public static void PerformMouseClick(this Control control, MouseButtons button = MouseButtons.Left, int x = 0, int y = 0)
        {
            if (control == null) return;

            try
            {
                var mouseEventArgs = new MouseEventArgs(button, 1, x, y, 0);

                // Mouse down
                var onMouseDownMethod = typeof(Control).GetMethod("OnMouseDown",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                onMouseDownMethod?.Invoke(control, new object[] { mouseEventArgs });

                // Mouse up
                var onMouseUpMethod = typeof(Control).GetMethod("OnMouseUp",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                onMouseUpMethod?.Invoke(control, new object[] { mouseEventArgs });

                // Click
                control.PerformClick();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing mouse click: {ex.Message}");
            }
        }

        /// <summary>
        /// Safely performs an action on a control, checking if it exists and is not disposed
        /// </summary>
        /// <param name="control">The control to perform action on</param>
        /// <param name="action">The action to perform</param>
        public static void SafePerform(this Control control, Action<Control> action)
        {
            if (control == null || control.IsDisposed || action == null)
                return;

            try
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(new Action(() => action(control)));
                }
                else
                {
                    action(control);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SafePerform: {ex.Message}");
            }
        }

        /// <summary>
        /// Triggers validation events on a control
        /// </summary>
        /// <param name="control">The control to validate</param>
        /// <returns>True if validation passed</returns>
        public static bool PerformValidation(this Control control)
        {
            if (control == null) return true;

            try
            {
                var cancelEventArgs = new System.ComponentModel.CancelEventArgs();

                var onValidatingMethod = typeof(Control).GetMethod("OnValidating",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (onValidatingMethod != null)
                {
                    onValidatingMethod.Invoke(control, new object[] { cancelEventArgs });

                    if (!cancelEventArgs.Cancel)
                    {
                        var onValidatedMethod = typeof(Control).GetMethod("OnValidated",
                            BindingFlags.Instance | BindingFlags.NonPublic);
                        onValidatedMethod?.Invoke(control, new object[] { EventArgs.Empty });
                    }

                    return !cancelEventArgs.Cancel;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error performing validation: {ex.Message}");
            }

            return true;
        }
    }
}
