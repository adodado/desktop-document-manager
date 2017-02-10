using DocumentManager.NET.Core.Documents;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DocumentManager.NET.Utils.Managers.Indexing
{
    public interface IIndexingManager
    {
        #region Methods

        void CleanUp();

        bool CreateLuceneIndex(ObservableCollection<Document> documents);

        List<Document> SearchDirectoriesRecursive(string path, bool officeSearch, bool pdfSearch,
                    bool textSearch);

        List<Document> SearchIndexForName(string searchString);

        List<Document> SearchIndexForTag(string searchString);

        List<Document> SearchRootDirectory(string path, bool officeSearch, bool pdfSearch, bool textSearch);

        #endregion
    }
}
