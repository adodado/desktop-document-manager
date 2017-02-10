using System.Diagnostics;
using System.Windows.Input;
using DocumentManager.NET.Core.Documents;
using DocumentManager.NET.Core.Tags;
using DocumentManager.NET.ViewModels;
using DocumentManager.NET.ViewModels.MainWindow;

namespace DocumentManager.NET.Views.Main
{
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        //TODO: For the moment leave these 2 methods in the codebehind, i'll transfer them to viewmodel later when i have finished the viewmodel

        private void DocumentListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DocumentListBox.SelectedItem == null) return;

            var path = ((Document) DocumentListBox.SelectedItem).Path;
            Process.Start(path);
        }

        private void CollectionsListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CollectionsListBox.SelectedItem == null) return;
            var collection = ((DocumentCollection)CollectionsListBox.SelectedItem);
            CollectionFiles.ItemsSource = collection.Documents;
        }

        private void TagCloudControl_OnTagClicked(object sender, TagClickedEventArgs e)
        {
           var viewModel = (MainViewModel)DataContext;
            viewModel.SearchString =e.Item.Name;
            if (viewModel.TagClickedRelayCommand.CanExecute(null))
                viewModel.TagClickedRelayCommand.Execute(null);
        }
    }
}