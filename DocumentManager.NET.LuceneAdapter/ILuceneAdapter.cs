using DocumentManager.NET.LuceneAdapter.Enums;
using System;
using System.Collections.Generic;

namespace DocumentManager.NET.LuceneAdapter
{
    public interface ILuceneAdapter<T, S>
    {
        #region Methods

        string ConvertFromLuceneDateTime(DateTime value);

        string ConvertFromLuceneDouble(double value);

        string ConvertFromLuceneLong(long value);

        bool PushToindex(List<T> luceneDocObjects);

        bool PushToIndex(T luceneDocObject);

        string RemoveDuplicateWords(string source);

        bool RemoveFromIndex(string field, string keyword);

        List<T> Search<T>(string searchString, LuceneSearchInfo searchInfo = null, uint page = 0,
                            uint pageSize = 10, Dictionary<string, SortingOrder> sortOption = null,
                            bool allPagesSorted = false, bool useLucenePaging = true) where T : new();

        List<T> Search<T>(S docSearchObject, LuceneSearchInfo searchInfo = null, uint page = 0,
                            uint pageSize = 10, Dictionary<string, SortingOrder> sortOption = null,
                            bool allPagesSorted = false, bool useLucenePaging = true) where T : new();

        List<T> Search<T>(string Keyword, bool isOrSearch, LuceneSearchInfo searchInfo = null, uint page = 0,
                            uint pageSize = 10, Dictionary<string, SortingOrder> sortOption = null,
                            bool allPagesSorted = false, bool useLucenePaging = true) where T : new();

        LuceneSearchInfo SearchCount(string searchString);

        LuceneSearchInfo SearchCount(S docSearchObject);

        LuceneSearchInfo SearchCount<T>(string keyword, bool isOrSearch) where T : new();

        #endregion
    }
}
