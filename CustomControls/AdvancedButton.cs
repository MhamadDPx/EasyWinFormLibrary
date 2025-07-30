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
        private int borderSize = DEFAULT_BORDER_SIZE;
        private int borderRadius = DEFAULT_BORDER_RADIUS;
        private Color borderColor = DEFAULT_BORDER_COLOR;
        private Color backgroundColor = DEFAULT_BACKGROUND_COLOR;
        private Color textColor = DEFAULT_TEXT_COLOR;
        private Image buttonImage = null;
        private ImageLayout imageLayout = ImageLayout.Zoom;
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        private Size imageSize = new Size(25, 25);
        private int imageTextSpacing = 5;
        private bool onHover = false;
        private Color hoverBackColor = DEFAULT_HOVER_BACKGROUND_COLOR;
        private Color hoverBorderColor = DEFAULT_HOVER_BORDER_COLOR;
        private Color pressedBackColor = DEFAULT_PRESSED_BACKGROUND_COLOR;
        private bool isPressed = false;
        #endregion

        #region Public Properties

        [Category("Advanced Appearance")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set
            {
                borderRadius = value <= this.Height ? value : this.Height;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public Image ButtonImage
        {
            get { return buttonImage; }
            set
            {
                buttonImage = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public ImageLayout ButtonImageLayout
        {
            get { return imageLayout; }
            set
            {
                imageLayout = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public new ContentAlignment ImageAlign
        {
            get { return imageAlign; }
            set
            {
                imageAlign = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public Size ImageSize
        {
            get { return imageSize; }
            set
            {
                imageSize = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public int ImageTextSpacing
        {
            get { return imageTextSpacing; }
            set
            {
                imageTextSpacing = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public Color HoverBackColor
        {
            get { return hoverBackColor; }
            set
            {
                hoverBackColor = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public Color HoverBorderColor
        {
            get { return hoverBorderColor; }
            set
            {
                hoverBorderColor = value;
                this.Invalidate();
            }
        }

        [Category("Advanced Appearance")]
        public Color PressedBackColor
        {
            get { return pressedBackColor; }
            set
            {
                pressedBackColor = value;
                this.Invalidate();
            }
        }
        #endregion

        #region Constructor
        public AdvancedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.Size = new Size(150, 40);
            this.BackColor = Color.Transparent;
            this.ForeColor = textColor;
            this.Resize += new EventHandler(Button_Resize);
            this.SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a graphics path for a rounded rectangle (improved version from AdvancedActionButton)
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

        /// <summary>
        /// Draws the custom border and shape of the button (improved version from AdvancedActionButton)
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
        /// Draws a rounded border for the button (improved version from AdvancedActionButton)
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
                    graphics.FillPath(brushSurface, pathBorder);
                }

                // Draw control border
                if (borderSize >= 1)
                {
                    graphics.DrawPath(penBorder, pathBorder);
                }
            }
        }

        /// <summary>
        /// Draws a rectangular border for the button (improved version from AdvancedActionButton)
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="rectSurface">Surface rectangle</param>
        /// <param name="smoothSize">Smoothing size</param>
        private void DrawRectangularBorder(Graphics graphics, Rectangle rectSurface, int smoothSize)
        {
            graphics.SmoothingMode = SmoothingMode.None;

            // Set button region
            Region = new Region(rectSurface);

            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);

            // Button background
            Color currentBackColor = backgroundColor;
            if (isPressed)
                currentBackColor = pressedBackColor;
            else if (onHover)
                currentBackColor = hoverBackColor;

            using (SolidBrush brushSurface = new SolidBrush(currentBackColor))
            {
                graphics.FillRectangle(brushSurface, rectBorder);
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
        /// Draws a simple border as fallback (improved version from AdvancedActionButton)
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="rect">Rectangle to draw</param>
        private void DrawSimpleBorder(Graphics graphics, Rectangle rect)
        {
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

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            try
            {
                // Set high-quality rendering (from AdvancedActionButton)
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Draw custom border and shape using improved method
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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.Parent.BackColorChanged += new EventHandler(Container_BackColorChanged);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            onHover = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            onHover = false;
            isPressed = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            isPressed = true;
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            this.Invalidate();
        }

        /// <summary>
        /// Handles the resize event to update border radius constraints
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Ensure border radius doesn't exceed button dimensions
            int maxRadius = Math.Min(Width, Height) / 2;
            if (borderRadius > maxRadius)
            {
                borderRadius = maxRadius;
            }

            Invalidate();
        }
        #endregion

        #region Event Handlers
        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void Button_Resize(object sender, EventArgs e)
        {
            if (borderRadius > this.Height)
                borderRadius = this.Height;
        }
        #endregion
    }
}