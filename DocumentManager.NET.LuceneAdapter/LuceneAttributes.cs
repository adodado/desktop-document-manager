using System;
using DocumentManager.NET.LuceneAdapter.Enums;

namespace DocumentManager.NET.LuceneAdapter
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LuceneStoreAttribute : Attribute
    {
        public StoreAttribute Value { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LuceneIndexAttribute : Attribute
    {
        public IndexAttribute Value { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LuceneTermVectorAttribute : Attribute
    {
        public TermVectorAttribute Value { get; set; }

    }
}
