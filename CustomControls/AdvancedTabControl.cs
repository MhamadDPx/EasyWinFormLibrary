using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace EasyWinFormLibrary.CustomControls
{
    /// <summary>
    /// Advanced TabControl with ability to hide/show tab headers and custom background color.
    /// This control provides full control over tab appearance and behavior, built from scratch
    /// without inheriting from the standard TabControl to avoid Windows theming issues.
    /// </summary>
    /// <remarks>
    /// Key features:
    /// - Show/hide tab headers completely
    /// - Custom background colors
    /// - No borders for clean appearance
    /// - Full Visual Studio designer support
    /// - Click tab headers to navigate or use SelectedIndex programmatically
    /// </remarks>
    /// <example>
    /// Basic usage:
    /// <code>
    /// var tabControl = new AdvancedTabControl();
    /// tabControl.SetBackgroundColor(Color.White);
    /// tabControl.ShowTabHeaders = false; // Hide headers for wizard-like interface
    /// tabControl.SelectedIndex = 1; // Navigate programmatically
    /// </code>
    /// </example>
    [Designer(typeof(AdvancedTabControlDesigner))]
    [DefaultProperty("TabPages")]
    [DefaultEvent("SelectedIndexChanged")]
    public class AdvancedTabControl : ContainerControl
    {
        #region Fields
        private AdvancedTabPageCollection _tabPages;
        private int _selectedIndex = -1;
        private bool _showTabHeaders = true;
        private Color _customBackColor = Color.White;
        private bool _useCustomBackColor = false;
        private int _tabHeight = 25;
        private Rectangle _tabHeadersRect;
        private Rectangle _contentRect;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedTabControl"/> class.
        /// Sets up default styling, creates the tab page collection, and configures
        /// the control for optimal performance with double buffering.
        /// </summary>
        public AdvancedTabControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

            _tabPages = new AdvancedTabPageCollection(this);
            this.BackColor = SystemColors.Control;
            this.Size = new Size(400, 300);
            UpdateRects();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of tab pages in this control.
        /// Use this collection to add, remove, or access individual tab pages.
        /// </summary>
        /// <value>
        /// An <see cref="AdvancedTabPageCollection"/> containing all tab pages.
        /// </value>
        /// <example>
        /// Adding a tab page:
        /// <code>
        /// tabControl.TabPages.Add(new AdvancedTabPage("My Tab"));
        /// </code>
        /// </example>
        [Category("Behavior")]
        [Description("The collection of tab pages")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(TabPageCollectionEditor), typeof(UITypeEditor))]
        public AdvancedTabPageCollection TabPages
        {
            get { return _tabPages; }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the currently selected tab page.
        /// Set to -1 if no tab is selected.
        /// </summary>
        /// <value>
        /// The index of the selected tab, or -1 if no tab is selected.
        /// </value>
        /// <example>
        /// Selecting the second tab:
        /// <code>
        /// tabControl.SelectedIndex = 1;
        /// </code>
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when trying to set an index that is less than -1 or greater than or equal to the number of tab pages.
        /// </exception>
        [Category("Behavior")]
        [Description("The index of the currently selected tab")]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value && value >= -1 && value < _tabPages.Count)
                {
                    _selectedIndex = value;
                    UpdateTabVisibility();
                    OnSelectedIndexChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the currently selected tab page, or null if no tab is selected.
        /// This is a read-only property; use <see cref="SelectedIndex"/> to change the selection.
        /// </summary>
        /// <value>
        /// The currently selected <see cref="AdvancedTabPage"/>, or null if no tab is selected.
        /// </value>
        [Browsable(false)]
        public AdvancedTabPage SelectedTab
        {
            get
            {
                if (_selectedIndex >= 0 && _selectedIndex < _tabPages.Count)
                    return _tabPages[_selectedIndex];
                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab headers are visible.
        /// When false, tab headers are completely hidden, useful for wizard-like interfaces
        /// where navigation is controlled programmatically.
        /// </summary>
        /// <value>
        /// true if tab headers are visible; otherwise, false. The default is true.
        /// </value>
        /// <example>
        /// Creating a wizard-like interface:
        /// <code>
        /// tabControl.ShowTabHeaders = false;
        /// // Navigate programmatically using SelectedIndex
        /// </code>
        /// </example>
        [Category("Appearance")]
        [Description("Determines whether tab headers are visible")]
        [DefaultValue(true)]
        public bool ShowTabHeaders
        {
            get { return _showTabHeaders; }
            set
            {
                if (_showTabHeaders != value)
                {
                    _showTabHeaders = value;
                    UpdateRects();
                    UpdateTabVisibility();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the custom background color for the control and its tab pages.
        /// This color is only applied when <see cref="UseCustomBackColor"/> is true.
        /// </summary>
        /// <value>
        /// The custom background color. The default is White.
        /// </value>
        /// <seealso cref="UseCustomBackColor"/>
        /// <seealso cref="SetBackgroundColor(Color)"/>
        [Category("Appearance")]
        [Description("The custom background color of the control")]
        [DefaultValue(typeof(Color), "White")]
        public Color CustomBackColor
        {
            get { return _customBackColor; }
            set
            {
                if (_customBackColor != value)
                {
                    _customBackColor = value;
                    if (_useCustomBackColor)
                    {
                        ApplyBackgroundColor();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the custom background color.
        /// When true, applies <see cref="CustomBackColor"/> to the control and all tab pages.
        /// When false, uses the system default colors.
        /// </summary>
        /// <value>
        /// true to use the custom background color; otherwise, false. The default is false.
        /// </value>
        /// <seealso cref="CustomBackColor"/>
        [Category("Appearance")]
        [Description("Determines whether to use custom background color")]
        [DefaultValue(false)]
        public bool UseCustomBackColor
        {
            get { return _useCustomBackColor; }
            set
            {
                if (_useCustomBackColor != value)
                {
                    _useCustomBackColor = value;
                    ApplyBackgroundColor();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the tab headers in pixels.
        /// This value is ignored when <see cref="ShowTabHeaders"/> is false.
        /// </summary>
        /// <value>
        /// The height of tab headers in pixels. Must be greater than 0. The default is 25.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when trying to set a value less than or equal to 0.
        /// </exception>
        [Category("Appearance")]
        [Description("The height of tab headers")]
        [DefaultValue(25)]
        public int TabHeight
        {
            get { return _tabHeight; }
            set
            {
                if (_tabHeight != value && value > 0)
                {
                    _tabHeight = value;
                    UpdateRects();
                    UpdateTabVisibility();
                    Invalidate();
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the <see cref="SelectedIndex"/> property changes.
        /// This event is raised whenever a different tab is selected, either by user interaction
        /// or programmatic changes to the SelectedIndex property.
        /// </summary>
        /// <example>
        /// Handling tab selection changes:
        /// <code>
        /// tabControl.SelectedIndexChanged += (sender, e) => {
        ///     Console.WriteLine($"Selected tab changed to index: {tabControl.SelectedIndex}");
        /// };
        /// </code>
        /// </example>
        [Category("Action")]
        [Description("Occurs when the selected tab index changes")]
        public event EventHandler SelectedIndexChanged;
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the background color and enables custom background color mode.
        /// This is a convenience method that sets both <see cref="CustomBackColor"/> 
        /// and <see cref="UseCustomBackColor"/> in a single call.
        /// </summary>
        /// <param name="color">The color to set as the background color.</param>
        /// <example>
        /// Setting a white background:
        /// <code>
        /// tabControl.SetBackgroundColor(Color.White);
        /// </code>
        /// </example>
        public void SetBackgroundColor(Color color)
        {
            CustomBackColor = color;
            UseCustomBackColor = true;
        }

        /// <summary>
        /// Resets the background color to the system default and disables custom background color mode.
        /// After calling this method, the control will use system colors.
        /// </summary>
        /// <example>
        /// Resetting to default colors:
        /// <code>
        /// tabControl.ResetBackgroundColor();
        /// </code>
        /// </example>
        public void ResetBackgroundColor()
        {
            UseCustomBackColor = false;
        }

        /// <summary>
        /// Toggles the visibility of tab headers.
        /// If headers are currently visible, they will be hidden, and vice versa.
        /// </summary>
        /// <example>
        /// Toggling header visibility:
        /// <code>
        /// tabControl.ToggleTabHeaders(); // Hide if shown, show if hidden
        /// </code>
        /// </example>
        public void ToggleTabHeaders()
        {
            ShowTabHeaders = !ShowTabHeaders;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Adds a tab page to the control. This method is called internally by the collection
        /// and handles the proper setup of the tab page including bounds, anchoring, and visibility.
        /// </summary>
        /// <param name="tabPage">The tab page to add.</param>
        internal void AddTabPage(AdvancedTabPage tabPage)
        {
            if (tabPage != null)
            {
                this.Controls.Add(tabPage);
                tabPage.Bounds = _contentRect;
                tabPage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

                if (_useCustomBackColor)
                {
                    tabPage.BackColor = _customBackColor;
                }

                if (_tabPages.Count == 1)
                {
                    SelectedIndex = 0;
                }
                else
                {
                    tabPage.Visible = false;
                }

                Invalidate();
            }
        }

        /// <summary>
        /// Removes a tab page from the control. This method is called internally by the collection
        /// and handles proper cleanup and index adjustment.
        /// </summary>
        /// <param name="tabPage">The tab page to remove.</param>
        internal void RemoveTabPage(AdvancedTabPage tabPage)
        {
            if (tabPage != null && this.Controls.Contains(tabPage))
            {
                int index = _tabPages.IndexOf(tabPage);
                this.Controls.Remove(tabPage);

                if (index == _selectedIndex && _tabPages.Count > 0)
                {
                    SelectedIndex = Math.Min(_selectedIndex, _tabPages.Count - 1);
                }
                else if (index < _selectedIndex)
                {
                    _selectedIndex--;
                }

                if (_tabPages.Count == 0)
                {
                    _selectedIndex = -1;
                }

                Invalidate();
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Raises the <see cref="SelectedIndexChanged"/> event.
        /// Override this method to add custom behavior when the selected tab changes.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        /// <inheritdoc/>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw background
            using (var brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            // Draw tab headers if visible
            if (_showTabHeaders && _tabPages.Count > 0)
            {
                DrawTabHeaders(e.Graphics);
            }
        }
        /// <inheritdoc/>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_showTabHeaders && e.Button == MouseButtons.Left && _tabPages.Count > 0)
            {
                int clickedTab = GetTabAtPoint(e.Location);
                if (clickedTab >= 0 && clickedTab < _tabPages.Count)
                {
                    SelectedIndex = clickedTab;
                }
            }
        }
        /// <inheritdoc/>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRects();
            UpdateTabVisibility();
        }
        #endregion

        #region Private Methods
        private void UpdateRects()
        {
            if (_showTabHeaders)
            {
                _tabHeadersRect = new Rectangle(0, 0, this.Width, _tabHeight);
                _contentRect = new Rectangle(0, _tabHeight, this.Width, this.Height - _tabHeight);
            }
            else
            {
                _tabHeadersRect = Rectangle.Empty;
                _contentRect = new Rectangle(0, 0, this.Width, this.Height);
            }
        }

        private void UpdateTabVisibility()
        {
            for (int i = 0; i < _tabPages.Count; i++)
            {
                var tabPage = _tabPages[i];
                tabPage.Visible = (i == _selectedIndex);
                tabPage.Bounds = _contentRect;
            }
        }

        private void ApplyBackgroundColor()
        {
            Color backColor = _useCustomBackColor ? _customBackColor : SystemColors.Control;
            this.BackColor = backColor;

            foreach (AdvancedTabPage tabPage in _tabPages)
            {
                tabPage.BackColor = backColor;
            }

            Invalidate();
        }

        private void DrawTabHeaders(Graphics g)
        {
            if (_tabPages.Count == 0) return;

            int tabWidth = this.Width / _tabPages.Count;
            int x = 0;

            for (int i = 0; i < _tabPages.Count; i++)
            {
                var tabRect = new Rectangle(x, 0, tabWidth, _tabHeight);
                bool isSelected = (i == _selectedIndex);

                // Draw tab background
                Color tabBackColor = isSelected ?
                    (_useCustomBackColor ? _customBackColor : Color.White) :
                    SystemColors.Control;

                using (var brush = new SolidBrush(tabBackColor))
                {
                    g.FillRectangle(brush, tabRect);
                }

                // Draw tab text
                using (var brush = new SolidBrush(SystemColors.ControlText))
                {
                    var textRect = new Rectangle(tabRect.X + 5, tabRect.Y, tabRect.Width - 10, tabRect.Height);
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    g.DrawString(_tabPages[i].Text, this.Font, brush, textRect, format);
                }

                x += tabWidth;
            }
        }

        private int GetTabAtPoint(Point point)
        {
            if (!_showTabHeaders || !_tabHeadersRect.Contains(point) || _tabPages.Count == 0)
                return -1;

            int tabWidth = this.Width / _tabPages.Count;
            int tabIndex = point.X / tabWidth;

            // Make sure we don't return an index that's out of bounds
            if (tabIndex >= 0 && tabIndex < _tabPages.Count)
                return tabIndex;

            return -1;
        }
        #endregion
    }

    /// <summary>
    /// Represents a single tab page within an <see cref="AdvancedTabControl"/>.
    /// This control inherits from Panel and can contain any child controls.
    /// </summary>
    /// <remarks>
    /// AdvancedTabPage is designed to be used exclusively within AdvancedTabControl.
    /// It provides a Text property that appears on the tab header and supports
    /// transparent backgrounds for better visual integration.
    /// </remarks>
    /// <example>
    /// Creating and configuring a tab page:
    /// <code>
    /// var tabPage = new AdvancedTabPage("Settings");
    /// tabPage.Controls.Add(new Label { Text = "Configuration options", Dock = DockStyle.Top });
    /// tabControl.TabPages.Add(tabPage);
    /// </code>
    /// </example>
    [Designer(typeof(AdvancedTabPageDesigner))]
    [ToolboxItem(false)]
    public class AdvancedTabPage : Panel
    {
        private string _text = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedTabPage"/> class with default text.
        /// </summary>
        public AdvancedTabPage()
        {
            this._text = "TabPage";
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedTabPage"/> class with the specified text.
        /// </summary>
        /// <param name="text">The text to display on the tab header.</param>
        public AdvancedTabPage(string text)
        {
            this._text = text ?? "TabPage";
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        /// Gets or sets the text displayed on the tab header.
        /// Changing this property will cause the parent tab control to repaint
        /// to reflect the new text on the tab header.
        /// </summary>
        /// <value>
        /// The text displayed on the tab header. If set to null, it will be converted to an empty string.
        /// </value>
        /// <example>
        /// Setting tab text:
        /// <code>
        /// tabPage.Text = "User Settings";
        /// </code>
        /// </example>
        [Category("Appearance")]
        [Description("The text displayed on the tab")]
        public override string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value ?? "";
                    // Notify parent control to repaint
                    if (this.Parent is AdvancedTabControl)
                    {
                        this.Parent.Invalidate();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Provides design-time behavior for <see cref="AdvancedTabPage"/> controls.
    /// This designer ensures that tab pages can contain child controls but cannot
    /// contain other tab pages.
    /// </summary>
    public class AdvancedTabPageDesigner : ParentControlDesigner
    {
        /// <summary>
        /// Initializes the designer with the specified component.
        /// </summary>
        /// <param name="component">The <see cref="AdvancedTabPage"/> component to design.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }

        /// <summary>
        /// Determines whether the specified control can be parented by this designer's control.
        /// Prevents tab pages from being added to other tab pages.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <returns>
        /// true if the control can be parented by this designer's control; otherwise, false.
        /// Returns false if the control is an AdvancedTabPage.
        /// </returns>
        public override bool CanParent(Control control)
        {
            return !(control is AdvancedTabPage);
        }
    }

    /// <summary>
    /// Collection class for managing tab pages
    /// </summary>
    [Editor(typeof(TabPageCollectionEditor), typeof(UITypeEditor))]
    public class AdvancedTabPageCollection : IList, ICollection, IEnumerable
    {
        private readonly AdvancedTabControl _owner;
        private readonly ArrayList _list;

        internal AdvancedTabPageCollection(AdvancedTabControl owner)
        {
            _owner = owner;
            _list = new ArrayList();
        }
        /// <inheritdoc/>
        public AdvancedTabPage this[int index]
        {
            get { return (AdvancedTabPage)_list[index]; }
            set
            {
                _list[index] = value;
                _owner.Invalidate();
            }
        }
        /// <inheritdoc/>
        public int Count => _list.Count;
        /// <inheritdoc/>
        public bool IsReadOnly => false;
        /// <inheritdoc/>
        public bool IsSynchronized => false;
        /// <inheritdoc/>
        public object SyncRoot => this;
        /// <inheritdoc/>
        public int Add(AdvancedTabPage value)
        {
            int result = _list.Add(value);
            _owner.AddTabPage(value);
            return result;
        }
        /// <inheritdoc/>
        public void Clear()
        {
            while (_list.Count > 0)
            {
                RemoveAt(0);
            }
        }
        /// <inheritdoc/>
        public bool Contains(AdvancedTabPage value)
        {
            return _list.Contains(value);
        }
        /// <inheritdoc/>
        public int IndexOf(AdvancedTabPage value)
        {
            return _list.IndexOf(value);
        }
        /// <inheritdoc/>
        public void Insert(int index, AdvancedTabPage value)
        {
            _list.Insert(index, value);
            _owner.AddTabPage(value);
        }
        /// <inheritdoc/>
        public void Remove(AdvancedTabPage value)
        {
            _list.Remove(value);
            _owner.RemoveTabPage(value);
        }
        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            AdvancedTabPage tabPage = this[index];
            _list.RemoveAt(index);
            _owner.RemoveTabPage(tabPage);
        }
        /// <inheritdoc/>
        public void CopyTo(Array array, int index)
        {
            _list.CopyTo(array, index);
        }
        /// <inheritdoc/>
        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        // IList implementation
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (AdvancedTabPage)value; }
        }

        int IList.Add(object value)
        {
            return Add((AdvancedTabPage)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((AdvancedTabPage)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((AdvancedTabPage)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (AdvancedTabPage)value);
        }

        void IList.Remove(object value)
        {
            Remove((AdvancedTabPage)value);
        }

        bool IList.IsFixedSize => false;
    }

    /// <summary>
    /// Designer for AdvancedTabControl
    /// </summary>
    public class AdvancedTabControlDesigner : ParentControlDesigner
    {
        private DesignerVerbCollection _verbs;
        /// <inheritdoc/>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection();
                    _verbs.Add(new DesignerVerb("Add Tab", OnAddTab));
                    _verbs.Add(new DesignerVerb("Remove Tab", OnRemoveTab));
                }
                return _verbs;
            }
        }
        /// <inheritdoc/>
        public override bool CanParent(Control control)
        {
            return control is AdvancedTabPage;
        }

        private void OnAddTab(object sender, EventArgs e)
        {
            var tabControl = (AdvancedTabControl)Control;
            var host = (IDesignerHost)GetService(typeof(IDesignerHost));

            if (host != null)
            {
                using (var transaction = host.CreateTransaction("Add Tab"))
                {
                    try
                    {
                        var tabPage = (AdvancedTabPage)host.CreateComponent(typeof(AdvancedTabPage));
                        tabPage.Text = $"tabPage{tabControl.TabPages.Count + 1}";

                        // Raise component changing event
                        var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                        changeService?.OnComponentChanging(tabControl, TypeDescriptor.GetProperties(tabControl)["TabPages"]);

                        tabControl.TabPages.Add(tabPage);

                        // Raise component changed event
                        changeService?.OnComponentChanged(tabControl, TypeDescriptor.GetProperties(tabControl)["TabPages"], null, null);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Cancel();
                        throw;
                    }
                }
            }
        }

        private void OnRemoveTab(object sender, EventArgs e)
        {
            var tabControl = (AdvancedTabControl)Control;
            if (tabControl.TabPages.Count > 0)
            {
                var host = (IDesignerHost)GetService(typeof(IDesignerHost));

                if (host != null)
                {
                    using (var transaction = host.CreateTransaction("Remove Tab"))
                    {
                        try
                        {
                            var tabPage = tabControl.TabPages[tabControl.TabPages.Count - 1];

                            // Raise component changing event
                            var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                            changeService?.OnComponentChanging(tabControl, TypeDescriptor.GetProperties(tabControl)["TabPages"]);

                            tabControl.TabPages.Remove(tabPage);
                            host.DestroyComponent(tabPage);

                            // Raise component changed event
                            changeService?.OnComponentChanged(tabControl, TypeDescriptor.GetProperties(tabControl)["TabPages"], null, null);

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Cancel();
                            throw;
                        }
                    }
                }
            }
        }
        /// <inheritdoc/>
        protected override bool GetHitTest(Point point)
        {
            var tabControl = (AdvancedTabControl)Control;
            point = tabControl.PointToClient(point);

            // Allow tab header clicks in designer
            if (tabControl.ShowTabHeaders && point.Y <= tabControl.TabHeight)
            {
                return true;
            }

            return base.GetHitTest(point);
        }
        /// <inheritdoc/>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            // Enable design-time selection of tabs
            var tabControl = (AdvancedTabControl)component;
            EnableDesignMode(tabControl, "TabPages");
        }
    }

    /// <summary>
    /// Collection editor for tab pages
    /// </summary>
    public class TabPageCollectionEditor : CollectionEditor
    {
        /// <inheritdoc/>
        public TabPageCollectionEditor(Type type) : base(type) { }
        /// <inheritdoc/>
        protected override Type CreateCollectionItemType()
        {
            return typeof(AdvancedTabPage);
        }
        /// <inheritdoc/>
        protected override object CreateInstance(Type itemType)
        {
            var tabPage = (AdvancedTabPage)base.CreateInstance(itemType);

            // Try to get current collection count in a safe way
            try
            {
                if (this.Context?.Instance != null)
                {
                    var currentItems = this.GetItems(this.Context.Instance);
                    tabPage.Text = $"tabPage{currentItems.Length + 1}";
                }
                else
                {
                    tabPage.Text = "tabPage1";
                }
            }
            catch
            {
                // Fallback naming
                tabPage.Text = $"tabPage{DateTime.Now.Millisecond}";
            }

            return tabPage;
        }

        /// <inheritdoc/>
        protected override string GetDisplayText(object value)
        {
            if (value is AdvancedTabPage tabPage && !string.IsNullOrEmpty(tabPage.Text))
            {
                return tabPage.Text;
            }
            return base.GetDisplayText(value);
        }
    }
}