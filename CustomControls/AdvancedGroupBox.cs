using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// An advanced GroupBox control that provides a modern design with a colored title bar and customizable styling options.
    /// Features include title bar with custom colors, content area styling, border customization, and predefined themes.
    /// Inherits from GroupBox to maintain standard functionality while adding enhanced visual appearance.
    /// </summary>
    [ToolboxItem(true)]
    [Description("Advanced GroupBox with title bar design and enhanced styling options")]
    [Localizable(true)]
    public class AdvancedGroupBox : GroupBox
    {
        #region Constants
        private readonly static Color DEFAULT_BORDER_COLOR = Color.FromArgb(213, 218, 223);
        private readonly static Color DEFAULT_TITLE_BAR_COLOR = Color.FromArgb(213, 218, 223);
        private readonly static Color DEFAULT_TITLE_FORE_COLOR = Color.FromArgb(40, 40, 40);
        #endregion

        #region Private Fields
        private Color _borderColor = DEFAULT_BORDER_COLOR;
        private Color _titleBarColor = DEFAULT_TITLE_BAR_COLOR;
        private Color _titleForeColor = DEFAULT_TITLE_FORE_COLOR;
        private Color _contentBackColor = Color.White;
        private Font _titleFont = null;
        private int _borderWidth = 1;
        private int _titleBarHeight = 30;
        private Padding _titlePadding = new Padding(8, 6, 8, 6);
        private StringAlignment _titleAlignment = StringAlignment.Near;
        private bool _showTitleBar = true; // New field for title bar visibility
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AdvancedGroupBox class with default styling and properties.
        /// </summary>
        public AdvancedGroupBox()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.DoubleBuffer, true);

            // Make the control non-selectable
            SetStyle(ControlStyles.Selectable, false);
            this.TabStop = false;

            // Set default size
            this.Size = new Size(300, 200);
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether the title bar is visible. When false, the control displays with uniform border width on all sides.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Determines whether the title bar is visible")]
        [DefaultValue(true)]
        public bool ShowTitleBar
        {
            get { return _showTitleBar; }
            set
            {
                _showTitleBar = value;
                UpdateContentPadding();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of the control border.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The color of the border")]
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
        /// Gets or sets the background color of the title bar section.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The background color of the title bar")]
        public Color TitleBarColor
        {
            get { return _titleBarColor; }
            set
            {
                _titleBarColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the title text.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The foreground color of the title text")]
        public Color TitleForeColor
        {
            get { return _titleForeColor; }
            set
            {
                _titleForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the background color of the content area below the title bar.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The background color of the content area")]
        public Color ContentBackColor
        {
            get { return _contentBackColor; }
            set
            {
                _contentBackColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the font used for the title text. If null, uses the control's Font property.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The font used for the title text")]
        [Localizable(true)]
        public Font TitleFont
        {
            get { return _titleFont ?? this.Font; }
            set
            {
                _titleFont = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the width of the border in pixels. Minimum value is 1.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The width of the border")]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = Math.Max(1, value);
                UpdateContentPadding();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the height of the title bar in pixels. Minimum value is 20.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The height of the title bar")]
        public int TitleBarHeight
        {
            get { return _titleBarHeight; }
            set
            {
                _titleBarHeight = Math.Max(20, value);
                UpdateContentPadding();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the padding around the title text within the title bar.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The padding around the title text")]
        [Localizable(true)]
        public Padding TitlePadding
        {
            get { return _titlePadding; }
            set
            {
                _titlePadding = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the title text within the title bar.
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The horizontal alignment of the title text")]
        [Localizable(true)]
        public StringAlignment TitleAlignment
        {
            get { return _titleAlignment; }
            set
            {
                _titleAlignment = value;
                Invalidate();
            }
        }

        #endregion

        #region Overridden Methods
        /// <summary>
        /// Handles the paint event to render the custom GroupBox with title bar, content area, and border.
        /// </summary>
        /// <param name="e">Paint event arguments containing the graphics context and clipping information</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            Rectangle clientRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            if (_showTitleBar)
            {
                // Draw title bar first (extends to edges to avoid white line)
                Rectangle titleBarRect = new Rectangle(
                    0,
                    0,
                    this.Width,
                    _titleBarHeight + _borderWidth
                );
                DrawTitleBar(g, titleBarRect);

                // Draw content area below title bar
                Rectangle contentRect = new Rectangle(
                    _borderWidth,
                    _titleBarHeight + _borderWidth,
                    this.Width - (_borderWidth * 2),
                    this.Height - _titleBarHeight - (_borderWidth * 2)
                );
                DrawContentArea(g, contentRect);

                // Draw title text
                if (!string.IsNullOrEmpty(this.Text))
                {
                    Rectangle textAreaRect = new Rectangle(
                        _borderWidth,
                        _borderWidth,
                        this.Width - (_borderWidth * 2),
                        _titleBarHeight
                    );
                    DrawTitleText(g, textAreaRect);
                }
            }
            else
            {
                // Draw content area filling the entire control (minus border)
                Rectangle contentRect = new Rectangle(
                    _borderWidth,
                    _borderWidth,
                    this.Width - (_borderWidth * 2),
                    this.Height - (_borderWidth * 2)
                );
                DrawContentArea(g, contentRect);
            }

            // Draw outer border on top
            DrawBorder(g, clientRect);
        }

        private void DrawBorder(Graphics g, Rectangle rect)
        {
            using (Pen borderPen = new Pen(_borderColor, _borderWidth))
            {
                g.DrawRectangle(borderPen, rect);
            }
        }

        private void DrawTitleBar(Graphics g, Rectangle titleBarRect)
        {
            using (SolidBrush titleBrush = new SolidBrush(_titleBarColor))
            {
                g.FillRectangle(titleBrush, titleBarRect);
            }
        }

        private void DrawContentArea(Graphics g, Rectangle contentRect)
        {
            using (SolidBrush contentBrush = new SolidBrush(_contentBackColor))
            {
                g.FillRectangle(contentBrush, contentRect);
            }
        }

        private void DrawTitleText(Graphics g, Rectangle titleBarRect)
        {
            using (SolidBrush textBrush = new SolidBrush(_titleForeColor))
            {
                StringFormat format = new StringFormat();
                format.Alignment = _titleAlignment;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;

                Rectangle textRect = new Rectangle(
                    titleBarRect.X + _titlePadding.Left,
                    titleBarRect.Y + _titlePadding.Top,
                    titleBarRect.Width - _titlePadding.Left - _titlePadding.Right,
                    titleBarRect.Height - _titlePadding.Top - _titlePadding.Bottom
                );

                // Use DrawString with high quality text rendering
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.DrawString(this.Text, this.TitleFont, textBrush, textRect, format);

                format.Dispose();
            }
        }

        private void UpdateContentPadding()
        {
            // Update the padding based on whether title bar is shown
            if (_showTitleBar)
            {
                // Title bar visible: different top padding
                this.Padding = new Padding(
                    _borderWidth + 1,
                    _titleBarHeight + _borderWidth + 1,
                    _borderWidth + 1,
                    _borderWidth + 1
                );
            }
            else
            {
                // Title bar hidden: uniform padding on all sides
                this.Padding = new Padding(
                    _borderWidth + 1,
                    _borderWidth + 1,
                    _borderWidth + 1,
                    _borderWidth + 1
                );
            }
        }

        /// <summary>
        /// Handles the text changed event to refresh the control's visual appearance.
        /// </summary>
        /// <param name="e">Event arguments containing information about the text change</param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Handles the font changed event to refresh the control when using default font settings.
        /// </summary>
        /// <param name="e">Event arguments containing information about the font change</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (_titleFont == null)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Handles the resize event to update content padding and maintain proper layout.
        /// </summary>
        /// <param name="e">Event arguments containing information about the resize operation</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateContentPadding();
        }

        /// <summary>
        /// Handles low-level bounds changes to ensure content padding is updated during any size or position modification.
        /// </summary>
        /// <param name="x">The new x-coordinate of the control's location</param>
        /// <param name="y">The new y-coordinate of the control's location</param>
        /// <param name="width">The new width of the control</param>
        /// <param name="height">The new height of the control</param>
        /// <param name="specified">A bitwise combination of BoundsSpecified values indicating which bounds are being set</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            UpdateContentPadding();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sets the title bar appearance with the specified colors and optional font.
        /// </summary>
        /// <param name="titleBarColor">The background color of the title bar</param>
        /// <param name="titleTextColor">The color of the title text</param>
        /// <param name="titleFont">Optional font for the title text</param>
        public void SetTitleBarStyle(Color titleBarColor, Color titleTextColor, Font titleFont = null)
        {
            _titleBarColor = titleBarColor;
            _titleForeColor = titleTextColor;
            if (titleFont != null)
                _titleFont = titleFont;
            Invalidate();
        }

        /// <summary>
        /// Applies a predefined color theme to the control.
        /// </summary>
        /// <param name="theme">The theme to apply</param>
        public void SetTheme(GroupBoxTheme theme)
        {
            switch (theme)
            {
                case GroupBoxTheme.Default:
                    _titleBarColor = Color.FromArgb(176, 196, 222);
                    _titleForeColor = Color.FromArgb(64, 64, 64);
                    _contentBackColor = Color.White;
                    _borderColor = Color.FromArgb(169, 169, 169);
                    break;

                case GroupBoxTheme.Blue:
                    _titleBarColor = Color.FromArgb(70, 130, 180);
                    _titleForeColor = Color.White;
                    _contentBackColor = Color.White;
                    _borderColor = Color.FromArgb(70, 130, 180);
                    break;

                case GroupBoxTheme.Green:
                    _titleBarColor = Color.FromArgb(60, 179, 113);
                    _titleForeColor = Color.White;
                    _contentBackColor = Color.White;
                    _borderColor = Color.FromArgb(60, 179, 113);
                    break;

                case GroupBoxTheme.Dark:
                    _titleBarColor = Color.FromArgb(64, 64, 64);
                    _titleForeColor = Color.White;
                    _contentBackColor = Color.FromArgb(248, 248, 248);
                    _borderColor = Color.FromArgb(64, 64, 64);
                    break;
            }
            Invalidate();
        }

        #endregion

        /// <summary>
        /// Defines predefined visual themes for the AdvancedGroupBox control.
        /// Each theme provides a coordinated set of colors for the title bar, text, content area, and border.
        /// </summary>
        public enum GroupBoxTheme
        {
            /// <summary>
            /// Default theme with light blue-gray title bar and dark gray text
            /// </summary>
            Default,

            /// <summary>
            /// Professional blue theme with white text on blue background
            /// </summary>
            Blue,

            /// <summary>
            /// Fresh green theme with white text on green background
            /// </summary>
            Green,

            /// <summary>
            /// Modern dark theme with white text on dark gray background
            /// </summary>
            Dark
        }
    }
}