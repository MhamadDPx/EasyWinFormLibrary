using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced Form Top Menu Bar with drag and drop functionality for moving the parent form.
    /// Provides multi-language support, custom styling, and intuitive form movement capabilities.
    /// Features include draggable area, minimize/close buttons, and customizable appearance.
    /// Optimized for .NET Framework 4.8 with smooth drag operations and proper event handling.
    /// </summary>
    public partial class AdvancedFormTopMenuBar : UserControl
    {
        #region Constants

        /// <summary>
        /// Default height of the top menu bar
        /// </summary>
        private const int DEFAULT_TOP_MENUBAR_HEIGHT = 30;

        /// <summary>
        /// Default background color for the top menu bar
        /// </summary>
        private static readonly Color DEFAULT_BACKGROUND_COLOR = Color.FromArgb(122, 121, 140);

        #endregion

        #region Private Fields

        private string _englishTitle = string.Empty;
        private string _kurdishTitle = string.Empty;
        private string _arabicTitle = string.Empty;
        private Color _backgroundColor = DEFAULT_BACKGROUND_COLOR;
        // Drag and drop related fields
        private bool _isDragging = false;
        private Point _lastMousePosition = Point.Empty;
        private bool _enableDragAndDrop = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether to show the minimize button
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Shows or hides the minimize button")]
        [DefaultValue(true)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowMinimizeButton
        {
            get { return btnMinimize.Visible; }
            set
            {
                btnMinimize.Visible = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Select Top Menu Bar Height
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Top Menu Bar Height")]
        [DefaultValue(30)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TopMenuBarHeight
        {
            get { return Height; }
            set
            {
                this.Height = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the form title (English)
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The title of the form (English en-001)")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string FormTitlEnglish
        {
            get { return _englishTitle; }
            set
            {
                _englishTitle = value;
                if (LanguageManager.SelectedLanguage == FormLanguage.English)
                {
                    lblFormTitle.Text = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the form title (Kurdish)
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The title of the form (Kurdish en-US)")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string FormTitlKurdish
        {
            get { return _kurdishTitle; }
            set
            {
                _kurdishTitle = value;
                if (LanguageManager.SelectedLanguage == FormLanguage.Kurdish)
                {
                    lblFormTitle.Text = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the form title (Arabic)
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The title of the form (Arabic en-GB)")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string FormTitlArabic
        {
            get { return _arabicTitle; }
            set
            {
                _arabicTitle = value;
                if (LanguageManager.SelectedLanguage == FormLanguage.Arabic)
                {
                    lblFormTitle.Text = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets Top Menu Bar background color
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Top Menu Bar background color")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                UpdateBackgroundColor();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether drag and drop functionality is enabled
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Enables or disables drag and drop functionality for moving the parent form")]
        [DefaultValue(true)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableDragAndDrop
        {
            get { return _enableDragAndDrop; }
            set
            {
                _enableDragAndDrop = value;
            }
        }

        /// <summary>
        /// Gets whether the control is currently being dragged
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDragging
        {
            get { return _isDragging; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedFormTopMenuBar class
        /// </summary>
        public AdvancedFormTopMenuBar()
        {
            InitializeComponent();
            this.Height = DEFAULT_TOP_MENUBAR_HEIGHT;
            InitializeDragAndDrop();
            UpdateBackgroundColor(); // Ensure initial color is applied
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Initializes drag and drop functionality for the control
        /// </summary>
        private void InitializeDragAndDrop()
        {
            // Subscribe to mouse events for drag and drop
            this.MouseDown += AdvancedFormTopMenuBar_MouseDown;
            this.MouseMove += AdvancedFormTopMenuBar_MouseMove;
            this.MouseUp += AdvancedFormTopMenuBar_MouseUp;

            // Also handle events for the title label to make it draggable
            if (lblFormTitle != null)
            {
                lblFormTitle.MouseDown += AdvancedFormTopMenuBar_MouseDown;
                lblFormTitle.MouseMove += AdvancedFormTopMenuBar_MouseMove;
                lblFormTitle.MouseUp += AdvancedFormTopMenuBar_MouseUp;
            }

            // Handle events for the main panel if it exists
            if (TopMenuPanel != null)
            {
                TopMenuPanel.MouseDown += AdvancedFormTopMenuBar_MouseDown;
                TopMenuPanel.MouseMove += AdvancedFormTopMenuBar_MouseMove;
                TopMenuPanel.MouseUp += AdvancedFormTopMenuBar_MouseUp;
            }

            // Handle events for the table layout panel if it exists
            if (tblpForm != null)
            {
                tblpForm.MouseDown += AdvancedFormTopMenuBar_MouseDown;
                tblpForm.MouseMove += AdvancedFormTopMenuBar_MouseMove;
                tblpForm.MouseUp += AdvancedFormTopMenuBar_MouseUp;
            }
        }


        #endregion

        #region Drag and Drop Event Handlers

        /// <summary>
        /// Handles mouse down event to start drag operation
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Mouse event arguments</param>
        private void AdvancedFormTopMenuBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_enableDragAndDrop || e.Button != MouseButtons.Left)
                return;

            try
            {
                Form parentForm = this.FindForm();
                if (parentForm != null && parentForm.WindowState == FormWindowState.Normal)
                {
                    _isDragging = true;

                    // Store the current mouse position in screen coordinates
                    _lastMousePosition = Control.MousePosition;

                    // Capture mouse input
                    this.Capture = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MouseDown: {ex.Message}");
                ResetDragState();
            }
        }

        /// <summary>
        /// Handles mouse move event to perform drag operation
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Mouse event arguments</param>
        private void AdvancedFormTopMenuBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || !_enableDragAndDrop)
                return;

            try
            {
                Form parentForm = this.FindForm();
                if (parentForm != null && parentForm.WindowState == FormWindowState.Normal)
                {
                    // Get current mouse position in screen coordinates
                    Point currentMousePosition = Control.MousePosition;

                    // Calculate the difference from the last position
                    int deltaX = currentMousePosition.X - _lastMousePosition.X;
                    int deltaY = currentMousePosition.Y - _lastMousePosition.Y;

                    // Update form location
                    Point newLocation = new Point(
                        parentForm.Location.X + deltaX,
                        parentForm.Location.Y + deltaY
                    );

                    // Constrain to screen bounds
                    newLocation = ConstrainToScreen(newLocation, parentForm.Size);

                    // Update form position
                    parentForm.Location = newLocation;

                    // Update last mouse position for next move event
                    _lastMousePosition = currentMousePosition;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MouseMove: {ex.Message}");
                ResetDragState();
            }
        }

        /// <summary>
        /// Handles mouse up event to end drag operation
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">Mouse event arguments</param>
        private void AdvancedFormTopMenuBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            try
            {
                ResetDragState();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MouseUp: {ex.Message}");
                ResetDragState();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the background color of all related controls
        /// </summary>
        private void UpdateBackgroundColor()
        {
            if (TopMenuPanel != null && btnClose != null && btnMinimize != null)
            {
                TopMenuPanel.GradientEndColor = _backgroundColor;
                TopMenuPanel.GradientStartColor = _backgroundColor;
                btnClose.BackgroundColor = _backgroundColor;
                btnMinimize.BackgroundColor = _backgroundColor;
            }
        }

        /// <summary>
        /// Resets the drag state and restores normal cursor
        /// </summary>
        private void ResetDragState()
        {
            _isDragging = false;
            this.Capture = false;
            _lastMousePosition = Point.Empty;
        }

        /// <summary>
        /// Constrains the form position to stay within screen bounds
        /// </summary>
        /// <param name="location">Desired form location</param>
        /// <param name="formSize">Size of the form</param>
        /// <returns>Constrained location within screen bounds</returns>
        private Point ConstrainToScreen(Point location, Size formSize)
        {
            try
            {
                Screen currentScreen = Screen.FromPoint(location);
                Rectangle workingArea = currentScreen.WorkingArea;

                // Ensure form doesn't go off the left edge
                if (location.X < workingArea.Left)
                    location.X = workingArea.Left;

                // Ensure form doesn't go off the right edge
                if (location.X + formSize.Width > workingArea.Right)
                    location.X = workingArea.Right - formSize.Width;

                // Ensure form doesn't go off the top edge
                if (location.Y < workingArea.Top)
                    location.Y = workingArea.Top;

                // Ensure form doesn't go off the bottom edge
                if (location.Y + formSize.Height > workingArea.Bottom)
                    location.Y = workingArea.Bottom - formSize.Height;

                return location;
            }
            catch
            {
                // Return original location if screen detection fails
                return location;
            }
        }

        /// <summary>
        /// Determines whether the ShowMinimizeButton property should be serialized
        /// </summary>
        private bool ShouldSerializeShowMinimizeButton()
        {
            return ShowMinimizeButton != true;
        }

        /// <summary>
        /// Resets the ShowMinimizeButton property to its default value
        /// </summary>
        private void ResetShowMinimizeButton()
        {
            ShowMinimizeButton = true;
        }

        /// <summary>
        /// Determines whether the BackgroundColor property should be serialized
        /// </summary>
        private bool ShouldSerializeBackgroundColor()
        {
            return _backgroundColor != DEFAULT_BACKGROUND_COLOR;
        }

        /// <summary>
        /// Resets the BackgroundColor property to its default value
        /// </summary>
        private void ResetBackgroundColor()
        {
            BackgroundColor = DEFAULT_BACKGROUND_COLOR;
        }

        /// <summary>
        /// Determines whether the EnableDragAndDrop property should be serialized
        /// </summary>
        private bool ShouldSerializeEnableDragAndDrop()
        {
            return _enableDragAndDrop != true;
        }

        /// <summary>
        /// Resets the EnableDragAndDrop property to its default value
        /// </summary>
        private void ResetEnableDragAndDrop()
        {
            EnableDragAndDrop = true;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles minimize button click
        /// </summary>
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                parentForm.WindowState = FormWindowState.Minimized;
            }
        }

        /// <summary>
        /// Handles close button click
        /// </summary>
        private void btnCloseForm_Click(object sender, EventArgs e)
        {
            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                parentForm.DialogResult = DialogResult.Cancel;
                parentForm.Close();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Programmatically starts a drag operation
        /// </summary>
        /// <param name="startPoint">The starting point for the drag operation in screen coordinates</param>
        public void StartDrag(Point startPoint)
        {
            if (!_enableDragAndDrop) return;

            Form parentForm = this.FindForm();
            if (parentForm != null && parentForm.WindowState == FormWindowState.Normal)
            {
                _isDragging = true;
                _lastMousePosition = startPoint;
                this.Capture = true;
            }
        }

        /// <summary>
        /// Programmatically stops the current drag operation
        /// </summary>
        public void StopDrag()
        {
            ResetDragState();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the disposal of the control and cleanup of event handlers
        /// </summary>
        /// <param name="disposing">True if disposing managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Cleanup event handlers
                this.MouseDown -= AdvancedFormTopMenuBar_MouseDown;
                this.MouseMove -= AdvancedFormTopMenuBar_MouseMove;
                this.MouseUp -= AdvancedFormTopMenuBar_MouseUp;

                if (lblFormTitle != null)
                {
                    lblFormTitle.MouseDown -= AdvancedFormTopMenuBar_MouseDown;
                    lblFormTitle.MouseMove -= AdvancedFormTopMenuBar_MouseMove;
                    lblFormTitle.MouseUp -= AdvancedFormTopMenuBar_MouseUp;
                }

                if (TopMenuPanel != null)
                {
                    TopMenuPanel.MouseDown -= AdvancedFormTopMenuBar_MouseDown;
                    TopMenuPanel.MouseMove -= AdvancedFormTopMenuBar_MouseMove;
                    TopMenuPanel.MouseUp -= AdvancedFormTopMenuBar_MouseUp;
                }

                if (tblpForm != null)
                {
                    tblpForm.MouseDown -= AdvancedFormTopMenuBar_MouseDown;
                    tblpForm.MouseMove -= AdvancedFormTopMenuBar_MouseMove;
                    tblpForm.MouseUp -= AdvancedFormTopMenuBar_MouseUp;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}