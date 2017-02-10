using DocumentManager.NET.Logging;
using GalaSoft.MvvmLight.Ioc;
using Directory = Lucene.Net.Store.Directory;
using DocumentManager.NET.LuceneAdapter.Enums;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Version = Lucene.Net.Util.Version;

namespace DocumentManager.NET.LuceneAdapter
{
    public class LuceneAdapter<T, S> : ILuceneAdapter<T, S>
    {
        #region Fields

        private readonly Directory _directory;
        private readonly Analyzer _analyzer;
        private IndexReader _indexReader;
        private Searcher _indexSearch;
        private readonly string _docContex = string.Empty;
        private const int DefaultPageSize = 10;
        private const float DecimalPoint = 4f;
        public string LuceneIndexPath = Environment.CurrentDirectory;
        public List<T> LstLuceneDocObjectTestList = new List<T>();
        private ILoger _loger;

        #endregion

        #region Constructor
        [PreferredConstructor]
        public LuceneAdapter(string document, ILoger loger)
        {
            _docContex = document;
            _directory = FSDirectory.Open(new DirectoryInfo(Environment.CurrentDirectory + "\\LuceneIndex\\" + _docContex));
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
            _loger = loger;
        }

        public LuceneAdapter()
        {
            _directory = FSDirectory.Open(new DirectoryInfo(Environment.CurrentDirectory + "\\LuceneIndex\\"));
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
        }

        public LuceneAdapter(string document, string luceneIndexPath)
        {
            LuceneIndexPath = luceneIndexPath;
            _docContex = document;
            _directory = FSDirectory.Open(new DirectoryInfo(luceneIndexPath + "\\LuceneIndex\\" + _docContex));
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
        }

        #endregion

        #region Methods

        public string ConvertFromLuceneDateTime(DateTime value)
        {
            return value != null ? value.ToString("yyyyMMddHHmmss") : string.Empty;
        }

        public string ConvertFromLuceneDouble(double value)
        {

            value = value * Math.Pow(10f, DecimalPoint);
            return ConvertFromLuceneLong((long)value);
        }

        public string ConvertFromLuceneLong(long value)
        {
            return String.Format("{0:000000000000000000000000}", value);
        }

        public bool PushToindex(List<T> luceneDocObjects)
        {
            var writer = new IndexWriter(_directory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            if (luceneDocObjects == null) return false;
            foreach (var p in luceneDocObjects)
            {
                var row = new Document();

                var pi = p.GetType().GetProperties();
                foreach (var ppi in pi)
                {
                    var name = ppi.Name.ToLower();

                    var value = ppi.GetValue(p, null);

                    var attrs = ppi.GetCustomAttributes(true);

                    var fS = Field.Store.YES;
                    var fI = Field.Index.ANALYZED;
                    var fT = Field.TermVector.NO;
                    if (attrs.Length > 0)
                    {

                        foreach (var item in attrs)
                        {
                            if (item.GetType() == typeof(LuceneStoreAttribute))
                            {
                                var lsA = item as LuceneStoreAttribute;
                                if (lsA != null) fS = (Field.Store)(int)lsA.Value;
                            }
                            else if (item.GetType() == typeof(LuceneIndexAttribute))
                            {
                                var lIA = item as LuceneIndexAttribute;
                                if (lIA != null) fI = (Field.Index)(int)lIA.Value;
                            }
                            else if (item.GetType() == typeof(LuceneTermVectorAttribute))
                            {
                                var lTA = item as LuceneTermVectorAttribute;
                                if (lTA != null) fT = (Field.TermVector)(int)lTA.Value;
                            }
                        }
                    }

                    if (value is int || value is long)
                    {
                        row.Add(new Field(name, ConvertFromLuceneLong(Convert.ToInt64(value)), fS, fI, fT));
                    }
                    else if (value is float || value is double)
                    {
                        row.Add(new Field(name, ConvertFromLuceneDouble(Convert.ToDouble(value)), fS, fI, fT));
                    }
                    else if (value is DateTime)
                    {
                        row.Add(new Field(name, ConvertFromLuceneDateTime(Convert.ToDateTime(value)), fS, fI, fT));
                    }
                    else if (value is string)
                    {

                        row.Add(new Field(name, value.ToString(), fS, fI, fT));
                    }

                }

                writer.AddDocument(row);
            }

            writer.Optimize();
            writer.Commit();
            writer.Dispose();

            return true;
        }

        public bool PushToIndex(T luceneDocObject)
        {
            if (luceneDocObject == null) return false;
            var row = new Document();

            var pi = luceneDocObject.GetType().GetProperties();
            foreach (var ppi in pi)
            {
                var name = ppi.Name.ToLower();

                var value = ppi.GetValue(luceneDocObject, null);

                if (value is int || value is long)
                {
                    row.Add(new Field(name, ConvertFromLuceneLong(Convert.ToInt64(value)), Field.Store.YES,
                    Field.Index.NOT_ANALYZED));
                }
                else if (value is float || value is double)
                {
                    row.Add(new Field(name, ConvertFromLuceneDouble(Convert.ToDouble(value)), Field.Store.YES,
                    Field.Index.NOT_ANALYZED));
                }
                else if (value is DateTime)
                {
                    row.Add(new Field(name, ConvertFromLuceneDateTime(Convert.ToDateTime(value)), Field.Store.YES,
                    Field.Index.NOT_ANALYZED));
                }
                else if (value is string)
                {
                    row.Add(new Field(name, value.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                }

            }

            var writer = new IndexWriter(_directory, _analyzer, true, IndexWriter.MaxFieldLength.LIMITED);
            writer.Optimize();
            writer.Commit();
            writer.Dispose();

            return true;
        }

        public string RemoveDuplicateWords(string source)
        {
            if (source != null)
            {
                var listofUniqueWords = new Dictionary<string, bool>();
                var destBuilder = new StringBuilder();
                var spilltedwords = source.Split(new[] { " ", ",", ";", ".", Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in spilltedwords)
                {
                    var lower = item.ToLower();
                    if (!listofUniqueWords.ContainsKey(lower))
                    {
                        destBuilder.Append(item).Append(' ');
                        listofUniqueWords.Add(lower, true);
                    }
                }

                return destBuilder.ToString().Trim();
            }
            return string.Empty;
        }

        public bool RemoveFromIndex(string field, string keyword)
        {
            var hits = Search(_docContex, field + ":" + keyword).ScoreDocs.ToList();
            var readers = new IndexReader[hits.Count];
            var multiReader = new MultiReader(readers);
            foreach (var hit in hits)
            {
                multiReader.DeleteDocument(hit.Doc);
            }
            multiReader.Commit();
            multiReader.Dispose();
            return true;
        }

        private TopDocs Search(string contex, string searchString, LuceneSearchInfo searchInfo = null)
        {
            try
            {
            if (contex != "" && searchString != "")
            {

                _indexReader = IndexReader.Open(_directory, true);
                _indexSearch = new IndexSearcher(_indexReader);
                var queryParser = new QueryParser(Version.LUCENE_30, "Id", _analyzer);
                queryParser.AllowLeadingWildcard = true;
                var response = _indexSearch.Search(queryParser.Parse(searchString), _indexReader.MaxDoc);

                Thread.Sleep(50);
                _indexReader.Dispose();
                _indexSearch.Dispose();

                if (searchInfo != null)
                {
                    searchInfo.TotalHits = response.TotalHits;
                    searchInfo.TopHits = response.ScoreDocs.Count();
                    searchInfo.MaxScore = response.MaxScore;

                }
                return response;
            }

            }
            catch (Exception ex)
            {
             
            }
            return null;
        }

        public List<T> Search<T>(string searchString, LuceneSearchInfo searchInfo = null, uint page = 0,
                uint pageSize = DefaultPageSize, Dictionary<string, SortingOrder> sortOption = null,
                bool allPagesSorted = false, bool useLucenePaging = true) where T : new()
        {
            return ToLuceneDocObjectList<T>(Search(_docContex, searchString, searchInfo), page, pageSize,
            sortOption, allPagesSorted);
        }

        public List<T> Search<T>(S docSearchObject, LuceneSearchInfo searchInfo = null, uint page = 0,
                uint pageSize = DefaultPageSize, Dictionary<string, SortingOrder> sortOption = null,
                bool allPagesSorted = false, bool useLucenePaging = true) where T : new()
        {
            if (docSearchObject != null)
            {
                var query = new StringBuilder();

                var piarr = docSearchObject.GetType().GetProperties();
                foreach (var ppi in piarr)
                {
                    var name = ppi.Name.ToLower();
                    var value = ppi.GetValue(docSearchObject, null);
                    if (value != null && value.ToString() != "")
                    {

                        value = ToLuceneRange(value);

                        if (name.Contains("from") || name.Contains("to"))
                        {

                            if (name.Substring(name.Length - 4, 4) == "from" ||
                            name.Substring(name.Length - 2, 2) == "to")
                            {
                                if (name.Contains("from"))
                                {
                                    var toValue =
                                    docSearchObject.GetType()
                                    .GetProperty(name.Replace("from", "to"))
                                    .GetValue(docSearchObject, null);
                                    toValue = ToLuceneRange(toValue);
                                    query.Append(name.Replace("from", "") + ":[" + value + " TO " +
                                    toValue + "] AND ");
                                }

                            }
                        }
                        else
                        {
                            query.Append(name + ":" + value + " AND ");
                        }

                    }

                }

                if (!String.IsNullOrEmpty(query.ToString()))
                    return
                    ToLuceneDocObjectList<T>(
                    Search(_docContex, query.ToString().Remove(query.Length - 5, 5), searchInfo),
                    page, pageSize, sortOption, allPagesSorted, useLucenePaging);

            }
            return null;
        }

        public List<T> Search<T>(string Keyword, bool isOrSearch, LuceneSearchInfo searchInfo = null, uint page = 0,
                uint pageSize = DefaultPageSize, Dictionary<string, SortingOrder> sortOption = null,
                bool allPagesSorted = false, bool useLucenePaging = true) where T : new()
        {
            if (!String.IsNullOrEmpty(Keyword))
            {
                var modifier = " AND ";
                if (isOrSearch) modifier = " OR ";

                var query = new StringBuilder();

                var pi = new T().GetType().GetProperties();
                foreach (var ppi in pi)
                {
                    var name = ppi.Name.ToLower();
                    query.Append(name + ":" + Keyword + modifier);

                }

                return
                ToLuceneDocObjectList<T>(
                Search(_docContex, query.ToString().Remove(query.Length - modifier.Length, modifier.Length),
                searchInfo), page, pageSize, sortOption, allPagesSorted, useLucenePaging);
            }
            return null;
        }

        private LuceneSearchInfo SearchCount(string contex, string searchString)
        {
            if (contex != "" || searchString != "")
            {
                var si = new LuceneSearchInfo();
                Search(contex, searchString, si);

                return si;
            }

            return null;
        }

        public LuceneSearchInfo SearchCount(string searchString)
        {
            return SearchCount(_docContex, searchString);
        }

        public LuceneSearchInfo SearchCount(S docSearchObject)
        {
            if (docSearchObject == null) return null;
            var query = new StringBuilder();

            var piarr = docSearchObject.GetType().GetProperties();
            foreach (var ppi in piarr)
            {
                var name = ppi.Name.ToLower();
                var value = ppi.GetValue(docSearchObject, null);
                if (value != null && value.ToString() != "")
                {

                    value = ToLuceneRange(value);

                    if (name.Substring(name.Length - 4, 4) == "from" || name.Substring(name.Length - 2, 2) == "to")
                    {
                        if (name.Contains("from"))
                        {
                            var toValue =
                            docSearchObject.GetType()
                            .GetProperty(name.Replace("from", "to"))
                            .GetValue(docSearchObject, null);
                            toValue = ToLuceneRange(toValue);
                            query.Append(name.Replace("from", "") + ":[" + value + " TO " +
                            toValue + "] AND ");
                        }
                    }

                    else
                    {
                        query.Append(name + ":" + value + " AND ");
                    }

                }

            }

            if (!String.IsNullOrEmpty(query.ToString()))
                return SearchCount(_docContex, query.ToString().Remove(query.Length - 5, 5));

            return null;
        }

        public LuceneSearchInfo SearchCount<T>(string keyword, bool isOrSearch) where T : new()
        {
            if (String.IsNullOrEmpty(keyword)) return null;
            var modifier = " AND ";
            if (isOrSearch) modifier = " OR ";

            var query = new StringBuilder();

            var pi = new T().GetType().GetProperties();
            foreach (var ppi in pi)
            {
                var name = ppi.Name.ToLower();
                query.Append(name + ":" + keyword + modifier);

            }

            return SearchCount(_docContex, query.ToString().Remove(query.Length - modifier.Length, modifier.Length));
        }

        private T ToLuceneDocObject<T>(ScoreDoc hit) where T : new()
        {
            _indexReader = IndexReader.Open(_directory, true);
            _indexSearch = new IndexSearcher(_indexReader);
            var luceneDocObject = new T();
            var hitExtractor = _indexSearch.Doc(hit.Doc);
            for (var i = 0; i < hitExtractor.GetFields().Count; i++)
            {
                var fieldName = hitExtractor.GetFields()[i].ToString();
                fieldName = fieldName.Split('<')[1].Split(':')[0];
                var fieldValue = hitExtractor.Get(fieldName);
                fieldName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
                var propertyType = typeof(T).GetProperty(fieldName).PropertyType;
                object convertedValue;
                var converter = TypeDescriptor.GetConverter(propertyType);
                if (converter != null && converter.CanConvertFrom(fieldValue.GetType()))
                {
                    if (propertyType == typeof(DateTime))
                    {
                        convertedValue = ToLuceneDocObjectDateTime(fieldValue);
                    }
                    else if (propertyType == typeof(double) || propertyType == typeof(float))
                    {
                        convertedValue = ToLuceneDocObjectDoubl(fieldValue);
                    }
                    else
                    {
                        convertedValue = converter.ConvertFrom(fieldValue);
                    }
                }
                else
                {
                    convertedValue = fieldValue.ToString(CultureInfo.InvariantCulture);
                }

                if (propertyType == typeof(float))
                {
                    luceneDocObject.GetType()
                    .GetProperty(fieldName)
                    .SetValue(luceneDocObject, Convert.ToSingle(convertedValue), null);
                }
                else
                {
                    luceneDocObject.GetType().GetProperty(fieldName).SetValue(luceneDocObject, convertedValue, null);
                }
            }
            _indexReader.Dispose();
            _indexSearch.Dispose();
            return luceneDocObject;
        }

        private DateTime ToLuceneDocObjectDateTime(string luceneDateTime)
        {
            return luceneDateTime != null ? DateTime.ParseExact(luceneDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture) : DateTime.MinValue;
        }

        private double ToLuceneDocObjectDoubl(string luceneDouble)
        {

            return ToLuceneDocObjectLong(luceneDouble) / Math.Pow(10f, DecimalPoint);
        }

        private List<T> ToLuceneDocObjectList<T>(TopDocs response, uint page, uint pageSize,
                Dictionary<string, SortingOrder> sortOption = null, bool allPagesSorted = false, bool useLucenePaging = true)
                where T : new()
        {
            var luceneDocObjectList = new List<T>();
            var hits = response.ScoreDocs.ToList();

            if (useLucenePaging)
            {
                luceneDocObjectList.AddRange(hits.Select(t => ToLuceneDocObject<T>(t)));

                return sortOption == null ? luceneDocObjectList : new List<T>(LuceneGenericSorter.Sort<T>(luceneDocObjectList, sortOption));
            }
            if (allPagesSorted)
            {

                var start = (int)(page * pageSize);
                var end = (int)(start + pageSize);
                end = end > hits.Count ? hits.Count : end;
                if (sortOption == null)
                {
                    for (var i = start; i < end; i++)
                    {
                        luceneDocObjectList.Add(ToLuceneDocObject<T>(hits[i]));
                    }

                    return luceneDocObjectList;
                }
                luceneDocObjectList.AddRange(hits.Select(t => ToLuceneDocObject<T>(t)));
                luceneDocObjectList = new List<T>(LuceneGenericSorter.Sort<T>(luceneDocObjectList, sortOption));
                var allSortedList = new List<T>();

                for (var i = start; i < end; i++)
                {
                    allSortedList.Add(luceneDocObjectList[i]);
                }

                return allSortedList;
            }
            luceneDocObjectList.AddRange(hits.Select(t => ToLuceneDocObject<T>(t)));

            return sortOption != null ? new List<T>(LuceneGenericSorter.Sort<T>(luceneDocObjectList, sortOption)) : luceneDocObjectList;
        }

        private long ToLuceneDocObjectLong(string luceneInt)
        {
            return Convert.ToInt64(luceneInt);
        }

        private object ToLuceneRange(object value)
        {
            if (value is int || value is long)
            {
                value = ConvertFromLuceneLong(Convert.ToInt64(value));
            }
            else if (value is float || value is double)
            {
                value = ConvertFromLuceneDouble(Convert.ToDouble(value));
            }
            else if (value is DateTime)
            {
                var dtValue = Convert.ToDateTime(value);
                value = ConvertFromLuceneDateTime(dtValue);
            }
            return value;
        }

        #endregion
    }
}
