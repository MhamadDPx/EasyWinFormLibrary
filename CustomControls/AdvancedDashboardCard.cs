using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced Dashboard Card control for displaying key metrics and information in a visually appealing format.
    /// Provides gradient backgrounds, rounded corners, customizable icons, and multi-language support.
    /// Features include dynamic layout adjustment, color customization, and professional styling options.
    /// Optimized for .NET Framework 4.8 with designer support and comprehensive property management.
    /// </summary>
    [ToolboxItem(true)]
    [DesignerCategory("Component")]
    public partial class AdvancedDashboardCard : UserControl
    {
        #region Constants

        /// <summary>
        /// Default column width when icon is visible
        /// </summary>
        private const float ICON_VISIBLE_COLUMN_WIDTH = 100f;

        /// <summary>
        /// Default column width when icon is hidden
        /// </summary>
        private const float ICON_HIDDEN_COLUMN_WIDTH = 1f;

        /// <summary>
        /// Index of the icon column in the table layout panel
        /// </summary>
        private const int ICON_COLUMN_INDEX = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the right-to-left layout direction of the card
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Sets the right-to-left layout direction of the card")]
        [DefaultValue(RightToLeft.No)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public RightToLeft CardRightToLeft
        {
            get { return this.RightToLeft; }
            set
            {
                if (this.RightToLeft != value)
                {
                    this.RightToLeft = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the radius of the card's rounded corners
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The radius of the card's rounded corners in pixels")]
        [DefaultValue(10)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CardRadius
        {
            get { return MainPanelElipse?.ElipseRadius ?? 0; }
            set
            {
                if (MainPanelElipse != null && MainPanelElipse.ElipseRadius != value)
                {
                    MainPanelElipse.ElipseRadius = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the start color of the card's gradient background
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The start color of the card's gradient background")]
        [DefaultValue(typeof(Color), "White")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CardFillColor1
        {
            get { return MainPanel?.GradientStartColor ?? Color.White; }
            set
            {
                if (MainPanel != null && MainPanel.GradientStartColor != value)
                {
                    MainPanel.GradientStartColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the end color of the card's gradient background
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The end color of the card's gradient background")]
        [DefaultValue(typeof(Color), "LightGray")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CardFillColor2
        {
            get { return MainPanel?.GradientEndColor ?? Color.LightGray; }
            set
            {
                if (MainPanel != null && MainPanel.GradientEndColor != value)
                {
                    MainPanel.GradientEndColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the card title text
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The color of the card title text")]
        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CardTitleTextColor
        {
            get { return lblTitle?.ForeColor ?? Color.Black; }
            set
            {
                if (lblTitle != null && lblTitle.ForeColor != value)
                {
                    lblTitle.ForeColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the card value text
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The color of the card value text")]
        [DefaultValue(typeof(Color), "DarkBlue")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CardTitleValueColor
        {
            get { return lblValue?.ForeColor ?? Color.DarkBlue; }
            set
            {
                if (lblValue != null && lblValue.ForeColor != value)
                {
                    lblValue.ForeColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the card icon is visible
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("Shows or hides the card icon")]
        [DefaultValue(true)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CardShowIcon
        {
            get { return picCardIcon?.Visible ?? false; }
            set
            {
                if (picCardIcon != null && picCardIcon.Visible != value)
                {
                    picCardIcon.Visible = value;
                    UpdateIconColumnLayout(value);
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the title text displayed on the card
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The title text displayed on the card")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CardTitleText
        {
            get { return lblTitle?.Text ?? string.Empty; }
            set
            {
                if (lblTitle != null && lblTitle.Text != value)
                {
                    lblTitle.Text = value ?? string.Empty;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value text displayed on the card
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The value text displayed on the card")]
        [DefaultValue("")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CardValue
        {
            get { return lblValue?.Text ?? string.Empty; }
            set
            {
                if (lblValue != null && lblValue.Text != value)
                {
                    lblValue.Text = value ?? string.Empty;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon image displayed on the card
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The icon image displayed on the card")]
        [DefaultValue(null)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image CardIcon
        {
            get { return picCardIcon?.Image; }
            set
            {
                if (picCardIcon != null && picCardIcon.Image != value)
                {
                    // Dispose of previous image if it exists and is different
                    if (picCardIcon.Image != null && picCardIcon.Image != value)
                    {
                        picCardIcon.Image.Dispose();
                    }

                    picCardIcon.Image = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of the card title text
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The font of the card title text")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font CardTitleFont
        {
            get { return lblTitle?.Font ?? this.Font; }
            set
            {
                if (lblTitle != null && lblTitle.Font != value)
                {
                    lblTitle.Font = value ?? this.Font;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of the card value text
        /// </summary>
        [Category("Advanced Appearance")]
        [Description("The font of the card value text")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font CardValueFont
        {
            get { return lblValue?.Font ?? this.Font; }
            set
            {
                if (lblValue != null && lblValue.Font != value)
                {
                    lblValue.Font = value ?? this.Font;
                    this.Invalidate();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AdvancedDashboardCard class
        /// </summary>
        public AdvancedDashboardCard()
        {
            InitializeComponent();
            InitializeDefaultProperties();
        }

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Initializes default properties for the dashboard card
        /// </summary>
        private void InitializeDefaultProperties()
        {
            try
            {
                // Set default values if components are available
                if (MainPanelElipse != null)
                {
                    MainPanelElipse.ElipseRadius = 10;
                }

                if (MainPanel != null)
                {
                    MainPanel.GradientStartColor = Color.White;
                    MainPanel.GradientEndColor = Color.LightGray;
                }

                if (lblTitle != null)
                {
                    lblTitle.ForeColor = Color.Black;
                    lblTitle.Text = "Title";
                }

                if (lblValue != null)
                {
                    lblValue.ForeColor = Color.DarkBlue;
                    lblValue.Text = "0";
                }

                if (picCardIcon != null)
                {
                    picCardIcon.Visible = true;
                }

                // Ensure proper layout
                UpdateIconColumnLayout(true);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing dashboard card properties: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the table layout panel column width based on icon visibility
        /// </summary>
        /// <param name="showIcon">Whether the icon should be visible</param>
        private void UpdateIconColumnLayout(bool showIcon)
        {
            try
            {
                if (tableLayoutPanel1?.ColumnStyles != null &&
                    tableLayoutPanel1.ColumnStyles.Count > ICON_COLUMN_INDEX)
                {
                    float columnWidth = showIcon ? ICON_VISIBLE_COLUMN_WIDTH : ICON_HIDDEN_COLUMN_WIDTH;
                    tableLayoutPanel1.ColumnStyles[ICON_COLUMN_INDEX].Width = columnWidth;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating icon column layout: {ex.Message}");
            }
        }

        /// <summary>
        /// Determines whether the CardRadius property should be serialized
        /// </summary>
        private bool ShouldSerializeCardRadius()
        {
            return CardRadius != 10;
        }

        /// <summary>
        /// Resets the CardRadius property to its default value
        /// </summary>
        private void ResetCardRadius()
        {
            CardRadius = 10;
        }

        /// <summary>
        /// Determines whether the CardFillColor1 property should be serialized
        /// </summary>
        private bool ShouldSerializeCardFillColor1()
        {
            return CardFillColor1 != Color.White;
        }

        /// <summary>
        /// Resets the CardFillColor1 property to its default value
        /// </summary>
        private void ResetCardFillColor1()
        {
            CardFillColor1 = Color.White;
        }

        /// <summary>
        /// Determines whether the CardFillColor2 property should be serialized
        /// </summary>
        private bool ShouldSerializeCardFillColor2()
        {
            return CardFillColor2 != Color.LightGray;
        }

        /// <summary>
        /// Resets the CardFillColor2 property to its default value
        /// </summary>
        private void ResetCardFillColor2()
        {
            CardFillColor2 = Color.LightGray;
        }

        /// <summary>
        /// Determines whether the CardTitleTextColor property should be serialized
        /// </summary>
        private bool ShouldSerializeCardTitleTextColor()
        {
            return CardTitleTextColor != Color.Black;
        }

        /// <summary>
        /// Resets the CardTitleTextColor property to its default value
        /// </summary>
        private void ResetCardTitleTextColor()
        {
            CardTitleTextColor = Color.Black;
        }

        /// <summary>
        /// Determines whether the CardTitleValueColor property should be serialized
        /// </summary>
        private bool ShouldSerializeCardTitleValueColor()
        {
            return CardTitleValueColor != Color.DarkBlue;
        }

        /// <summary>
        /// Resets the CardTitleValueColor property to its default value
        /// </summary>
        private void ResetCardTitleValueColor()
        {
            CardTitleValueColor = Color.DarkBlue;
        }

        /// <summary>
        /// Determines whether the CardShowIcon property should be serialized
        /// </summary>
        private bool ShouldSerializeCardShowIcon()
        {
            return CardShowIcon != true;
        }

        /// <summary>
        /// Resets the CardShowIcon property to its default value
        /// </summary>
        private void ResetCardShowIcon()
        {
            CardShowIcon = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the card with new title and value
        /// </summary>
        /// <param name="title">New title text</param>
        /// <param name="value">New value text</param>
        public void UpdateCard(string title, string value)
        {
            CardTitleText = title;
            CardValue = value;
        }

        /// <summary>
        /// Updates the card with new title, value, and icon
        /// </summary>
        /// <param name="title">New title text</param>
        /// <param name="value">New value text</param>
        /// <param name="icon">New icon image</param>
        public void UpdateCard(string title, string value, Image icon)
        {
            CardTitleText = title;
            CardValue = value;
            CardIcon = icon;
        }

        /// <summary>
        /// Sets the gradient colors for the card background
        /// </summary>
        /// <param name="startColor">Gradient start color</param>
        /// <param name="endColor">Gradient end color</param>
        public void SetGradientColors(Color startColor, Color endColor)
        {
            CardFillColor1 = startColor;
            CardFillColor2 = endColor;
        }

        /// <summary>
        /// Sets the text colors for title and value
        /// </summary>
        /// <param name="titleColor">Title text color</param>
        /// <param name="valueColor">Value text color</param>
        public void SetTextColors(Color titleColor, Color valueColor)
        {
            CardTitleTextColor = titleColor;
            CardTitleValueColor = valueColor;
        }

        /// <summary>
        /// Resets all card properties to their default values
        /// </summary>
        public void ResetCardToDefaults()
        {
            ResetCardRadius();
            ResetCardFillColor1();
            ResetCardFillColor2();
            ResetCardTitleTextColor();
            ResetCardTitleValueColor();
            ResetCardShowIcon();
            CardTitleText = "Title";
            CardValue = "0";
            CardIcon = null;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the disposal of the control and cleanup of resources
        /// </summary>
        /// <param name="disposing">True if disposing managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    // Dispose of the card icon image if it exists
                    if (picCardIcon?.Image != null)
                    {
                        picCardIcon.Image.Dispose();
                        picCardIcon.Image = null;
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing dashboard card resources: {ex.Message}");
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles control invalidation for property changes
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);

            // Ensure proper refresh of child controls
            try
            {
                if (MainPanel != null)
                {
                    MainPanel.Invalidate();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error invalidating dashboard card: {ex.Message}");
            }
        }

        #endregion
    }
}