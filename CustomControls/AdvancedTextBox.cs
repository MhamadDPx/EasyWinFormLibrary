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
    public enum AdvancedTextBoxInput
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

    /// <summary>
    /// Border sides enumeration with flags support
    /// </summary>
    [Flags]
    public enum AdvancedTextBoxBorderSides
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
    /// Advanced TextBox UserControl with custom border styling, comprehensive input validation, and RTL support.
    /// Wraps a standard TextBox with custom border rendering for better designer compatibility.
    /// Supports multiple input types, Arabic/English numeral conversion, selective border sides, and Enter key navigation.
    /// Optimized for .NET Framework 4.8 with full designer support and perfect RTL text positioning.
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
        private AdvancedTextBoxInput _AdvancedTextBoxInputType = AdvancedTextBoxInput.TextInput;

        // Border styling fields
        private int _borderRadius = 5;
        private int _borderSize = 1;
        private Color _borderColor = Color.Gray;
        private Color _borderFocusColor = Color.Blue;
        private bool _useFocusColor = true;
        private Padding _textPadding = new Padding(8, 4, 8, 4);
        private ContentAlignment _textAlignment = ContentAlignment.MiddleLeft;
        private AdvancedTextBoxBorderSides _AdvancedTextBoxBorderSides = AdvancedTextBoxBorderSides.All;

        // Performance optimization
        private Size _lastSize = Size.Empty;
        private bool _updateBoundsRequired = true;

        private string _placeholderText = string.Empty;
        private Color _placeholderColor = Color.Gray;
        private bool _isPlaceholderActive = false;

        #endregion

        #region Enhanced Properties

        /// <summary>
        /// Gets or sets whether Arabic numerals are allowed in input
        /// </summary>
        [Category("Advanced Validation")]
        [Description("Allows Arabic numerals to be entered")]
        [DefaultValue(false)]
        public bool AllowArabicNumber
        {
            get => _allowArabicNumber;
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
        [Category("Advanced Validation")]
        [Description("Allows decimal points in numeric input")]
        [DefaultValue(false)]
        public bool AllowPoint
        {
            get => _allowPoint;
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
        [Category("Advanced Validation")]
        [Description("Allows negative numbers to be entered")]
        [DefaultValue(false)]
        public bool AllowNegativeNumber
        {
            get => _allowNegativeNumber;
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
            get => _isValidText;
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
        [Category("Advanced Validation")]
        [Description("The type of input validation and formatting to apply")]
        [DefaultValue(AdvancedTextBoxInput.TextInput)]
        public AdvancedTextBoxInput AdvancedTextBoxInputType
        {
            get => _AdvancedTextBoxInputType;
            set
            {
                if (_AdvancedTextBoxInputType != value)
                {
                    _AdvancedTextBoxInputType = value;
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
            get => _borderColor;
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
            get => _borderFocusColor;
            set
            {
                if (_borderFocusColor != value)
                {
                    _borderFocusColor = value;
                    if (_textBox?.Focused == true)
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
            get => _useFocusColor;
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
            get => _borderRadius;
            set
            {
                if (value < 0) value = 0;
                if (Width > 0 && Height > 0 && value > Math.Min(Width, Height) / 2)
                    value = Math.Min(Width, Height) / 2;

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
            get => _borderSize;
            set
            {
                if (value < 0) value = 0;
                if (value > 10) value = 10;

                if (_borderSize != value)
                {
                    _borderSize = value;
                    _updateBoundsRequired = true;
                    UpdateTextBoxBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets which sides of the border to draw
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Specifies which sides of the border to draw")]
        [DefaultValue(AdvancedTextBoxBorderSides.All)]
        public AdvancedTextBoxBorderSides AdvancedTextBoxBorderSides
        {
            get => _AdvancedTextBoxBorderSides;
            set
            {
                if (_AdvancedTextBoxBorderSides != value)
                {
                    _AdvancedTextBoxBorderSides = value;
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
            get
            {
                if (_textBox == null) return string.Empty;
                return _isPlaceholderActive ? string.Empty : _textBox.Text;
            }
            set
            {
                if (_textBox != null)
                {
                    if (_isPlaceholderActive)
                    {
                        HidePlaceholder();
                    }
                    _textBox.Text = value ?? string.Empty;
                    UpdatePlaceholder();
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
            get => base.Font;
            set
            {
                base.Font = value;
                if (_textBox != null)
                {
                    _textBox.Font = value;
                    _updateBoundsRequired = true;
                    UpdateTextBoxBounds();
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
            get => base.ForeColor;
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
            get => _textBox?.Multiline ?? false;
            set
            {
                if (_textBox != null)
                {
                    _textBox.Multiline = value;
                    _updateBoundsRequired = true;
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
            get => _textBox?.MaxLength ?? 32767;
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
            get => _textBox?.ReadOnly ?? false;
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
            get => _textBox?.TextAlign ?? HorizontalAlignment.Left;
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
            get => _textAlignment;
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    UpdateTextBoxAlignment();
                    _updateBoundsRequired = true;
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
            get => _textPadding;
            set
            {
                if (_textPadding != value)
                {
                    _textPadding = value;
                    _updateBoundsRequired = true;
                    UpdateTextBoxBounds();
                }
            }
        }

        /// <summary>
        /// Gets or sets the password character for the textbox
        /// </summary>
        [Category("Behavior")]
        [Description("The password character for the textbox")]
        [DefaultValue('\0')]
        public char PasswordChar
        {
            get => _textBox?.PasswordChar ?? '\0';
            set
            {
                if (_textBox != null)
                {
                    _textBox.PasswordChar = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to use system password character
        /// </summary>
        [Category("Behavior")]
        [Description("Whether to use system password character")]
        [DefaultValue(false)]
        public bool UseSystemPasswordChar
        {
            get => _textBox?.UseSystemPasswordChar ?? false;
            set
            {
                if (_textBox != null)
                {
                    _textBox.UseSystemPasswordChar = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the placeholder text hint
        /// </summary>
        [Category("Appearance")]
        [Description("The placeholder text hint")]
        [DefaultValue("")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                if (_placeholderText != value)
                {
                    _placeholderText = value ?? string.Empty;
                    UpdatePlaceholder();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the placeholder text
        /// </summary>
        [Category("Appearance")]
        [Description("The color of the placeholder text")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color PlaceholderColor
        {
            get => _placeholderColor;
            set
            {
                if (_placeholderColor != value)
                {
                    _placeholderColor = value;
                    if (_isPlaceholderActive)
                    {
                        _textBox.ForeColor = _placeholderColor;
                    }
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

            // Initialize placeholder if needed
            UpdatePlaceholder();
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

                // Add placeholder-specific events
                _textBox.Enter += OnTextBoxEnterPlaceholder;
                _textBox.Leave += OnTextBoxLeavePlaceholder;
            }

            Resize += OnControlResize;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Enter event for placeholder management
        /// </summary>
        private void OnTextBoxEnterPlaceholder(object sender, EventArgs e)
        {
            try
            {
                if (_isPlaceholderActive)
                {
                    HidePlaceholder();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnTextBoxEnterPlaceholder: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the Leave event for placeholder management
        /// </summary>
        private void OnTextBoxLeavePlaceholder(object sender, EventArgs e)
        {
            try
            {
                UpdatePlaceholder();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnTextBoxLeavePlaceholder: {ex.Message}");
            }
        }

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
                // Don't process if placeholder is active
                if (_isPlaceholderActive) return;

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

                _updateBoundsRequired = true;
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

        /// <summary>
        /// FIXED: Handles RightToLeft property changes for proper RTL text positioning
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            if (_textBox != null)
            {
                _textBox.RightToLeft = RightToLeft;

                // CRITICAL FIX: Update text alignment for RTL
                UpdateTextBoxAlignment();
                _updateBoundsRequired = true;
                UpdateTextBoxBounds();

                Invalidate();
            }
        }

        #endregion

        #region Enhanced Drawing Methods





        /// <summary>
        /// FINAL: DrawCustomBorder with perfect border radius handling
        /// </summary>
        private void DrawCustomBorder(Graphics graphics)
        {
            if (_borderSize <= 0 || _AdvancedTextBoxBorderSides == AdvancedTextBoxBorderSides.None) return;

            try
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                // Determine border color based on focus state
                Color currentBorderColor = _borderColor;
                if (_useFocusColor && _textBox?.Focused == true)
                {
                    currentBorderColor = _borderFocusColor;
                }

                // Use proper pen alignment for even borders
                using (var borderPen = new Pen(currentBorderColor, _borderSize))
                {
                    borderPen.Alignment = PenAlignment.Center;

                    if (_borderRadius > 0)
                    {
                        DrawRoundedBorderWithSides_Alternative(graphics, ClientRectangle, borderPen);
                    }
                    else
                    {
                        DrawRectangularBorderWithSides_Alternative(graphics, ClientRectangle, borderPen);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error drawing custom border: {ex.Message}");
            }
        }


        /// <summary>
        /// CORRECTED: Perfect rounded border solution
        /// </summary>
        private void DrawRoundedBorderWithSides_Alternative(Graphics graphics, Rectangle drawRect, Pen borderPen)
        {
            // CRITICAL FIX: For perfect rounded corners, we need to account for the pen width properly
            Rectangle borderRect;

            if (_borderSize <= 1)
            {
                // For thin borders, use the full rectangle minus 1 pixel
                borderRect = new Rectangle(0, 0, Width - 1, Height - 1);
            }
            else
            {
                // For thicker borders, account for the pen width by centering the rectangle
                float halfPen = _borderSize / 2.0f;
                int offset = (int)Math.Round(halfPen);

                borderRect = new Rectangle(
                    offset,
                    offset,
                    Width - (offset * 2),
                    Height - (offset * 2)
                );

                // Ensure minimum size
                if (borderRect.Width < 1) borderRect.Width = 1;
                if (borderRect.Height < 1) borderRect.Height = 1;
            }

            if (_AdvancedTextBoxBorderSides == AdvancedTextBoxBorderSides.All)
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
                DrawCustomRoundedAdvancedTextBoxBorderSides(graphics, borderRect, borderPen);
            }
        }

        /// <summary>
        /// ALTERNATIVE: Simple rectangular border solution
        /// </summary>
        private void DrawRectangularBorderWithSides_Alternative(Graphics graphics, Rectangle drawRect, Pen borderPen)
        {
            // Simple approach: Use the full control rectangle
            Rectangle borderRect = new Rectangle(0, 0, Width, Height);

            if (_AdvancedTextBoxBorderSides == AdvancedTextBoxBorderSides.All)
            {
                graphics.DrawRectangle(borderPen, borderRect);
            }
            else
            {
                // Draw individual sides
                if (_AdvancedTextBoxBorderSides.HasFlag(AdvancedTextBoxBorderSides.Top))
                {
                    graphics.DrawLine(borderPen, 0, 0, Width, 0);
                }
                if (_AdvancedTextBoxBorderSides.HasFlag(AdvancedTextBoxBorderSides.Right))
                {
                    graphics.DrawLine(borderPen, Width - 1, 0, Width - 1, Height);
                }
                if (_AdvancedTextBoxBorderSides.HasFlag(AdvancedTextBoxBorderSides.Bottom))
                {
                    graphics.DrawLine(borderPen, 0, Height - 1, Width, Height - 1);
                }
                if (_AdvancedTextBoxBorderSides.HasFlag(AdvancedTextBoxBorderSides.Left))
                {
                    graphics.DrawLine(borderPen, 0, 0, 0, Height);
                }
            }
        }


        /// <summary>
        /// Draws custom rounded border sides using arcs and lines
        /// </summary>
        private void DrawCustomRoundedAdvancedTextBoxBorderSides(Graphics graphics, Rectangle rect, Pen borderPen)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            int effectiveRadius = Math.Min(_borderRadius, Math.Min(rect.Width / 2, rect.Height / 2));
            if (effectiveRadius <= 1)
            {
                // Fall back to rectangular drawing if radius is too small
                DrawRectangularBorderWithSides_Alternative(graphics, rect, borderPen);
                return;
            }

            int diameter = effectiveRadius * 2;

            // Define corner arc rectangles
            Rectangle topLeftArc = new Rectangle(rect.X, rect.Y, diameter, diameter);
            Rectangle topRightArc = new Rectangle(rect.Right - diameter, rect.Y, diameter, diameter);
            Rectangle bottomRightArc = new Rectangle(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter);
            Rectangle bottomLeftArc = new Rectangle(rect.X, rect.Bottom - diameter, diameter, diameter);

            // Draw corners and sides based on AdvancedTextBoxBorderSides flags
            bool drawTop = _AdvancedTextBoxBorderSides.HasFlag(AdvancedTextBoxBorderSides.Top);
            bool drawRight = _AdvancedTextBoxBorderSides.HasFlag(AdvancedTextBoxBorderSides.Right);
            bool drawBottom = _AdvancedTextBoxBorderSides.HasFlag(AdvancedTextBoxBorderSides.Bottom);
            bool drawLeft = _AdvancedTextBoxBorderSides.HasFlag(AdvancedTextBoxBorderSides.Left);

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
        /// IMPROVED: Creates a graphics path for a rounded rectangle with perfect precision
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
                // CRITICAL FIX: Ensure radius doesn't exceed rectangle dimensions and is properly calculated
                int effectiveRadius = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));

                if (effectiveRadius <= 1)
                {
                    path.AddRectangle(rect);
                    return path;
                }

                // IMPROVED: Use float precision for perfect arcs
                float diameter = effectiveRadius * 2.0f;
                RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);

                // CRITICAL: Start from top-left and go clockwise for consistent corners

                // Top left arc (180° to 270°)
                path.AddArc(arc, 180, 90);

                // Top right arc (270° to 360°/0°)
                arc.X = rect.Right - diameter;
                path.AddArc(arc, 270, 90);

                // Bottom right arc (0° to 90°)
                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);

                // Bottom left arc (90° to 180°)
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
        /// ENHANCED: Updates the bounds of the internal textbox with RTL support
        /// </summary>
        private void UpdateTextBoxBounds()
        {
            if (_textBox == null || (!_updateBoundsRequired && Size == _lastSize)) return;

            try
            {
                // Calculate available space accounting for border and padding
                int borderPadding = Math.Max(1, _borderSize);
                int totalPaddingLeft = borderPadding + _textPadding.Left;
                int totalPaddingTop = borderPadding + _textPadding.Top;
                int totalPaddingRight = borderPadding + _textPadding.Right;
                int totalPaddingBottom = borderPadding + _textPadding.Bottom;

                // CRITICAL RTL FIX: Swap padding for RTL layout
                if (RightToLeft == RightToLeft.Yes)
                {
                    int temp = totalPaddingLeft;
                    totalPaddingLeft = totalPaddingRight;
                    totalPaddingRight = temp;
                }

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
                            y = totalPaddingTop + Math.Max(0, (availableHeight - height) / 2);
                            break;

                        case ContentAlignment.BottomLeft:
                        case ContentAlignment.BottomCenter:
                        case ContentAlignment.BottomRight:
                            y = totalPaddingTop + Math.Max(0, (availableHeight - height));
                            break;
                    }

                    // Ensure y is not negative and within bounds
                    y = Math.Max(totalPaddingTop, Math.Min(y, Height - totalPaddingBottom - height));

                    _textBox.Location = new Point(x, y);
                    _textBox.Size = new Size(width, height);
                }

                _lastSize = Size;
                _updateBoundsRequired = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating textbox bounds: {ex.Message}");
            }
        }

        /// <summary>
        /// ENHANCED: Updates the text alignment of the internal textbox with RTL support
        /// </summary>
        private void UpdateTextBoxAlignment()
        {
            if (_textBox == null) return;

            try
            {
                // Map ContentAlignment to HorizontalAlignment with RTL consideration
                HorizontalAlignment horizontalAlign = HorizontalAlignment.Left;

                switch (_textAlignment)
                {
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.BottomLeft:
                        horizontalAlign = RightToLeft == RightToLeft.Yes ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                        break;

                    case ContentAlignment.TopCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.BottomCenter:
                        horizontalAlign = HorizontalAlignment.Center;
                        break;

                    case ContentAlignment.TopRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.BottomRight:
                        horizontalAlign = RightToLeft == RightToLeft.Yes ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                        break;

                    default:
                        horizontalAlign = RightToLeft == RightToLeft.Yes ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                        break;
                }

                _textBox.TextAlign = horizontalAlign;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating textbox alignment: {ex.Message}");
            }
        }

        #endregion

        #region Input Validation Methods

        private void ConvertArabicToEnglishNumerals(KeyPressEventArgs e)
        {
            try
            {
                if (!_allowArabicNumber)
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
            switch (_AdvancedTextBoxInputType)
            {
                case AdvancedTextBoxInput.IntegerInput:
                case AdvancedTextBoxInput.TextIntegerNumberInput:
                    ValidateIntegerInput(e);
                    break;

                case AdvancedTextBoxInput.DoubleInput:
                case AdvancedTextBoxInput.TextDoubleNumberInput:
                    ValidateDoubleInput(e);
                    break;

                case AdvancedTextBoxInput.TextInput:
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

                switch (_AdvancedTextBoxInputType)
                {
                    case AdvancedTextBoxInput.IntegerInput:
                        ValidText = NumberInputUtils.IsValidNumericString(normalizedText, allowDecimal: false, allowNegative: _allowNegativeNumber) &&
                                   long.TryParse(normalizedText, out _);
                        break;

                    case AdvancedTextBoxInput.DoubleInput:
                        ValidText = NumberInputUtils.IsValidNumericString(normalizedText, allowDecimal: true, allowNegative: _allowNegativeNumber) &&
                                   double.TryParse(normalizedText, out _);
                        break;

                    case AdvancedTextBoxInput.TextInput:
                    case AdvancedTextBoxInput.TextIntegerNumberInput:
                    case AdvancedTextBoxInput.TextDoubleNumberInput:
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
                switch (_AdvancedTextBoxInputType)
                {
                    case AdvancedTextBoxInput.IntegerInput:
                        ValidText = long.TryParse(Text, out _);
                        break;

                    case AdvancedTextBoxInput.DoubleInput:
                        ValidText = double.TryParse(Text, out _);
                        break;

                    case AdvancedTextBoxInput.TextInput:
                    case AdvancedTextBoxInput.TextIntegerNumberInput:
                    case AdvancedTextBoxInput.TextDoubleNumberInput:
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

        #region Text Formatting Methods

        private void PrepareTextForEditing()
        {
            try
            {
                switch (_AdvancedTextBoxInputType)
                {
                    case AdvancedTextBoxInput.IntegerInput:
                        PrepareIntegerForEditing();
                        break;

                    case AdvancedTextBoxInput.DoubleInput:
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
                switch (_AdvancedTextBoxInputType)
                {
                    case AdvancedTextBoxInput.IntegerInput:
                        FormatIntegerText();
                        break;

                    case AdvancedTextBoxInput.DoubleInput:
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
                if (Multiline)
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

        #region Enhanced Public Methods

        /// <summary>
        /// Sets input validation properties in one method call
        /// </summary>
        /// <param name="inputType">The input type to use</param>
        /// <param name="allowArabicNumbers">Whether to allow Arabic numerals</param>
        /// <param name="allowNegative">Whether to allow negative numbers</param>
        /// <param name="allowDecimalPoint">Whether to allow decimal points</param>
        public void SetInputValidation(AdvancedTextBoxInput inputType, bool allowArabicNumbers = false,
            bool allowNegative = false, bool allowDecimalPoint = false)
        {
            _AdvancedTextBoxInputType = inputType;
            _allowArabicNumber = allowArabicNumbers;
            _allowNegativeNumber = allowNegative;
            _allowPoint = allowDecimalPoint;

            ValidateCurrentText();
        }

        /// <summary>
        /// ENHANCED: Sets border styling properties in one method call with selective sides support
        /// </summary>
        /// <param name="size">Border size</param>
        /// <param name="color">Border color</param>
        /// <param name="radius">Border radius for rounded corners</param>
        /// <param name="sides">Which sides to draw (optional, defaults to All)</param>
        /// <param name="focusColor">Border color when focused (optional)</param>
        /// <param name="textPadding">Text padding (optional)</param>
        public void SetBorderStyle(int size, Color color, int radius = 0, AdvancedTextBoxBorderSides sides = AdvancedTextBoxBorderSides.All,
            Color? focusColor = null, Padding? textPadding = null)
        {
            _borderSize = Math.Max(0, Math.Min(size, 10));
            _borderColor = color;
            _borderRadius = Math.Max(0, radius);
            _AdvancedTextBoxBorderSides = sides;

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

            _updateBoundsRequired = true;
            UpdateTextBoxBounds();
            UpdateRegion();
            Invalidate();
        }

        /// <summary>
        /// Sets which sides of the border to draw
        /// </summary>
        /// <param name="sides">Border sides to draw</param>
        public void SetAdvancedTextBoxBorderSides(AdvancedTextBoxBorderSides sides)
        {
            if (_AdvancedTextBoxBorderSides != sides)
            {
                _AdvancedTextBoxBorderSides = sides;
                Invalidate();
            }
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
            _updateBoundsRequired = true;
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
            _updateBoundsRequired = true;
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
            get => _textBox?.SelectedText ?? string.Empty;
            set { if (_textBox != null) _textBox.SelectedText = value; }
        }

        /// <summary>
        /// Gets or sets the selection start position
        /// </summary>
        public int SelectionStart
        {
            get => _textBox?.SelectionStart ?? 0;
            set { if (_textBox != null) _textBox.SelectionStart = value; }
        }

        /// <summary>
        /// Gets or sets the selection length
        /// </summary>
        public int SelectionLength
        {
            get => _textBox?.SelectionLength ?? 0;
            set { if (_textBox != null) _textBox.SelectionLength = value; }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Updates the placeholder display based on current text state
        /// </summary>
        private void UpdatePlaceholder()
        {
            if (_textBox == null) return;

            try
            {
                if (string.IsNullOrEmpty(_textBox.Text) && !string.IsNullOrEmpty(_placeholderText) && !_textBox.Focused)
                {
                    ShowPlaceholder();
                }
                else if (_isPlaceholderActive)
                {
                    HidePlaceholder();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating placeholder: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows the placeholder text
        /// </summary>
        private void ShowPlaceholder()
        {
            if (_textBox == null || _isPlaceholderActive) return;

            try
            {
                _isPlaceholderActive = true;
                _textBox.Text = _placeholderText;
                _textBox.ForeColor = _placeholderColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing placeholder: {ex.Message}");
            }
        }

        /// <summary>
        /// Hides the placeholder text
        /// </summary>
        private void HidePlaceholder()
        {
            if (_textBox == null || !_isPlaceholderActive) return;

            try
            {
                _isPlaceholderActive = false;
                _textBox.Text = string.Empty;
                _textBox.ForeColor = ForeColor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error hiding placeholder: {ex.Message}");
            }
        }

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

                    // Remove placeholder-specific events
                    _textBox.Enter -= OnTextBoxEnterPlaceholder;
                    _textBox.Leave -= OnTextBoxLeavePlaceholder;
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
            get => true;
            set => base.DoubleBuffered = value;
        }

        #endregion
    }
}