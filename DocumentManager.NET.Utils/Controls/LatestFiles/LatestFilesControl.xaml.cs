using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DocumentManager.NET.Core.Documents;

namespace DocumentManager.NET.Utils.Controls.LatestFiles
{
    /// <summary>
    /// Interaction logic for LatestFilesControl.xaml
    /// </summary>
    public partial class LatestFilesControl : UserControl
    {
        //TODO: Transfer this code to viewmodel
        #region Events

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields

        /// <summary>
        /// The list source property
        /// </summary>
        public static DependencyProperty ListSourceProperty = DependencyProperty.Register(
                                     "ListSource",
                                     typeof(IEnumerable<Document>),
                                     typeof(LatestFilesControl),
                                     new PropertyMetadata(OnValueChanged));

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the list source.
        /// </summary>
        /// <value>The list source.</value>
        public ObservableCollection<Document> ListSource
        {
            get
            {
                return (ObservableCollection<Document>)GetValue(ListSourceProperty);
            }
            set
            {
                SetValue(ListSourceProperty, value);
                NotifyPropertyChanged("ListSource");
                RefreshFilesList();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LatestFilesControl" /> class.
        /// </summary>
        public LatestFilesControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var args = ListSource.FirstOrDefault(a => a.Name == ((Button)sender).Content);

            if (args != null) Process.Start(args.Path);
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
        /// Handles the <see cref="E:ValueChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LatestFilesControl)d).ListSource = (ObservableCollection<Document>)e.NewValue;
        }

        /// <summary>
        /// Refreshes the files list.
        /// </summary>
        private void RefreshFilesList()
        {
            LatestFilesListBox.ItemsSource = ListSource;
        }

        #endregion
    }
}
