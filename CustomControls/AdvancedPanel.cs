using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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

    #endregion
    /// <summary>
    /// Advanced Panel control with custom border styling, gradient backgrounds, and enhanced visual effects.
    /// Supports rounded corners, gradient fills, custom borders, and shadow effects.
    /// Optimized for .NET Framework 4.8 with designer support and comprehensive customization options.
    /// </summary>
    [ToolboxItem(true)]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design")]
    public class AdvancedPanel : Panel
    {

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
                
                _borderRadius = value;
                Invalidate();
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
                
                _borderThickness = value;
                Invalidate();
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
                _borderColor = value;
                Invalidate();
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
                _useGradient = value;
                Invalidate();
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
                _gradientStartColor = value;
                if (_useGradient) Invalidate();
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
                _gradientEndColor = value;
                if (_useGradient) Invalidate();
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
                _gradientDirection = value;
                if (_useGradient) Invalidate();
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
                _enableShadow = value;
                Invalidate();
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
                _shadowColor = value;
                if (_enableShadow) Invalidate();
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
                
                _shadowOffset = value;
                if (_enableShadow) Invalidate();
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
                
                _shadowBlur = value;
                if (_enableShadow) Invalidate();
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
                _borderStyle = value;
                Invalidate();
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
                _highlightColor = value;
                if (_borderStyle != BorderStyle3D.None) Invalidate();
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
                _shadowBorderColor = value;
                if (_borderStyle != BorderStyle3D.None) Invalidate();
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

        #region Overridden Methods

        /// <summary>
        /// Handles the paint event to draw custom appearance
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Don't call base.OnPaint to have full control over rendering
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Calculate drawing rectangle (account for shadow if enabled)
            Rectangle drawRect = GetDrawingRectangle();

            // Draw shadow first if enabled
            if (_enableShadow)
            {
                DrawShadow(e.Graphics, drawRect);
            }

            // Draw background
            DrawBackground(e.Graphics, drawRect);

            // Draw border
            DrawBorder(e.Graphics, drawRect);

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
            
            Invalidate();
        }

        /// <summary>
        /// Creates the region for the panel based on border radius
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateRegion();
        }

        #endregion

        #region Drawing Methods

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
            
            // Account for border thickness
            if (_borderThickness > 0)
            {
                int borderOffset = _borderThickness / 2;
                rect.X += borderOffset;
                rect.Y += borderOffset;
                rect.Width -= _borderThickness;
                rect.Height -= _borderThickness;
            }
            
            return rect;
        }

        /// <summary>
        /// Draws the drop shadow effect
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="drawRect">Drawing rectangle</param>
        private void DrawShadow(Graphics graphics, Rectangle drawRect)
        {
            // Create shadow rectangle
            Rectangle shadowRect = new Rectangle(
                drawRect.X + _shadowOffset,
                drawRect.Y + _shadowOffset,
                drawRect.Width,
                drawRect.Height
            );
            
            // Create shadow brush with gradient for blur effect
            using (var shadowBrush = new SolidBrush(_shadowColor))
            {
                if (_borderRadius > 0)
                {
                    using (var shadowPath = CreateRoundedRectanglePath(shadowRect, _borderRadius))
                    {
                        // Draw multiple shadow layers for blur effect
                        for (int i = 0; i < _shadowBlur; i++)
                        {
                            int alpha = Math.Max(1, _shadowColor.A * (1 - i) / _shadowBlur);
                            using (var blurBrush = new SolidBrush(Color.FromArgb(alpha, _shadowColor)))
                            {
                                Rectangle blurRect = new Rectangle(
                                    shadowRect.X - i,
                                    shadowRect.Y - i,
                                    shadowRect.Width + (i * 2),
                                    shadowRect.Height + (i * 2)
                                );
                                
                                using (var blurPath = CreateRoundedRectanglePath(blurRect, _borderRadius + i))
                                {
                                    graphics.FillPath(blurBrush, blurPath);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Draw rectangular shadow with blur
                    for (int i = 0; i < _shadowBlur; i++)
                    {
                        int alpha = Math.Max(1, _shadowColor.A * (1 - i) / _shadowBlur);
                        using (var blurBrush = new SolidBrush(Color.FromArgb(alpha, _shadowColor)))
                        {
                            Rectangle blurRect = new Rectangle(
                                shadowRect.X - i,
                                shadowRect.Y - i,
                                shadowRect.Width + (i * 2),
                                shadowRect.Height + (i * 2)
                            );
                            graphics.FillRectangle(blurBrush, blurRect);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the background of the panel
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="drawRect">Drawing rectangle</param>
        private void DrawBackground(Graphics graphics, Rectangle drawRect)
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
                // Draw solid background
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
        /// Draws the border of the panel
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="drawRect">Drawing rectangle</param>
        private void DrawBorder(Graphics graphics, Rectangle drawRect)
        {
            if (_borderThickness <= 0) return;

            using (var borderPen = new Pen(_borderColor, _borderThickness))
            {
                if (_borderRadius > 0)
                {
                    using (var borderPath = CreateRoundedRectanglePath(drawRect, _borderRadius))
                    {
                        graphics.DrawPath(borderPen, borderPath);
                    }
                }
                else
                {
                    graphics.DrawRectangle(borderPen, drawRect);
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
        /// Creates a graphics path for a rounded rectangle
        /// </summary>
        /// <param name="rect">Rectangle to create path for</param>
        /// <param name="radius">Corner radius</param>
        /// <returns>Graphics path for rounded rectangle</returns>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);
            
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

        /// <summary>
        /// Updates the control's region based on border radius
        /// </summary>
        private void UpdateRegion()
        {
            if (_borderRadius > 0)
            {
                using (var path = CreateRoundedRectanglePath(ClientRectangle, _borderRadius))
                {
                    Region = new Region(path);
                }
            }
            else
            {
                Region = null;
            }
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
        public void SetBorder(int radius, int thickness, Color color)
        {
            _borderRadius = Math.Max(0, radius);
            _borderThickness = Math.Max(0, thickness);
            _borderColor = color;
            UpdateRegion();
            Invalidate();
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

        #region Preset Styles

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
        /// Applies modern style preset
        /// </summary>
        private void ApplyModernStyle()
        {
            SetBorder(10, 1, Color.FromArgb(200, 200, 200));
            SetGradient(Color.FromArgb(250, 250, 250), Color.FromArgb(240, 240, 240), LinearGradientMode.Vertical);
            SetShadow(true, Color.FromArgb(30, 0, 0, 0), 2, 4);
            _borderStyle = BorderStyle3D.None;
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
        }

        /// <summary>
        /// Applies card style preset
        /// </summary>
        private void ApplyCardStyle()
        {
            SetBorder(8, 0, Color.Transparent);
            _useGradient = false;
            BackColor = Color.White;
            SetShadow(true, Color.FromArgb(25, 0, 0, 0), 4, 8);
            _borderStyle = BorderStyle3D.None;
        }

        /// <summary>
        /// Applies neon style preset
        /// </summary>
        private void ApplyNeonStyle()
        {
            SetBorder(15, 2, Color.FromArgb(0, 255, 255));
            SetGradient(Color.FromArgb(20, 20, 40), Color.FromArgb(40, 40, 80), LinearGradientMode.Vertical);
            SetShadow(true, Color.FromArgb(100, 0, 255, 255), 0, 10);
            _borderStyle = BorderStyle3D.None;
        }

        #endregion

        #region Designer Support

        /// <summary>
        /// Provides design-time support for the control
        /// </summary>
        protected override bool DoubleBuffered
        {
            get { return true; }
            set { base.DoubleBuffered = value; }
        }

        #endregion
    }
}