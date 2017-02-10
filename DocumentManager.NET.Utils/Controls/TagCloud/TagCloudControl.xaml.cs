using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DocumentManager.NET.Core.Tags;

namespace DocumentManager.NET.Utils.Controls.TagCloud
{
    public partial class TagCloudControl
    {
        #region Events
        //TODO: Transfer this code to viewmodel

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when [tag clicked].
        /// </summary>
        public event TagClickedHandler TagClicked;

        #endregion

        #region Delegates of TagCloudControl

        /// <summary>
        /// Delegate TagClickedHandler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TagClickedEventArgs" /> instance containing the event data.</param>
        public delegate void TagClickedHandler(object sender, TagClickedEventArgs e);

        #endregion

        #region Fields

        /// <summary>
        /// The list source property
        /// </summary>
        public static DependencyProperty ListSourceProperty = DependencyProperty.Register(
                     "ListSource",
                     typeof(IEnumerable<TagItem>),
                     typeof(TagCloudControl),
                     new PropertyMetadata(OnValueChanged));
        /// <summary>
        /// The _values
        /// </summary>
        private List<TagItem> _values = new List<TagItem>();

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the list source.
        /// </summary>
        /// <value>The list source.</value>
        public ObservableCollection<TagItem> ListSource
        {
            get
            {
                return (ObservableCollection<TagItem>)GetValue(ListSourceProperty);
            }
            set
            {
                SetValue(ListSourceProperty, value);
                NotifyPropertyChanged("ListSource");
                RefreshItems();
            }
        }
        /// <summary>
        /// Gets or sets the maximum size of the font.
        /// </summary>
        /// <value>The maximum size of the font.</value>
        public int MaxFontSize { get; set; }
        /// <summary>
        /// Gets or sets the minimum size of the font.
        /// </summary>
        /// <value>The minimum size of the font.</value>
        public int MinFontSize { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TagCloudControl" /> class.
        /// </summary>
        public TagCloudControl()
        {
            InitializeComponent();
            MinFontSize = 8;
            MaxFontSize = 20;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the TagItem.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(TagItem item)
        {
            _values.Add(item);
            AddLabel(item);
            Calculate();
            RefreshTags();
        }

        /// <summary>
        /// Adds the TagItem label.
        /// </summary>
        /// <param name="item">The item.</param>
        private void AddLabel(TagItem item)
        {
            var tagLabel = new Label { Content = item.Name, FontSize = MinFontSize + item.Index, Tag = item };
            tagLabel.MouseLeftButtonUp += TagItemClick;
            tagLabel.Cursor = Cursors.Hand;
            tagLabel.Margin = new Thickness(5);
            tagLabel.Padding = new Thickness(5);
            tagLabel.MouseEnter += TagItemMouseEnter;
            tagLabel.MouseLeave += TagItemMouseLeave;
            TagWrapPanel.Children.Add(tagLabel);
        }

        /// <summary>
        /// Calculates this instance.
        /// </summary>
        public void Calculate()
        {
            var minimum = _values.Min(p => p.Value);
            var maximum = _values.Max(p => p.Value);
            var factor = (maximum - minimum) / (decimal)(MaxFontSize - MinFontSize);

            foreach (var item in _values.Where(item => factor != 0))
            {
                item.Index = (int)Math.Floor((item.Value - minimum) / factor);
            }
        }

        /// <summary>
        /// Clears the wrap panel.
        /// </summary>
        private void ClearWrapPanel()
        {
            TagWrapPanel.Children.Clear();
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Handles the <see cref="E:TagClicked" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TagClickedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnTagClicked(TagClickedEventArgs e)
        {
            if (TagClicked != null)
                TagClicked(this, e);
        }

        /// <summary>
        /// Handles the <see cref="E:ValueChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TagCloudControl)d).ListSource = (ObservableCollection<TagItem>)e.NewValue;
        }

        /// <summary>
        /// Refreshes the items.
        /// </summary>
        private void RefreshItems()
        {
            ClearWrapPanel();

            foreach (var item in ListSource)
            {
                AddItem(item);
            }
        }

        /// <summary>
        /// Refreshes the TagCloud.
        /// </summary>
        public void RefreshTags()
        {
            foreach (Label item in TagWrapPanel.Children)
            {
                var nvi = item.Tag as TagItem;
                if (nvi != null) item.FontSize = MinFontSize + nvi.Index;
            }
        }

        /// <summary>
        /// Tagitem click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TagItemClick(object sender, EventArgs e)
        {
            var item = ((Control)sender).Tag as TagItem;
            OnTagClicked(new TagClickedEventArgs(item));
        }

        /// <summary>
        /// Tagitem mouse enter event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void TagItemMouseEnter(object sender, EventArgs e)
        {
            var lbl = sender as Label;
            if (lbl == null) return;
            lbl.FontStyle = FontStyles.Italic;
            lbl.Foreground = Brushes.CornflowerBlue;
        }

        /// <summary>
        /// Tagitem mouse leave event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void TagItemMouseLeave(object sender, EventArgs e)
        {
            var lbl = sender as Label;
            if (lbl == null) return;
            lbl.FontStyle = FontStyles.Normal;
            lbl.Foreground = Brushes.White;
        }

        #endregion
    }
}
