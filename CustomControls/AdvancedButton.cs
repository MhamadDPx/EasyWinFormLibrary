using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced button control with rounded corners, custom borders, and flexible image positioning.
    /// Provides enhanced styling options including border radius, border size, custom colors,
    /// and precise image alignment with custom sizing capabilities.
    /// </summary>
    [ToolboxItem(true)]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design")]
    public partial class AdvancedButton : Button
    {
        #region Constants

        /// <summary>
        /// Default background color for the button.
        /// </summary>
        private static readonly Color DEFAULT_BACKGROUND_COLOR = Color.White;

        /// <summary>
        /// Default hover background color for the button.
        /// </summary>
        private static readonly Color DEFAULT_HOVER_BACKGROUND_COLOR = Color.WhiteSmoke;

        /// <summary>
        /// Default pressed background color for the button.
        /// </summary>
        private static readonly Color DEFAULT_PRESSED_BACKGROUND_COLOR = Color.Gainsboro;

        /// <summary>
        /// Default text color for the button.
        /// </summary>
        /// 
        private static readonly Color DEFAULT_TEXT_COLOR = Color.FromArgb(40, 40, 40);

        /// <summary>
        /// Default border color for the button.
        /// </summary>
        private static readonly Color DEFAULT_BORDER_COLOR = Color.FromArgb(213, 218, 223);

        /// <summary>
        /// Default hover border color for the button.
        /// </summary>
        private static readonly Color DEFAULT_HOVER_BORDER_COLOR = Color.FromArgb(142, 145, 148);

        /// <summary>
        /// Default border size for the button.
        /// </summary>
        private const int DEFAULT_BORDER_SIZE = 2;

        /// <summary>
        /// Default border radius for the button.
        /// </summary>
        private const int DEFAULT_BORDER_RADIUS = 5;

        #endregion

        #region Private Fields

        /// <summary>
        /// Current border size of the button.
        /// </summary>
        private int borderSize = DEFAULT_BORDER_SIZE;

        /// <summary>
        /// Current border radius of the button corners.
        /// </summary>
        private int borderRadius = DEFAULT_BORDER_RADIUS;

        /// <summary>
        /// Current border color of the button.
        /// </summary>
        private Color borderColor = DEFAULT_BORDER_COLOR;

        /// <summary>
        /// Current background color of the button.
        /// </summary>
        private Color backgroundColor = DEFAULT_BACKGROUND_COLOR;

        /// <summary>
        /// Current text color of the button.
        /// </summary>
        private Color textColor = DEFAULT_TEXT_COLOR;

        /// <summary>
        /// Image to display on the button.
        /// </summary>
        private Image buttonImage = null;

        /// <summary>
        /// Layout style for the button image.
        /// </summary>
        private ImageLayout imageLayout = ImageLayout.Zoom;

        /// <summary>
        /// Alignment position for the button image.
        /// </summary>
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;

        /// <summary>
        /// Size of the button image.
        /// </summary>
        private Size imageSize = new Size(25, 25);

        /// <summary>
        /// Spacing between image and text in pixels.
        /// </summary>
        private int imageTextSpacing = 5;

        /// <summary>
        /// Indicates whether the mouse is hovering over the button.
        /// </summary>
        private bool onHover = false;

        /// <summary>
        /// Background color when the button is in hover state.
        /// </summary>
        private Color hoverBackColor = DEFAULT_HOVER_BACKGROUND_COLOR;

        /// <summary>
        /// Border color when the button is in hover state.
        /// </summary>
        private Color hoverBorderColor = DEFAULT_HOVER_BORDER_COLOR;

        /// <summary>
        /// Background color when the button is pressed.
        /// </summary>
        private Color pressedBackColor = DEFAULT_PRESSED_BACKGROUND_COLOR;

        /// <summary>
        /// Indicates whether the button is currently pressed.
        /// </summary>
        private bool isPressed = false;

        /// <summary>
        /// Indicates whether the control has been fully initialized.
        /// </summary>
        private bool _isInitialized = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the border size of the button. Valid range is 0-20 pixels.
        /// </summary>
        [Category("Advanced Appearance")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                if (value < 0) value = 0;
                if (value > 20) value = 20;

                if (borderSize != value)
                {
                    borderSize = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the border radius for rounded corners. Must not exceed half of the smallest dimension.
        /// </summary>
        [Category("Advanced Appearance")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                if (value < 0) value = 0;
                if (Width > 0 && Height > 0 && value > Math.Min(Width, Height) / 2)
                    value = Math.Min(Width, Height) / 2;

                if (borderRadius != value)
                {
                    borderRadius = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color of the button.
        /// </summary>
        [Category("Advanced Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the button.
        /// </summary>
        [Category("Advanced Appearance")]
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the text color of the button.
        /// </summary>
        [Category("Advanced Appearance")]
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                if (textColor != value)
                {
                    textColor = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the image displayed on the button.
        /// </summary>
        [Category("Advanced Appearance")]
        public Image ButtonImage
        {
            get { return buttonImage; }
            set
            {
                if (buttonImage != value)
                {
                    buttonImage = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the layout style for the button image.
        /// </summary>
        [Category("Advanced Appearance")]
        public ImageLayout ButtonImageLayout
        {
            get { return imageLayout; }
            set
            {
                if (imageLayout != value)
                {
                    imageLayout = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment position of the image on the button.
        /// </summary>
        [Category("Advanced Appearance")]
        public new ContentAlignment ImageAlign
        {
            get { return imageAlign; }
            set
            {
                if (imageAlign != value)
                {
                    imageAlign = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the image displayed on the button.
        /// </summary>
        [Category("Advanced Appearance")]
        public Size ImageSize
        {
            get { return imageSize; }
            set
            {
                if (imageSize != value)
                {
                    imageSize = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the spacing between the image and text in pixels.
        /// </summary>
        [Category("Advanced Appearance")]
        public int ImageTextSpacing
        {
            get { return imageTextSpacing; }
            set
            {
                if (imageTextSpacing != value)
                {
                    imageTextSpacing = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color when the button is in hover state.
        /// </summary>
        [Category("Advanced Appearance")]
        public Color HoverBackColor
        {
            get { return hoverBackColor; }
            set
            {
                if (hoverBackColor != value)
                {
                    hoverBackColor = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color when the button is in hover state.
        /// </summary>
        [Category("Advanced Appearance")]
        public Color HoverBorderColor
        {
            get { return hoverBorderColor; }
            set
            {
                if (hoverBorderColor != value)
                {
                    hoverBorderColor = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color when the button is pressed.
        /// </summary>
        [Category("Advanced Appearance")]
        public Color PressedBackColor
        {
            get { return pressedBackColor; }
            set
            {
                if (pressedBackColor != value)
                {
                    pressedBackColor = value;
                    if (_isInitialized)
                    {
                        this.Invalidate();
                    }
                }
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedButton class with default properties.
        /// </summary>
        public AdvancedButton()
        {
            InitializeComponent();
            SetDefaultProperties();
            _isInitialized = true;
        }

        /// <summary>
        /// Initialize component with optimal rendering settings
        /// </summary>
        private void InitializeComponent()
        {
            // Enable double buffering and optimized rendering (from AdvancedActionButton)
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// Set default properties for the button
        /// </summary>
        private void SetDefaultProperties()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0; // Important for custom border rendering
            this.Size = new Size(150, 40);
            this.BackColor = Color.Transparent; // Let custom drawing handle background
            this.ForeColor = textColor;
            this.Resize += new EventHandler(Button_Resize);
        }
        #endregion

        #region Drawing Methods (from AdvancedActionButton)

        /// <summary>
        /// Draws the custom border and shape of the button (from AdvancedActionButton)
        /// </summary>
        /// <param name="graphics">Graphics object for drawing</param>
        private void DrawCustomBorder(Graphics graphics)
        {
            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = Math.Max(2, borderSize);

            try
            {
                if (borderRadius > 2) // Rounded button
                {
                    DrawRoundedBorder(graphics, rectSurface, rectBorder, smoothSize);
                }
                else // Normal rectangular button
                {
                    DrawRectangularBorder(graphics, rectSurface, smoothSize);
                }
            }
            catch
            {
                DrawSimpleBorder(graphics, rectSurface);
            }
        }

        /// <summary>
        /// Draws a rounded border for the button (from AdvancedActionButton)
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="rectSurface">Surface rectangle</param>
        /// <param name="rectBorder">Border rectangle</param>
        /// <param name="smoothSize">Smoothing size</param>
        private void DrawRoundedBorder(Graphics graphics, Rectangle rectSurface, Rectangle rectBorder, int smoothSize)
        {
            using (GraphicsPath pathSurface = CreateRoundedRectanglePath(rectSurface, borderRadius))
            using (GraphicsPath pathBorder = CreateRoundedRectanglePath(rectBorder, Math.Max(0, borderRadius - borderSize)))
            using (Pen penSurface = new Pen(Parent?.BackColor ?? SystemColors.Control, smoothSize))
            using (Pen penBorder = new Pen(onHover ? hoverBorderColor : borderColor, borderSize))
            {
                // Set button region
                Region = new Region(pathSurface);

                // Draw surface border for smooth result
                graphics.DrawPath(penSurface, pathSurface);

                // Button background
                Color currentBackColor = backgroundColor;
                if (isPressed)
                    currentBackColor = pressedBackColor;
                else if (onHover)
                    currentBackColor = hoverBackColor;

                using (SolidBrush brushSurface = new SolidBrush(currentBackColor))
                {
                    graphics.FillPath(brushSurface, pathSurface);
                }

                // Draw control border
                if (borderSize >= 1)
                {
                    graphics.DrawPath(penBorder, pathBorder);
                }
            }
        }

        /// <summary>
        /// Draws a rectangular border for the button (from AdvancedActionButton)
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="rectSurface">Surface rectangle</param>
        /// <param name="smoothSize">Smoothing size</param>
        private void DrawRectangularBorder(Graphics graphics, Rectangle rectSurface, int smoothSize)
        {
            graphics.SmoothingMode = SmoothingMode.None;

            // Set button region
            Region = new Region(rectSurface);

            // Button background
            Color currentBackColor = backgroundColor;
            if (isPressed)
                currentBackColor = pressedBackColor;
            else if (onHover)
                currentBackColor = hoverBackColor;

            using (SolidBrush brushSurface = new SolidBrush(currentBackColor))
            {
                graphics.FillRectangle(brushSurface, rectSurface);
            }

            // Draw border
            if (borderSize >= 1)
            {
                using (Pen penBorder = new Pen(onHover ? hoverBorderColor : borderColor, borderSize))
                {
                    penBorder.Alignment = PenAlignment.Inset;
                    graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
                }
            }
        }

        /// <summary>
        /// Draws a simple border as fallback (from AdvancedActionButton)
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="rect">Rectangle to draw</param>
        private void DrawSimpleBorder(Graphics graphics, Rectangle rect)
        {
            // Button background
            Color currentBackColor = backgroundColor;
            if (isPressed)
                currentBackColor = pressedBackColor;
            else if (onHover)
                currentBackColor = hoverBackColor;

            using (SolidBrush brush = new SolidBrush(currentBackColor))
            {
                graphics.FillRectangle(brush, rect);
            }

            if (borderSize >= 1)
            {
                using (Pen pen = new Pen(onHover ? hoverBorderColor : borderColor, borderSize))
                {
                    graphics.DrawRectangle(pen, rect);
                }
            }
        }

        /// <summary>
        /// Creates a graphics path for a rounded rectangle (from AdvancedActionButton)
        /// </summary>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="radius">Corner radius</param>
        /// <returns>Graphics path for the rounded rectangle</returns>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            float curveSize = Math.Min(radius * 2F, Math.Min(rect.Width, rect.Height));

            try
            {
                path.StartFigure();
                path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
                path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
                path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
                path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
                path.CloseFigure();
            }
            catch
            {
                path.Reset();
                path.AddRectangle(rect);
            }

            return path;
        }

        #endregion

        #region Image and Text Drawing Methods

        /// <summary>
        /// Calculates and returns the rectangle bounds for the button image based on alignment settings.
        /// </summary>
        /// <returns>Rectangle defining the image position and size.</returns>
        private Rectangle GetImageRectangle()
        {
            Rectangle imageRect = new Rectangle();

            if (buttonImage == null)
                return imageRect;

            int imgWidth = imageSize.Width;
            int imgHeight = imageSize.Height;

            // Calculate position based on alignment
            switch (imageAlign)
            {
                case ContentAlignment.TopLeft:
                    imageRect = new Rectangle(borderSize + 5, borderSize + 5, imgWidth, imgHeight);
                    break;
                case ContentAlignment.TopCenter:
                    imageRect = new Rectangle((this.Width - imgWidth) / 2, borderSize + 5, imgWidth, imgHeight);
                    break;
                case ContentAlignment.TopRight:
                    imageRect = new Rectangle(this.Width - imgWidth - borderSize - 5, borderSize + 5, imgWidth, imgHeight);
                    break;
                case ContentAlignment.MiddleLeft:
                    imageRect = new Rectangle(borderSize + 5, (this.Height - imgHeight) / 2, imgWidth, imgHeight);
                    break;
                case ContentAlignment.MiddleCenter:
                    imageRect = new Rectangle((this.Width - imgWidth) / 2, (this.Height - imgHeight) / 2, imgWidth, imgHeight);
                    break;
                case ContentAlignment.MiddleRight:
                    imageRect = new Rectangle(this.Width - imgWidth - borderSize - 5, (this.Height - imgHeight) / 2, imgWidth, imgHeight);
                    break;
                case ContentAlignment.BottomLeft:
                    imageRect = new Rectangle(borderSize + 5, this.Height - imgHeight - borderSize - 5, imgWidth, imgHeight);
                    break;
                case ContentAlignment.BottomCenter:
                    imageRect = new Rectangle((this.Width - imgWidth) / 2, this.Height - imgHeight - borderSize - 5, imgWidth, imgHeight);
                    break;
                case ContentAlignment.BottomRight:
                    imageRect = new Rectangle(this.Width - imgWidth - borderSize - 5, this.Height - imgHeight - borderSize - 5, imgWidth, imgHeight);
                    break;
            }

            return imageRect;
        }

        /// <summary>
        /// Calculates and returns the rectangle bounds for the button text based on image position and spacing.
        /// </summary>
        /// <returns>Rectangle defining the text position and size.</returns>
        private Rectangle GetTextRectangle()
        {
            Rectangle textRect = new Rectangle();
            Rectangle imageRect = GetImageRectangle();

            if (buttonImage == null)
            {
                textRect = new Rectangle(borderSize, borderSize,
                    this.Width - (borderSize * 2), this.Height - (borderSize * 2));
            }
            else
            {
                // Adjust text position based on image position
                switch (imageAlign)
                {
                    case ContentAlignment.MiddleLeft:
                        textRect = new Rectangle(imageRect.Right + imageTextSpacing, borderSize,
                            this.Width - imageRect.Right - imageTextSpacing - borderSize, this.Height - (borderSize * 2));
                        break;
                    case ContentAlignment.MiddleRight:
                        textRect = new Rectangle(borderSize, borderSize,
                            imageRect.Left - imageTextSpacing - borderSize, this.Height - (borderSize * 2));
                        break;
                    case ContentAlignment.TopCenter:
                        textRect = new Rectangle(borderSize, imageRect.Bottom + imageTextSpacing,
                            this.Width - (borderSize * 2), this.Height - imageRect.Bottom - imageTextSpacing - borderSize);
                        break;
                    case ContentAlignment.BottomCenter:
                        textRect = new Rectangle(borderSize, borderSize,
                            this.Width - (borderSize * 2), imageRect.Top - imageTextSpacing - borderSize);
                        break;
                    default:
                        textRect = new Rectangle(borderSize, borderSize,
                            this.Width - (borderSize * 2), this.Height - (borderSize * 2));
                        break;
                }
            }

            return textRect;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Handle creation event - safe initialization
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            try
            {
                base.OnHandleCreated(e);

                // Ensure all required objects are initialized
                if (!_isInitialized)
                {
                    _isInitialized = true;
                }

                // Safe parent event subscription
                if (this.Parent != null)
                {
                    this.Parent.BackColorChanged += new EventHandler(Container_BackColorChanged);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnHandleCreated: {ex.Message}");
                // Don't throw, continue with safe defaults
            }
        }

        /// <summary>
        /// Handles the paint event to draw custom appearance (updated with AdvancedActionButton style)
        /// </summary>
        /// <param name="pevent">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (!_isInitialized)
                return;

            try
            {
                // Set high-quality rendering (from AdvancedActionButton)
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Draw custom border and shape using AdvancedActionButton method
                DrawCustomBorder(pevent.Graphics);

                // Draw image
                if (buttonImage != null)
                {
                    Rectangle imageRect = GetImageRectangle();
                    pevent.Graphics.DrawImage(buttonImage, imageRect);
                }

                // Draw text
                if (!string.IsNullOrEmpty(this.Text))
                {
                    Rectangle textRect = GetTextRectangle();
                    using (SolidBrush brushText = new SolidBrush(textColor))
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;
                        pevent.Graphics.DrawString(this.Text, this.Font, brushText, textRect, stringFormat);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnPaint: {ex.Message}");
                // Continue with base painting if custom painting fails
            }
        }

        /// <summary>
        /// Handles the mouse enter event to trigger hover state.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            onHover = true;
            if (_isInitialized)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles the mouse leave event to reset hover and pressed states.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            onHover = false;
            isPressed = false;
            if (_isInitialized)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles the mouse down event to trigger pressed state.
        /// </summary>
        /// <param name="mevent">Mouse event arguments</param>
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            isPressed = true;
            if (_isInitialized)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles the mouse up event to reset pressed state.
        /// </summary>
        /// <param name="mevent">Mouse event arguments</param>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            if (_isInitialized)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles the resize event to update border radius constraints (from AdvancedActionButton)
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Ensure border radius doesn't exceed button dimensions (from AdvancedActionButton)
            int maxRadius = Math.Min(Width, Height) / 2;
            if (borderRadius > maxRadius)
            {
                borderRadius = maxRadius;
            }

            if (_isInitialized)
            {
                Invalidate();
            }
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles parent container background color changes to ensure proper rendering.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            if (_isInitialized)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles button resize events to maintain border radius constraints.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Button_Resize(object sender, EventArgs e)
        {
            if (Width > 0 && Height > 0 && borderRadius > this.Height)
            {
                borderRadius = this.Height;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets border properties in one method call (from AdvancedActionButton)
        /// </summary>
        /// <param name="radius">Border radius</param>
        /// <param name="size">Border size</param>
        /// <param name="color">Border color</param>
        public void SetBorder(int radius, int size, Color color)
        {
            borderRadius = Math.Max(0, radius);
            borderSize = Math.Max(0, size);
            borderColor = color;

            if (_isInitialized)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Sets hover colors in one method call
        /// </summary>
        /// <param name="backColor">Hover background color</param>
        /// <param name="borderColor">Hover border color</param>
        public void SetHoverColors(Color backColor, Color borderColor)
        {
            hoverBackColor = backColor;
            hoverBorderColor = borderColor;

            if (_isInitialized)
            {
                Invalidate();
            }
        }

        #endregion
    }
}