using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced Alert Form for displaying non-blocking notification messages with auto-close functionality.
    /// Provides a modern, customizable alert dialog with multi-language support and automatic positioning.
    /// Features include custom icons, countdown timer, and top-most display for critical notifications.
    /// Optimized for .NET Framework 4.8 with proper resource management and thread-safe operations.
    /// </summary>
    public partial class AdvancedAlertForm : Form
    {
        #region Constants

        /// <summary>
        /// Default countdown duration in seconds before auto-close
        /// </summary>
        private const int DEFAULT_COUNTDOWN_SECONDS = 3;

        /// <summary>
        /// Default horizontal offset from screen center for positioning
        /// </summary>
        private const int DEFAULT_HORIZONTAL_OFFSET = 180;

        /// <summary>
        /// Default vertical offset from screen top for positioning
        /// </summary>
        private const int DEFAULT_VERTICAL_OFFSET = 10;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the icon image displayed in the alert
        /// </summary>
        public Image MessageIcon
        {
            get { return pictureBox.Image; }
            set
            {
                pictureBox.Image = value;
                // Ensure proper disposal of previous image if needed
                if (pictureBox.Image != null && pictureBox.Image != value)
                {
                    pictureBox.Image?.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets or sets the message text displayed in the alert
        /// </summary>
        public string Message
        {
            get { return lblMessage.Text; }
            set
            {
                lblMessage.Text = value ?? string.Empty;
                // Adjust form size based on message length if needed
                AdjustFormSizeForMessage();
            }
        }

        /// <summary>
        /// Gets or sets the countdown duration in seconds before the alert automatically closes
        /// </summary>
        public int CountDown { get; set; } = DEFAULT_COUNTDOWN_SECONDS;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedAlertForm class with default positioning and settings
        /// </summary>
        public AdvancedAlertForm()
        {
            InitializeComponent();
            InitializeFormProperties();
            SetOptimalPosition();
        }

        /// <summary>
        /// Initializes a new instance of the AdvancedAlertForm class with custom message and icon
        /// </summary>
        /// <param name="message">The message to display in the alert</param>
        /// <param name="icon">The icon to display with the message</param>
        public AdvancedAlertForm(string message, Image icon) : this()
        {
            Message = message;
            MessageIcon = icon;
        }

        /// <summary>
        /// Initializes a new instance of the AdvancedAlertForm class with custom message, icon, and countdown
        /// </summary>
        /// <param name="message">The message to display in the alert</param>
        /// <param name="icon">The icon to display with the message</param>
        /// <param name="countDown">The countdown duration in seconds before auto-close</param>
        public AdvancedAlertForm(string message, Image icon, int countDown) : this(message, icon)
        {
            CountDown = Math.Max(1, countDown); // Ensure minimum 1 second
        }

        #endregion

        #region Form Events

        /// <summary>
        /// Handles the form load event to initialize controls and start the auto-close timer
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Event arguments</param>
        private async void AdvancedAlertForm_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeFormFocus();
                await StartAutoCloseTimer();
            }
            catch
            {
                SafeClose();
            }
        }

        /// <summary>
        /// Handles form closing event to ensure proper cleanup
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Form closing event arguments</param>
        private void AdvancedAlertForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanupResources();
        }

        #endregion

        #region Control Events

        /// <summary>
        /// Handles the close label click event to manually close the alert
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Event arguments</param>
        private void LblClose_Click(object sender, EventArgs e)
        {
            SafeClose();
        }

        /// <summary>
        /// Handles key down events for keyboard shortcuts
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Key event arguments</param>
        private void AdvancedAlertForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                SafeClose();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the form properties for optimal display
        /// </summary>
        private void InitializeFormProperties()
        {
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            // Enable key events
            this.KeyPreview = true;
            this.KeyDown += AdvancedAlertForm_KeyDown;
            this.FormClosing += AdvancedAlertForm_FormClosing;
        }

        /// <summary>
        /// Sets the optimal position for the alert form on the screen
        /// </summary>
        private void SetOptimalPosition()
        {
            try
            {
                Screen primaryScreen = Screen.PrimaryScreen;
                if (primaryScreen != null)
                {
                    // Center horizontally with offset, position at top with offset
                    this.Left = (primaryScreen.Bounds.Width / 2) - DEFAULT_HORIZONTAL_OFFSET;
                    this.Top = primaryScreen.Bounds.Top + DEFAULT_VERTICAL_OFFSET;

                    // Ensure the form is visible within screen bounds
                    EnsureVisiblePosition(primaryScreen);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting form position: {ex.Message}");
                // Fallback to default position
                this.Left = 100;
                this.Top = 100;
            }
        }

        /// <summary>
        /// Ensures the alert form is positioned within visible screen bounds
        /// </summary>
        /// <param name="screen">The target screen for positioning</param>
        private void EnsureVisiblePosition(Screen screen)
        {
            // Adjust left position if form extends beyond screen width
            if (this.Left + this.Width > screen.Bounds.Right)
            {
                this.Left = screen.Bounds.Right - this.Width - 10;
            }

            // Adjust left position if form is too far left
            if (this.Left < screen.Bounds.Left)
            {
                this.Left = screen.Bounds.Left + 10;
            }

            // Adjust top position if form extends beyond screen height
            if (this.Top + this.Height > screen.Bounds.Bottom)
            {
                this.Top = screen.Bounds.Bottom - this.Height - 10;
            }
        }

        /// <summary>
        /// Initializes form focus to the close button for immediate user interaction
        /// </summary>
        private void InitializeFormFocus()
        {
            if (LblClose != null && !LblClose.IsDisposed)
            {
                LblClose.Focus();
            }
        }

        /// <summary>
        /// Starts the auto-close timer with the specified countdown duration
        /// </summary>
        private async Task StartAutoCloseTimer()
        {
            if (CountDown <= 0) return;

            try
            {
                // Create a unique task name to avoid conflicts with multiple alert instances
                string uniqueTaskName = GenerateUniqueTaskName();

                await TaskDelayUtils.TaskDelayNamed(uniqueTaskName, () =>
                {
                    // Ensure form is still valid before closing
                    if (IsFormValid())
                    {
                        SafeClose();
                    }
                }, CountDown * 1000);
            }
            catch
            {
                SafeClose();
            }
        }

        /// <summary>
        /// Generates a unique task name for the auto-close timer to prevent conflicts
        /// </summary>
        /// <returns>A unique string identifier for the timer task</returns>
        private string GenerateUniqueTaskName()
        {
            return $"AlertClose_{this.GetHashCode()}_{DateTime.Now.Ticks}_{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Checks if the form is in a valid state for operations
        /// </summary>
        /// <returns>True if the form is valid and can be operated on, false otherwise</returns>
        private bool IsFormValid()
        {
            return !this.IsDisposed && !this.Disposing && this.Created;
        }

        /// <summary>
        /// Safely closes the form with proper error handling
        /// </summary>
        private void SafeClose()
        {
            try
            {
                if (IsFormValid())
                {
                    // Use BeginInvoke for thread-safe closing
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new Action(() => this.Close()));
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing alert form: {ex.Message}");
            }
        }

        /// <summary>
        /// Adjusts the form size based on the message content length
        /// </summary>
        private void AdjustFormSizeForMessage()
        {
            if (lblMessage == null) return;

            try
            {
                // Calculate required size based on message length
                using (Graphics g = this.CreateGraphics())
                {
                    SizeF textSize = g.MeasureString(lblMessage.Text, lblMessage.Font, lblMessage.MaximumSize.Width);

                    // Adjust form height if needed
                    int requiredHeight = (int)textSize.Height + 60; // Add padding
                    if (requiredHeight > this.Height)
                    {
                        this.Height = Math.Min(requiredHeight, Screen.PrimaryScreen.WorkingArea.Height / 3);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adjusting form size: {ex.Message}");
            }
        }

        /// <summary>
        /// Performs cleanup of resources when the form is closing
        /// </summary>
        private void CleanupResources()
        {
            try
            {
                // Dispose of image resources if they exist
                if (pictureBox?.Image != null)
                {
                    pictureBox.Image.Dispose();
                    pictureBox.Image = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during resource cleanup: {ex.Message}");
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Overrides the SetVisibleCore method to ensure proper display behavior
        /// </summary>
        /// <param name="value">Visibility state</param>
        protected override void SetVisibleCore(bool value)
        {
            // Ensure the form can be made visible
            base.SetVisibleCore(value);
        }

        /// <summary>
        /// Handles the disposal of the form and its resources
        /// </summary>
        /// <param name="disposing">True if disposing managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CleanupResources();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}