using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Console
{
    internal static class Extensions
    {
        //public static ParallelQuery<TResult> Select<TSource, TResult>(
        //    this ParallelQuery<TSource> source, Func<TSource, TSource, int, TResult> func)
        //{
        //    var current = source.FirstOrDefault();
        //    return source.Skip(1).Select((x, i) =>
        //    {
        //        var t = current;
        //        current = x;
        //        return func(t, x, i);
        //    });
        //}
        //public static IEnumerable<TResult> Select<TSource, TResult>(
        //    this IEnumerable<TSource> source, Func<TSource, TSource, TResult> selector)
        //{
        //    var current = source.FirstOrDefault();
        //    return source.Skip(1).Select(x =>
        //    {
        //        var t = current;
        //        current = x;
        //        return selector(t, x);
        //    });
        //}
    }

}