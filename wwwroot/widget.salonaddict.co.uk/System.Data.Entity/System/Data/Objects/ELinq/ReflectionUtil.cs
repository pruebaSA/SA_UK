﻿namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class ReflectionUtil
    {
        private static readonly Dictionary<SequenceMethod, MethodInfo> s_inverseMap;
        private static readonly Dictionary<MethodInfo, SequenceMethod> s_methodMap;

        static ReflectionUtil()
        {
            Dictionary<string, SequenceMethod> dictionary = new Dictionary<string, SequenceMethod> {
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Double>>)->Double",
                    SequenceMethod.SumDoubleSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Double>>>)->Nullable`1<Double>",
                    SequenceMethod.SumNullableDoubleSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Decimal>>)->Decimal",
                    SequenceMethod.SumDecimalSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Decimal>>>)->Nullable`1<Decimal>",
                    SequenceMethod.SumNullableDecimalSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Int32>>)->Double",
                    SequenceMethod.AverageIntSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Int32>>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableIntSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Single>>)->Single",
                    SequenceMethod.AverageSingleSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Single>>>)->Nullable`1<Single>",
                    SequenceMethod.AverageNullableSingleSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Int64>>)->Double",
                    SequenceMethod.AverageLongSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Int64>>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableLongSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Double>>)->Double",
                    SequenceMethod.AverageDoubleSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Double>>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableDoubleSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Decimal>>)->Decimal",
                    SequenceMethod.AverageDecimalSelector
                },
                { 
                    "Average(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Decimal>>>)->Nullable`1<Decimal>",
                    SequenceMethod.AverageNullableDecimalSelector
                },
                { 
                    "Aggregate(IQueryable`1<T0>, Expression`1<Func`3<T0, T0, T0>>)->T0",
                    SequenceMethod.Aggregate
                },
                { 
                    "Aggregate(IQueryable`1<T0>, T1, Expression`1<Func`3<T1, T0, T1>>)->T1",
                    SequenceMethod.AggregateSeed
                },
                { 
                    "Aggregate(IQueryable`1<T0>, T1, Expression`1<Func`3<T1, T0, T1>>, Expression`1<Func`2<T1, T2>>)->T2",
                    SequenceMethod.AggregateSeedSelector
                },
                { 
                    "AsQueryable(IEnumerable`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.AsQueryableGeneric
                },
                { 
                    "Where(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->IQueryable`1<T0>",
                    SequenceMethod.Where
                },
                { 
                    "Where(IQueryable`1<T0>, Expression`1<Func`3<T0, Int32, Boolean>>)->IQueryable`1<T0>",
                    SequenceMethod.WhereOrdinal
                },
                { 
                    "OfType(IQueryable)->IQueryable`1<T0>",
                    SequenceMethod.OfType
                },
                { 
                    "Cast(IQueryable)->IQueryable`1<T0>",
                    SequenceMethod.Cast
                },
                { 
                    "Select(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->IQueryable`1<T1>",
                    SequenceMethod.Select
                },
                { 
                    "Select(IQueryable`1<T0>, Expression`1<Func`3<T0, Int32, T1>>)->IQueryable`1<T1>",
                    SequenceMethod.SelectOrdinal
                },
                { 
                    "SelectMany(IQueryable`1<T0>, Expression`1<Func`2<T0, IEnumerable`1<T1>>>)->IQueryable`1<T1>",
                    SequenceMethod.SelectMany
                },
                { 
                    "SelectMany(IQueryable`1<T0>, Expression`1<Func`3<T0, Int32, IEnumerable`1<T1>>>)->IQueryable`1<T1>",
                    SequenceMethod.SelectManyOrdinal
                },
                { 
                    "SelectMany(IQueryable`1<T0>, Expression`1<Func`3<T0, Int32, IEnumerable`1<T1>>>, Expression`1<Func`3<T0, T1, T2>>)->IQueryable`1<T2>",
                    SequenceMethod.SelectManyOrdinalResultSelector
                },
                { 
                    "SelectMany(IQueryable`1<T0>, Expression`1<Func`2<T0, IEnumerable`1<T1>>>, Expression`1<Func`3<T0, T1, T2>>)->IQueryable`1<T2>",
                    SequenceMethod.SelectManyResultSelector
                },
                { 
                    "Join(IQueryable`1<T0>, IEnumerable`1<T1>, Expression`1<Func`2<T0, T2>>, Expression`1<Func`2<T1, T2>>, Expression`1<Func`3<T0, T1, T3>>)->IQueryable`1<T3>",
                    SequenceMethod.Join
                },
                { 
                    "Join(IQueryable`1<T0>, IEnumerable`1<T1>, Expression`1<Func`2<T0, T2>>, Expression`1<Func`2<T1, T2>>, Expression`1<Func`3<T0, T1, T3>>, IEqualityComparer`1<T2>)->IQueryable`1<T3>",
                    SequenceMethod.JoinComparer
                },
                { 
                    "GroupJoin(IQueryable`1<T0>, IEnumerable`1<T1>, Expression`1<Func`2<T0, T2>>, Expression`1<Func`2<T1, T2>>, Expression`1<Func`3<T0, IEnumerable`1<T1>, T3>>)->IQueryable`1<T3>",
                    SequenceMethod.GroupJoin
                },
                { 
                    "GroupJoin(IQueryable`1<T0>, IEnumerable`1<T1>, Expression`1<Func`2<T0, T2>>, Expression`1<Func`2<T1, T2>>, Expression`1<Func`3<T0, IEnumerable`1<T1>, T3>>, IEqualityComparer`1<T2>)->IQueryable`1<T3>",
                    SequenceMethod.GroupJoinComparer
                },
                { 
                    "OrderBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->IOrderedQueryable`1<T0>",
                    SequenceMethod.OrderBy
                },
                { 
                    "OrderBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, IComparer`1<T1>)->IOrderedQueryable`1<T0>",
                    SequenceMethod.OrderByComparer
                },
                { 
                    "OrderByDescending(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->IOrderedQueryable`1<T0>",
                    SequenceMethod.OrderByDescending
                },
                { 
                    "OrderByDescending(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, IComparer`1<T1>)->IOrderedQueryable`1<T0>",
                    SequenceMethod.OrderByDescendingComparer
                },
                { 
                    "ThenBy(IOrderedQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->IOrderedQueryable`1<T0>",
                    SequenceMethod.ThenBy
                },
                { 
                    "ThenBy(IOrderedQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, IComparer`1<T1>)->IOrderedQueryable`1<T0>",
                    SequenceMethod.ThenByComparer
                },
                { 
                    "ThenByDescending(IOrderedQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->IOrderedQueryable`1<T0>",
                    SequenceMethod.ThenByDescending
                },
                { 
                    "ThenByDescending(IOrderedQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, IComparer`1<T1>)->IOrderedQueryable`1<T0>",
                    SequenceMethod.ThenByDescendingComparer
                },
                { 
                    "Take(IQueryable`1<T0>, Int32)->IQueryable`1<T0>",
                    SequenceMethod.Take
                },
                { 
                    "TakeWhile(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->IQueryable`1<T0>",
                    SequenceMethod.TakeWhile
                },
                { 
                    "TakeWhile(IQueryable`1<T0>, Expression`1<Func`3<T0, Int32, Boolean>>)->IQueryable`1<T0>",
                    SequenceMethod.TakeWhileOrdinal
                },
                { 
                    "Skip(IQueryable`1<T0>, Int32)->IQueryable`1<T0>",
                    SequenceMethod.Skip
                },
                { 
                    "SkipWhile(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->IQueryable`1<T0>",
                    SequenceMethod.SkipWhile
                },
                { 
                    "SkipWhile(IQueryable`1<T0>, Expression`1<Func`3<T0, Int32, Boolean>>)->IQueryable`1<T0>",
                    SequenceMethod.SkipWhileOrdinal
                },
                { 
                    "GroupBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->IQueryable`1<IGrouping`2<T1, T0>>",
                    SequenceMethod.GroupBy
                },
                { 
                    "GroupBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, Expression`1<Func`2<T0, T2>>)->IQueryable`1<IGrouping`2<T1, T2>>",
                    SequenceMethod.GroupByElementSelector
                },
                { 
                    "GroupBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, IEqualityComparer`1<T1>)->IQueryable`1<IGrouping`2<T1, T0>>",
                    SequenceMethod.GroupByComparer
                },
                { 
                    "GroupBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, Expression`1<Func`2<T0, T2>>, IEqualityComparer`1<T1>)->IQueryable`1<IGrouping`2<T1, T2>>",
                    SequenceMethod.GroupByElementSelectorComparer
                },
                { 
                    "GroupBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, Expression`1<Func`2<T0, T2>>, Expression`1<Func`3<T1, IEnumerable`1<T2>, T3>>)->IQueryable`1<T3>",
                    SequenceMethod.GroupByElementSelectorResultSelector
                },
                { 
                    "GroupBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, Expression`1<Func`3<T1, IEnumerable`1<T0>, T2>>)->IQueryable`1<T2>",
                    SequenceMethod.GroupByResultSelector
                },
                { 
                    "GroupBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, Expression`1<Func`3<T1, IEnumerable`1<T0>, T2>>, IEqualityComparer`1<T1>)->IQueryable`1<T2>",
                    SequenceMethod.GroupByResultSelectorComparer
                },
                { 
                    "GroupBy(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>, Expression`1<Func`2<T0, T2>>, Expression`1<Func`3<T1, IEnumerable`1<T2>, T3>>, IEqualityComparer`1<T1>)->IQueryable`1<T3>",
                    SequenceMethod.GroupByElementSelectorResultSelectorComparer
                },
                { 
                    "Distinct(IQueryable`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.Distinct
                },
                { 
                    "Distinct(IQueryable`1<T0>, IEqualityComparer`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.DistinctComparer
                },
                { 
                    "Concat(IQueryable`1<T0>, IEnumerable`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.Concat
                },
                { 
                    "Union(IQueryable`1<T0>, IEnumerable`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.Union
                },
                { 
                    "Union(IQueryable`1<T0>, IEnumerable`1<T0>, IEqualityComparer`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.UnionComparer
                },
                { 
                    "Intersect(IQueryable`1<T0>, IEnumerable`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.Intersect
                },
                { 
                    "Intersect(IQueryable`1<T0>, IEnumerable`1<T0>, IEqualityComparer`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.IntersectComparer
                },
                { 
                    "Except(IQueryable`1<T0>, IEnumerable`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.Except
                },
                { 
                    "Except(IQueryable`1<T0>, IEnumerable`1<T0>, IEqualityComparer`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.ExceptComparer
                },
                { 
                    "First(IQueryable`1<T0>)->T0",
                    SequenceMethod.First
                },
                { 
                    "First(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->T0",
                    SequenceMethod.FirstPredicate
                },
                { 
                    "FirstOrDefault(IQueryable`1<T0>)->T0",
                    SequenceMethod.FirstOrDefault
                },
                { 
                    "FirstOrDefault(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->T0",
                    SequenceMethod.FirstOrDefaultPredicate
                },
                { 
                    "Last(IQueryable`1<T0>)->T0",
                    SequenceMethod.Last
                },
                { 
                    "Last(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->T0",
                    SequenceMethod.LastPredicate
                },
                { 
                    "LastOrDefault(IQueryable`1<T0>)->T0",
                    SequenceMethod.LastOrDefault
                },
                { 
                    "LastOrDefault(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->T0",
                    SequenceMethod.LastOrDefaultPredicate
                },
                { 
                    "Single(IQueryable`1<T0>)->T0",
                    SequenceMethod.Single
                },
                { 
                    "Single(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->T0",
                    SequenceMethod.SinglePredicate
                },
                { 
                    "SingleOrDefault(IQueryable`1<T0>)->T0",
                    SequenceMethod.SingleOrDefault
                },
                { 
                    "SingleOrDefault(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->T0",
                    SequenceMethod.SingleOrDefaultPredicate
                },
                { 
                    "ElementAt(IQueryable`1<T0>, Int32)->T0",
                    SequenceMethod.ElementAt
                },
                { 
                    "ElementAtOrDefault(IQueryable`1<T0>, Int32)->T0",
                    SequenceMethod.ElementAtOrDefault
                },
                { 
                    "DefaultIfEmpty(IQueryable`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.DefaultIfEmpty
                },
                { 
                    "DefaultIfEmpty(IQueryable`1<T0>, T0)->IQueryable`1<T0>",
                    SequenceMethod.DefaultIfEmptyValue
                },
                { 
                    "Contains(IQueryable`1<T0>, T0)->Boolean",
                    SequenceMethod.Contains
                },
                { 
                    "Contains(IQueryable`1<T0>, T0, IEqualityComparer`1<T0>)->Boolean",
                    SequenceMethod.ContainsComparer
                },
                { 
                    "Reverse(IQueryable`1<T0>)->IQueryable`1<T0>",
                    SequenceMethod.Reverse
                },
                { 
                    "SequenceEqual(IQueryable`1<T0>, IEnumerable`1<T0>)->Boolean",
                    SequenceMethod.SequenceEqual
                },
                { 
                    "SequenceEqual(IQueryable`1<T0>, IEnumerable`1<T0>, IEqualityComparer`1<T0>)->Boolean",
                    SequenceMethod.SequenceEqualComparer
                },
                { 
                    "Any(IQueryable`1<T0>)->Boolean",
                    SequenceMethod.Any
                },
                { 
                    "Any(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->Boolean",
                    SequenceMethod.AnyPredicate
                },
                { 
                    "All(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->Boolean",
                    SequenceMethod.All
                },
                { 
                    "Count(IQueryable`1<T0>)->Int32",
                    SequenceMethod.Count
                },
                { 
                    "Count(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->Int32",
                    SequenceMethod.CountPredicate
                },
                { 
                    "LongCount(IQueryable`1<T0>)->Int64",
                    SequenceMethod.LongCount
                },
                { 
                    "LongCount(IQueryable`1<T0>, Expression`1<Func`2<T0, Boolean>>)->Int64",
                    SequenceMethod.LongCountPredicate
                },
                { 
                    "Min(IQueryable`1<T0>)->T0",
                    SequenceMethod.Min
                },
                { 
                    "Min(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->T1",
                    SequenceMethod.MinSelector
                },
                { 
                    "Max(IQueryable`1<T0>)->T0",
                    SequenceMethod.Max
                },
                { 
                    "Max(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->T1",
                    SequenceMethod.MaxSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Int32>>)->Int32",
                    SequenceMethod.SumIntSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Int32>>>)->Nullable`1<Int32>",
                    SequenceMethod.SumNullableIntSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Int64>>)->Int64",
                    SequenceMethod.SumLongSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Int64>>>)->Nullable`1<Int64>",
                    SequenceMethod.SumNullableLongSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Single>>)->Single",
                    SequenceMethod.SumSingleSelector
                },
                { 
                    "Sum(IQueryable`1<T0>, Expression`1<Func`2<T0, Nullable`1<Single>>>)->Nullable`1<Single>",
                    SequenceMethod.SumNullableSingleSelector
                },
                { 
                    "AsQueryable(IEnumerable)->IQueryable",
                    SequenceMethod.AsQueryable
                },
                { 
                    "Sum(IQueryable`1<Int32>)->Int32",
                    SequenceMethod.SumInt
                },
                { 
                    "Sum(IQueryable`1<Nullable`1<Int32>>)->Nullable`1<Int32>",
                    SequenceMethod.SumNullableInt
                },
                { 
                    "Sum(IQueryable`1<Int64>)->Int64",
                    SequenceMethod.SumLong
                },
                { 
                    "Sum(IQueryable`1<Nullable`1<Int64>>)->Nullable`1<Int64>",
                    SequenceMethod.SumNullableLong
                },
                { 
                    "Sum(IQueryable`1<Single>)->Single",
                    SequenceMethod.SumSingle
                },
                { 
                    "Sum(IQueryable`1<Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.SumNullableSingle
                },
                { 
                    "Sum(IQueryable`1<Double>)->Double",
                    SequenceMethod.SumDouble
                },
                { 
                    "Sum(IQueryable`1<Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.SumNullableDouble
                },
                { 
                    "Sum(IQueryable`1<Decimal>)->Decimal",
                    SequenceMethod.SumDecimal
                },
                { 
                    "Sum(IQueryable`1<Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.SumNullableDecimal
                },
                { 
                    "Average(IQueryable`1<Int32>)->Double",
                    SequenceMethod.AverageInt
                },
                { 
                    "Average(IQueryable`1<Nullable`1<Int32>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableInt
                },
                { 
                    "Average(IQueryable`1<Int64>)->Double",
                    SequenceMethod.AverageLong
                },
                { 
                    "Average(IQueryable`1<Nullable`1<Int64>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableLong
                },
                { 
                    "Average(IQueryable`1<Single>)->Single",
                    SequenceMethod.AverageSingle
                },
                { 
                    "Average(IQueryable`1<Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.AverageNullableSingle
                },
                { 
                    "Average(IQueryable`1<Double>)->Double",
                    SequenceMethod.AverageDouble
                },
                { 
                    "Average(IQueryable`1<Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableDouble
                },
                { 
                    "Average(IQueryable`1<Decimal>)->Decimal",
                    SequenceMethod.AverageDecimal
                },
                { 
                    "Average(IQueryable`1<Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.AverageNullableDecimal
                },
                { 
                    "First(IEnumerable`1<T0>)->T0",
                    SequenceMethod.First
                },
                { 
                    "First(IEnumerable`1<T0>, Func`2<T0, Boolean>)->T0",
                    SequenceMethod.FirstPredicate
                },
                { 
                    "FirstOrDefault(IEnumerable`1<T0>)->T0",
                    SequenceMethod.FirstOrDefault
                },
                { 
                    "FirstOrDefault(IEnumerable`1<T0>, Func`2<T0, Boolean>)->T0",
                    SequenceMethod.FirstOrDefaultPredicate
                },
                { 
                    "Last(IEnumerable`1<T0>)->T0",
                    SequenceMethod.Last
                },
                { 
                    "Last(IEnumerable`1<T0>, Func`2<T0, Boolean>)->T0",
                    SequenceMethod.LastPredicate
                },
                { 
                    "LastOrDefault(IEnumerable`1<T0>)->T0",
                    SequenceMethod.LastOrDefault
                },
                { 
                    "LastOrDefault(IEnumerable`1<T0>, Func`2<T0, Boolean>)->T0",
                    SequenceMethod.LastOrDefaultPredicate
                },
                { 
                    "Single(IEnumerable`1<T0>)->T0",
                    SequenceMethod.Single
                },
                { 
                    "Single(IEnumerable`1<T0>, Func`2<T0, Boolean>)->T0",
                    SequenceMethod.SinglePredicate
                },
                { 
                    "SingleOrDefault(IEnumerable`1<T0>)->T0",
                    SequenceMethod.SingleOrDefault
                },
                { 
                    "SingleOrDefault(IEnumerable`1<T0>, Func`2<T0, Boolean>)->T0",
                    SequenceMethod.SingleOrDefaultPredicate
                },
                { 
                    "ElementAt(IEnumerable`1<T0>, Int32)->T0",
                    SequenceMethod.ElementAt
                },
                { 
                    "ElementAtOrDefault(IEnumerable`1<T0>, Int32)->T0",
                    SequenceMethod.ElementAtOrDefault
                },
                { 
                    "Repeat(T0, Int32)->IEnumerable`1<T0>",
                    SequenceMethod.NotSupported
                },
                { 
                    "Empty()->IEnumerable`1<T0>",
                    SequenceMethod.Empty
                },
                { 
                    "Any(IEnumerable`1<T0>)->Boolean",
                    SequenceMethod.Any
                },
                { 
                    "Any(IEnumerable`1<T0>, Func`2<T0, Boolean>)->Boolean",
                    SequenceMethod.AnyPredicate
                },
                { 
                    "All(IEnumerable`1<T0>, Func`2<T0, Boolean>)->Boolean",
                    SequenceMethod.All
                },
                { 
                    "Count(IEnumerable`1<T0>)->Int32",
                    SequenceMethod.Count
                },
                { 
                    "Count(IEnumerable`1<T0>, Func`2<T0, Boolean>)->Int32",
                    SequenceMethod.CountPredicate
                },
                { 
                    "LongCount(IEnumerable`1<T0>)->Int64",
                    SequenceMethod.LongCount
                },
                { 
                    "LongCount(IEnumerable`1<T0>, Func`2<T0, Boolean>)->Int64",
                    SequenceMethod.LongCountPredicate
                },
                { 
                    "Contains(IEnumerable`1<T0>, T0)->Boolean",
                    SequenceMethod.Contains
                },
                { 
                    "Contains(IEnumerable`1<T0>, T0, IEqualityComparer`1<T0>)->Boolean",
                    SequenceMethod.ContainsComparer
                },
                { 
                    "Aggregate(IEnumerable`1<T0>, Func`3<T0, T0, T0>)->T0",
                    SequenceMethod.Aggregate
                },
                { 
                    "Aggregate(IEnumerable`1<T0>, T1, Func`3<T1, T0, T1>)->T1",
                    SequenceMethod.AggregateSeed
                },
                { 
                    "Aggregate(IEnumerable`1<T0>, T1, Func`3<T1, T0, T1>, Func`2<T1, T2>)->T2",
                    SequenceMethod.AggregateSeedSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Int32>)->Int32",
                    SequenceMethod.SumIntSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Int32>>)->Nullable`1<Int32>",
                    SequenceMethod.SumNullableIntSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Int64>)->Int64",
                    SequenceMethod.SumLongSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Int64>>)->Nullable`1<Int64>",
                    SequenceMethod.SumNullableLongSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Single>)->Single",
                    SequenceMethod.SumSingleSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.SumNullableSingleSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Double>)->Double",
                    SequenceMethod.SumDoubleSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.SumNullableDoubleSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Decimal>)->Decimal",
                    SequenceMethod.SumDecimalSelector
                },
                { 
                    "Sum(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.SumNullableDecimalSelector
                },
                { 
                    "Min(IEnumerable`1<T0>)->T0",
                    SequenceMethod.Min
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Int32>)->Int32",
                    SequenceMethod.MinIntSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Int32>>)->Nullable`1<Int32>",
                    SequenceMethod.MinNullableIntSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Int64>)->Int64",
                    SequenceMethod.MinLongSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Int64>>)->Nullable`1<Int64>",
                    SequenceMethod.MinNullableLongSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Single>)->Single",
                    SequenceMethod.MinSingleSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.MinNullableSingleSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Double>)->Double",
                    SequenceMethod.MinDoubleSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.MinNullableDoubleSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Decimal>)->Decimal",
                    SequenceMethod.MinDecimalSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.MinNullableDecimalSelector
                },
                { 
                    "Min(IEnumerable`1<T0>, Func`2<T0, T1>)->T1",
                    SequenceMethod.MinSelector
                },
                { 
                    "Max(IEnumerable`1<T0>)->T0",
                    SequenceMethod.Max
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Int32>)->Int32",
                    SequenceMethod.MaxIntSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Int32>>)->Nullable`1<Int32>",
                    SequenceMethod.MaxNullableIntSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Int64>)->Int64",
                    SequenceMethod.MaxLongSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Int64>>)->Nullable`1<Int64>",
                    SequenceMethod.MaxNullableLongSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Single>)->Single",
                    SequenceMethod.MaxSingleSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.MaxNullableSingleSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Double>)->Double",
                    SequenceMethod.MaxDoubleSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.MaxNullableDoubleSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Decimal>)->Decimal",
                    SequenceMethod.MaxDecimalSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.MaxNullableDecimalSelector
                },
                { 
                    "Max(IEnumerable`1<T0>, Func`2<T0, T1>)->T1",
                    SequenceMethod.MaxSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Int32>)->Double",
                    SequenceMethod.AverageIntSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Int32>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableIntSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Int64>)->Double",
                    SequenceMethod.AverageLongSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Int64>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableLongSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Single>)->Single",
                    SequenceMethod.AverageSingleSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.AverageNullableSingleSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Double>)->Double",
                    SequenceMethod.AverageDoubleSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableDoubleSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Decimal>)->Decimal",
                    SequenceMethod.AverageDecimalSelector
                },
                { 
                    "Average(IEnumerable`1<T0>, Func`2<T0, Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.AverageNullableDecimalSelector
                },
                { 
                    "Where(IEnumerable`1<T0>, Func`2<T0, Boolean>)->IEnumerable`1<T0>",
                    SequenceMethod.Where
                },
                { 
                    "Where(IEnumerable`1<T0>, Func`3<T0, Int32, Boolean>)->IEnumerable`1<T0>",
                    SequenceMethod.WhereOrdinal
                },
                { 
                    "Select(IEnumerable`1<T0>, Func`2<T0, T1>)->IEnumerable`1<T1>",
                    SequenceMethod.Select
                },
                { 
                    "Select(IEnumerable`1<T0>, Func`3<T0, Int32, T1>)->IEnumerable`1<T1>",
                    SequenceMethod.SelectOrdinal
                },
                { 
                    "SelectMany(IEnumerable`1<T0>, Func`2<T0, IEnumerable`1<T1>>)->IEnumerable`1<T1>",
                    SequenceMethod.SelectMany
                },
                { 
                    "SelectMany(IEnumerable`1<T0>, Func`3<T0, Int32, IEnumerable`1<T1>>)->IEnumerable`1<T1>",
                    SequenceMethod.SelectManyOrdinal
                },
                { 
                    "SelectMany(IEnumerable`1<T0>, Func`3<T0, Int32, IEnumerable`1<T1>>, Func`3<T0, T1, T2>)->IEnumerable`1<T2>",
                    SequenceMethod.SelectManyOrdinalResultSelector
                },
                { 
                    "SelectMany(IEnumerable`1<T0>, Func`2<T0, IEnumerable`1<T1>>, Func`3<T0, T1, T2>)->IEnumerable`1<T2>",
                    SequenceMethod.SelectManyResultSelector
                },
                { 
                    "Take(IEnumerable`1<T0>, Int32)->IEnumerable`1<T0>",
                    SequenceMethod.Take
                },
                { 
                    "TakeWhile(IEnumerable`1<T0>, Func`2<T0, Boolean>)->IEnumerable`1<T0>",
                    SequenceMethod.TakeWhile
                },
                { 
                    "TakeWhile(IEnumerable`1<T0>, Func`3<T0, Int32, Boolean>)->IEnumerable`1<T0>",
                    SequenceMethod.TakeWhileOrdinal
                },
                { 
                    "Skip(IEnumerable`1<T0>, Int32)->IEnumerable`1<T0>",
                    SequenceMethod.Skip
                },
                { 
                    "SkipWhile(IEnumerable`1<T0>, Func`2<T0, Boolean>)->IEnumerable`1<T0>",
                    SequenceMethod.SkipWhile
                },
                { 
                    "SkipWhile(IEnumerable`1<T0>, Func`3<T0, Int32, Boolean>)->IEnumerable`1<T0>",
                    SequenceMethod.SkipWhileOrdinal
                },
                { 
                    "Join(IEnumerable`1<T0>, IEnumerable`1<T1>, Func`2<T0, T2>, Func`2<T1, T2>, Func`3<T0, T1, T3>)->IEnumerable`1<T3>",
                    SequenceMethod.Join
                },
                { 
                    "Join(IEnumerable`1<T0>, IEnumerable`1<T1>, Func`2<T0, T2>, Func`2<T1, T2>, Func`3<T0, T1, T3>, IEqualityComparer`1<T2>)->IEnumerable`1<T3>",
                    SequenceMethod.JoinComparer
                },
                { 
                    "GroupJoin(IEnumerable`1<T0>, IEnumerable`1<T1>, Func`2<T0, T2>, Func`2<T1, T2>, Func`3<T0, IEnumerable`1<T1>, T3>)->IEnumerable`1<T3>",
                    SequenceMethod.GroupJoin
                },
                { 
                    "GroupJoin(IEnumerable`1<T0>, IEnumerable`1<T1>, Func`2<T0, T2>, Func`2<T1, T2>, Func`3<T0, IEnumerable`1<T1>, T3>, IEqualityComparer`1<T2>)->IEnumerable`1<T3>",
                    SequenceMethod.GroupJoinComparer
                },
                { 
                    "OrderBy(IEnumerable`1<T0>, Func`2<T0, T1>)->IOrderedEnumerable`1<T0>",
                    SequenceMethod.OrderBy
                },
                { 
                    "OrderBy(IEnumerable`1<T0>, Func`2<T0, T1>, IComparer`1<T1>)->IOrderedEnumerable`1<T0>",
                    SequenceMethod.OrderByComparer
                },
                { 
                    "OrderByDescending(IEnumerable`1<T0>, Func`2<T0, T1>)->IOrderedEnumerable`1<T0>",
                    SequenceMethod.OrderByDescending
                },
                { 
                    "OrderByDescending(IEnumerable`1<T0>, Func`2<T0, T1>, IComparer`1<T1>)->IOrderedEnumerable`1<T0>",
                    SequenceMethod.OrderByDescendingComparer
                },
                { 
                    "ThenBy(IOrderedEnumerable`1<T0>, Func`2<T0, T1>)->IOrderedEnumerable`1<T0>",
                    SequenceMethod.ThenBy
                },
                { 
                    "ThenBy(IOrderedEnumerable`1<T0>, Func`2<T0, T1>, IComparer`1<T1>)->IOrderedEnumerable`1<T0>",
                    SequenceMethod.ThenByComparer
                },
                { 
                    "ThenByDescending(IOrderedEnumerable`1<T0>, Func`2<T0, T1>)->IOrderedEnumerable`1<T0>",
                    SequenceMethod.ThenByDescending
                },
                { 
                    "ThenByDescending(IOrderedEnumerable`1<T0>, Func`2<T0, T1>, IComparer`1<T1>)->IOrderedEnumerable`1<T0>",
                    SequenceMethod.ThenByDescendingComparer
                },
                { 
                    "GroupBy(IEnumerable`1<T0>, Func`2<T0, T1>)->IEnumerable`1<IGrouping`2<T1, T0>>",
                    SequenceMethod.GroupBy
                },
                { 
                    "GroupBy(IEnumerable`1<T0>, Func`2<T0, T1>, IEqualityComparer`1<T1>)->IEnumerable`1<IGrouping`2<T1, T0>>",
                    SequenceMethod.GroupByComparer
                },
                { 
                    "GroupBy(IEnumerable`1<T0>, Func`2<T0, T1>, Func`2<T0, T2>)->IEnumerable`1<IGrouping`2<T1, T2>>",
                    SequenceMethod.GroupByElementSelector
                },
                { 
                    "GroupBy(IEnumerable`1<T0>, Func`2<T0, T1>, Func`2<T0, T2>, IEqualityComparer`1<T1>)->IEnumerable`1<IGrouping`2<T1, T2>>",
                    SequenceMethod.GroupByElementSelectorComparer
                },
                { 
                    "GroupBy(IEnumerable`1<T0>, Func`2<T0, T1>, Func`3<T1, IEnumerable`1<T0>, T2>)->IEnumerable`1<T2>",
                    SequenceMethod.GroupByResultSelector
                },
                { 
                    "GroupBy(IEnumerable`1<T0>, Func`2<T0, T1>, Func`2<T0, T2>, Func`3<T1, IEnumerable`1<T2>, T3>)->IEnumerable`1<T3>",
                    SequenceMethod.GroupByElementSelectorResultSelector
                },
                { 
                    "GroupBy(IEnumerable`1<T0>, Func`2<T0, T1>, Func`3<T1, IEnumerable`1<T0>, T2>, IEqualityComparer`1<T1>)->IEnumerable`1<T2>",
                    SequenceMethod.GroupByResultSelectorComparer
                },
                { 
                    "GroupBy(IEnumerable`1<T0>, Func`2<T0, T1>, Func`2<T0, T2>, Func`3<T1, IEnumerable`1<T2>, T3>, IEqualityComparer`1<T1>)->IEnumerable`1<T3>",
                    SequenceMethod.GroupByElementSelectorResultSelectorComparer
                },
                { 
                    "Concat(IEnumerable`1<T0>, IEnumerable`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.Concat
                },
                { 
                    "Distinct(IEnumerable`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.Distinct
                },
                { 
                    "Distinct(IEnumerable`1<T0>, IEqualityComparer`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.DistinctComparer
                },
                { 
                    "Union(IEnumerable`1<T0>, IEnumerable`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.Union
                },
                { 
                    "Union(IEnumerable`1<T0>, IEnumerable`1<T0>, IEqualityComparer`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.UnionComparer
                },
                { 
                    "Intersect(IEnumerable`1<T0>, IEnumerable`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.Intersect
                },
                { 
                    "Intersect(IEnumerable`1<T0>, IEnumerable`1<T0>, IEqualityComparer`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.IntersectComparer
                },
                { 
                    "Except(IEnumerable`1<T0>, IEnumerable`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.Except
                },
                { 
                    "Except(IEnumerable`1<T0>, IEnumerable`1<T0>, IEqualityComparer`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.ExceptComparer
                },
                { 
                    "Reverse(IEnumerable`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.Reverse
                },
                { 
                    "SequenceEqual(IEnumerable`1<T0>, IEnumerable`1<T0>)->Boolean",
                    SequenceMethod.SequenceEqual
                },
                { 
                    "SequenceEqual(IEnumerable`1<T0>, IEnumerable`1<T0>, IEqualityComparer`1<T0>)->Boolean",
                    SequenceMethod.SequenceEqualComparer
                },
                { 
                    "AsEnumerable(IEnumerable`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.AsEnumerable
                },
                { 
                    "ToArray(IEnumerable`1<T0>)->TSource[]",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToList(IEnumerable`1<T0>)->List`1<T0>",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToDictionary(IEnumerable`1<T0>, Func`2<T0, T1>)->Dictionary`2<T1, T0>",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToDictionary(IEnumerable`1<T0>, Func`2<T0, T1>, IEqualityComparer`1<T1>)->Dictionary`2<T1, T0>",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToDictionary(IEnumerable`1<T0>, Func`2<T0, T1>, Func`2<T0, T2>)->Dictionary`2<T1, T2>",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToDictionary(IEnumerable`1<T0>, Func`2<T0, T1>, Func`2<T0, T2>, IEqualityComparer`1<T1>)->Dictionary`2<T1, T2>",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToLookup(IEnumerable`1<T0>, Func`2<T0, T1>)->ILookup`2<T1, T0>",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToLookup(IEnumerable`1<T0>, Func`2<T0, T1>, IEqualityComparer`1<T1>)->ILookup`2<T1, T0>",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToLookup(IEnumerable`1<T0>, Func`2<T0, T1>, Func`2<T0, T2>)->ILookup`2<T1, T2>",
                    SequenceMethod.NotSupported
                },
                { 
                    "ToLookup(IEnumerable`1<T0>, Func`2<T0, T1>, Func`2<T0, T2>, IEqualityComparer`1<T1>)->ILookup`2<T1, T2>",
                    SequenceMethod.NotSupported
                },
                { 
                    "DefaultIfEmpty(IEnumerable`1<T0>)->IEnumerable`1<T0>",
                    SequenceMethod.DefaultIfEmpty
                },
                { 
                    "DefaultIfEmpty(IEnumerable`1<T0>, T0)->IEnumerable`1<T0>",
                    SequenceMethod.DefaultIfEmptyValue
                },
                { 
                    "OfType(IEnumerable)->IEnumerable`1<T0>",
                    SequenceMethod.OfType
                },
                { 
                    "Cast(IEnumerable)->IEnumerable`1<T0>",
                    SequenceMethod.Cast
                },
                { 
                    "Range(Int32, Int32)->IEnumerable`1<Int32>",
                    SequenceMethod.NotSupported
                },
                { 
                    "Sum(IEnumerable`1<Int32>)->Int32",
                    SequenceMethod.SumInt
                },
                { 
                    "Sum(IEnumerable`1<Nullable`1<Int32>>)->Nullable`1<Int32>",
                    SequenceMethod.SumNullableInt
                },
                { 
                    "Sum(IEnumerable`1<Int64>)->Int64",
                    SequenceMethod.SumLong
                },
                { 
                    "Sum(IEnumerable`1<Nullable`1<Int64>>)->Nullable`1<Int64>",
                    SequenceMethod.SumNullableLong
                },
                { 
                    "Sum(IEnumerable`1<Single>)->Single",
                    SequenceMethod.SumSingle
                },
                { 
                    "Sum(IEnumerable`1<Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.SumNullableSingle
                },
                { 
                    "Sum(IEnumerable`1<Double>)->Double",
                    SequenceMethod.SumDouble
                },
                { 
                    "Sum(IEnumerable`1<Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.SumNullableDouble
                },
                { 
                    "Sum(IEnumerable`1<Decimal>)->Decimal",
                    SequenceMethod.SumDecimal
                },
                { 
                    "Sum(IEnumerable`1<Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.SumNullableDecimal
                },
                { 
                    "Min(IEnumerable`1<Int32>)->Int32",
                    SequenceMethod.MinInt
                },
                { 
                    "Min(IEnumerable`1<Nullable`1<Int32>>)->Nullable`1<Int32>",
                    SequenceMethod.MinNullableInt
                },
                { 
                    "Min(IEnumerable`1<Int64>)->Int64",
                    SequenceMethod.MinLong
                },
                { 
                    "Min(IEnumerable`1<Nullable`1<Int64>>)->Nullable`1<Int64>",
                    SequenceMethod.MinNullableLong
                },
                { 
                    "Min(IEnumerable`1<Single>)->Single",
                    SequenceMethod.MinSingle
                },
                { 
                    "Min(IEnumerable`1<Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.MinNullableSingle
                },
                { 
                    "Min(IEnumerable`1<Double>)->Double",
                    SequenceMethod.MinDouble
                },
                { 
                    "Min(IEnumerable`1<Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.MinNullableDouble
                },
                { 
                    "Min(IEnumerable`1<Decimal>)->Decimal",
                    SequenceMethod.MinDecimal
                },
                { 
                    "Min(IEnumerable`1<Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.MinNullableDecimal
                },
                { 
                    "Max(IEnumerable`1<Int32>)->Int32",
                    SequenceMethod.MaxInt
                },
                { 
                    "Max(IEnumerable`1<Nullable`1<Int32>>)->Nullable`1<Int32>",
                    SequenceMethod.MaxNullableInt
                },
                { 
                    "Max(IEnumerable`1<Int64>)->Int64",
                    SequenceMethod.MaxLong
                },
                { 
                    "Max(IEnumerable`1<Nullable`1<Int64>>)->Nullable`1<Int64>",
                    SequenceMethod.MaxNullableLong
                },
                { 
                    "Max(IEnumerable`1<Double>)->Double",
                    SequenceMethod.MaxDouble
                },
                { 
                    "Max(IEnumerable`1<Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.MaxNullableDouble
                },
                { 
                    "Max(IEnumerable`1<Single>)->Single",
                    SequenceMethod.MaxSingle
                },
                { 
                    "Max(IEnumerable`1<Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.MaxNullableSingle
                },
                { 
                    "Max(IEnumerable`1<Decimal>)->Decimal",
                    SequenceMethod.MaxDecimal
                },
                { 
                    "Max(IEnumerable`1<Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.MaxNullableDecimal
                },
                { 
                    "Average(IEnumerable`1<Int32>)->Double",
                    SequenceMethod.AverageInt
                },
                { 
                    "Average(IEnumerable`1<Nullable`1<Int32>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableInt
                },
                { 
                    "Average(IEnumerable`1<Int64>)->Double",
                    SequenceMethod.AverageLong
                },
                { 
                    "Average(IEnumerable`1<Nullable`1<Int64>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableLong
                },
                { 
                    "Average(IEnumerable`1<Single>)->Single",
                    SequenceMethod.AverageSingle
                },
                { 
                    "Average(IEnumerable`1<Nullable`1<Single>>)->Nullable`1<Single>",
                    SequenceMethod.AverageNullableSingle
                },
                { 
                    "Average(IEnumerable`1<Double>)->Double",
                    SequenceMethod.AverageDouble
                },
                { 
                    "Average(IEnumerable`1<Nullable`1<Double>>)->Nullable`1<Double>",
                    SequenceMethod.AverageNullableDouble
                },
                { 
                    "Average(IEnumerable`1<Decimal>)->Decimal",
                    SequenceMethod.AverageDecimal
                },
                { 
                    "Average(IEnumerable`1<Nullable`1<Decimal>>)->Nullable`1<Decimal>",
                    SequenceMethod.AverageNullableDecimal
                }
            };
            s_methodMap = new Dictionary<MethodInfo, SequenceMethod>();
            s_inverseMap = new Dictionary<SequenceMethod, MethodInfo>();
            foreach (MethodInfo info in GetAllLinqOperators())
            {
                SequenceMethod method;
                string canonicalMethodDescription = GetCanonicalMethodDescription(info);
                if (dictionary.TryGetValue(canonicalMethodDescription, out method))
                {
                    s_methodMap.Add(info, method);
                    s_inverseMap[method] = info;
                }
            }
        }

        private static void AppendCanonicalTypeDescription(Type type, Dictionary<Type, int> genericArgumentOrdinals, StringBuilder description)
        {
            int num;
            if ((genericArgumentOrdinals != null) && genericArgumentOrdinals.TryGetValue(type, out num))
            {
                description.Append("T").Append(num.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                description.Append(type.Name);
                if (type.IsGenericType)
                {
                    description.Append("<");
                    bool flag = true;
                    foreach (Type type2 in type.GetGenericArguments())
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        else
                        {
                            description.Append(", ");
                        }
                        AppendCanonicalTypeDescription(type2, genericArgumentOrdinals, description);
                    }
                    description.Append(">");
                }
            }
        }

        internal static IEnumerable<MethodInfo> GetAllLinqOperators() => 
            typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).Concat<MethodInfo>(typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static));

        internal static string GetCanonicalMethodDescription(MethodInfo method)
        {
            EntityUtil.CheckArgumentNull<MethodInfo>(method, "method");
            Dictionary<Type, int> genericArgumentOrdinals = null;
            if (method.IsGenericMethodDefinition)
            {
                genericArgumentOrdinals = (from t in method.GetGenericArguments()
                    where t.IsGenericParameter
                    select t).Select<Type, KeyValuePair<Type, int>>((t, i) => new KeyValuePair<Type, int>(t, i)).ToDictionary<KeyValuePair<Type, int>, Type, int>(r => r.Key, r => r.Value);
            }
            StringBuilder description = new StringBuilder();
            description.Append(method.Name).Append("(");
            bool flag = true;
            foreach (ParameterInfo info in method.GetParameters())
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    description.Append(", ");
                }
                AppendCanonicalTypeDescription(info.ParameterType, genericArgumentOrdinals, description);
            }
            description.Append(")");
            if (method.ReturnType != null)
            {
                description.Append("->");
                AppendCanonicalTypeDescription(method.ReturnType, genericArgumentOrdinals, description);
            }
            return description.ToString();
        }

        internal static bool TryIdentifySequenceMethod(MethodInfo method, out SequenceMethod sequenceMethod)
        {
            method = method.IsGenericMethod ? method.GetGenericMethodDefinition() : method;
            return s_methodMap.TryGetValue(method, out sequenceMethod);
        }

        internal static bool TryIdentifySequenceMethod(Expression expression, bool unwrapLambda, out SequenceMethod sequenceMethod)
        {
            if ((expression.NodeType == ExpressionType.Lambda) && unwrapLambda)
            {
                expression = ((LambdaExpression) expression).Body;
            }
            if (expression.NodeType == ExpressionType.Call)
            {
                MethodCallExpression expression2 = (MethodCallExpression) expression;
                return TryIdentifySequenceMethod(expression2.Method, out sequenceMethod);
            }
            sequenceMethod = SequenceMethod.Where;
            return false;
        }

        internal static bool TryLookupMethod(SequenceMethod sequenceMethod, out MethodInfo method) => 
            s_inverseMap.TryGetValue(sequenceMethod, out method);
    }
}

