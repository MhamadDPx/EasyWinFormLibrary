using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// High-performance rounded corners component using Windows API.
    /// Provides efficient rounded corner effects for Windows Forms controls with caching and advanced corner styling options.
    /// </summary>
    [ToolboxItem(true)]
    [Description("High-performance rounded corners using Windows API")]
    public class AdvancedElipse : Component
    {
        #region Windows API Declarations

        /// <summary>
        /// Creates a rounded rectangular region.
        /// </summary>
        /// <param name="nLeftRect">x-coordinate of upper-left corner</param>
        /// <param name="nTopRect">y-coordinate of upper-left corner</param>
        /// <param name="nRightRect">x-coordinate of lower-right corner</param>
        /// <param name="nBottomRect">y-coordinate of lower-right corner</param>
        /// <param name="nWidthEllipse">width of ellipse</param>
        /// <param name="nHeightEllipse">height of ellipse</param>
        /// <returns>Handle to the created region</returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,      // x-coordinate of upper-left corner
            int nTopRect,       // y-coordinate of upper-left corner  
            int nRightRect,     // x-coordinate of lower-right corner
            int nBottomRect,    // y-coordinate of lower-right corner
            int nWidthEllipse,  // width of ellipse
            int nHeightEllipse  // height of ellipse
        );

        /// <summary>
        /// Deletes a logical pen, brush, font, bitmap, region, or palette.
        /// </summary>
        /// <param name="hObject">Handle to the object to delete</param>
        /// <returns>True if the function succeeds</returns>
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Sets the window region of a window.
        /// </summary>
        /// <param name="hWnd">Handle to the window</param>
        /// <param name="hRgn">Handle to the region</param>
        /// <param name="bRedraw">Specifies whether the window is redrawn</param>
        /// <returns>Nonzero if the function succeeds</returns>
        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        /// <summary>
        /// Creates a rectangular region.
        /// </summary>
        /// <param name="nLeftRect">x-coordinate of upper-left corner</param>
        /// <param name="nTopRect">y-coordinate of upper-left corner</param>
        /// <param name="nRightRect">x-coordinate of lower-right corner</param>
        /// <param name="nBottomRect">y-coordinate of lower-right corner</param>
        /// <returns>Handle to the created region</returns>
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        /// <summary>
        /// Combines two regions and stores the result in one of them.
        /// </summary>
        /// <param name="hrgnDest">Handle to the destination region</param>
        /// <param name="hrgnSrc1">Handle to the first source region</param>
        /// <param name="hrgnSrc2">Handle to the second source region</param>
        /// <param name="fnCombineMode">Specifies how the regions are combined</param>
        /// <returns>The type of the resulting region</returns>
        [DllImport("gdi32.dll")]
        private static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);

        /// <summary>
        /// Region combine mode: Intersection of the two regions.
        /// </summary>
        private const int RGN_AND = 1;

        /// <summary>
        /// Region combine mode: Union of the two regions.
        /// </summary>
        private const int RGN_OR = 2;

        /// <summary>
        /// Region combine mode: Exclusive OR of the two regions.
        /// </summary>
        private const int RGN_XOR = 3;

        /// <summary>
        /// Region combine mode: Difference of the two regions.
        /// </summary>
        private const int RGN_DIFF = 4;

        /// <summary>
        /// Region combine mode: Copy the first region to the destination.
        /// </summary>
        private const int RGN_COPY = 5;
        #endregion

        #region Fields

        /// <summary>
        /// The control to apply rounded corners to.
        /// </summary>
        private Control _targetControl;

        /// <summary>
        /// The radius of the rounded corners in pixels.
        /// </summary>
        private int _radius = 10;

        /// <summary>
        /// Indicates whether the rounded corners effect is enabled.
        /// </summary>
        private bool _enabled = true;

        /// <summary>
        /// Handle to the current region applied to the control.
        /// </summary>
        private IntPtr _currentRegion = IntPtr.Zero;

        /// <summary>
        /// The last known size of the target control for optimization.
        /// </summary>
        private Size _lastSize = Size.Empty;

        /// <summary>
        /// Indicates whether advanced corner styling is being used.
        /// </summary>
        private bool _useAdvancedCorners = false;

        /// <summary>
        /// The current corner style configuration.
        /// </summary>
        private CornerStyle _cornerStyle = CornerStyle.All;

        /// <summary>
        /// Indicates whether corner style changes are enabled.
        /// </summary>
        private bool _cornerChangeEnabled = true;

        /// <summary>
        /// Cache for storing frequently used regions to improve performance.
        /// </summary>
        private static readonly Dictionary<string, IntPtr> _regionCache = new Dictionary<string, IntPtr>();

        /// <summary>
        /// Maximum number of regions to cache.
        /// </summary>
        private const int MAX_CACHE_SIZE = 50;
        #endregion

        #region Enums

        /// <summary>
        /// Defines which corners of the control should be rounded.
        /// </summary>
        public enum CornerStyle
        {
            /// <summary>
            /// All corners are rounded.
            /// </summary>
            All,

            /// <summary>
            /// Only the top-left corner is rounded.
            /// </summary>
            TopLeft,

            /// <summary>
            /// Only the top-right corner is rounded.
            /// </summary>
            TopRight,

            /// <summary>
            /// Only the bottom-left corner is rounded.
            /// </summary>
            BottomLeft,

            /// <summary>
            /// Only the bottom-right corner is rounded.
            /// </summary>
            BottomRight,

            /// <summary>
            /// Both top corners are rounded.
            /// </summary>
            Top,

            /// <summary>
            /// Both bottom corners are rounded.
            /// </summary>
            Bottom,

            /// <summary>
            /// Both left corners are rounded.
            /// </summary>
            Left,

            /// <summary>
            /// Both right corners are rounded.
            /// </summary>
            Right,

            /// <summary>
            /// Top-left and bottom-right corners are rounded.
            /// </summary>
            TopLeftBottomRight,

            /// <summary>
            /// Top-right and bottom-left corners are rounded.
            /// </summary>
            TopRightBottomLeft
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the control to apply rounded corners to.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The control to apply rounded corners to")]
        public Control TargetControl
        {
            get { return _targetControl; }
            set
            {
                if (_targetControl != null)
                {
                    UnsubscribeEvents();
                    CleanupRegion();
                }

                _targetControl = value;

                if (_targetControl != null)
                {
                    SubscribeEvents();
                    ApplyRoundedCorners();
                }
            }
        }

        /// <summary>
        /// Gets or sets the radius of the rounded corners in pixels.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The radius of the rounded corners")]
        [DefaultValue(10)]
        public int ElipseRadius
        {
            get { return _radius; }
            set
            {
                if (value >= 0 && value != _radius)
                {
                    _radius = value;

                    // Clear cache when radius changes to force recreation
                    _lastSize = Size.Empty;

                    if (_enabled)
                        ApplyRoundedCorners();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the rounded corners effect is enabled.
        /// </summary>
        [Category("Behavior")]
        [Description("Enable or disable the rounded corners effect")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (_enabled)
                        ApplyRoundedCorners();
                    else
                        RemoveRoundedCorners();
                }
            }
        }

        /// <summary>
        /// Gets or sets which corners should be rounded.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Which corners to round")]
        [DefaultValue(CornerStyle.All)]
        public CornerStyle RoundedCorners
        {
            get { return _cornerStyle; }
            set
            {
                if (_cornerStyle != value && _cornerChangeEnabled)
                {
                    _cornerStyle = value;
                    _useAdvancedCorners = value != CornerStyle.All;

                    // Clear cache when corner style changes
                    _lastSize = Size.Empty;

                    if (_enabled)
                        ApplyRoundedCorners();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether corner style changes are enabled.
        /// </summary>
        [Category("Behavior")]
        [Description("Enable or disable corner style changes")]
        [DefaultValue(true)]
        public bool CornerChangeEnabled
        {
            get { return _cornerChangeEnabled; }
            set
            {
                _cornerChangeEnabled = value;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedElipse class with default settings.
        /// </summary>
        public AdvancedElipse()
        {
            _radius = 10;
            _enabled = true;
            _cornerStyle = CornerStyle.All;
        }

        /// <summary>
        /// Initializes a new instance of the AdvancedElipse class with specified target control and radius.
        /// </summary>
        /// <param name="targetControl">The control to apply rounded corners to</param>
        /// <param name="radius">The radius of the rounded corners (default: 10)</param>
        public AdvancedElipse(Control targetControl, int radius = 10)
        {
            _radius = radius;
            _enabled = true;
            _cornerStyle = CornerStyle.All;
            TargetControl = targetControl;
        }

        /// <summary>
        /// Initializes a new instance of the AdvancedElipse class with specified target control, radius, and corner style.
        /// </summary>
        /// <param name="targetControl">The control to apply rounded corners to</param>
        /// <param name="radius">The radius of the rounded corners</param>
        /// <param name="cornerStyle">Which corners to round</param>
        public AdvancedElipse(Control targetControl, int radius, CornerStyle cornerStyle)
        {
            _radius = radius;
            _enabled = true;
            _cornerStyle = cornerStyle;
            _useAdvancedCorners = cornerStyle != CornerStyle.All;
            TargetControl = targetControl;
        }
        #endregion

        #region Event Management

        /// <summary>
        /// Subscribes to necessary events from the target control.
        /// </summary>
        private void SubscribeEvents()
        {
            if (_targetControl != null)
            {
                _targetControl.Resize += TargetControl_Resize;
                _targetControl.HandleCreated += TargetControl_HandleCreated;
                _targetControl.HandleDestroyed += TargetControl_HandleDestroyed;
            }
        }

        /// <summary>
        /// Unsubscribes from events of the target control.
        /// </summary>
        private void UnsubscribeEvents()
        {
            if (_targetControl != null)
            {
                _targetControl.Resize -= TargetControl_Resize;
                _targetControl.HandleCreated -= TargetControl_HandleCreated;
                _targetControl.HandleDestroyed -= TargetControl_HandleDestroyed;
            }
        }

        /// <summary>
        /// Handles the resize event of the target control.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event arguments</param>
        private void TargetControl_Resize(object sender, EventArgs e)
        {
            if (_enabled && _targetControl.IsHandleCreated)
            {
                ApplyRoundedCorners();
            }
        }

        /// <summary>
        /// Handles the handle created event of the target control.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event arguments</param>
        private void TargetControl_HandleCreated(object sender, EventArgs e)
        {
            if (_enabled)
            {
                ApplyRoundedCorners();
            }
        }

        /// <summary>
        /// Handles the handle destroyed event of the target control.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event arguments</param>
        private void TargetControl_HandleDestroyed(object sender, EventArgs e)
        {
            CleanupRegion();
        }
        #endregion

        #region Core Methods

        /// <summary>
        /// Applies rounded corners to the target control using Windows API regions.
        /// </summary>
        private void ApplyRoundedCorners()
        {
            if (_targetControl == null || !_enabled || !_targetControl.IsHandleCreated)
                return;

            try
            {
                // Always cleanup current region first
                CleanupRegion();

                // Check if we can skip recreation (only for simple corners and same size)
                bool canUseCache = !_useAdvancedCorners &&
                                  _targetControl.Size == _lastSize &&
                                  _lastSize != Size.Empty;

                _lastSize = _targetControl.Size;

                IntPtr hRgn = IntPtr.Zero;

                if (_useAdvancedCorners && _cornerChangeEnabled)
                {
                    hRgn = CreateAdvancedRegion();
                }
                else if (_cornerStyle == CornerStyle.All || !_cornerChangeEnabled)
                {
                    // Use cache for simple rounded rectangles
                    string cacheKey = $"{_targetControl.Width}x{_targetControl.Height}x{_radius}";

                    if (canUseCache && _regionCache.ContainsKey(cacheKey))
                    {
                        // Create a copy of cached region
                        IntPtr cachedRgn = _regionCache[cacheKey];
                        hRgn = CreateRectRgn(0, 0, 0, 0);
                        CombineRgn(hRgn, cachedRgn, IntPtr.Zero, RGN_COPY);
                    }
                    else
                    {
                        hRgn = CreateSimpleRoundedRegion();

                        // Cache the region if cache isn't full
                        if (_regionCache.Count < MAX_CACHE_SIZE)
                        {
                            IntPtr cacheRgn = CreateRectRgn(0, 0, 0, 0);
                            CombineRgn(cacheRgn, hRgn, IntPtr.Zero, RGN_COPY);
                            _regionCache[cacheKey] = cacheRgn;
                        }
                    }
                }
                else
                {
                    // Advanced corners but change disabled - use simple
                    hRgn = CreateSimpleRoundedRegion();
                }

                if (hRgn != IntPtr.Zero)
                {
                    // Apply the region to the control
                    SetWindowRgn(_targetControl.Handle, hRgn, true);
                    _currentRegion = hRgn;

                    // Force redraw for better visual update
                    _targetControl.Invalidate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying rounded corners: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a simple rounded rectangle region with all corners rounded.
        /// </summary>
        /// <returns>Handle to the created region</returns>
        private IntPtr CreateSimpleRoundedRegion()
        {
            return CreateRoundRectRgn(
                0, 0,
                _targetControl.Width,
                _targetControl.Height,
                _radius * 2,
                _radius * 2
            );
        }

        /// <summary>
        /// Creates an advanced region with selective corner rounding based on the corner style.
        /// </summary>
        /// <returns>Handle to the created region</returns>
        private IntPtr CreateAdvancedRegion()
        {
            int width = _targetControl.Width;
            int height = _targetControl.Height;
            int diameter = _radius * 2;

            // Start with a rectangular region
            IntPtr resultRgn = CreateRectRgn(0, 0, width, height);

            // Apply rounded corners based on style
            switch (_cornerStyle)
            {
                case CornerStyle.TopLeft:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, true, false, false, false);
                    break;

                case CornerStyle.TopRight:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, false, true, false, false);
                    break;

                case CornerStyle.BottomLeft:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, false, false, true, false);
                    break;

                case CornerStyle.BottomRight:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, false, false, false, true);
                    break;

                case CornerStyle.Top:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, true, true, false, false);
                    break;

                case CornerStyle.Bottom:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, false, false, true, true);
                    break;

                case CornerStyle.Left:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, true, false, true, false);
                    break;

                case CornerStyle.Right:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, false, true, false, true);
                    break;

                case CornerStyle.TopLeftBottomRight:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, true, false, false, true);
                    break;

                case CornerStyle.TopRightBottomLeft:
                    ApplyRoundedCorner(resultRgn, 0, 0, width, height, false, true, true, false);
                    break;
            }

            return resultRgn;
        }

        /// <summary>
        /// Applies selective corner rounding to a region by combining square and rounded regions.
        /// </summary>
        /// <param name="baseRgn">The base region to modify</param>
        /// <param name="x">X coordinate of the rectangle</param>
        /// <param name="y">Y coordinate of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="topLeft">Whether to round the top-left corner</param>
        /// <param name="topRight">Whether to round the top-right corner</param>
        /// <param name="bottomLeft">Whether to round the bottom-left corner</param>
        /// <param name="bottomRight">Whether to round the bottom-right corner</param>
        private void ApplyRoundedCorner(IntPtr baseRgn, int x, int y, int width, int height,
            bool topLeft, bool topRight, bool bottomLeft, bool bottomRight)
        {
            // Create a complex region by combining multiple regions
            IntPtr tempRgn = CreateRectRgn(0, 0, 0, 0);

            // Start with the full rectangle
            IntPtr fullRect = CreateRectRgn(x, y, x + width, y + height);
            CombineRgn(tempRgn, fullRect, IntPtr.Zero, RGN_COPY);

            int diameter = _radius * 2;

            // Subtract square corners and add rounded corners where needed
            if (topLeft)
            {
                // Remove square corner
                IntPtr squareCorner = CreateRectRgn(x, y, x + _radius, y + _radius);
                CombineRgn(tempRgn, tempRgn, squareCorner, RGN_DIFF);

                // Add rounded corner
                IntPtr roundCorner = CreateRoundRectRgn(x, y, x + diameter, y + diameter, diameter, diameter);
                CombineRgn(tempRgn, tempRgn, roundCorner, RGN_OR);

                DeleteObject(squareCorner);
                DeleteObject(roundCorner);
            }

            if (topRight)
            {
                IntPtr squareCorner = CreateRectRgn(x + width - _radius, y, x + width, y + _radius);
                CombineRgn(tempRgn, tempRgn, squareCorner, RGN_DIFF);

                IntPtr roundCorner = CreateRoundRectRgn(x + width - diameter, y, x + width, y + diameter, diameter, diameter);
                CombineRgn(tempRgn, tempRgn, roundCorner, RGN_OR);

                DeleteObject(squareCorner);
                DeleteObject(roundCorner);
            }

            if (bottomLeft)
            {
                IntPtr squareCorner = CreateRectRgn(x, y + height - _radius, x + _radius, y + height);
                CombineRgn(tempRgn, tempRgn, squareCorner, RGN_DIFF);

                IntPtr roundCorner = CreateRoundRectRgn(x, y + height - diameter, x + diameter, y + height, diameter, diameter);
                CombineRgn(tempRgn, tempRgn, roundCorner, RGN_OR);

                DeleteObject(squareCorner);
                DeleteObject(roundCorner);
            }

            if (bottomRight)
            {
                IntPtr squareCorner = CreateRectRgn(x + width - _radius, y + height - _radius, x + width, y + height);
                CombineRgn(tempRgn, tempRgn, squareCorner, RGN_DIFF);

                IntPtr roundCorner = CreateRoundRectRgn(x + width - diameter, y + height - diameter, x + width, y + height, diameter, diameter);
                CombineRgn(tempRgn, tempRgn, roundCorner, RGN_OR);

                DeleteObject(squareCorner);
                DeleteObject(roundCorner);
            }

            // Copy result back to base region
            CombineRgn(baseRgn, tempRgn, IntPtr.Zero, RGN_COPY);

            DeleteObject(fullRect);
            DeleteObject(tempRgn);
        }

        /// <summary>
        /// Removes rounded corners from the target control, restoring its original shape.
        /// </summary>
        private void RemoveRoundedCorners()
        {
            if (_targetControl != null && _targetControl.IsHandleCreated)
            {
                SetWindowRgn(_targetControl.Handle, IntPtr.Zero, true);
                _targetControl.Invalidate();
            }
            CleanupRegion();
        }

        /// <summary>
        /// Cleans up the current region handle to prevent memory leaks.
        /// </summary>
        private void CleanupRegion()
        {
            if (_currentRegion != IntPtr.Zero)
            {
                // Don't delete cached regions
                if (!_regionCache.ContainsValue(_currentRegion))
                {
                    DeleteObject(_currentRegion);
                }
                _currentRegion = IntPtr.Zero;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Applies rounded corners to the target control.
        /// </summary>
        public void Apply()
        {
            ApplyRoundedCorners();
        }

        /// <summary>
        /// Removes rounded corners from the target control.
        /// </summary>
        public void Remove()
        {
            RemoveRoundedCorners();
        }

        /// <summary>
        /// Forces a refresh of the rounded corners effect by clearing the cache and reapplying.
        /// </summary>
        public void Refresh()
        {
            _lastSize = Size.Empty; // Force recreation
            ApplyRoundedCorners();
        }

        /// <summary>
        /// Sets the corner style and applies changes immediately.
        /// </summary>
        /// <param name="cornerStyle">The corner style to apply</param>
        public void SetCornerStyle(CornerStyle cornerStyle)
        {
            RoundedCorners = cornerStyle;
        }

        /// <summary>
        /// Sets the radius and applies changes immediately.
        /// </summary>
        /// <param name="radius">The radius value in pixels</param>
        public void SetRadius(int radius)
        {
            if (radius >= 0 && radius != _radius)
            {
                _radius = radius;

                // Force immediate update by clearing cache state
                _lastSize = Size.Empty;

                if (_enabled)
                    ApplyRoundedCorners();
            }
        }
        #endregion

        #region Static Cache Management

        /// <summary>
        /// Clears the region cache to free memory.
        /// This method should be called periodically or when memory usage needs to be reduced.
        /// </summary>
        public static void ClearCache()
        {
            foreach (var region in _regionCache.Values)
            {
                DeleteObject(region);
            }
            _regionCache.Clear();
        }

        /// <summary>
        /// Gets the current number of cached regions.
        /// </summary>
        /// <returns>Number of cached regions</returns>
        public static int GetCacheSize()
        {
            return _regionCache.Count;
        }

        /// <summary>
        /// Gets cache statistics for monitoring and debugging purposes.
        /// </summary>
        /// <returns>String containing cache information</returns>
        public static string GetCacheInfo()
        {
            return $"Cache Size: {_regionCache.Count}/{MAX_CACHE_SIZE}";
        }
        #endregion

        #region Dispose

        /// <summary>
        /// Releases the unmanaged resources used by the AdvancedElipse and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_targetControl != null)
                {
                    UnsubscribeEvents();
                    RemoveRoundedCorners();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finalizer to ensure regions are cleaned up if Dispose is not called.
        /// </summary>
        ~AdvancedElipse()
        {
            CleanupRegion();
        }
        #endregion
    }
}