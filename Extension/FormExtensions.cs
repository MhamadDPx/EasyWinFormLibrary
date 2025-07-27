using System;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Extension methods for Form resizing and positioning
    /// </summary>
    public static class FormExtensions
    {

        /// <summary>
        /// Resizes form to fit the working area (excluding taskbar)
        /// </summary>
        /// <param name="form">The form to resize</param>
        public static void ResizeFormForTaskBar(this Form form)
        {
            if (form == null) return;

            var workingArea = Screen.PrimaryScreen.WorkingArea;
            form.Height = workingArea.Height;
            form.Width = workingArea.Width;
            form.Location = workingArea.Location;
            form.WindowState = FormWindowState.Normal; // Ensure it's not minimized/maximized
        }

        /// <summary>
        /// Resizes form to fit the working area of the screen containing the form
        /// </summary>
        /// <param name="form">The form to resize</param>
        public static void ResizeForCurrentScreen(this Form form)
        {
            if (form == null) return;

            var currentScreen = Screen.FromControl(form);
            var workingArea = currentScreen.WorkingArea;

            form.Height = workingArea.Height;
            form.Width = workingArea.Width;
            form.Location = workingArea.Location;
            form.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Maximizes form to working area without changing WindowState
        /// </summary>
        /// <param name="form">The form to maximize</param>
        public static void MaximizeToWorkingArea(this Form form)
        {
            if (form == null) return;

            var workingArea = Screen.PrimaryScreen.WorkingArea;
            form.StartPosition = FormStartPosition.Manual;
            form.Bounds = workingArea;
        }

        /// <summary>
        /// Resizes form to percentage of working area
        /// </summary>
        /// <param name="form">The form to resize</param>
        /// <param name="widthPercent">Width percentage (0.1 to 1.0)</param>
        /// <param name="heightPercent">Height percentage (0.1 to 1.0)</param>
        /// <param name="centerForm">Whether to center the form</param>
        public static void ResizeToPercentage(this Form form, double widthPercent = 0.8, double heightPercent = 0.8, bool centerForm = true)
        {
            if (form == null) return;

            // Clamp percentages
            widthPercent = Math.Max(0.1, Math.Min(1.0, widthPercent));
            heightPercent = Math.Max(0.1, Math.Min(1.0, heightPercent));

            var workingArea = Screen.PrimaryScreen.WorkingArea;

            int newWidth = (int)(workingArea.Width * widthPercent);
            int newHeight = (int)(workingArea.Height * heightPercent);

            form.Size = new Size(newWidth, newHeight);

            if (centerForm)
            {
                form.CenterToScreen();
            }
        }

        /// <summary>
        /// Centers form on the current screen
        /// </summary>
        /// <param name="form">The form to center</param>
        public static void CenterToScreen(this Form form)
        {
            if (form == null) return;

            var workingArea = Screen.FromControl(form).WorkingArea;

            int x = workingArea.Left + (workingArea.Width - form.Width) / 2;
            int y = workingArea.Top + (workingArea.Height - form.Height) / 2;

            form.Location = new Point(x, y);
        }

        /// <summary>
        /// Ensures form is visible on screen (moves if off-screen)
        /// </summary>
        /// <param name="form">The form to check</param>
        public static void EnsureVisible(this Form form)
        {
            if (form == null) return;

            var formBounds = form.Bounds;
            var screenBounds = Screen.FromControl(form).WorkingArea;

            // Check if form is completely off-screen
            if (!screenBounds.IntersectsWith(formBounds))
            {
                form.CenterToScreen();
                return;
            }

            // Adjust position if partially off-screen
            int newX = form.Left;
            int newY = form.Top;

            if (form.Right > screenBounds.Right)
                newX = screenBounds.Right - form.Width;
            if (form.Left < screenBounds.Left)
                newX = screenBounds.Left;

            if (form.Bottom > screenBounds.Bottom)
                newY = screenBounds.Bottom - form.Height;
            if (form.Top < screenBounds.Top)
                newY = screenBounds.Top;

            form.Location = new Point(newX, newY);
        }

        /// <summary>
        /// Resizes form to fit content with optional padding
        /// </summary>
        /// <param name="form">The form to resize</param>
        /// <param name="padding">Additional padding around content</param>
        public static void ResizeToContent(this Form form, int padding = 20)
        {
            if (form == null) return;

            form.AutoSize = true;
            form.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Add padding
            form.Padding = new Padding(padding);

            // Ensure it fits on screen
            form.EnsureVisible();
        }

        /// <summary>
        /// Toggles between windowed and maximized to working area
        /// </summary>
        /// <param name="form">The form to toggle</param>
        public static void ToggleMaximized(this Form form)
        {
            if (form == null) return;

            if (form.WindowState == FormWindowState.Maximized)
            {
                form.WindowState = FormWindowState.Normal;
            }
            else
            {
                form.ResizeFormForTaskBar();
            }
        }

        /// <summary>
        /// Sets form to specific screen
        /// </summary>
        /// <param name="form">The form to move</param>
        /// <param name="screenIndex">Screen index (0 = primary)</param>
        /// <param name="maximize">Whether to maximize to working area</param>
        public static void MoveToScreen(this Form form, int screenIndex, bool maximize = false)
        {
            if (form == null) return;

            if (screenIndex < 0 || screenIndex >= Screen.AllScreens.Length)
                screenIndex = 0; // Default to primary screen

            var targetScreen = Screen.AllScreens[screenIndex];
            var workingArea = targetScreen.WorkingArea;

            if (maximize)
            {
                form.Bounds = workingArea;
            }
            else
            {
                // Just move to screen, keep current size
                form.Location = new Point(
                    workingArea.Left + (workingArea.Width - form.Width) / 2,
                    workingArea.Top + (workingArea.Height - form.Height) / 2
                );
            }
        }
    }
}
