using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced Action Button control with multi-language support, custom styling, and embedded icons.
    /// Supports rounded corners, custom borders, and automatic text/icon switching based on action type.
    /// Optimized for .NET Framework 4.8 with designer support and comprehensive localization.
    /// </summary>
    [ToolboxItem(true)]
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
    public partial class AdvancedActionButton : Button
    {
        #region Enums
        /// <summary>
        /// Defines the available button image types
        /// </summary>
        public enum ButtonImageEnum
        {
            /// <summary>Add/Insert icon</summary>
            Add = 0,
            /// <summary>Edit/Update icon</summary>
            Edit = 1,
            /// <summary>Print icon</summary>
            Print = 2,
            /// <summary>Delete icon</summary>
            Delete = 3,
            /// <summary>Refresh icon</summary>
            Refresh = 4,
            /// <summary>Search icon</summary>
            Search = 5,
        }

        /// <summary>
        /// Defines the available button action types
        /// </summary>
        public enum ButtonActionTypes
        {
            /// <summary>Insert/Add action</summary>
            Insert,
            /// <summary>Update/Save action</summary>
            Update,
            /// <summary>Delete action</summary>
            Delete,
            /// <summary>Refresh action</summary>
            Refresh,
            /// <summary>Print action</summary>
            Print,
            /// <summary>Search action</summary>
            Search,
        }

        #endregion

        #region Constants
        /// <summary>
        /// Default forecolor for the button text
        /// </summary>
        private static readonly Color DEFAULT_FORECOLOR = Color.FromArgb(40, 40, 40);

        /// <summary>
        /// Default button border size
        /// </summary>
        private const int DEFAULT_BUTTON_BORDER_SIZE = 2;

        /// <summary>
        /// Default button border radius
        /// </summary>
        private const int DEFAULT_BUTTON_BORDER_RADIUS = 5;

        /// <summary>
        /// Default button border color
        /// </summary>
        private static readonly Color DEFAULT_BUTTON_BORDER_COLOR = Color.FromArgb(213, 218, 223);

        /// <summary>
        /// Default button image size
        /// </summary>
        private const int DEFAULT_BUTTON_IMAGE_SIZE = 25;
        #endregion

        #region Private Fields

        private ButtonActionTypes _actionType;
        private int _borderSize = DEFAULT_BUTTON_BORDER_SIZE;
        private int _borderRadius = DEFAULT_BUTTON_BORDER_RADIUS;
        private Color _borderColor = DEFAULT_BUTTON_BORDER_COLOR;
        private int _buttonImageSize = DEFAULT_BUTTON_IMAGE_SIZE;
        private bool _invalidateRequired = false;
        private bool _showIconOnly = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the action type of the button, which determines the displayed text and icon
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The action type that determines the button's text and icon")]
        [DefaultValue(ButtonActionTypes.Insert)]
        public ButtonActionTypes ActionType
        {
            get { return _actionType; }
            set
            {
                if (_actionType != value)
                {
                    _actionType = value;
                    _invalidateRequired = true;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the border around the button
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The thickness of the button's border")]
        public int BorderSize
        {
            get { return _borderSize; }
            set
            {
                if (value < 0) value = 0;
                if (value > 20) value = 20;

                if (_borderSize != value)
                {
                    _borderSize = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the radius of the button's rounded corners
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The radius of the button's rounded corners")]
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
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the button's border
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The color of the button's border")]
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
        /// Gets or sets the size of the button's icon in pixels
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The size of the button's icon in pixels")]
        public int ButtonImageSize
        {
            get { return _buttonImageSize; }
            set
            {
                if (value < 16) value = 16;
                if (value > 64) value = 64;

                if (_buttonImageSize != value)
                {
                    _buttonImageSize = value;
                    _invalidateRequired = true;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show only the icon without text
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Shows only the icon without displaying text")]
        [DefaultValue(false)]
        public bool ShowIconOnly
        {
            get { return _showIconOnly; }
            set
            {
                if (_showIconOnly != value)
                {
                    _showIconOnly = value;
                    _invalidateRequired = true;
                    UpdateButtonLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the localized text for the current action type and selected language
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private string ButtonText
        {
            get
            {
                try
                {
                    return GetLocalizedText(_actionType);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting button text: {ex.Message}");
                    return _actionType.ToString(); // Fallback to enum name
                }
            }
        }

        /// <summary>
        /// Gets the icon image for the current action type
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private Image ButtonImage
        {
            get
            {
                try
                {
                    ButtonImageEnum imageType = GetButtonImageType(_actionType);
                    return GetButtonImage(imageType);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting button image: {ex.Message}");
                    return null; // Return null if image loading fails
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedActionButton class
        /// </summary>
        public AdvancedActionButton()
        {
            InitializeComponent();
            SetDefaultProperties();
        }

        /// <summary>
        /// Initializes the component with optimal rendering settings
        /// </summary>
        private void InitializeComponent()
        {
            // Enable double buffering and optimized rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// Sets the default properties for the button
        /// </summary>
        private void SetDefaultProperties()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size = new Size(150, 40);
            BackColor = Color.White;
            ForeColor = DEFAULT_FORECOLOR;
            ImageAlign = ContentAlignment.MiddleRight;
            TextAlign = ContentAlignment.MiddleLeft;
            Padding = new Padding(10, 0, 10, 0);
            RightToLeft = LanguageManager.SelectedLanguage == FormLanguage.English ? RightToLeft.No : RightToLeft.Yes;
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Handles the paint event to draw custom appearance
        /// </summary>
        /// <param name="pevent">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            try
            {
                // Set high-quality rendering
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Draw custom border and shape
                DrawCustomBorder(pevent.Graphics);

                // Update text and image if needed
                UpdateButtonContent();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnPaint: {ex.Message}");
                // Continue with base painting if custom painting fails
            }
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
            if (_borderRadius > maxRadius)
            {
                _borderRadius = maxRadius;
            }

            Invalidate();
        }

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Draws the custom border and shape of the button
        /// </summary>
        /// <param name="graphics">Graphics object for drawing</param>
        private void DrawCustomBorder(Graphics graphics)
        {
            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -_borderSize, -_borderSize);
            int smoothSize = Math.Max(2, _borderSize);

            try
            {
                if (_borderRadius > 2) // Rounded button
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
        /// Draws a rounded border for the button
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="rectSurface">Surface rectangle</param>
        /// <param name="rectBorder">Border rectangle</param>
        /// <param name="smoothSize">Smoothing size</param>
        private void DrawRoundedBorder(Graphics graphics, Rectangle rectSurface, Rectangle rectBorder, int smoothSize)
        {
            using (GraphicsPath pathSurface = CreateRoundedRectanglePath(rectSurface, _borderRadius))
            using (GraphicsPath pathBorder = CreateRoundedRectanglePath(rectBorder, Math.Max(0, _borderRadius - _borderSize)))
            using (Pen penSurface = new Pen(Parent?.BackColor ?? SystemColors.Control, smoothSize))
            using (Pen penBorder = new Pen(_borderColor, _borderSize))
            {
                // Set button region
                Region = new Region(pathSurface);

                // Draw surface border for smooth result
                graphics.DrawPath(penSurface, pathSurface);

                // Draw control border
                if (_borderSize >= 1)
                {
                    graphics.DrawPath(penBorder, pathBorder);
                }
            }
        }

        /// <summary>
        /// Draws a rectangular border for the button
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="rectSurface">Surface rectangle</param>
        /// <param name="smoothSize">Smoothing size</param>
        private void DrawRectangularBorder(Graphics graphics, Rectangle rectSurface, int smoothSize)
        {
            graphics.SmoothingMode = SmoothingMode.None;

            // Set button region
            Region = new Region(rectSurface);

            // Draw border
            if (_borderSize >= 1)
            {
                using (Pen penBorder = new Pen(_borderColor, _borderSize))
                {
                    penBorder.Alignment = PenAlignment.Inset;
                    graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
                }
            }
        }

        /// <summary>
        /// Draws a simple border as fallback
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="rect">Rectangle to draw</param>
        private void DrawSimpleBorder(Graphics graphics, Rectangle rect)
        {
            if (_borderSize >= 1)
            {
                using (Pen pen = new Pen(_borderColor, _borderSize))
                {
                    graphics.DrawRectangle(pen, rect);
                }
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
        /// Updates the button's text and image content
        /// </summary>
        private void UpdateButtonContent()
        {
            if (_invalidateRequired)
            {
                try
                {
                    // Set text based on ShowIconOnly property
                    Text = _showIconOnly ? string.Empty : ButtonText;
                    Image = ButtonImage;
                }
                finally
                {
                    _invalidateRequired = false;
                }
            }
        }

        /// <summary>
        /// Updates the button layout based on ShowIconOnly setting
        /// </summary>
        private void UpdateButtonLayout()
        {
            if (_showIconOnly)
            {
                // Center the image when showing icon only
                ImageAlign = ContentAlignment.MiddleCenter;
                TextAlign = ContentAlignment.MiddleCenter;
                // Adjust padding for better icon centering
                Padding = new Padding(5, 5, 5, 5);
            }
            else
            {
                // Restore default layout for text + icon
                ImageAlign = ContentAlignment.MiddleRight;
                TextAlign = ContentAlignment.MiddleLeft;
                Padding = new Padding(10, 0, 10, 0);
            }
        }

        #endregion

        #region Localization Methods

        /// <summary>
        /// Gets the localized text for the specified action type
        /// </summary>
        /// <param name="actionType">The action type to get text for</param>
        /// <returns>Localized text string</returns>
        private string GetLocalizedText(ButtonActionTypes actionType)
        {
            FormLanguage language = LanguageManager.SelectedLanguage;

            switch (actionType)
            {
                case ButtonActionTypes.Insert:
                    return GetInsertText(language);
                case ButtonActionTypes.Update:
                    return GetUpdateText(language);
                case ButtonActionTypes.Delete:
                    return GetDeleteText(language);
                case ButtonActionTypes.Refresh:
                    return GetRefreshText(language);
                case ButtonActionTypes.Print:
                    return GetPrintText(language);
                case ButtonActionTypes.Search:
                    return GetSearchText(language);
                default:
                    return actionType.ToString();
            }
        }

        /// <summary>
        /// Gets localized text for Insert action
        /// </summary>
        private string GetInsertText(FormLanguage language)
        {
            switch (language)
            {
                case FormLanguage.English: return "Add";
                case FormLanguage.Kurdish: return "زیادکردن";
                case FormLanguage.Arabic: return "إضافة";
                default: return "Add";
            }
        }

        /// <summary>
        /// Gets localized text for Update action
        /// </summary>
        private string GetUpdateText(FormLanguage language)
        {
            switch (language)
            {
                case FormLanguage.English: return "Save";
                case FormLanguage.Kurdish: return "نوێکردنەوە";
                case FormLanguage.Arabic: return "تحدیث";
                default: return "Save";
            }
        }

        /// <summary>
        /// Gets localized text for Delete action
        /// </summary>
        private string GetDeleteText(FormLanguage language)
        {
            switch (language)
            {
                case FormLanguage.English: return "Delete";
                case FormLanguage.Kurdish: return "سڕینەوە";
                case FormLanguage.Arabic: return "حذف";
                default: return "Delete";
            }
        }

        /// <summary>
        /// Gets localized text for Refresh action
        /// </summary>
        private string GetRefreshText(FormLanguage language)
        {
            switch (language)
            {
                case FormLanguage.English: return "Refresh";
                case FormLanguage.Kurdish: return "پاککردنەوە";
                case FormLanguage.Arabic: return "تنظيف";
                default: return "Refresh";
            }
        }

        /// <summary>
        /// Gets localized text for Print action
        /// </summary>
        private string GetPrintText(FormLanguage language)
        {
            switch (language)
            {
                case FormLanguage.English: return "Print";
                case FormLanguage.Kurdish: return "چاپکردن";
                case FormLanguage.Arabic: return "الطباعة";
                default: return "Print";
            }
        }

        /// <summary>
        /// Gets localized text for Search action
        /// </summary>
        private string GetSearchText(FormLanguage language)
        {
            switch (language)
            {
                case FormLanguage.English: return "Search";
                case FormLanguage.Kurdish: return "گەڕان";
                case FormLanguage.Arabic: return "بحث";
                default: return "Search";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets border properties in one method call
        /// </summary>
        /// <param name="radius">Border radius</param>
        /// <param name="size">Border size</param>
        /// <param name="color">Border color</param>
        public void SetBorder(int radius, int size, Color color)
        {
            _borderRadius = Math.Max(0, radius);
            _borderSize = Math.Max(0, size);
            _borderColor = color;
            Invalidate();
        }

        /// <summary>
        /// Updates the button's language-dependent content
        /// </summary>
        public void RefreshLanguageContent()
        {
            _invalidateRequired = true;
            SetDefaultProperties(); // Update RTL setting
            Invalidate();
        }

        #endregion

        #region Image Methods

        /// <summary>
        /// Gets the button image type for the specified action type
        /// </summary>
        /// <param name="actionType">The action type</param>
        /// <returns>Corresponding button image type</returns>
        private ButtonImageEnum GetButtonImageType(ButtonActionTypes actionType)
        {
            switch (actionType)
            {
                case ButtonActionTypes.Insert: return ButtonImageEnum.Add;
                case ButtonActionTypes.Update: return ButtonImageEnum.Edit;
                case ButtonActionTypes.Delete: return ButtonImageEnum.Delete;
                case ButtonActionTypes.Refresh: return ButtonImageEnum.Refresh;
                case ButtonActionTypes.Print: return ButtonImageEnum.Print;
                case ButtonActionTypes.Search: return ButtonImageEnum.Search;
                default: return ButtonImageEnum.Add;
            }
        }

        /// <summary>
        /// Creates an icon image from base64 string with proper error handling
        /// </summary>
        /// <param name="imageBase64">Base64 encoded image string</param>
        /// <returns>Resized image or null if loading fails</returns>
        private Image CreateIconFromBase64(string imageBase64)
        {
            if (string.IsNullOrEmpty(imageBase64))
            {
                System.Diagnostics.Debug.WriteLine("Warning: Empty or null image base64 string");
                return null;
            }

            try
            {
                byte[] imageBytes = Convert.FromBase64String(imageBase64);

                using (var ms = new MemoryStream(imageBytes))
                using (Image originalImage = Image.FromStream(ms))
                {
                    return CreateResizedImage(originalImage, _buttonImageSize, _buttonImageSize);
                }
            }
            catch (FormatException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid base64 format: {ex.Message}");
                return null;
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Invalid image data: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading icon: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates a resized copy of an image with high quality settings
        /// </summary>
        /// <param name="originalImage">Source image to resize</param>
        /// <param name="width">Target width</param>
        /// <param name="height">Target height</param>
        /// <returns>Resized image</returns>
        private Image CreateResizedImage(Image originalImage, int width, int height)
        {
            if (originalImage == null)
                throw new ArgumentNullException(nameof(originalImage));

            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive values");

            try
            {
                Bitmap resizedImage = new Bitmap(width, height);

                using (Graphics graphics = Graphics.FromImage(resizedImage))
                {
                    // Set high-quality rendering settings
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    // Draw the resized image
                    graphics.DrawImage(originalImage, 0, 0, width, height);
                }

                return resizedImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resizing image: {ex.Message}");
                throw; // Re-throw to be handled by caller
            }
        }

        /// <summary>
        /// Gets the button image for the specified image type
        /// </summary>
        /// <param name="buttonImage">The button image type</param>
        /// <returns>Image object or null if loading fails</returns>
        private Image GetButtonImage(ButtonImageEnum buttonImage)
        {
            try
            {
                int index = (int)buttonImage;
                if (index >= 0 && index < ImagesList.Length)
                {
                    return CreateIconFromBase64(ImagesList[index]);
                }

                System.Diagnostics.Debug.WriteLine($"Warning: Invalid button image index: {index}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting button image: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Base64 Images

        private const string AddImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAABmJLR0QA/wD/AP+gvaeTAAAA5klEQVRYCe1V2w2CQBCcJf7Qg4X46AO7UEtBbUKxDqURe+DPdZfccV+GW34wcUmGmwuz3DIkO8C/XzTVgPLMSxDWfT2j7fb06rnxVhj1Sc7YEKNRQHh6YGPTG7Cd81XtDbgD7oA7MLsDiziiwmzfxv3YWjBWHESFZEJ54fxcYTxidgwNaLDIXL+Gd44u8XAVCj9KrdIsSKuVCO8C/M4vkERrmbHTrnKgtuuXq1a8r98Sycoz0Uad1EZqW8sTV0S4aZU23h2oUW7F7L/AG3AH3AF3YHYHUhpaZyjhGVJNK4fZrhuHxYEPBMYxLGWfkNMAAAAASUVORK5CYII=";
        private const string EditImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAABmJLR0QA/wD/AP+gvaeTAAACIElEQVRYCe1Uv0/bQBS+F4SQWKiEysCGEBJqxcSKVIKN7YBUlakSKgQGCL//CMZupALBgm0murQdKUqW0pWZCaGOlUBqN1DiHN87iJXEJg1O0snRu5d7P+597747nxDxL2YgZiBm4D8xoLlWRnPN7Vq4jlpHO2zdMZZRdx/jzcC7weLVt8szzJUklG6j0hxrSwpicAKMR1Jc4t+XDn/WholuG5uCxA5KK3Ah5Yf84ukxbF844ButnDC4JKoLznhtYWDCNTeEoCwAeINe2M4RU8IJatIqxeA454bAGbOlDWiOuY6inzC4bt2dI0cJJ6pJsyoKOGMGPkP9yBozbGOYg40OzTXSyC3Tjq+O1mtvO+KhUtWAblvjsiS/F4l+6PbUSOiKGideuDUhyYabazHts/n0yQHshoQX+YmlhFyC0Y1zeSnJO5m0J4dgPykTtjmHGx5p5+WiVQ38vb1eEEJ+FQ+//hIlfiYd8/WDWa3xvK4SCRde/pQ9NPKsnWOdkqoGzjPnhT93N+8rmuhDQl47TL1S2Y+KwXHQuzAJg8EDLxz8DQkXCCSOHox2vujq/YzHZOYx+JsSXjI3n7sA7SvY+R78vLYpcNTAS806ZKSyqa5Cj/dFSppSYRK/0FAWVH+EzbSDBFp9zoXDuoBQwFPhCGGiHG165+VCdRvgpJAmWgbO9f/ZACdVNPEWRxD5wnGtyIPvhO6Y05ELxAtjBp5g4B6W5cTElezejAAAAABJRU5ErkJggg==";
        private const string PrintImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAABAlJREFUeJztm0924kYQxr9qYTK7aD/hRTnBkBMMPoHhQbIdzcJkduEGcU4QZ+cHC8vbCX4iJwicYJgTjBwm68BuwtBdWSASjBG0hETz77eCpqtV+qxuVVW3CQnwHdeW6uwVGGWASknGSA/ugtCp/dn6NYk1xTVoF+plALcA7CQXzJBAKa58/1erH8colgD3hUuXQbfx/NoqQ6X4PI4I2gL4zhtHSvUOu/eXXySwrPG3lcAb6nQWuqNKqa6w+zcPAI5SeVe3cy7GwBdL2u4sa9zQVTttfMe1lcx3GHg5387AKwDXOmNoTYG3zy+LQtC7R42Mh9rHpqPpa2b4jmtLmf97sb02aGrdm9YUILKePPpECHRssyZ8+t4ntddeA3YZAhJPwYMQYBNOAph2wDQnAUw7YJqTAKYdMM1JANMOmOboBViaMPiOa3+enL2YfRcCRTA9zq4IfaW4kbF/WghB12AU59sUc2n++1nu8/tlWesjAcKs7ycA5Swc3QE6SvHP8xWj/wTYg3JXahD4dXXQ8qafcVw3P2MmAoUFhQ/Yj3JXmgwta/xNTk6+uALxYdw84wHEfQLZDBQBfLmitz1R+YaA4JcrOu0Td1ZuXKwNWuXqoFmyrLED8O+rDAi4oHahzltyMDMI6FUHzdJiu++4tpzk+yB8HWV7IIEQe8tap+996qyyPAgBFFtB1G8seGW98CAEgFClqJ+I4aw0TdsXExDjR99xn7zJ3j6/LGK6SRJtewiLYEjAoEbO+qcHPLOlkhdgusKa+CbO1tiu4xC4I2UegILuvu9BTIFNOAlg2gHTHL0AiRfBqPDTBPeFenfxjIAuR/8EnAQw7YBpUg2EFqvJWRBV3U1KqgJMJs+KgtQfaY655BrnALppjSewwfmafYeAniCCZ9oRYxA6QoixB2Bk2hcDjIQYe7lK4A3bhboLwN90RGY5FES9zX2LRrFMawF0K4E3zAFAbdDs3BcuXzPoGqtLySsJt5xKKTmYFSMCN6qDVgeYiwOqg5ZnWaII4A6HOSVGAO4sSxRn22LAwmuwEtwEAFxgWk6anRAVQhWZ8cu6K0w3V9f3W0dt0DrfdAxmcf7/ZzmMOkIfGQfMG/z21RsQqbUXnQoWXaDcJt99vOnq9Dv6UPgkgGkHTJNqLpDLfeqHsfrekKoAYZbWTXPMrDmIKcCUPHjTEoCXhJ+M6C3nbeI7rr14QiwOWgKEMcFidOi0Cz/cLtuT2xbT4z1ny3IY7RQ/zhrQwZONRnalzLvtQj3GMOkh5fL2OCm+9hpgWeIK+5AjMB7CFF8LbQEqwU1A2I2ToSsYKeZynJphrLdAmEVVsItPAuNBKS5l+s/TM3zHtZXKu2CUk+7IpAUBPYC9+RQ3Dv8CfDBn+eF40uIAAAAASUVORK5CYII=";
        private const string DeleteImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAABHNCSVQICAgIfAhkiAAAAsNJREFUeJztm01y2kAQRr8epD03CNk6JgU7F4ayOEHICcwVuAE3iI9AThByAouS8JZUHJItvgFsDZ7Ohrj4ke2RGKmpMK/KmynRfvMhqY1RE3JiEgRlb7W6JuYOgCBjmZCJhivP+1oPw7lFvWcoj6LTRqPGRLcAypZKzom5/eHu7oeles9YD+DPxUXlqVSawN7m/zFf+v5722eCslkMAJ5KpT7sbx4Ayv5yeWO7qPUAAFznUDO32lYvgftmMyDm2911pdTnsygapqn1u9XqaK2/7a4zUbsax+EBmttutgq9BDGP0m4eAM6iaEjMozycNnnzDJg2GjUAn0yKMVGA/ZY3I+ZBWrF1vS6AytYa81ABpt3g+1ud49UA1u1sYvjLjhJirr8WQmIAP1utKwBQWncBdHMxK46BVmoAAB+jaO+S8pJeobQO83UqlO76jQQS3vDcb4LHjgtAWkCaxHtAEf3X4TgOjD4LrP8g+pK3jE2IuWfy/4PEe8AuWqkyMQcHWxWIVsroI/nJdwEXgLSANC4AaQFpjLpACh6YaLC5oLQOmOhqc42YR1qpMMtxxNwF8M6WsNUAiHl2Ph73N9fum80+MW9tTCsVVuM403HTRiNgImsBnPwl4AKQFpDGBSAtII0LQFpAGheAtIA0LgBpAWlcANIC0rgApAWkcQFIC0jjApAWkMYFIC0gjQtAWkAaF4C0gDQuAGkBaU4+AKOHpF4ahEhgjv1H2SvYeeQdwGz9k+W4GgxGckwHK2x/PV6G2YhcBfubPeS4zPy3l4DS2mi6zHhm6NflJWfXKZ7z8dhob2nOgIeMLhIYuxoHsPvoyzGTxtU4gJXn3RCwyGRUIAQsVp5nPGBpHEA9DOeaqJNNqzg0USfNeG2qLlCN45CJ2sd4JhCwIOZ62qHK1G2wGsfho+9XAPSOYbBi7dB79P1Klunyv7vz35+tuznWAAAAAElFTkSuQmCC";
        private const string RefreshImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAARzQklUCAgICHwIZIgAAAKwSURBVEiJtZVPUttYEMZ//QIJNUOqdIMoJxixTDlmnBNEN4g4QeTNxNppp+CN4ARoThBxgtgmLpaCE4TcACpmygbzehayRWTHKVxFeqX31O/7+n8Lv0PSwmEy+gqamd9CMB6FgAPyRh4dvLIeB5G2IS0cuoO/H41g/P0AcACwT3LDZNTDSo9kkFVKyUmLtHDWBk9OXUTelQc5Jnp1YUDd8mycClz4zM11sL75t3H1qZIDbKCbHubO5+mfpQdb22fcXLer80OlO/Swdma9XrH1Rw7weElOTl24jRF8lJxoNwDYWFKS2/dgHIwc8k/jbDXgMEBsCjigF8g04+nzEAgZj6v8LfTBtAcSggZY+7m06ieSFg7YmHm1IC5KzGT0lcl1+qPqPUGZ3Bc//HMwt95PCdo7l0RNFzV7wHntDRog04Lu0KsTbG2fgV7VgOzmcoj2Bz4fTwqSkxZRI6PT9FDeoPpvjchqXCdo71yi4s8sOkfNHtGrixp4d+ihcgTUPYuaPaLdAN14CRyCXqHqwDpVlJzECO+ZjQA+vD5YqZsWDjffW3zYzX9NkJy6mOk7VAMQF6D0rJGtfLM/8LGbZ3Pv62W6/yVE7QsQr+zwqYsCCGWMN+OlsC1aPhl9guk3wF0mUE3voyag9DGSY5/kvwSey3y8CPn8aoHA7CH2CLhEdI/Obs46YjVEAN2o8rOcg7JDj2anEGP6qH2LpU/U7NV0u0Ov6vb9gY/KJ5BjOq/9ucryRosaWdlAegUcYG2BRWrgaeHw8csRd/Z+pFsJyyhorbpWV1E5HTPgr5lqhrEZVjwgBulXls69VvpEzdbDCOaSDAOw8cIYAYxPp3FcW5HG7CwOyIc1Wlo4jP/zwQYIq9brIZ1muHi5/j5ITl2wLbhrIeJRhvCcZ9st2juXi+r/A4smHbqAu/mhAAAAAElFTkSuQmCC";
        private const string SearchImageBase64 = "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";

        /// <summary>
        /// List of base64 encoded images for button actions
        /// </summary>
        public static string[] ImagesList = {
            AddImageBase64,
            EditImageBase64,
            PrintImageBase64,
            DeleteImageBase64,
            RefreshImageBase64,
            SearchImageBase64,
        };

        #endregion
    }
}
