using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// A component that enables drag and drop functionality to move the parent form 
    /// by dragging any target control. Perfect for custom title bars and moveable panels.
    /// </summary>
    [ToolboxItem(true)]
    [Description("Enables drag and drop functionality to move the parent form by dragging a target control")]
    public class AdvancedFormDragDrop : Component
    {
        #region Windows API Declarations

        /// <summary>
        /// Windows API constant for left mouse button down message
        /// </summary>
        private const int WM_NCLBUTTONDOWN = 0xA1;

        /// <summary>
        /// Windows API constant for window caption area
        /// </summary>
        private const int HT_CAPTION = 0x2;

        /// <summary>
        /// Windows API function to release mouse capture
        /// </summary>
        /// <returns>True if successful</returns>
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        /// <summary>
        /// Windows API function to send messages to windows
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="Msg">Message to send</param>
        /// <param name="wParam">Additional message parameter</param>
        /// <param name="lParam">Additional message parameter</param>
        /// <returns>Result of message processing</returns>
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        #endregion

        #region Private Fields

        private Control _targetControl;
        private Form _parentForm;
        private bool _isDragging;
        private Point _lastCursorPosition;
        private Point _lastFormPosition;
        private bool _enableDragDrop = true;
        private bool _constrainToScreen = true;
        private Rectangle _dragBounds = Rectangle.Empty;
        private bool _showDragCursor = true;
        private Cursor _originalCursor;
        private bool _enableMaximizedDrag = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the control that will act as the drag handle for the form
        /// </summary>
        [Category("Behavior")]
        [Description("The control that will act as the drag handle for the form")]
        [DefaultValue(null)]
        public Control TargetControl
        {
            get { return _targetControl; }
            set
            {
                if (_targetControl != value)
                {
                    // Unsubscribe from old control events
                    UnsubscribeFromControlEvents();

                    _targetControl = value;

                    // Subscribe to new control events
                    SubscribeToControlEvents();

                    // Update parent form reference
                    UpdateParentForm();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether drag and drop functionality is enabled
        /// </summary>
        [Category("Behavior")]
        [Description("Enables or disables the drag and drop functionality")]
        [DefaultValue(true)]
        public bool EnableDragDrop
        {
            get { return _enableDragDrop; }
            set { _enableDragDrop = value; }
        }

        /// <summary>
        /// Gets or sets whether the form should be constrained to stay within screen bounds
        /// </summary>
        [Category("Behavior")]
        [Description("Constrains the form to stay within screen bounds during dragging")]
        [DefaultValue(true)]
        public bool ConstrainToScreen
        {
            get { return _constrainToScreen; }
            set { _constrainToScreen = value; }
        }

        /// <summary>
        /// Gets or sets custom bounds for constraining the form movement. 
        /// If empty, screen bounds will be used when ConstrainToScreen is true.
        /// </summary>
        [Category("Behavior")]
        [Description("Custom bounds for constraining form movement. Leave empty to use screen bounds.")]
        public Rectangle DragBounds
        {
            get { return _dragBounds; }
            set { _dragBounds = value; }
        }

        /// <summary>
        /// Gets or sets whether to show a move cursor during dragging
        /// </summary>
        [Category("Appearance")]
        [Description("Shows a move cursor when hovering over the drag control")]
        [DefaultValue(true)]
        public bool ShowDragCursor
        {
            get { return _showDragCursor; }
            set { _showDragCursor = value; }
        }

        /// <summary>
        /// Gets or sets whether dragging is enabled when the form is maximized.
        /// When true, dragging will restore the form to normal size first.
        /// </summary>
        [Category("Behavior")]
        [Description("Enables dragging when form is maximized (will restore to normal size first)")]
        [DefaultValue(true)]
        public bool EnableMaximizedDrag
        {
            get { return _enableMaximizedDrag; }
            set { _enableMaximizedDrag = value; }
        }

        /// <summary>
        /// Gets whether the form is currently being dragged
        /// </summary>
        [Browsable(false)]
        public bool IsDragging
        {
            get { return _isDragging; }
        }

        /// <summary>
        /// Gets the parent form of the target control
        /// </summary>
        [Browsable(false)]
        public Form ParentForm
        {
            get { return _parentForm; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when dragging starts
        /// </summary>
        [Category("Action")]
        [Description("Occurs when dragging starts")]
        public event EventHandler DragStarted;

        /// <summary>
        /// Occurs during dragging
        /// </summary>
        [Category("Action")]
        [Description("Occurs during dragging")]
        public event EventHandler<AdvancedFormDragDropEventArgs> Dragging;

        /// <summary>
        /// Occurs when dragging ends
        /// </summary>
        [Category("Action")]
        [Description("Occurs when dragging ends")]
        public event EventHandler DragEnded;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the FormDragDropComponent
        /// </summary>
        public AdvancedFormDragDrop()
        {
            // Component initialization
        }

        /// <summary>
        /// Initializes a new instance of the FormDragDropComponent with a target control
        /// </summary>
        /// <param name="targetControl">The control to use as drag handle</param>
        public AdvancedFormDragDrop(Control targetControl) : this()
        {
            TargetControl = targetControl;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enables drag functionality for the specified control
        /// </summary>
        /// <param name="control">The control to enable dragging for</param>
        public void EnableDragFor(Control control)
        {
            TargetControl = control;
        }

        /// <summary>
        /// Disables drag functionality
        /// </summary>
        public void DisableDrag()
        {
            TargetControl = null;
        }

        /// <summary>
        /// Manually starts dragging (useful for custom scenarios)
        /// </summary>
        public void StartDrag()
        {
            if (_parentForm != null && _enableDragDrop)
            {
                BeginDrag();
            }
        }

        /// <summary>
        /// Sets custom drag bounds
        /// </summary>
        /// <param name="bounds">The bounds to constrain movement to</param>
        public void SetDragBounds(Rectangle bounds)
        {
            _dragBounds = bounds;
        }

        /// <summary>
        /// Clears custom drag bounds (will use screen bounds if ConstrainToScreen is true)
        /// </summary>
        public void ClearDragBounds()
        {
            _dragBounds = Rectangle.Empty;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the parent form reference
        /// </summary>
        private void UpdateParentForm()
        {
            if (_targetControl != null)
            {
                _parentForm = _targetControl.FindForm();
            }
            else
            {
                _parentForm = null;
            }
        }

        /// <summary>
        /// Subscribes to target control events
        /// </summary>
        private void SubscribeToControlEvents()
        {
            if (_targetControl != null)
            {
                _targetControl.MouseDown += TargetControl_MouseDown;
                _targetControl.MouseMove += TargetControl_MouseMove;
                _targetControl.MouseUp += TargetControl_MouseUp;

                if (_showDragCursor)
                {
                    _targetControl.MouseEnter += TargetControl_MouseEnter;
                    _targetControl.MouseLeave += TargetControl_MouseLeave;
                }
            }
        }

        /// <summary>
        /// Unsubscribes from target control events
        /// </summary>
        private void UnsubscribeFromControlEvents()
        {
            if (_targetControl != null)
            {
                _targetControl.MouseDown -= TargetControl_MouseDown;
                _targetControl.MouseMove -= TargetControl_MouseMove;
                _targetControl.MouseUp -= TargetControl_MouseUp;
                _targetControl.MouseEnter -= TargetControl_MouseEnter;
                _targetControl.MouseLeave -= TargetControl_MouseLeave;

                // Restore original cursor
                if (_originalCursor != null)
                {
                    _targetControl.Cursor = _originalCursor;
                    _originalCursor = null;
                }
            }
        }

        /// <summary>
        /// Begins the drag operation using Windows API (most reliable method)
        /// </summary>
        private void BeginDrag()
        {
            if (_parentForm != null)
            {
                try
                {
                    // Handle maximized/fullscreen forms
                    if (_parentForm.WindowState == FormWindowState.Maximized)
                    {
                        RestoreAndRepositionForm();
                    }

                    // Only proceed with drag if form is now in normal state
                    if (_parentForm.WindowState == FormWindowState.Normal)
                    {
                        ReleaseCapture();
                        SendMessage(_parentForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in BeginDrag: {ex.Message}");
                    // Fallback to manual dragging
                    BeginManualDrag();
                }
            }
        }

        /// <summary>
        /// Begins manual drag operation (fallback method)
        /// </summary>
        private void BeginManualDrag()
        {
            if (_parentForm != null)
            {
                // Handle maximized/fullscreen forms
                if (_parentForm.WindowState == FormWindowState.Maximized)
                {
                    RestoreAndRepositionForm();
                }

                // Only start dragging if form is in normal state
                if (_parentForm.WindowState == FormWindowState.Normal)
                {
                    _isDragging = true;
                    _lastCursorPosition = Cursor.Position;
                    _lastFormPosition = _parentForm.Location;

                    // Raise drag started event
                    OnDragStarted();
                }
            }
        }

        /// <summary>
        /// Processes manual dragging
        /// </summary>
        private void ProcessManualDrag()
        {
            if (_isDragging && _parentForm != null)
            {
                Point currentCursorPosition = Cursor.Position;
                Point delta = new Point(
                    currentCursorPosition.X - _lastCursorPosition.X,
                    currentCursorPosition.Y - _lastCursorPosition.Y
                );

                Point newLocation = new Point(
                    _lastFormPosition.X + delta.X,
                    _lastFormPosition.Y + delta.Y
                );

                // Apply constraints if enabled
                if (_constrainToScreen || !_dragBounds.IsEmpty)
                {
                    newLocation = ConstrainLocation(newLocation);
                }

                _parentForm.Location = newLocation;

                // Raise dragging event
                OnDragging(new AdvancedFormDragDropEventArgs(newLocation, delta));
            }
        }

        /// <summary>
        /// Constrains the form location to specified bounds
        /// </summary>
        /// <param name="location">Proposed location</param>
        /// <returns>Constrained location</returns>
        private Point ConstrainLocation(Point location)
        {
            Rectangle bounds = _dragBounds.IsEmpty ? Screen.FromControl(_parentForm).WorkingArea : _dragBounds;

            // Ensure form stays within bounds
            int x = Math.Max(bounds.Left, Math.Min(location.X, bounds.Right - _parentForm.Width));
            int y = Math.Max(bounds.Top, Math.Min(location.Y, bounds.Bottom - _parentForm.Height));

            return new Point(x, y);
        }

        /// <summary>
        /// Restores a maximized form and positions it under the cursor for dragging
        /// </summary>
        private void RestoreAndRepositionForm()
        {
            if (_parentForm == null || _parentForm.WindowState != FormWindowState.Maximized)
                return;

            try
            {
                // Get current cursor position
                Point cursorPosition = Cursor.Position;

                // Store the maximized bounds for reference
                Rectangle maximizedBounds = _parentForm.Bounds;

                // Calculate the relative position of cursor in the maximized form
                Point relativeCursor = _parentForm.PointToClient(cursorPosition);
                double relativeX = (double)relativeCursor.X / maximizedBounds.Width;
                double relativeY = (double)relativeCursor.Y / maximizedBounds.Height;

                // Restore the form to normal state
                _parentForm.WindowState = FormWindowState.Normal;

                // Wait for the form to actually restore (sometimes needed)
                Application.DoEvents();

                // Calculate new position so the cursor is at the same relative position
                int newX = cursorPosition.X - (int)(_parentForm.Width * relativeX);
                int newY = cursorPosition.Y - (int)(_parentForm.Height * relativeY);

                // Apply constraints if enabled
                Point newLocation = new Point(newX, newY);
                if (_constrainToScreen || !_dragBounds.IsEmpty)
                {
                    newLocation = ConstrainLocation(newLocation);
                }

                // Set the new position
                _parentForm.Location = newLocation;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RestoreAndRepositionForm: {ex.Message}");
                // Fallback: just restore to normal state
                _parentForm.WindowState = FormWindowState.Normal;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles mouse down event on target control
        /// </summary>
        private void TargetControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _enableDragDrop)
            {
                UpdateParentForm(); // Ensure we have the correct parent form

                // Check if we should handle maximized forms
                if (_parentForm != null && _parentForm.WindowState == FormWindowState.Maximized && !_enableMaximizedDrag)
                {
                    return; // Don't start drag if maximized drag is disabled
                }

                BeginDrag();
            }
        }

        /// <summary>
        /// Handles mouse move event on target control
        /// </summary>
        private void TargetControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                ProcessManualDrag();
            }
        }

        /// <summary>
        /// Handles mouse up event on target control
        /// </summary>
        private void TargetControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                OnDragEnded();
            }
        }

        /// <summary>
        /// Handles mouse enter event for cursor change
        /// </summary>
        private void TargetControl_MouseEnter(object sender, EventArgs e)
        {
            if (_showDragCursor && _targetControl != null)
            {
                _originalCursor = _targetControl.Cursor;
                _targetControl.Cursor = Cursors.SizeAll;
            }
        }

        /// <summary>
        /// Handles mouse leave event for cursor restoration
        /// </summary>
        private void TargetControl_MouseLeave(object sender, EventArgs e)
        {
            if (_targetControl != null && _originalCursor != null)
            {
                _targetControl.Cursor = _originalCursor;
            }
        }

        #endregion

        #region Event Raising Methods

        /// <summary>
        /// Raises the DragStarted event
        /// </summary>
        protected virtual void OnDragStarted()
        {
            DragStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the Dragging event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnDragging(AdvancedFormDragDropEventArgs e)
        {
            Dragging?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the DragEnded event
        /// </summary>
        protected virtual void OnDragEnded()
        {
            DragEnded?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Disposes the component and cleans up resources
        /// </summary>
        /// <param name="disposing">True if disposing managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeFromControlEvents();
                _targetControl = null;
                _parentForm = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }

    /// <summary>
    /// Event arguments for form dragging events
    /// </summary>
    public class AdvancedFormDragDropEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the new location of the form
        /// </summary>
        public Point NewLocation { get; private set; }

        /// <summary>
        /// Gets the movement delta
        /// </summary>
        public Point Delta { get; private set; }

        /// <summary>
        /// Initializes a new instance of FormDragEventArgs
        /// </summary>
        /// <param name="newLocation">The new form location</param>
        /// <param name="delta">The movement delta</param>
        public AdvancedFormDragDropEventArgs(Point newLocation, Point delta)
        {
            NewLocation = newLocation;
            Delta = delta;
        }
    }
}