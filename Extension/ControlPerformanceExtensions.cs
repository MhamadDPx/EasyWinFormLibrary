using System;
using System.Reflection;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Extension methods for control performance optimization
    /// </summary>
    public static class ControlPerformanceExtensions
    {
        /// <summary>
        /// Enables or disables double buffering for a control to reduce flicker
        /// </summary>
        /// <param name="control">The control to modify</param>
        /// <param name="enable">True to enable double buffering</param>
        public static void DoubleBuffered(this Control control, bool enable)
        {
            if (control == null) return;

            try
            {
                var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (doubleBufferPropertyInfo != null && doubleBufferPropertyInfo.CanWrite)
                {
                    doubleBufferPropertyInfo.SetValue(control, enable, null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting DoubleBuffered property: {ex.Message}");
            }
        }

        /// <summary>
        /// Enables double buffering and sets additional performance-related properties
        /// </summary>
        /// <param name="control">The control to optimize</param>
        public static void OptimizeForPerformance(this Control control)
        {
            if (control == null) return;

            try
            {
                // Enable double buffering
                control.DoubleBuffered(true);

                // Set additional performance-related control styles
                control.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                control.SetStyle(ControlStyles.UserPaint, true);
                control.SetStyle(ControlStyles.ResizeRedraw, true);
                control.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error optimizing control performance: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets control style using reflection (for protected SetStyle method)
        /// </summary>
        /// <param name="control">The control to modify</param>
        /// <param name="style">The control style to set</param>
        /// <param name="value">The value to set</param>
        public static void SetStyle(this Control control, ControlStyles style, bool value)
        {
            if (control == null) return;

            try
            {
                var setStyleMethod = control.GetType().GetMethod("SetStyle",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (setStyleMethod != null)
                {
                    setStyleMethod.Invoke(control, new object[] { style, value });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting control style: {ex.Message}");
            }
        }

        /// <summary>
        /// Suspends layout operations for better performance during bulk updates
        /// </summary>
        /// <param name="control">The control to suspend layout for</param>
        /// <param name="action">The action to perform while layout is suspended</param>
        public static void WithSuspendedLayout(this Control control, Action action)
        {
            if (control == null || action == null) return;

            try
            {
                control.SuspendLayout();
                action();
            }
            finally
            {
                control.ResumeLayout(true);
            }
        }

        /// <summary>
        /// Suspends drawing operations for better performance during bulk updates
        /// </summary>
        /// <param name="control">The control to suspend drawing for</param>
        /// <param name="action">The action to perform while drawing is suspended</param>
        public static void WithSuspendedDrawing(this Control control, Action action)
        {
            if (control == null || action == null) return;

            const int WM_SETREDRAW = 0x000B;

            try
            {
                // Suspend drawing
                NativeMethods.SendMessage(control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
                action();
            }
            finally
            {
                // Resume drawing
                NativeMethods.SendMessage(control.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                control.Invalidate();
            }
        }

        /// <summary>
        /// Combines layout and drawing suspension for maximum performance
        /// </summary>
        /// <param name="control">The control to optimize</param>
        /// <param name="action">The action to perform</param>
        public static void WithSuspendedUpdates(this Control control, Action action)
        {
            if (control == null || action == null) return;

            control.WithSuspendedDrawing(() =>
            {
                control.WithSuspendedLayout(action);
            });
        }

        /// <summary>
        /// Enables or disables all animations for a control hierarchy
        /// </summary>
        /// <param name="control">The root control</param>
        /// <param name="enable">True to enable animations</param>
        public static void SetAnimations(this Control control, bool enable)
        {
            if (control == null) return;

            try
            {
                // Disable animations system-wide (affects all controls)
                SystemParametersInfo(SPI_SETMENUANIMATION, enable ? 1 : 0, IntPtr.Zero, SPIF_SENDCHANGE);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting animations: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current double buffering state of a control
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <returns>True if double buffering is enabled</returns>
        public static bool IsDoubleBuffered(this Control control)
        {
            if (control == null) return false;

            try
            {
                var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (doubleBufferPropertyInfo != null && doubleBufferPropertyInfo.CanRead)
                {
                    return (bool)doubleBufferPropertyInfo.GetValue(control, null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting DoubleBuffered property: {ex.Message}");
            }

            return false;
        }

        #region Native Methods

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        }

        private const int SPI_SETMENUANIMATION = 0x1003;
        private const int SPIF_SENDCHANGE = 0x0002;

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(int uiAction, int uiParam, IntPtr pvParam, int fWinIni);

        #endregion
    }
}
