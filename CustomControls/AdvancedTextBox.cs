using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    #region Enums

    /// <summary>
    /// Defines the input types supported by the AdvancedTextBox
    /// </summary>
    public enum TextBoxInput
    {
        /// <summary>Standard text input (allows all characters)</summary>
        TextInput,
        /// <summary>Integer numbers only</summary>
        IntegerInput,
        /// <summary>Decimal numbers only</summary>
        DoubleInput,
        /// <summary>Text with integer numbers</summary>
        TextIntegerNumberInput,
        /// <summary>Text with decimal numbers</summary>
        TextDoubleNumberInput
    }

    #endregion

    /// <summary>
    /// Advanced TextBox UserControl with custom border styling and comprehensive input validation.
    /// Wraps a standard TextBox with custom border rendering for better designer compatibility.
    /// Supports multiple input types, Arabic/English numeral conversion, and Enter key navigation.
    /// Optimized for .NET Framework 4.8 with full designer support.
    /// </summary>
    [ToolboxItem(true)]
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design")]
    public partial class AdvancedTextBox : UserControl
    {
        #region Private Fields

        private TextBox _textBox;
        private bool _allowArabicNumber = false;
        private bool _allowPoint = false;
        private bool _allowNegativeNumber = false;
        private bool _isValidText = false;
        private TextBoxInput _textBoxInputType = TextBoxInput.TextInput;

        // Border styling fields
        private int _borderRadius = 5;
        private int _borderSize = 1;
        private Color _borderColor = Color.Gray;
        private Color _borderFocusColor = Color.Blue;
        private bool _useFocusColor = true;
        private Padding _textPadding = new Padding(8, 4, 8, 4);
        private ContentAlignment _textAlignment = ContentAlignment.MiddleLeft;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether Arabic numerals are allowed in input
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Allows Arabic numerals to be entered")]
        [DefaultValue(false)]
        public bool AllowArabicNumber
        {
            get { return _allowArabicNumber; }
            set
            {
                if (_allowArabicNumber != value)
                {
                    _allowArabicNumber = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether decimal points are allowed in numeric input
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Allows decimal points in numeric input")]
        [DefaultValue(false)]
        public bool AllowPoint
        {
            get { return _allowPoint; }
            set
            {
                if (_allowPoint != value)
                {
                    _allowPoint = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether negative numbers are allowed
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Allows negative numbers to be entered")]
        [DefaultValue(false)]
        public bool AllowNegativeNumber
        {
            get { return _allowNegativeNumber; }
            set
            {
                if (_allowNegativeNumber != value)
                {
                    _allowNegativeNumber = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current text is valid for the specified input type
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ValidText
        {
            get { return _isValidText; }
            private set
            {
                if (_isValidText != value)
                {
                    _isValidText = value;
                    OnValidationChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the input type that determines validation and formatting behavior
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The type of input validation and formatting to apply")]
        [DefaultValue(TextBoxInput.TextInput)]
        public TextBoxInput TextBoxInputType
        {
            get { return _textBoxInputType; }
            set
            {
                if (_textBoxInputType != value)
                {
                    _textBoxInputType = value;
                    ValidateCurrentText();
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color of the control
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The border color of the control")]
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
        /// Gets or sets the border color when the control has focus
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The border color when the control has focus")]
        [DefaultValue(typeof(Color), "Blue")]
        public Color BorderFocusColor
        {
            get { return _borderFocusColor; }
            set
            {
                if (_borderFocusColor != value)
                {
                    _borderFocusColor = value;
                    if (_textBox != null && _textBox.Focused)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to use focus color for border
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Whether to use focus color for border")]
        [DefaultValue(true)]
        public bool UseFocusColor
        {
            get { return _useFocusColor; }
            set
            {
                if (_useFocusColor != value)
                {
                    _useFocusColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the border radius of the control
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The border radius of the control")]
        [DefaultValue(5)]
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
                    UpdateRegion();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the border size of the control
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The border size of the control")]
        [DefaultValue(1)]
        public int BorderSize
        {
            get { return _borderSize; }
            set
            {
                if (value < 0) value = 0;
                if (value > 10) value = 10;

                if (_borderSize != value)
                {
                    _borderSize = value;
                    UpdateTextBoxBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text content of the textbox
        /// </summary>
        [Category("Appearance")]
        [Description("The text content of the textbox")]
        [DefaultValue("")]
        public override string Text
        {
            get { return _textBox?.Text ?? string.Empty; }
            set
            {
                if (_textBox != null)
                {
                    _textBox.Text = value ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of the textbox
        /// </summary>
        [Category("Appearance")]
        [Description("The font of the textbox")]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                if (_textBox != null)
                {
                    _textBox.Font = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the textbox
        /// </summary>
        [Category("Appearance")]
        [Description("The foreground color of the textbox")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                if (_textBox != null)
                {
                    _textBox.ForeColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the textbox is multiline
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the textbox is multiline")]
        [DefaultValue(false)]
        public bool Multiline
        {
            get { return _textBox?.Multiline ?? false; }
            set
            {
                if (_textBox != null)
                {
                    _textBox.Multiline = value;
                    UpdateTextBoxBounds();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum length of text in the textbox
        /// </summary>
        [Category("Behavior")]
        [Description("The maximum length of text in the textbox")]
        [DefaultValue(32767)]
        public int MaxLength
        {
            get { return _textBox?.MaxLength ?? 32767; }
            set
            {
                if (_textBox != null)
                {
                    _textBox.MaxLength = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the textbox is read-only
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the textbox is read-only")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _textBox?.ReadOnly ?? false; }
            set
            {
                if (_textBox != null)
                {
                    _textBox.ReadOnly = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text alignment in the textbox
        /// </summary>
        [Category("Appearance")]
        [Description("The text alignment in the textbox")]
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment TextAlign
        {
            get { return _textBox?.TextAlign ?? HorizontalAlignment.Left; }
            set
            {
                if (_textBox != null)
                {
                    _textBox.TextAlign = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text alignment for single-line mode (affects vertical alignment)
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The text alignment for single-line mode (affects vertical and horizontal alignment)")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TextAlignment
        {
            get { return _textAlignment; }
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    UpdateTextBoxAlignment();
                    UpdateTextBoxBounds();
                }
            }
        }

        /// <summary>
        /// Gets or sets the padding around the text
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The padding around the text content")]
        public Padding TextPadding
        {
            get { return _textPadding; }
            set
            {
                if (_textPadding != value)
                {
                    _textPadding = value;
                    UpdateTextBoxBounds();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the validation state of the text changes
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when the validation state of the text changes")]
        public event EventHandler ValidationChanged;

        /// <summary>
        /// Occurs when the text changes
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when the text changes")]
        public new event EventHandler TextChanged;

        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        [Category("Key")]
        [Description("Occurs when a key is pressed")]
        public new event KeyPressEventHandler KeyPress;

        /// <summary>
        /// Occurs when a key is released
        /// </summary>
        [Category("Key")]
        [Description("Occurs when a key is released")]
        public new event KeyEventHandler KeyUp;

        /// <summary>
        /// Occurs when a key is pressed down
        /// </summary>
        [Category("Key")]
        [Description("Occurs when a key is pressed down")]
        public new event KeyEventHandler KeyDown;

        /// <summary>
        /// Raises the ValidationChanged event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnValidationChanged(EventArgs e)
        {
            ValidationChanged?.Invoke(this, e);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedTextBox class
        /// </summary>
        public AdvancedTextBox()
        {
            InitializeComponent();
            CreateTextBox();
            SetDefaultProperties();
            AttachEventHandlers();
        }

        /// <summary>
        /// Initializes the component with optimal settings
        /// </summary>
        private void InitializeComponent()
        {
            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// Creates and configures the internal TextBox
        /// </summary>
        private void CreateTextBox()
        {
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = BackColor,
                ForeColor = ForeColor,
                Font = Font,
                TextAlign = HorizontalAlignment.Left
            };

            Controls.Add(_textBox);
            UpdateTextBoxAlignment();
            UpdateTextBoxBounds();
        }

        /// <summary>
        /// Sets the default properties for the control
        /// </summary>
        private void SetDefaultProperties()
        {
            Size = new Size(200, 30);
            BackColor = Color.White;
            ForeColor = Color.Black;
        }

        /// <summary>
        /// Attaches necessary event handlers
        /// </summary>
        private void AttachEventHandlers()
        {
            if (_textBox != null)
            {
                _textBox.KeyPress += OnTextBoxKeyPress;
                _textBox.KeyUp += OnTextBoxKeyUp;
                _textBox.KeyDown += OnTextBoxKeyDown;
                _textBox.Leave += OnTextBoxLeave;
                _textBox.TextChanged += OnTextBoxTextChanged;
                _textBox.GotFocus += OnTextBoxGotFocus;
                _textBox.LostFocus += OnTextBoxLostFocus;
                _textBox.Enter += OnTextBoxEnter;
            }

            Resize += OnControlResize;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the KeyPress event for input validation and character conversion
        /// </summary>
        private void OnTextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // Convert Arabic numerals to English if not allowed
                if (!_allowArabicNumber)
                {
                    ConvertArabicToEnglishNumerals(e);
                }

                // Apply input type validation
                ApplyInputTypeValidation(e);

                // Forward the event
                KeyPress?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnTextBoxKeyPress: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the KeyUp event for special key processing
        /// </summary>
        private void OnTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    HandleEnterKeyNavigation(e);
                }

                // Forward the event
                KeyUp?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnTextBoxKeyUp: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the KeyDown event
        /// </summary>
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            // Forward the event
            KeyDown?.Invoke(this, e);
        }

        /// <summary>
        /// Handles the Leave event for text formatting
        /// </summary>
        private void OnTextBoxLeave(object sender, EventArgs e)
        {
            try
            {
                FormatTextOnLeave();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnTextBoxLeave: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the TextChanged event for validation
        /// </summary>
        private void OnTextBoxTextChanged(object sender, EventArgs e)
        {
            try
            {
                // Remove single quotes for security
                if (_textBox.Text.Contains("'"))
                {
                    _textBox.Text = _textBox.Text.Replace("'", "");
                    return;
                }

                ValidateCurrentText();

                // Forward the event
                TextChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnTextBoxTextChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the GotFocus event for text preparation
        /// </summary>
        private void OnTextBoxGotFocus(object sender, EventArgs e)
        {
            try
            {
                PrepareTextForEditing();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnTextBoxGotFocus: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the Enter event to refresh border
        /// </summary>
        private void OnTextBoxEnter(object sender, EventArgs e)
        {
            if (_useFocusColor)
            {
                Invalidate(); // Redraw with focus color
            }
        }

        /// <summary>
        /// Handles the LostFocus event to refresh border
        /// </summary>
        private void OnTextBoxLostFocus(object sender, EventArgs e)
        {
            if (_useFocusColor)
            {
                Invalidate(); // Redraw with normal color
            }
        }

        /// <summary>
        /// Handles the control resize event
        /// </summary>
        private void OnControlResize(object sender, EventArgs e)
        {
            try
            {
                // Ensure border radius doesn't exceed control dimensions
                int maxRadius = Math.Min(Width, Height) / 2;
                if (_borderRadius > maxRadius)
                {
                    _borderRadius = maxRadius;
                }

                UpdateTextBoxBounds();
                UpdateRegion();
                Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnControlResize: {ex.Message}");
            }
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Handles the paint event to draw custom border
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawCustomBorder(e.Graphics);
        }

        /// <summary>
        /// Focuses the internal textbox when the control receives focus
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _textBox?.Focus();
        }

        /// <summary>
        /// Handles background color changes
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            if (_textBox != null)
            {
                _textBox.BackColor = BackColor;
            }
        }

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Draws the custom border around the control
        /// </summary>
        /// <param name="graphics">Graphics object for drawing</param>
        private void DrawCustomBorder(Graphics graphics)
        {
            if (_borderSize <= 0) return;

            try
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle borderRect = new Rectangle(0, 0, Width - 1, Height - 1);

                // Determine border color based on focus state
                Color currentBorderColor = _borderColor;
                if (_useFocusColor && _textBox != null && _textBox.Focused)
                {
                    currentBorderColor = _borderFocusColor;
                }

                using (var borderPen = new Pen(currentBorderColor, _borderSize))
                {
                    if (_borderRadius > 0)
                    {
                        using (var borderPath = CreateRoundedRectanglePath(borderRect, _borderRadius))
                        {
                            graphics.DrawPath(borderPen, borderPath);
                        }
                    }
                    else
                    {
                        graphics.DrawRectangle(borderPen, borderRect);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error drawing custom border: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a graphics path for a rounded rectangle
        /// </summary>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="radius">Corner radius</param>
        /// <returns>Graphics path for the rounded rectangle</returns>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();

            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            try
            {
                int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
                var size = new Size(diameter, diameter);
                var arc = new Rectangle(rect.Location, size);

                path.StartFigure();

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating rounded path: {ex.Message}");
                // Fallback to rectangle
                path.Reset();
                path.AddRectangle(rect);
            }

            return path;
        }

        /// <summary>
        /// Updates the control's region based on border radius
        /// </summary>
        private void UpdateRegion()
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating region: {ex.Message}");
                Region = null;
            }
        }

        /// <summary>
        /// Updates the bounds of the internal textbox
        /// </summary>
        private void UpdateTextBoxBounds()
        {
            if (_textBox == null) return;

            try
            {
                // Calculate available space accounting for border and padding
                int borderPadding = Math.Max(1, _borderSize);
                int totalPaddingLeft = borderPadding + _textPadding.Left;
                int totalPaddingTop = borderPadding + _textPadding.Top;
                int totalPaddingRight = borderPadding + _textPadding.Right;
                int totalPaddingBottom = borderPadding + _textPadding.Bottom;

                int availableWidth = Math.Max(1, Width - totalPaddingLeft - totalPaddingRight);
                int availableHeight = Math.Max(1, Height - totalPaddingTop - totalPaddingBottom);

                if (_textBox.Multiline)
                {
                    // For multiline, use all available space
                    _textBox.Location = new Point(totalPaddingLeft, totalPaddingTop);
                    _textBox.Size = new Size(availableWidth, availableHeight);
                }
                else
                {
                    // For single line, handle vertical alignment
                    int textBoxHeight = _textBox.PreferredHeight;
                    int x = totalPaddingLeft;
                    int y = totalPaddingTop;
                    int width = availableWidth;
                    int height = Math.Min(textBoxHeight, availableHeight);

                    // Calculate vertical position based on alignment
                    switch (_textAlignment)
                    {
                        case ContentAlignment.TopLeft:
                        case ContentAlignment.TopCenter:
                        case ContentAlignment.TopRight:
                            y = totalPaddingTop;
                            break;

                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.MiddleRight:
                            y = totalPaddingTop + (availableHeight - height) / 2;
                            break;

                        case ContentAlignment.BottomLeft:
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.BottomRight:
                            y = totalPaddingTop + (availableHeight - height);
                            break;
                    }

                    // Ensure y is not negative
                    y = Math.Max(totalPaddingTop, y);

                    _textBox.Location = new Point(x, y);
                    _textBox.Size = new Size(width, height);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating textbox bounds: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the text alignment of the internal textbox based on ContentAlignment
        /// </summary>
        private void UpdateTextBoxAlignment()
        {
            if (_textBox == null) return;

            try
            {
                // Map ContentAlignment to HorizontalAlignment
                switch (_textAlignment)
                {
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.BottomLeft:
                        _textBox.TextAlign = HorizontalAlignment.Left;
                        break;

                    case ContentAlignment.TopCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.BottomCenter:
                        _textBox.TextAlign = HorizontalAlignment.Center;
                        break;

                    case ContentAlignment.TopRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.BottomRight:
                        _textBox.TextAlign = HorizontalAlignment.Right;
                        break;

                    default:
                        _textBox.TextAlign = HorizontalAlignment.Left;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating textbox alignment: {ex.Message}");
            }
        }

        #endregion

        #region Input Validation Methods (same as before)

        private void ConvertArabicToEnglishNumerals(KeyPressEventArgs e)
        {
            try
            {
                if (_allowArabicNumber)
                {
                    return;
                }
                else
                {
                    NumberInputUtils.ConvertToEnglishNumerals(e);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting numerals: {ex.Message}");
            }
        }

        private void ApplyInputTypeValidation(KeyPressEventArgs e)
        {
            switch (_textBoxInputType)
            {
                case TextBoxInput.IntegerInput:
                case TextBoxInput.TextIntegerNumberInput:
                    ValidateIntegerInput(e);
                    break;

                case TextBoxInput.DoubleInput:
                case TextBoxInput.TextDoubleNumberInput:
                    ValidateDoubleInput(e);
                    break;

                case TextBoxInput.TextInput:
                default:
                    break;
            }
        }

        private void ValidateIntegerInput(KeyPressEventArgs e)
        {
            try
            {
                NumberInputUtils.RestrictToNumericInput(e, allowDecimal: false, allowNegative: _allowNegativeNumber);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating integer input: {ex.Message}");
                ValidateIntegerInputFallback(e);
            }
        }

        private void ValidateDoubleInput(KeyPressEventArgs e)
        {
            try
            {
                NumberInputUtils.RestrictToNumericInput(e, allowDecimal: true, allowNegative: _allowNegativeNumber);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating double input: {ex.Message}");
                ValidateDoubleInputFallback(e);
            }
        }

        private void ValidateIntegerInputFallback(KeyPressEventArgs e)
        {
            if (_allowNegativeNumber)
            {
                if (!char.IsControl(e.KeyChar) && e.KeyChar != '-' && !char.IsDigit(e.KeyChar))
                    e.Handled = true;

                if ((e.KeyChar == '-') && (Text.IndexOf('-') > -1))
                    e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;

                if ((e.KeyChar == '-') && (Text.IndexOf('-') > -1))
                    e.Handled = true;
            }
        }

        private void ValidateDoubleInputFallback(KeyPressEventArgs e)
        {
            if (_allowNegativeNumber)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && e.KeyChar != '-')
                    e.Handled = true;

                if ((e.KeyChar == '.') && (Text.IndexOf('.') > -1))
                    e.Handled = true;

                if ((e.KeyChar == '-') && (Text.IndexOf('-') > -1))
                    e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                    e.Handled = true;

                if ((e.KeyChar == '.') && (Text.IndexOf('.') > -1))
                    e.Handled = true;

                if ((e.KeyChar == '-') && (Text.IndexOf('-') > -1))
                    e.Handled = true;
            }
        }

        private void ValidateCurrentText()
        {
            if (string.IsNullOrEmpty(Text))
            {
                ValidText = true;
                return;
            }

            try
            {
                string normalizedText = NumberInputUtils.NormalizeNumericString(Text, preserveDecimal: true);

                switch (_textBoxInputType)
                {
                    case TextBoxInput.IntegerInput:
                        ValidText = NumberInputUtils.IsValidNumericString(normalizedText, allowDecimal: false, allowNegative: _allowNegativeNumber) &&
                                   long.TryParse(normalizedText, out _);
                        break;

                    case TextBoxInput.DoubleInput:
                        ValidText = NumberInputUtils.IsValidNumericString(normalizedText, allowDecimal: true, allowNegative: _allowNegativeNumber) &&
                                   double.TryParse(normalizedText, out _);
                        break;

                    case TextBoxInput.TextInput:
                    case TextBoxInput.TextIntegerNumberInput:
                    case TextBoxInput.TextDoubleNumberInput:
                    default:
                        ValidText = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating text: {ex.Message}");
                ValidateCurrentTextFallback();
            }
        }

        private void ValidateCurrentTextFallback()
        {
            try
            {
                switch (_textBoxInputType)
                {
                    case TextBoxInput.IntegerInput:
                        ValidText = long.TryParse(Text, out _);
                        break;

                    case TextBoxInput.DoubleInput:
                        ValidText = double.TryParse(Text, out _);
                        break;

                    case TextBoxInput.TextInput:
                    case TextBoxInput.TextIntegerNumberInput:
                    case TextBoxInput.TextDoubleNumberInput:
                    default:
                        ValidText = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in fallback validation: {ex.Message}");
                ValidText = false;
            }
        }

        #endregion

        #region Text Formatting Methods (same as before)

        private void PrepareTextForEditing()
        {
            try
            {
                switch (_textBoxInputType)
                {
                    case TextBoxInput.IntegerInput:
                        PrepareIntegerForEditing();
                        break;

                    case TextBoxInput.DoubleInput:
                        PrepareDoubleForEditing();
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error preparing text for editing: {ex.Message}");
            }
        }

        private void PrepareIntegerForEditing()
        {
            if (double.TryParse(Text, out double input) &&
                long.TryParse(input.ToString(), out long number))
            {
                Text = number.ToString();
            }
        }

        private void PrepareDoubleForEditing()
        {
            if (double.TryParse(Text, out double number))
            {
                Text = number.ToString();
            }
        }

        private void FormatTextOnLeave()
        {
            try
            {
                switch (_textBoxInputType)
                {
                    case TextBoxInput.IntegerInput:
                        FormatIntegerText();
                        break;

                    case TextBoxInput.DoubleInput:
                        FormatDoubleText();
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formatting text on leave: {ex.Message}");
            }
        }

        private void FormatIntegerText()
        {
            if (long.TryParse(Text, out long number))
            {
                Text = number.ToString("n0");
            }
        }

        private void FormatDoubleText()
        {
            if (double.TryParse(Text, out double number))
            {
                try
                {
                    string format = _allowPoint ? "N" : $"N{LibrarySettings.NumberDefaultRound}";
                    Text = NumberInputUtils.FormatNumericString(number.ToString(), format);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error formatting with NumberInputUtils: {ex.Message}");
                    try
                    {
                        string format = _allowPoint ? "n" : $"n{LibrarySettings.NumberDefaultRound}";
                        Text = number.ToString(format);
                    }
                    catch (Exception ex2)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error accessing LibrarySetting: {ex2.Message}");
                        Text = number.ToString("n2");
                    }
                }
            }
        }

        #endregion

        #region Navigation Methods

        /// <summary>
        /// Handles Enter key navigation to the next control
        /// </summary>
        /// <param name="e">KeyUp event arguments</param>
        private void HandleEnterKeyNavigation(KeyEventArgs e)
        {
            try
            {
                e.SuppressKeyPress = true;

                Form parentForm = FindForm();
                if (parentForm == null)
                    return;

                Control nextControl = parentForm.GetNextControl(this, true);

                while (nextControl != null && !nextControl.TabStop)
                {
                    nextControl = parentForm.GetNextControl(nextControl, true);
                }

                nextControl?.Focus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling Enter key navigation: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets input validation properties in one method call
        /// </summary>
        /// <param name="inputType">The input type to use</param>
        /// <param name="allowArabicNumbers">Whether to allow Arabic numerals</param>
        /// <param name="allowNegative">Whether to allow negative numbers</param>
        /// <param name="allowDecimalPoint">Whether to allow decimal points</param>
        public void SetInputValidation(TextBoxInput inputType, bool allowArabicNumbers = false,
            bool allowNegative = false, bool allowDecimalPoint = false)
        {
            _textBoxInputType = inputType;
            _allowArabicNumber = allowArabicNumbers;
            _allowNegativeNumber = allowNegative;
            _allowPoint = allowDecimalPoint;

            ValidateCurrentText();
        }

        /// <summary>
        /// Sets border styling properties in one method call
        /// </summary>
        /// <param name="size">Border size</param>
        /// <param name="color">Border color</param>
        /// <param name="radius">Border radius for rounded corners</param>
        /// <param name="focusColor">Border color when focused (optional)</param>
        /// <param name="textPadding">Text padding (optional)</param>
        public void SetBorderStyle(int size, Color color, int radius = 0, Color? focusColor = null, Padding? textPadding = null)
        {
            _borderSize = Math.Max(0, Math.Min(size, 10));
            _borderColor = color;
            _borderRadius = Math.Max(0, radius);

            if (focusColor.HasValue)
            {
                _borderFocusColor = focusColor.Value;
                _useFocusColor = true;
            }

            if (textPadding.HasValue)
            {
                _textPadding = textPadding.Value;
            }

            if (Width > 0 && Height > 0 && _borderRadius > Math.Min(Width, Height) / 2)
            {
                _borderRadius = Math.Min(Width, Height) / 2;
            }

            UpdateTextBoxBounds();
            UpdateRegion();
            Invalidate();
        }

        /// <summary>
        /// Sets text alignment and padding in one method call
        /// </summary>
        /// <param name="alignment">Text alignment</param>
        /// <param name="padding">Text padding</param>
        public void SetTextLayout(ContentAlignment alignment, Padding padding)
        {
            _textAlignment = alignment;
            _textPadding = padding;
            UpdateTextBoxAlignment();
            UpdateTextBoxBounds();
        }

        /// <summary>
        /// Sets text alignment for common scenarios
        /// </summary>
        /// <param name="horizontal">Horizontal alignment</param>
        /// <param name="vertical">Vertical alignment for single-line mode</param>
        public void SetTextAlignment(HorizontalAlignment horizontal, StringAlignment vertical = StringAlignment.Center)
        {
            // Convert to ContentAlignment
            switch (vertical)
            {
                case StringAlignment.Near: // Top
                    switch (horizontal)
                    {
                        case HorizontalAlignment.Left: _textAlignment = ContentAlignment.TopLeft; break;
                        case HorizontalAlignment.Center: _textAlignment = ContentAlignment.TopCenter; break;
                        case HorizontalAlignment.Right: _textAlignment = ContentAlignment.TopRight; break;
                    }
                    break;

                case StringAlignment.Center: // Middle
                    switch (horizontal)
                    {
                        case HorizontalAlignment.Left: _textAlignment = ContentAlignment.MiddleLeft; break;
                        case HorizontalAlignment.Center: _textAlignment = ContentAlignment.MiddleCenter; break;
                        case HorizontalAlignment.Right: _textAlignment = ContentAlignment.MiddleRight; break;
                    }
                    break;

                case StringAlignment.Far: // Bottom
                    switch (horizontal)
                    {
                        case HorizontalAlignment.Left: _textAlignment = ContentAlignment.BottomLeft; break;
                        case HorizontalAlignment.Center: _textAlignment = ContentAlignment.BottomCenter; break;
                        case HorizontalAlignment.Right: _textAlignment = ContentAlignment.BottomRight; break;
                    }
                    break;
            }

            UpdateTextBoxAlignment();
            UpdateTextBoxBounds();
        }

        /// <summary>
        /// Gets the numeric value as a long integer (for integer input types)
        /// </summary>
        /// <returns>The numeric value or null if parsing fails</returns>
        public long? GetLongValue()
        {
            try
            {
                if (long.TryParse(Text, out long result))
                    return result;
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting long value: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the numeric value as a double (for decimal input types)
        /// </summary>
        /// <returns>The numeric value or null if parsing fails</returns>
        public double? GetDoubleValue()
        {
            try
            {
                if (double.TryParse(Text, out double result))
                    return result;
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting double value: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Clears the text and resets validation state
        /// </summary>
        public void ClearText()
        {
            try
            {
                Text = string.Empty;
                ValidateCurrentText();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing text: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the normalized numeric string using NumberInputUtils
        /// </summary>
        /// <param name="preserveDecimal">Whether to preserve decimal points</param>
        /// <returns>Normalized numeric string</returns>
        public string GetNormalizedText(bool preserveDecimal = true)
        {
            try
            {
                return NumberInputUtils.NormalizeNumericString(Text, preserveDecimal);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error normalizing text: {ex.Message}");
                return Text;
            }
        }

        /// <summary>
        /// Detects the numeral system used in the current text
        /// </summary>
        /// <returns>Detected numeral system</returns>
        public NumeralSystem DetectNumeralSystem()
        {
            try
            {
                return NumberInputUtils.DetectNumeralSystem(Text);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error detecting numeral system: {ex.Message}");
                return NumeralSystem.English;
            }
        }

        /// <summary>
        /// Focuses the internal textbox
        /// </summary>
        public new void Focus()
        {
            _textBox?.Focus();
        }

        /// <summary>
        /// Selects all text in the textbox
        /// </summary>
        public void SelectAll()
        {
            _textBox?.SelectAll();
        }

        /// <summary>
        /// Selects text in the textbox
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="length">Length of selection</param>
        public void Select(int start, int length)
        {
            _textBox?.Select(start, length);
        }

        /// <summary>
        /// Gets or sets the selected text
        /// </summary>
        public string SelectedText
        {
            get { return _textBox?.SelectedText ?? string.Empty; }
            set { if (_textBox != null) _textBox.SelectedText = value; }
        }

        /// <summary>
        /// Gets or sets the selection start position
        /// </summary>
        public int SelectionStart
        {
            get { return _textBox?.SelectionStart ?? 0; }
            set { if (_textBox != null) _textBox.SelectionStart = value; }
        }

        /// <summary>
        /// Gets or sets the selection length
        /// </summary>
        public int SelectionLength
        {
            get { return _textBox?.SelectionLength ?? 0; }
            set { if (_textBox != null) _textBox.SelectionLength = value; }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Detaches event handlers to prevent memory leaks
        /// </summary>
        private void DetachEventHandlers()
        {
            try
            {
                if (_textBox != null)
                {
                    _textBox.KeyPress -= OnTextBoxKeyPress;
                    _textBox.KeyUp -= OnTextBoxKeyUp;
                    _textBox.KeyDown -= OnTextBoxKeyDown;
                    _textBox.Leave -= OnTextBoxLeave;
                    _textBox.TextChanged -= OnTextBoxTextChanged;
                    _textBox.GotFocus -= OnTextBoxGotFocus;
                    _textBox.LostFocus -= OnTextBoxLostFocus;
                    _textBox.Enter -= OnTextBoxEnter;
                }

                Resize -= OnControlResize;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error detaching event handlers: {ex.Message}");
            }
        }

        #endregion

        #region Overridden Dispose

        /// <summary>
        /// Handles the disposal of resources
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DetachEventHandlers();
                _textBox?.Dispose();
            }
            base.Dispose(disposing);
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