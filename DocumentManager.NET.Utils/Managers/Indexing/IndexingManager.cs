using DocumentManager.NET.Core.Documents;
using DocumentManager.NET.Logging;
using DocumentManager.NET.LuceneAdapter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DocumentManager.NET.Utils.Managers.Indexing
{
    public class IndexingManager : IIndexingManager
    {
        #region Fields

        private List<Document> _searchResultList;
        private ILuceneAdapter<Document, SearchDocuments> _luceneAdapter;
        private ILoger _loger;
        #endregion

        #region Constructor

        public IndexingManager(ILoger loger)
        {
            _loger = loger;
            _luceneAdapter = new LuceneAdapter<Document, SearchDocuments>("Documents", _loger);
            _searchResultList = new List<Document>();
        }

        #endregion

        #region Methods

        public void CleanUp()
        {
            _searchResultList = new List<Document>();
        }

        public bool CreateLuceneIndex(ObservableCollection<Document> documents)
        {
            var list = documents.Where(a => a.IsSelected).ToList();
            return _luceneAdapter.PushToindex(list);
        }

        public List<Document> SearchDirectoriesRecursive(string path, bool officeSearch, bool pdfSearch, bool textSearch)
        {
            SearchSubDirectories(path, officeSearch, pdfSearch, textSearch);
            SearchRootDirectory(path, officeSearch, pdfSearch, textSearch);
            return _searchResultList;
        }

        public List<Document> SearchIndexForName(string searchString)
        {
            var se = new SearchDocuments { Name = searchString, Path = string.Empty, Tag = string.Empty };
            return _luceneAdapter.Search<Document>(se);
        }

        public List<Document> SearchIndexForTag(string searchString)
        {
            var se = new SearchDocuments { Name = string.Empty, Path = string.Empty, Tag = searchString };
            return _luceneAdapter.Search<Document>(se);
        }

        public List<Document> SearchRootDirectory(string path, bool officeSearch, bool pdfSearch, bool textSearch)
        {
            try
            {
                if (officeSearch)
                {
                    foreach (var doc in Directory.GetFiles(path, "*.doc").Select(f => new FileInfo(f))
                    .Select(file => file != null ? new Document { Name = file.Name, Path = file.FullName, Tag = file.Directory.Name } : null))
                    {
                        _searchResultList.Add(doc);
                    }
                    foreach (var doc in Directory.GetFiles(path, "*.xls").Select(f => new FileInfo(f))
                    .Select(file => file != null ? new Document { Name = file.Name, Path = file.FullName, Tag = file.Directory.Name } : null))
                    {
                        _searchResultList.Add(doc);
                    }
                }
                else if (pdfSearch)
                {
                    foreach (var doc in Directory.GetFiles(path, "*.pdf").Select(f => new FileInfo(f))
                    .Select(file => file != null ? new Document { Name = file.Name, Path = file.FullName, Tag = file.Directory.Name } : null))
                    {
                        _searchResultList.Add(doc);
                    }
                }
                else if (textSearch)
                {
                    foreach (var doc in Directory.GetFiles(path, "*.txt").Select(f => new FileInfo(f))
                    .Select(file => file != null ? new Document { Name = file.Name, Path = file.FullName, Tag = file.Directory.Name } : null))
                    {
                        _searchResultList.Add(doc);
                    }
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            return _searchResultList;
        }

        private void SearchSubDirectories(string path, bool officeSearch, bool pdfSearch,
        bool textSearch)
        {
            try
            {
                foreach (var directoryToIndex in Directory.GetDirectories(path))
                {
                    if (officeSearch)
                    {
                        foreach (var doc in Directory.GetFiles(directoryToIndex, "*.doc").Select(f => new FileInfo(f)).Select(file => new Document
                        {
                            Name = file.Name,
                            Path = file.FullName,
                            Tag = file.Directory.Name
                        }))
                        {
                            _searchResultList.Add(doc);
                        }
                        foreach (var doc in Directory.GetFiles(directoryToIndex, "*.xls").Select(f => new FileInfo(f)).Select(file => new Document
                        {
                            Name = file.Name,
                            Path = file.FullName,
                            Tag = file.Directory.Name
                        }))
                        {
                            _searchResultList.Add(doc);
                        }
                    }
                    else if (pdfSearch)
                    {
                        foreach (var doc in Directory.GetFiles(directoryToIndex, "*.pdf").Select(f => new FileInfo(f)).Select(file => new Document
                        {
                            Name = file.Name,
                            Path = file.FullName,
                            Tag = file.Directory.Name
                        }))
                        {
                            _searchResultList.Add(doc);
                        }
                    }
                    else if (textSearch)
                    {
                        foreach (var doc in Directory.GetFiles(directoryToIndex, "*.txt").Select(f => new FileInfo(f)).Select(file => new Document
                        {
                            Name = file.Name,
                            Path = file.FullName,
                            Tag = file.Directory.Name
                        }))
                        {
                            _searchResultList.Add(doc);
                        }
                    }
                    SearchSubDirectories(directoryToIndex, officeSearch, pdfSearch,
                    textSearch);
                }
            }
            catch (Exception exception)
            {
            }
        }

        #endregion
    }
}

