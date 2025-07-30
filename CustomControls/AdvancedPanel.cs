using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace EasyWinFormLibrary.CustomControls
{
    #region Enums

    /// <summary>
    /// 3D border styles for the panel
    /// </summary>
    public enum BorderStyle3D
    {
        /// <summary>No 3D border</summary>
        None,
        /// <summary>Raised 3D border</summary>
        Raised,
        /// <summary>Sunken 3D border</summary>
        Sunken,
        /// <summary>Etched 3D border</summary>
        Etched,
        /// <summary>Bump 3D border</summary>
        Bump
    }

    /// <summary>
    /// Preset style options
    /// </summary>
    public enum PresetStyle
    {
        Modern,
        Classic,
        Flat,
        Card,
        Neon
    }

    /// <summary>
    /// Border sides enumeration with flags support
    /// </summary>
    [Flags]
    public enum AdvancedPanelBorderSides
    {
        /// <summary>No borders</summary>
        None = 0,
        /// <summary>Top border</summary>
        Top = 1,
        /// <summary>Right border</summary>
        Right = 2,
        /// <summary>Bottom border</summary>
        Bottom = 4,
        /// <summary>Left border</summary>
        Left = 8,
        /// <summary>All borders</summary>
        All = Top | Right | Bottom | Left
    }

    #endregion

    /// <summary>
    /// High-performance Advanced Panel control using Windows API for rounded corners and visual effects.
    /// Supports rounded corners, gradient backgrounds, custom borders, shadow effects, and 3D styling.
    /// Optimized for .NET Framework 4.8 with hardware acceleration and intelligent caching.
    /// </summary>
    [ToolboxItem(true)]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design")]
    public class AdvancedPanel : Panel
    {
        #region Windows API Declarations
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        private static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);

        // Region combine modes
        private const int RGN_COPY = 5;
        #endregion

        #region Private Fields

        private int _borderRadius = 0;
        private int _borderThickness = 1;
        private Color _borderColor = Color.Gray;
        private Color _gradientStartColor = Color.White;
        private Color _gradientEndColor = Color.LightGray;
        private LinearGradientMode _gradientDirection = LinearGradientMode.Vertical;
        private bool _useGradient = false;
        private bool _enableShadow = false;
        private Color _shadowColor = Color.FromArgb(50, 0, 0, 0);
        private int _shadowOffset = 3;
        private int _shadowBlur = 5;
        private BorderStyle3D _borderStyle = BorderStyle3D.None;
        private Color _highlightColor = Color.White;
        private Color _shadowBorderColor = Color.Gray;
        private AdvancedPanelBorderSides _AdvancedPanelBorderSides = AdvancedPanelBorderSides.All;

        // Performance optimization fields
        private IntPtr _currentRegion = IntPtr.Zero;
        private Size _lastSize = Size.Empty;
        private int _lastRadius = -1;
        private bool _regionUpdateRequired = true;

        // Region caching for better performance
        private static readonly Dictionary<string, IntPtr> _regionCache = new Dictionary<string, IntPtr>();
        private const int MAX_CACHE_SIZE = 30;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the radius of the panel's corners
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The radius of the panel's rounded corners")]
        [DefaultValue(0)]
        public int BorderRadius
        {
            get { return _borderRadius; }
            set
            {
                if (value < 0) value = 0;
                if (value > Math.Min(Width, Height) / 2) value = Math.Min(Width, Height) / 2;

                if (_borderRadius != value)
                {
                    _borderRadius = value;
                    _regionUpdateRequired = true;
                    UpdateRegionIfNeeded();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the panel's border
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The thickness of the panel's border")]
        [DefaultValue(1)]
        public int BorderThickness
        {
            get { return _borderThickness; }
            set
            {
                if (value < 0) value = 0;
                if (value > 20) value = 20;

                if (_borderThickness != value)
                {
                    _borderThickness = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the panel's border
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The color of the panel's border")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to use gradient background
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Enables gradient background instead of solid color")]
        [DefaultValue(false)]
        public bool UseGradient
        {
            get { return _useGradient; }
            set
            {
                if (_useGradient != value)
                {
                    _useGradient = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the start color of the gradient
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The starting color of the gradient background")]
        [DefaultValue(typeof(Color), "White")]
        public Color GradientStartColor
        {
            get { return _gradientStartColor; }
            set
            {
                if (_gradientStartColor != value)
                {
                    _gradientStartColor = value;
                    if (_useGradient) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the end color of the gradient
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The ending color of the gradient background")]
        [DefaultValue(typeof(Color), "LightGray")]
        public Color GradientEndColor
        {
            get { return _gradientEndColor; }
            set
            {
                if (_gradientEndColor != value)
                {
                    _gradientEndColor = value;
                    if (_useGradient) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the direction of the gradient
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The direction of the gradient background")]
        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode GradientDirection
        {
            get { return _gradientDirection; }
            set
            {
                if (_gradientDirection != value)
                {
                    _gradientDirection = value;
                    if (_useGradient) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to enable drop shadow effect
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Enables drop shadow effect for the panel")]
        [DefaultValue(false)]
        public bool EnableShadow
        {
            get { return _enableShadow; }
            set
            {
                if (_enableShadow != value)
                {
                    _enableShadow = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the drop shadow
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The color of the drop shadow")]
        [DefaultValue(typeof(Color), "50, 0, 0, 0")]
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set
            {
                if (_shadowColor != value)
                {
                    _shadowColor = value;
                    if (_enableShadow) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the offset distance of the drop shadow
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The offset distance of the drop shadow")]
        [DefaultValue(3)]
        public int ShadowOffset
        {
            get { return _shadowOffset; }
            set
            {
                if (value < 0) value = 0;
                if (value > 20) value = 20;

                if (_shadowOffset != value)
                {
                    _shadowOffset = value;
                    if (_enableShadow) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the blur radius of the drop shadow
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The blur radius of the drop shadow")]
        [DefaultValue(5)]
        public int ShadowBlur
        {
            get { return _shadowBlur; }
            set
            {
                if (value < 0) value = 0;
                if (value > 20) value = 20;

                if (_shadowBlur != value)
                {
                    _shadowBlur = value;
                    if (_enableShadow) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the 3D border style
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The 3D border style of the panel")]
        [DefaultValue(BorderStyle3D.None)]
        public BorderStyle3D BorderStyle3D
        {
            get { return _borderStyle; }
            set
            {
                if (_borderStyle != value)
                {
                    _borderStyle = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the highlight color for 3D borders
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The highlight color for 3D border effects")]
        [DefaultValue(typeof(Color), "White")]
        public Color HighlightColor
        {
            get { return _highlightColor; }
            set
            {
                if (_highlightColor != value)
                {
                    _highlightColor = value;
                    if (_borderStyle != BorderStyle3D.None) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the shadow color for 3D borders
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The shadow color for 3D border effects")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color ShadowBorderColor
        {
            get { return _shadowBorderColor; }
            set
            {
                if (_shadowBorderColor != value)
                {
                    _shadowBorderColor = value;
                    if (_borderStyle != BorderStyle3D.None) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets which sides of the border to draw
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Specifies which sides of the border to draw")]
        [DefaultValue(AdvancedPanelBorderSides.All)]
        public AdvancedPanelBorderSides AdvancedPanelBorderSides
        {
            get { return _AdvancedPanelBorderSides; }
            set
            {
                if (_AdvancedPanelBorderSides != value)
                {
                    _AdvancedPanelBorderSides = value;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedPanel class
        /// </summary>
        public AdvancedPanel()
        {
            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            // Set default properties
            BackColor = Color.Transparent;
            Size = new Size(200, 100);
        }

        #endregion

        #region High-Performance Region Management

        /// <summary>
        /// IMPROVED: Updates the control's region using Windows API for better performance
        /// </summary>
        private void UpdateRegionIfNeeded()
        {
            if (!IsHandleCreated || (!_regionUpdateRequired && Size == _lastSize && _borderRadius == _lastRadius))
                return;

            try
            {
                CleanupCurrentRegion();

                if (_borderRadius > 0 && Width > 0 && Height > 0)
                {
                    // Ensure we have a reasonable radius
                    int effectiveRadius = Math.Min(_borderRadius, Math.Min(Width / 2, Height / 2));

                    if (effectiveRadius > 0)
                    {
                        // Try to get from cache first
                        string cacheKey = $"{Width}x{Height}x{effectiveRadius}";

                        IntPtr hRgn = IntPtr.Zero;

                        if (_regionCache.ContainsKey(cacheKey))
                        {
                            // Create a copy of cached region
                            IntPtr cachedRgn = _regionCache[cacheKey];
                            hRgn = CreateRectRgn(0, 0, 0, 0);
                            CombineRgn(hRgn, cachedRgn, IntPtr.Zero, RGN_COPY);
                        }
                        else
                        {
                            // Create new rounded region using Windows API
                            // IMPROVED: Better ellipse sizing for cleaner corners
                            int ellipseSize = effectiveRadius * 2;
                            hRgn = CreateRoundRectRgn(0, 0, Width + 1, Height + 1, ellipseSize, ellipseSize);

                            // Cache the region if cache isn't full
                            if (_regionCache.Count < MAX_CACHE_SIZE && hRgn != IntPtr.Zero)
                            {
                                IntPtr cacheRgn = CreateRectRgn(0, 0, 0, 0);
                                CombineRgn(cacheRgn, hRgn, IntPtr.Zero, RGN_COPY);
                                _regionCache[cacheKey] = cacheRgn;
                            }
                        }

                        if (hRgn != IntPtr.Zero)
                        {
                            // Apply the region using Windows API
                            SetWindowRgn(Handle, hRgn, true);
                            _currentRegion = hRgn;
                        }
                    }
                }
                else
                {
                    // Remove region for rectangular panels
                    SetWindowRgn(Handle, IntPtr.Zero, true);
                }

                _lastSize = Size;
                _lastRadius = _borderRadius;
                _regionUpdateRequired = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating region: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleans up the current region
        /// </summary>
        private void CleanupCurrentRegion()
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

        #region Overridden Methods

        /// <summary>
        /// Handles the paint event to draw custom appearance
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Ensure region is up to date
            UpdateRegionIfNeeded();

            // Use high-quality rendering
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            // Calculate drawing rectangle (account for shadow if enabled)
            Rectangle drawRect = GetDrawingRectangle();

            // Draw shadow first if enabled
            if (_enableShadow)
            {
                DrawShadowOptimized(e.Graphics, drawRect);
            }

            // Draw background
            DrawBackgroundOptimized(e.Graphics, drawRect);

            // Draw border
            DrawBorderOptimized(e.Graphics, drawRect);

            // Draw 3D effects if enabled
            if (_borderStyle != BorderStyle3D.None)
            {
                Draw3DBorder(e.Graphics, drawRect);
            }
        }

        /// <summary>
        /// Handles the resize event to update border radius constraints
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Ensure border radius doesn't exceed panel dimensions
            int maxRadius = Math.Min(Width, Height) / 2;
            if (_borderRadius > maxRadius)
            {
                _borderRadius = maxRadius;
            }

            _regionUpdateRequired = true;
            UpdateRegionIfNeeded();
            Invalidate();
        }

        /// <summary>
        /// UPDATED: Handle creation to ensure region is set with better timing
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _regionUpdateRequired = true;

            // Use BeginInvoke to ensure the handle is fully created
            if (IsHandleCreated)
            {
                BeginInvoke(new MethodInvoker(() => {
                    UpdateRegionIfNeeded();
                    Invalidate();
                }));
            }
        }

        /// <summary>
        /// Handle destruction to clean up resources
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            CleanupCurrentRegion();
            base.OnHandleDestroyed(e);
        }

        #endregion

        #region Optimized Drawing Methods

        /// <summary>
        /// Gets the rectangle used for drawing, accounting for shadows and borders
        /// </summary>
        /// <returns>Drawing rectangle</returns>
        private Rectangle GetDrawingRectangle()
        {
            Rectangle rect = ClientRectangle;

            // Account for shadow
            if (_enableShadow)
            {
                rect.Width -= _shadowOffset + _shadowBlur;
                rect.Height -= _shadowOffset + _shadowBlur;
            }

            // FIXED: Proper border thickness handling
            if (_borderThickness > 0)
            {
                // Ensure we have proper space for the border by using half thickness as inset
                float halfBorder = _borderThickness / 2.0f;
                int inset = (int)Math.Ceiling(halfBorder);

                rect.X += inset;
                rect.Y += inset;
                rect.Width -= inset * 2;
                rect.Height -= inset * 2;

                // Ensure minimum size
                if (rect.Width < 1) rect.Width = 1;
                if (rect.Height < 1) rect.Height = 1;
            }

            return rect;
        }

        /// <summary>
        /// Optimized shadow drawing with reduced GDI+ calls
        /// </summary>
        private void DrawShadowOptimized(Graphics graphics, Rectangle drawRect)
        {
            // Create shadow rectangle
            Rectangle shadowRect = new Rectangle(
                drawRect.X + _shadowOffset,
                drawRect.Y + _shadowOffset,
                drawRect.Width,
                drawRect.Height
            );

            // Use simplified shadow drawing for better performance
            int blurSteps = Math.Min(_shadowBlur, 8); // Limit blur steps for performance

            for (int i = 0; i < blurSteps; i++)
            {
                int alpha = Math.Max(1, _shadowColor.A * (blurSteps - i) / blurSteps);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha / 2, _shadowColor)))
                {
                    Rectangle blurRect = new Rectangle(
                        shadowRect.X - i,
                        shadowRect.Y - i,
                        shadowRect.Width + (i * 2),
                        shadowRect.Height + (i * 2)
                    );

                    if (_borderRadius > 0)
                    {
                        using (var shadowPath = CreateRoundedRectanglePath(blurRect, _borderRadius + i))
                        {
                            graphics.FillPath(shadowBrush, shadowPath);
                        }
                    }
                    else
                    {
                        graphics.FillRectangle(shadowBrush, blurRect);
                    }
                }
            }
        }

        /// <summary>
        /// Optimized background drawing
        /// </summary>
        private void DrawBackgroundOptimized(Graphics graphics, Rectangle drawRect)
        {
            if (drawRect.Width <= 0 || drawRect.Height <= 0) return;

            if (_useGradient)
            {
                // Draw gradient background
                using (var gradientBrush = new LinearGradientBrush(drawRect, _gradientStartColor, _gradientEndColor, _gradientDirection))
                {
                    if (_borderRadius > 0)
                    {
                        using (var backgroundPath = CreateRoundedRectanglePath(drawRect, _borderRadius))
                        {
                            graphics.FillPath(gradientBrush, backgroundPath);
                        }
                    }
                    else
                    {
                        graphics.FillRectangle(gradientBrush, drawRect);
                    }
                }
            }
            else if (BackColor != Color.Transparent)
            {
                // Draw solid background - use Windows API region clipping for rounded corners
                using (var backgroundBrush = new SolidBrush(BackColor))
                {
                    if (_borderRadius > 0)
                    {
                        using (var backgroundPath = CreateRoundedRectanglePath(drawRect, _borderRadius))
                        {
                            graphics.FillPath(backgroundBrush, backgroundPath);
                        }
                    }
                    else
                    {
                        graphics.FillRectangle(backgroundBrush, drawRect);
                    }
                }
            }
        }

        /// <summary>
        /// FIXED: Optimized border drawing with proper pen alignment and custom sides support
        /// </summary>
        private void DrawBorderOptimized(Graphics graphics, Rectangle drawRect)
        {
            if (_borderThickness <= 0 || _AdvancedPanelBorderSides == AdvancedPanelBorderSides.None) return;

            // CRITICAL FIX: Set pen alignment to center to ensure even border thickness
            using (var borderPen = new Pen(_borderColor, _borderThickness))
            {
                borderPen.Alignment = PenAlignment.Center; // This is the key fix!

                if (_borderRadius > 0)
                {
                    // For rounded rectangles with custom sides
                    DrawRoundedBorderWithSides(graphics, drawRect, borderPen);
                }
                else
                {
                    // For rectangular borders with custom sides
                    DrawRectangularBorderWithSides(graphics, drawRect, borderPen);
                }
            }
        }

        /// <summary>
        /// Draws rounded border with specific sides
        /// </summary>
        private void DrawRoundedBorderWithSides(Graphics graphics, Rectangle drawRect, Pen borderPen)
        {
            Rectangle borderRect = drawRect;

            // Expand the rectangle slightly to ensure the border is drawn properly
            if (_borderThickness > 1)
            {
                float halfPen = _borderThickness / 2.0f;
                int adjust = (int)Math.Floor(halfPen);
                borderRect.X -= adjust;
                borderRect.Y -= adjust;
                borderRect.Width += adjust * 2;
                borderRect.Height += adjust * 2;
            }

            if (_AdvancedPanelBorderSides == AdvancedPanelBorderSides.All)
            {
                // Draw complete rounded rectangle
                using (var borderPath = CreateRoundedRectanglePath(borderRect, _borderRadius))
                {
                    graphics.DrawPath(borderPen, borderPath);
                }
            }
            else
            {
                // Draw custom rounded border sides
                DrawCustomRoundedAdvancedPanelBorderSides(graphics, borderRect, borderPen);
            }
        }

        /// <summary>
        /// Draws rectangular border with specific sides
        /// </summary>
        private void DrawRectangularBorderWithSides(Graphics graphics, Rectangle drawRect, Pen borderPen)
        {
            Rectangle borderRect = drawRect;

            // Adjust for pen width to ensure even borders
            if (_borderThickness > 1)
            {
                float halfPen = _borderThickness / 2.0f;
                int adjust = (int)Math.Floor(halfPen);
                borderRect.X -= adjust;
                borderRect.Y -= adjust;
                borderRect.Width += adjust * 2;
                borderRect.Height += adjust * 2;
            }

            if (_AdvancedPanelBorderSides == AdvancedPanelBorderSides.All)
            {
                graphics.DrawRectangle(borderPen, borderRect);
            }
            else
            {
                // Draw individual sides
                if (_AdvancedPanelBorderSides.HasFlag(AdvancedPanelBorderSides.Top))
                {
                    graphics.DrawLine(borderPen, borderRect.Left, borderRect.Top, borderRect.Right, borderRect.Top);
                }
                if (_AdvancedPanelBorderSides.HasFlag(AdvancedPanelBorderSides.Right))
                {
                    graphics.DrawLine(borderPen, borderRect.Right, borderRect.Top, borderRect.Right, borderRect.Bottom);
                }
                if (_AdvancedPanelBorderSides.HasFlag(AdvancedPanelBorderSides.Bottom))
                {
                    graphics.DrawLine(borderPen, borderRect.Left, borderRect.Bottom, borderRect.Right, borderRect.Bottom);
                }
                if (_AdvancedPanelBorderSides.HasFlag(AdvancedPanelBorderSides.Left))
                {
                    graphics.DrawLine(borderPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom);
                }
            }
        }

        /// <summary>
        /// Draws custom rounded border sides using arcs and lines
        /// </summary>
        private void DrawCustomRoundedAdvancedPanelBorderSides(Graphics graphics, Rectangle rect, Pen borderPen)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            int effectiveRadius = Math.Min(_borderRadius, Math.Min(rect.Width / 2, rect.Height / 2));
            if (effectiveRadius <= 1)
            {
                // Fall back to rectangular drawing if radius is too small
                DrawRectangularBorderWithSides(graphics, rect, borderPen);
                return;
            }

            int diameter = effectiveRadius * 2;

            // Define corner arc rectangles
            Rectangle topLeftArc = new Rectangle(rect.X, rect.Y, diameter, diameter);
            Rectangle topRightArc = new Rectangle(rect.Right - diameter, rect.Y, diameter, diameter);
            Rectangle bottomRightArc = new Rectangle(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter);
            Rectangle bottomLeftArc = new Rectangle(rect.X, rect.Bottom - diameter, diameter, diameter);

            // Draw corners and sides based on AdvancedPanelBorderSides flags
            bool drawTop = _AdvancedPanelBorderSides.HasFlag(AdvancedPanelBorderSides.Top);
            bool drawRight = _AdvancedPanelBorderSides.HasFlag(AdvancedPanelBorderSides.Right);
            bool drawBottom = _AdvancedPanelBorderSides.HasFlag(AdvancedPanelBorderSides.Bottom);
            bool drawLeft = _AdvancedPanelBorderSides.HasFlag(AdvancedPanelBorderSides.Left);

            // Top side and corners
            if (drawTop)
            {
                // Top line
                graphics.DrawLine(borderPen, rect.Left + effectiveRadius, rect.Top, rect.Right - effectiveRadius, rect.Top);

                // Top-left corner arc (if left side is also drawn)
                if (drawLeft)
                {
                    graphics.DrawArc(borderPen, topLeftArc, 180, 90);
                }
                else
                {
                    // Just the top part of the arc
                    graphics.DrawArc(borderPen, topLeftArc, 180, 45);
                }

                // Top-right corner arc (if right side is also drawn)
                if (drawRight)
                {
                    graphics.DrawArc(borderPen, topRightArc, 270, 90);
                }
                else
                {
                    // Just the top part of the arc
                    graphics.DrawArc(borderPen, topRightArc, 315, 45);
                }
            }

            // Right side
            if (drawRight)
            {
                // Right line
                graphics.DrawLine(borderPen, rect.Right, rect.Top + effectiveRadius, rect.Right, rect.Bottom - effectiveRadius);

                // Top-right corner arc (if not already drawn)
                if (!drawTop)
                {
                    graphics.DrawArc(borderPen, topRightArc, 270, 45);
                }

                // Bottom-right corner arc (if bottom side is also drawn)
                if (drawBottom)
                {
                    graphics.DrawArc(borderPen, bottomRightArc, 0, 90);
                }
                else
                {
                    // Just the right part of the arc
                    graphics.DrawArc(borderPen, bottomRightArc, 315, 45);
                }
            }

            // Bottom side
            if (drawBottom)
            {
                // Bottom line
                graphics.DrawLine(borderPen, rect.Left + effectiveRadius, rect.Bottom, rect.Right - effectiveRadius, rect.Bottom);

                // Bottom-right corner arc (if not already drawn)
                if (!drawRight)
                {
                    graphics.DrawArc(borderPen, bottomRightArc, 0, 45);
                }

                // Bottom-left corner arc (if left side is also drawn)
                if (drawLeft)
                {
                    graphics.DrawArc(borderPen, bottomLeftArc, 90, 90);
                }
                else
                {
                    // Just the bottom part of the arc
                    graphics.DrawArc(borderPen, bottomLeftArc, 90, 45);
                }
            }

            // Left side
            if (drawLeft)
            {
                // Left line
                graphics.DrawLine(borderPen, rect.Left, rect.Top + effectiveRadius, rect.Left, rect.Bottom - effectiveRadius);

                // Top-left corner arc (if not already drawn)
                if (!drawTop)
                {
                    graphics.DrawArc(borderPen, topLeftArc, 225, 45);
                }

                // Bottom-left corner arc (if not already drawn)
                if (!drawBottom)
                {
                    graphics.DrawArc(borderPen, bottomLeftArc, 135, 45);
                }
            }
        }

        /// <summary>
        /// Draws 3D border effects
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="drawRect">Drawing rectangle</param>
        private void Draw3DBorder(Graphics graphics, Rectangle drawRect)
        {
            if (_borderStyle == BorderStyle3D.None) return;

            Color lightColor = _highlightColor;
            Color darkColor = _shadowBorderColor;

            // Adjust colors based on border style
            switch (_borderStyle)
            {
                case BorderStyle3D.Raised:
                    DrawRaised3DBorder(graphics, drawRect, lightColor, darkColor);
                    break;
                case BorderStyle3D.Sunken:
                    DrawSunken3DBorder(graphics, drawRect, lightColor, darkColor);
                    break;
                case BorderStyle3D.Etched:
                    DrawEtched3DBorder(graphics, drawRect, lightColor, darkColor);
                    break;
                case BorderStyle3D.Bump:
                    DrawBump3DBorder(graphics, drawRect, lightColor, darkColor);
                    break;
            }
        }

        /// <summary>
        /// Draws a raised 3D border effect
        /// </summary>
        private void DrawRaised3DBorder(Graphics graphics, Rectangle rect, Color lightColor, Color darkColor)
        {
            using (var lightPen = new Pen(lightColor))
            using (var darkPen = new Pen(darkColor))
            {
                // Top and left edges (light)
                graphics.DrawLine(lightPen, rect.Left, rect.Top, rect.Right - 1, rect.Top);
                graphics.DrawLine(lightPen, rect.Left, rect.Top, rect.Left, rect.Bottom - 1);

                // Bottom and right edges (dark)
                graphics.DrawLine(darkPen, rect.Right - 1, rect.Top + 1, rect.Right - 1, rect.Bottom - 1);
                graphics.DrawLine(darkPen, rect.Left + 1, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
            }
        }

        /// <summary>
        /// Draws a sunken 3D border effect
        /// </summary>
        private void DrawSunken3DBorder(Graphics graphics, Rectangle rect, Color lightColor, Color darkColor)
        {
            using (var lightPen = new Pen(lightColor))
            using (var darkPen = new Pen(darkColor))
            {
                // Top and left edges (dark)
                graphics.DrawLine(darkPen, rect.Left, rect.Top, rect.Right - 1, rect.Top);
                graphics.DrawLine(darkPen, rect.Left, rect.Top, rect.Left, rect.Bottom - 1);

                // Bottom and right edges (light)
                graphics.DrawLine(lightPen, rect.Right - 1, rect.Top + 1, rect.Right - 1, rect.Bottom - 1);
                graphics.DrawLine(lightPen, rect.Left + 1, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
            }
        }

        /// <summary>
        /// Draws an etched 3D border effect
        /// </summary>
        private void DrawEtched3DBorder(Graphics graphics, Rectangle rect, Color lightColor, Color darkColor)
        {
            Rectangle outerRect = rect;
            Rectangle innerRect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);

            DrawSunken3DBorder(graphics, outerRect, lightColor, darkColor);
            DrawRaised3DBorder(graphics, innerRect, lightColor, darkColor);
        }

        /// <summary>
        /// Draws a bump 3D border effect
        /// </summary>
        private void DrawBump3DBorder(Graphics graphics, Rectangle rect, Color lightColor, Color darkColor)
        {
            Rectangle outerRect = rect;
            Rectangle innerRect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);

            DrawRaised3DBorder(graphics, outerRect, lightColor, darkColor);
            DrawSunken3DBorder(graphics, innerRect, lightColor, darkColor);
        }

        /// <summary>
        /// IMPROVED: Creates a graphics path for a rounded rectangle with better precision
        /// </summary>
        /// <param name="rect">Rectangle to create path for</param>
        /// <param name="radius">Corner radius</param>
        /// <returns>Graphics path for rounded rectangle</returns>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();

            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Ensure radius doesn't exceed rectangle dimensions
            int effectiveRadius = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));

            if (effectiveRadius <= 1)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = effectiveRadius * 2;

            // IMPROVED: More precise arc calculations
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc  
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets gradient colors in one method call
        /// </summary>
        /// <param name="startColor">Start color of gradient</param>
        /// <param name="endColor">End color of gradient</param>
        /// <param name="direction">Direction of gradient</param>
        public void SetGradient(Color startColor, Color endColor, LinearGradientMode direction = LinearGradientMode.Vertical)
        {
            _gradientStartColor = startColor;
            _gradientEndColor = endColor;
            _gradientDirection = direction;
            _useGradient = true;
            Invalidate();
        }

        /// <summary>
        /// Sets border properties in one method call
        /// </summary>
        /// <param name="radius">Border radius</param>
        /// <param name="thickness">Border thickness</param>
        /// <param name="color">Border color</param>
        /// <param name="sides">Which sides to draw (optional, defaults to All)</param>
        public void SetBorder(int radius, int thickness, Color color, AdvancedPanelBorderSides sides = AdvancedPanelBorderSides.All)
        {
            bool radiusChanged = _borderRadius != Math.Max(0, radius);

            _borderRadius = Math.Max(0, radius);
            _borderThickness = Math.Max(0, thickness);
            _borderColor = color;
            _AdvancedPanelBorderSides = sides;

            if (radiusChanged)
            {
                _regionUpdateRequired = true;
                UpdateRegionIfNeeded();
            }

            Invalidate();
        }

        /// <summary>
        /// Sets which sides of the border to draw
        /// </summary>
        /// <param name="sides">Border sides to draw</param>
        public void SetAdvancedPanelBorderSides(AdvancedPanelBorderSides sides)
        {
            if (_AdvancedPanelBorderSides != sides)
            {
                _AdvancedPanelBorderSides = sides;
                Invalidate();
            }
        }

        /// <summary>
        /// Sets shadow properties in one method call
        /// </summary>
        /// <param name="enabled">Whether shadow is enabled</param>
        /// <param name="color">Shadow color</param>
        /// <param name="offset">Shadow offset</param>
        /// <param name="blur">Shadow blur radius</param>
        public void SetShadow(bool enabled, Color color, int offset = 3, int blur = 5)
        {
            _enableShadow = enabled;
            _shadowColor = color;
            _shadowOffset = Math.Max(0, offset);
            _shadowBlur = Math.Max(0, blur);
            Invalidate();
        }

        /// <summary>
        /// Forces region update - useful for troubleshooting
        /// </summary>
        public void RefreshRegion()
        {
            _regionUpdateRequired = true;
            UpdateRegionIfNeeded();
            Invalidate();
        }

        /// <summary>
        /// Applies a preset style to the panel
        /// </summary>
        /// <param name="style">Preset style to apply</param>
        public void ApplyPresetStyle(PresetStyle style)
        {
            switch (style)
            {
                case PresetStyle.Modern:
                    ApplyModernStyle();
                    break;
                case PresetStyle.Classic:
                    ApplyClassicStyle();
                    break;
                case PresetStyle.Flat:
                    ApplyFlatStyle();
                    break;
                case PresetStyle.Card:
                    ApplyCardStyle();
                    break;
                case PresetStyle.Neon:
                    ApplyNeonStyle();
                    break;
            }
        }

        #endregion

        #region Static Cache Management

        /// <summary>
        /// Clears the region cache to free memory
        /// </summary>
        public static void ClearRegionCache()
        {
            foreach (var region in _regionCache.Values)
            {
                DeleteObject(region);
            }
            _regionCache.Clear();
        }

        /// <summary>
        /// Gets the current region cache size
        /// </summary>
        /// <returns>Number of cached regions</returns>
        public static int GetRegionCacheSize()
        {
            return _regionCache.Count;
        }

        /// <summary>
        /// Gets region cache information
        /// </summary>
        /// <returns>Cache statistics</returns>
        public static string GetRegionCacheInfo()
        {
            return $"Region Cache: {_regionCache.Count}/{MAX_CACHE_SIZE}";
        }

        #endregion

        #region Preset Styles

        /// <summary>
        /// Applies modern style preset
        /// </summary>
        private void ApplyModernStyle()
        {
            SetBorder(12, 1, Color.FromArgb(200, 200, 200));
            SetGradient(Color.FromArgb(250, 250, 250), Color.FromArgb(240, 240, 240), LinearGradientMode.Vertical);
            SetShadow(true, Color.FromArgb(30, 0, 0, 0), 3, 6);
            _borderStyle = BorderStyle3D.None;
            Invalidate();
        }

        /// <summary>
        /// Applies classic style preset
        /// </summary>
        private void ApplyClassicStyle()
        {
            SetBorder(0, 2, Color.Gray);
            _useGradient = false;
            BackColor = SystemColors.Control;
            _enableShadow = false;
            _borderStyle = BorderStyle3D.Raised;
            _highlightColor = Color.White;
            _shadowBorderColor = Color.Gray;
            Invalidate();
        }

        /// <summary>
        /// Applies flat style preset
        /// </summary>
        private void ApplyFlatStyle()
        {
            SetBorder(0, 1, Color.FromArgb(180, 180, 180));
            _useGradient = false;
            BackColor = Color.White;
            _enableShadow = false;
            _borderStyle = BorderStyle3D.None;
            Invalidate();
        }

        /// <summary>
        /// Applies card style preset
        /// </summary>
        private void ApplyCardStyle()
        {
            SetBorder(10, 0, Color.Transparent);
            _useGradient = false;
            BackColor = Color.White;
            SetShadow(true, Color.FromArgb(25, 0, 0, 0), 4, 10);
            _borderStyle = BorderStyle3D.None;
            Invalidate();
        }

        /// <summary>
        /// Applies neon style preset
        /// </summary>
        private void ApplyNeonStyle()
        {
            SetBorder(18, 3, Color.FromArgb(0, 255, 255));
            SetGradient(Color.FromArgb(20, 20, 40), Color.FromArgb(40, 40, 80), LinearGradientMode.Vertical);
            SetShadow(true, Color.FromArgb(120, 0, 255, 255), 0, 12);
            _borderStyle = BorderStyle3D.None;
            Invalidate();
        }

        #endregion

        #region Designer Support and Cleanup

        /// <summary>
        /// Provides design-time support for the control
        /// </summary>
        protected override bool DoubleBuffered
        {
            get { return true; }
            set { base.DoubleBuffered = value; }
        }

        /// <summary>
        /// Clean up any resources being used
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CleanupCurrentRegion();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finalizer to ensure native resources are cleaned up
        /// </summary>
        ~AdvancedPanel()
        {
            CleanupCurrentRegion();
        }

        #endregion
    }
}