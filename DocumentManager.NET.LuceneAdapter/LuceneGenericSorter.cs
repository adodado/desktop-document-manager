using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DocumentManager.NET.LuceneAdapter.Enums;

namespace DocumentManager.NET.LuceneAdapter
{
    public static class LuceneGenericSorter
    {
        public static IOrderedEnumerable<T> Sort<T>(IEnumerable<T> toSort, Dictionary<string, SortingOrder> sortOptions)
        {
            IOrderedEnumerable<T> orderedList = null;

            foreach (var entry in sortOptions)
            {
                if (orderedList != null)
                {
                    orderedList = orderedList.ApplyOrder<T>(entry.Key, entry.Value == SortingOrder.Ascending ? "ThenBy" : "ThenByDescending");
                }
                else
                {
                    orderedList = toSort.ApplyOrder<T>(entry.Key, entry.Value == SortingOrder.Ascending ? "OrderBy" : "OrderByDescending");
                }
            }

            return orderedList;
        }

        private static IOrderedEnumerable<T> ApplyOrder<T>(this IEnumerable<T> source, string property, string methodName)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression expr = property.Split('.').Aggregate<string, Expression>(param, (current, prop) => Expression.PropertyOrField(current, prop));
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), expr.Type);
            var lambda = Expression.Lambda(delegateType, expr, param);

            var mi = typeof(Enumerable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), expr.Type);
            return (IOrderedEnumerable<T>)mi.Invoke(null, new object[] { source, lambda.Compile() });
        }
    }
}
