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
        #region Private Fields
        private int borderSize = 0;
        private int borderRadius = 0;
        private Color borderColor = Color.Gray;
        private Color backgroundColor = Color.MediumSlateBlue;
        private Color textColor = Color.White;
        private Image buttonImage = null;
        private ImageLayout imageLayout = ImageLayout.None;
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        private Size imageSize = new Size(16, 16);
        private int imageTextSpacing = 5;
        private bool onHover = false;
        private Color hoverBackColor = Color.SlateBlue;
        private Color hoverBorderColor = Color.MediumSlateBlue;
        private Color pressedBackColor = Color.DarkSlateBlue;
        private bool isPressed = false;
        #endregion

        #region Constructor
        public AdvancedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.Size = new Size(150, 40);
            this.BackColor = Color.Transparent;
            this.ForeColor = textColor;
            this.Resize += new EventHandler(Button_Resize);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
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

        #region Private Methods

        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
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

            Rectangle rectSurface = this.ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = 2;

            if (borderSize > 0)
                smoothSize = borderSize;

            if (borderRadius > 2) // Rounded button
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, borderRadius - borderSize))
                using (Pen penSurface = new Pen(this.Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(onHover ? hoverBorderColor : borderColor, borderSize))
                {
                    pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    // Button surface
                    this.Region = new Region(pathSurface);
                    // Draw surface border for HD result
                    pevent.Graphics.DrawPath(penSurface, pathSurface);

                    // Button background
                    Color currentBackColor = backgroundColor;
                    if (isPressed)
                        currentBackColor = pressedBackColor;
                    else if (onHover)
                        currentBackColor = hoverBackColor;

                    using (SolidBrush brushSurface = new SolidBrush(currentBackColor))
                    {
                        pevent.Graphics.FillPath(brushSurface, pathBorder);
                    }

                    // Button border                    
                    if (borderSize >= 1)
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else // Normal button
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.None;
                this.Region = new Region(rectSurface);

                Color currentBackColor = backgroundColor;
                if (isPressed)
                    currentBackColor = pressedBackColor;
                else if (onHover)
                    currentBackColor = hoverBackColor;

                // Button background
                using (SolidBrush brushSurface = new SolidBrush(currentBackColor))
                {
                    pevent.Graphics.FillRectangle(brushSurface, rectBorder);
                }

                // Button border
                if (borderSize >= 1)
                {
                    using (Pen penBorder = new Pen(onHover ? hoverBorderColor : borderColor, borderSize))
                    {
                        pevent.Graphics.DrawRectangle(penBorder, rectBorder);
                    }
                }
            }

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