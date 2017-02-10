using DocumentManager.NET.Core.Documents;
using DocumentManager.NET.Core.Tags;
using DocumentManager.NET.Logging;
using DocumentManager.NET.LuceneAdapter;
using DocumentManager.NET.Managers.DialogManager;
using DocumentManager.NET.Utils.Managers.Indexing;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DocumentManager.NET.ViewModels.MainWindow
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDialogManager _dialogService;
        private readonly ILoger _logger;
        private readonly IIndexingManager _indexingManager;
        private readonly ILuceneAdapter<Document, SearchDocuments> _luceneAdapter; 
        private RelayCommand _backupCollectionsCommand;
        private RelayCommand _backupDocumentsCommand;
        private RelayCommand _backupAllIndexedDocumentsCommand;
        private RelayCommand _editSettingsCommand;
        private RelayCommand _searchLuceneIndexCommand;
        private ObservableCollection<LogItem> _logCollection;
        private ObservableCollection<DocumentCollection> _observableCollections = new ObservableCollection<DocumentCollection>();
        private ObservableCollection<Document> _searchObservableCollection = new ObservableCollection<Document>();
        private List<Document> _documentCollection = new List<Document>();
        private ObservableCollection<Document> _lastFiveIndexeDocumentsCollection;
        private string _searchString = string.Empty;
        private RelayCommand _createNewCollectionCommand;
        private ObservableCollection<TagItem> _tagItemsCollection;
        private RelayCommand _tagClickedRelayCommand;
        private RelayCommand _rebuildIndexRelayCommand;
        private bool _viewmodelIsBusy;
        #endregion

        #region Public properties

        public RelayCommand BackupAllIndexedDocumentsCommand
        {
            get
            {
                return _backupAllIndexedDocumentsCommand
                ?? (_backupAllIndexedDocumentsCommand = new RelayCommand(BackupAllDocuments));
            }
        }
        public RelayCommand BackupCollectionsCommand
        {
            get
            {
                return _backupCollectionsCommand
                ?? (_backupCollectionsCommand = new RelayCommand(BackupCollections));
            }
        }
        public RelayCommand RebuildIndexRelayCommand
        {
            get
            {
                return _rebuildIndexRelayCommand
                ?? (_rebuildIndexRelayCommand = new RelayCommand(RebuildIndex));
            }
        }

        private void RebuildIndex()
        {
            var pathList = _documentCollection.Select(x => x.Path).Distinct(); 

        }
        public RelayCommand BackupDocumentsCommand
        {
            get
            {
                return _backupDocumentsCommand
                ?? (_backupDocumentsCommand = new RelayCommand(BackupSingleDocuments));
            }
        }
        public RelayCommand CreateNewCollectionCommand
        {
            get
            {
                return _createNewCollectionCommand
                ?? (_createNewCollectionCommand = new RelayCommand(CreateNewCollection));
            }
        }
        public RelayCommand EditSettingsCommand
        {
            get
            {
                return _editSettingsCommand
                ?? (_editSettingsCommand = new RelayCommand(EditSettings));
            }
        }
        public ObservableCollection<Document> LastFiveIndexeDocumentsCollection
        {
            get { return _lastFiveIndexeDocumentsCollection; }
            set
            {
                _lastFiveIndexeDocumentsCollection = value;
                RaisePropertyChanged(() => LastFiveIndexeDocumentsCollection);
            }
        }
        public ObservableCollection<LogItem> LogCollection
        {
            get { return _logCollection; }
            set
            {
                _logCollection = value;
                RaisePropertyChanged(() => LogCollection);
            }
        }
        public ObservableCollection<DocumentCollection> ObservableCollections
        {
            get { return _observableCollections; }
            set
            {
                _observableCollections = value;
                RaisePropertyChanged(() => ObservableCollections);
            }
        }
        public RelayCommand SearchLuceneIndexCommand
        {
            get
            {
                return _searchLuceneIndexCommand
                ?? (_searchLuceneIndexCommand = new RelayCommand(Search));
            }
        }
        public ObservableCollection<Document> SearchObservableCollection
        {
            get { return _searchObservableCollection; }
            set
            {
                _searchObservableCollection = value;
                RaisePropertyChanged(() => SearchObservableCollection);
            }
        }
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                RaisePropertyChanged(() => SearchString);
            }
        }
        public RelayCommand TagClickedRelayCommand
        {
            get
            {
                return _tagClickedRelayCommand
                ?? (_tagClickedRelayCommand = new RelayCommand(TagClicked));
            }
        }
        public ObservableCollection<TagItem> TagCollection
        {
            get { return _tagItemsCollection; }
            set
            {
                _tagItemsCollection = value;
                RaisePropertyChanged(() => TagCollection);
            }
        }

        public bool IsBusy
        {
            get { return _viewmodelIsBusy; }
            set
            {
                _viewmodelIsBusy = value;
                RaisePropertyChanged(()=>IsBusy);
            }
        }
        #endregion

        #region Constructor

        public MainViewModel(IDialogManager dialogService, ILoger logger)
        {
            _logger = logger;
            _dialogService = dialogService;
            _indexingManager = new IndexingManager(_logger);
            _logCollection = new ObservableCollection<LogItem>();
            _logger.OnLogItemAdded += OnLogItemAdded;

            if (Directory.Exists(Environment.CurrentDirectory + "\\LuceneIndex\\Documents\\"))
            {
                IsBusy = true;
                CreateAllFilesCollection();
                IsBusy = false;
            }
        }

        #endregion

        #region Methods

        private void BackupAllDocuments()
        {
            _dialogService.ShowBackupAllIndexedDocuments(
            () => _logger.Information("Message afterwards here!"));
        }

        private void BackupCollections()
        {
            _dialogService.ShowBackupCollections(
            () => _logger.Information("Message afterward here!"));
        }

        private void BackupSingleDocuments()
        {
            _dialogService.ShowBackupDocuments(
            () => _logger.Information("Message afterwards here!"));
        }

        private void CreateAllFilesCollection()
        {
            _logger.Information(@"Initializing collection ""All indexed files"" containing all the files that are indexed.");

            var result = _indexingManager.SearchIndexForName("*");
            
            var allIndexedFiles = new DocumentCollection
            {
                Documents = new List<Document>(),
                Name = "All indexed files"
            };

            var tags = new List<string>();
            _tagItemsCollection = new ObservableCollection<TagItem>();

            foreach (var res in result)
            {
                allIndexedFiles.Documents.Add(res);
                tags.Add(res.Tag);
            }

            _logger.Information(@"Reading ""TagCloud"" data and initializing control.");
            var tagdictionary = tags.GroupBy(str => str)
            .ToDictionary(group => group.Key, group => group.Count());
            foreach (var dict in tagdictionary)
            {
                TagCollection.Add(new TagItem(dict.Key, dict.Value));
            }

            var query = allIndexedFiles.Documents.Distinct().ToList();
            var lastFive = allIndexedFiles.Documents.Skip(query.Count - 9).ToList();

            LastFiveIndexeDocumentsCollection = new ObservableCollection<Document>(lastFive);

            _logger.Information(@"Collection ""All indexed files"" Initialized.");
            _logger.Information(@"""TagCloud"" control initialized.");
            _documentCollection=allIndexedFiles.Documents;
            ObservableCollections.Add(allIndexedFiles);
        }

        private void CreateNewCollection()
        {
            _logger.Information("Opening create new collections window.");
            _dialogService.ShowNewCollections(
            () => _logger.Information("Collections window closed. When creating collections remember that an file can exist in several collections."));
        }

        private void EditSettings()
        {
            _logger.Information("Opening settings window.");
            _dialogService.ShowSettings(
            () => _logger.Information("Settings window closed. New settings will not be in affect until the application is restarted"));
        }

        private void OnLogItemAdded(object sender, LogManager e)
        {
            DispatcherHelper.RunAsync(() =>
            {
                if (_logCollection != null && _logCollection.Count > 100)
                    _logCollection.Remove(_logCollection.Last());

                if (_logCollection != null)
                    _logCollection.Insert(0, e.Item);
            });
        }

        private void Search()
        {
            SearchObservableCollection.Clear();
            _logger.Information("Searching for document with document name containing: " + SearchString);

            var result = _indexingManager.SearchIndexForName(SearchString);

            if (result != null)
            {
                foreach (var res in result)
                {
                    SearchObservableCollection.Add(res);
                }
            
                _logger.Information("Search completed, there were: " + result.Count + " files that had " + SearchString + " in the document name.");
            }
            else
            {
                _logger.Error(@"There was an error processing the search with the following searchword: """ + SearchString + @" """+" Please try another searchword. ");                
            }
        }

        private void TagClicked()
        {
            SearchObservableCollection.Clear();
            _logger.Information("Searching for documents with the tag: " + SearchString);

            var result = _indexingManager.SearchIndexForTag(SearchString);

            if (result != null)
            {
                foreach (var res in result)
                {
                    SearchObservableCollection.Add(res);
                }

                _logger.Information("Search completed, there were: " + result.Count + " files that had the tag: " +
                                    SearchString);
            }
            else
            {
                _logger.Error(@"There was an error processing the tag search with the following searchword: """ + SearchString + @" """ + " Please try another tag. ");     
            }
        }

        #endregion
    }
}
