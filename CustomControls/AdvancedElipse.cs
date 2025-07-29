using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace EasyWinFormLibrary.CustomControls
{
    [ToolboxItem(true)]
    [Description("High-performance rounded corners using Windows API")]
    public class AdvancedElipse : Component
    {
        #region Windows API Declarations
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,      // x-coordinate of upper-left corner
            int nTopRect,       // y-coordinate of upper-left corner  
            int nRightRect,     // x-coordinate of lower-right corner
            int nBottomRect,    // y-coordinate of lower-right corner
            int nWidthEllipse,  // width of ellipse
            int nHeightEllipse  // height of ellipse
        );

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        private static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);

        // Region combine modes
        private const int RGN_AND = 1;
        private const int RGN_OR = 2;
        private const int RGN_XOR = 3;
        private const int RGN_DIFF = 4;
        private const int RGN_COPY = 5;
        #endregion

        #region Fields
        private Control _targetControl;
        private int _radius = 10;
        private bool _enabled = true;
        private IntPtr _currentRegion = IntPtr.Zero;
        private Size _lastSize = Size.Empty;
        private bool _useAdvancedCorners = false;
        private CornerStyle _cornerStyle = CornerStyle.All;
        private bool _cornerChangeEnabled = true;

        // Cache for better performance
        private static readonly Dictionary<string, IntPtr> _regionCache = new Dictionary<string, IntPtr>();
        private const int MAX_CACHE_SIZE = 50;
        #endregion

        #region Enums
        public enum CornerStyle
        {
            All,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Top,
            Bottom,
            Left,
            Right,
            TopLeftBottomRight,
            TopRightBottomLeft
        }
        #endregion

        #region Properties
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
        public AdvancedElipse()
        {
            _radius = 10;
            _enabled = true;
            _cornerStyle = CornerStyle.All;
        }

        public AdvancedElipse(Control targetControl, int radius = 10)
        {
            _radius = radius;
            _enabled = true;
            _cornerStyle = CornerStyle.All;
            TargetControl = targetControl;
        }

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
        private void SubscribeEvents()
        {
            if (_targetControl != null)
            {
                _targetControl.Resize += TargetControl_Resize;
                _targetControl.HandleCreated += TargetControl_HandleCreated;
                _targetControl.HandleDestroyed += TargetControl_HandleDestroyed;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_targetControl != null)
            {
                _targetControl.Resize -= TargetControl_Resize;
                _targetControl.HandleCreated -= TargetControl_HandleCreated;
                _targetControl.HandleDestroyed -= TargetControl_HandleDestroyed;
            }
        }

        private void TargetControl_Resize(object sender, EventArgs e)
        {
            if (_enabled && _targetControl.IsHandleCreated)
            {
                ApplyRoundedCorners();
            }
        }

        private void TargetControl_HandleCreated(object sender, EventArgs e)
        {
            if (_enabled)
            {
                ApplyRoundedCorners();
            }
        }

        private void TargetControl_HandleDestroyed(object sender, EventArgs e)
        {
            CleanupRegion();
        }
        #endregion

        #region Core Methods
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

        private void RemoveRoundedCorners()
        {
            if (_targetControl != null && _targetControl.IsHandleCreated)
            {
                SetWindowRgn(_targetControl.Handle, IntPtr.Zero, true);
                _targetControl.Invalidate();
            }
            CleanupRegion();
        }

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
        /// Applies rounded corners to the target control
        /// </summary>
        public void Apply()
        {
            ApplyRoundedCorners();
        }

        /// <summary>
        /// Removes rounded corners from the target control
        /// </summary>
        public void Remove()
        {
            RemoveRoundedCorners();
        }

        /// <summary>
        /// Forces a refresh of the rounded corners effect
        /// </summary>
        public void Refresh()
        {
            _lastSize = Size.Empty; // Force recreation
            ApplyRoundedCorners();
        }

        /// <summary>
        /// Sets the corner style and applies changes
        /// </summary>
        /// <param name="cornerStyle">The corner style to apply</param>
        public void SetCornerStyle(CornerStyle cornerStyle)
        {
            RoundedCorners = cornerStyle;
        }

        /// <summary>
        /// Sets the radius and applies changes immediately
        /// </summary>
        /// <param name="radius">The radius value</param>
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
        /// Clears the region cache to free memory
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
        /// Gets the current cache size
        /// </summary>
        /// <returns>Number of cached regions</returns>
        public static int GetCacheSize()
        {
            return _regionCache.Count;
        }

        /// <summary>
        /// Gets cache statistics
        /// </summary>
        /// <returns>Cache information</returns>
        public static string GetCacheInfo()
        {
            return $"Cache Size: {_regionCache.Count}/{MAX_CACHE_SIZE}";
        }
        #endregion

        #region Dispose
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

        ~AdvancedElipse()
        {
            CleanupRegion();
        }
        #endregion
    }
}
