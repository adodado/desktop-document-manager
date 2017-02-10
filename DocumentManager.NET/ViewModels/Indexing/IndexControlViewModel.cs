using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using DocumentManager.NET.Core.Documents;
using DocumentManager.NET.Logging;
using DocumentManager.NET.Utils.Managers.Indexing;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DocumentManager.NET.ViewModels.Indexing
{
    public class IndexControlViewModel : ViewModelBase
    {
        #region Fields

        private string _directoryToindex;
        private ObservableCollection<Document> _documentCollection;
        private List<Document> _documentList;
        private bool _officeDocumentsSearch = true;
        private bool _pdfDocumentsSearch;
        private bool _selectAllDocuments;
        private bool _textfileSearch;
        private readonly IIndexingManager _indexingManager;
        private readonly ILoger _logger;
        private bool _recursiveSearch;

        #endregion

        #region Public properties

        public string DirectoryToIndex
        {
            get { return _directoryToindex; }
            set
            {
                _directoryToindex = value;
                RaisePropertyChanged(() => DirectoryToIndex);
            }
        }

        public ObservableCollection<Document> DocumentCollection
        {
            get { return _documentCollection; }
            set
            {
                if (value == _documentCollection) return;
                _documentCollection = value;
                RaisePropertyChanged(() => DocumentCollection);
            }
        }

        public List<Document> DocumentList
        {
            set { _documentList = value; }
            get { return _documentList; }
        }

        public RelayCommand ImportDocumentsRelayCommand { get; private set; }
        public RelayCommand SearchRelayCommand { get; private set; }

        public bool OfficeDocumentsSearch
        {
            get { return _officeDocumentsSearch; }
            set
            {
                _officeDocumentsSearch = value;
                RaisePropertyChanged(() => OfficeDocumentsSearch);
                
                if (!value) return;
                
                PdfDocumentsSearch = false;
                TextfileSearch = false;
            }
        }

        public bool PdfDocumentsSearch
        {
            get { return _pdfDocumentsSearch; }
            set
            {
                _pdfDocumentsSearch = value;
                RaisePropertyChanged(() => PdfDocumentsSearch);
                
                if (!value) return;
                
                OfficeDocumentsSearch = false;
                TextfileSearch = false;
            }
        }

        public bool RecursiveSearch
        {
            get { return _recursiveSearch; }
            set
            {
                _recursiveSearch = value;
                RaisePropertyChanged(() => RecursiveSearch);
            }
        }

        public bool SelectAllDocuments
        {
            get { return _selectAllDocuments; }
            set
            {
                foreach (var doc in DocumentCollection)
                {
                    doc.IsSelected = value;
                }
                
                _selectAllDocuments = value;
                RaisePropertyChanged(() => SelectAllDocuments);
            }
        }

        public RelayCommand SelectDirectoryRelayCommand { get; private set; }

        public bool TextfileSearch
        {
            get { return _textfileSearch; }
            set
            {
                _textfileSearch = value;
                RaisePropertyChanged(() => TextfileSearch);
            
                if (!value) return;
                
                OfficeDocumentsSearch = false;
                PdfDocumentsSearch = false;
            }
        }

        #endregion

        #region Constructor

        public IndexControlViewModel(ILoger logger)
        {
            SelectDirectoryRelayCommand = new RelayCommand(SelectDirectoryCommand);
            ImportDocumentsRelayCommand = new RelayCommand(ImportCommand);
            SearchRelayCommand = new RelayCommand(StartIndexing);
            _logger = logger;
            _indexingManager = new IndexingManager(_logger);
        }

        #endregion

        #region Methods

        private void ImportCommand()
        {
            _indexingManager.CreateLuceneIndex(DocumentCollection);
        }

        private void SelectDirectoryCommand()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            
            if (result != DialogResult.OK) return;
            
            DirectoryToIndex = dialog.SelectedPath;
            
            if (DocumentCollection != null) 
                DocumentCollection.Clear();
            _logger.Information("Directory selected!");
        }

        public void StartIndexing()
        {
            if (string.IsNullOrEmpty(DirectoryToIndex))
                return;

            _documentList = new List<Document>();

            if (RecursiveSearch)
            {
                _documentList.AddRange(_indexingManager.SearchDirectoriesRecursive(DirectoryToIndex,
                    OfficeDocumentsSearch,
                    PdfDocumentsSearch, TextfileSearch));
            }
            else
                _documentList.AddRange(_indexingManager.SearchRootDirectory(DirectoryToIndex, OfficeDocumentsSearch,
                    PdfDocumentsSearch, TextfileSearch));

            _indexingManager.CleanUp();
            DocumentCollection = new ObservableCollection<Document>(_documentList.Distinct());
        }

        #endregion
    }
}
