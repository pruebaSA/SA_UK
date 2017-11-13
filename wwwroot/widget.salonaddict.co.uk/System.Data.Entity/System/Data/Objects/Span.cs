namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Text;

    internal sealed class Span
    {
        private string _cacheKey;
        private List<SpanPath> _spanList = new List<SpanPath>();

        internal Span()
        {
        }

        internal void AddSpanPath(SpanPath spanPath)
        {
            if (this.ValidateSpanPath(spanPath))
            {
                this.RemoveExistingSubPaths(spanPath);
                this._spanList.Add(spanPath);
            }
        }

        internal Span Clone()
        {
            Span span = new Span();
            span.SpanList.AddRange(this._spanList);
            span._cacheKey = this._cacheKey;
            return span;
        }

        internal static Span CopyUnion(Span span1, Span span2)
        {
            if (span1 == null)
            {
                return span2;
            }
            if (span2 == null)
            {
                return span1;
            }
            Span span = span1.Clone();
            foreach (SpanPath path in span2.SpanList)
            {
                span.AddSpanPath(path);
            }
            return span;
        }

        internal string GetCacheKey()
        {
            if ((this._cacheKey == null) && (this._spanList.Count > 0))
            {
                if ((this._spanList.Count == 1) && (this._spanList[0].Navigations.Count == 1))
                {
                    this._cacheKey = this._spanList[0].Navigations[0];
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < this._spanList.Count; i++)
                    {
                        if (i > 0)
                        {
                            builder.Append(";");
                        }
                        SpanPath path = this._spanList[i];
                        builder.Append(path.Navigations[0]);
                        for (int j = 1; j < path.Navigations.Count; j++)
                        {
                            builder.Append(".");
                            builder.Append(path.Navigations[j]);
                        }
                    }
                    this._cacheKey = builder.ToString();
                }
            }
            return this._cacheKey;
        }

        public void Include(string path)
        {
            EntityUtil.CheckStringArgument(path, "path");
            SpanPath spanPath = new SpanPath(ParsePath(path));
            this.AddSpanPath(spanPath);
            this._cacheKey = null;
        }

        internal static Span IncludeIn(Span spanToIncludeIn, string pathToInclude)
        {
            if (spanToIncludeIn == null)
            {
                spanToIncludeIn = new Span();
            }
            spanToIncludeIn.Include(pathToInclude);
            return spanToIncludeIn;
        }

        private static List<string> ParsePath(string path)
        {
            List<string> list = new List<string>();
            string[] strArray = MultipartIdentifier.ParseMultipartIdentifier(path, "[", "]", '.', 8, true, "Include", false);
            for (int i = strArray.Length - 1; i >= 0; i--)
            {
                if (strArray[i] != null)
                {
                    if (strArray[i].Length == 0)
                    {
                        throw EntityUtil.SpanPathSyntaxError();
                    }
                    list.Add(strArray[i]);
                }
            }
            if (list.Count > 1)
            {
                list.Reverse();
            }
            return list;
        }

        private void RemoveExistingSubPaths(SpanPath spanPath)
        {
            List<SpanPath> list = new List<SpanPath>();
            for (int i = 0; i < this._spanList.Count; i++)
            {
                if (this._spanList[i].IsSubPath(spanPath))
                {
                    list.Add(this._spanList[i]);
                }
            }
            foreach (SpanPath path in list)
            {
                this._spanList.Remove(path);
            }
        }

        internal static bool RequiresRelationshipSpan(MergeOption mergeOption) => 
            (mergeOption != MergeOption.NoTracking);

        private bool ValidateSpanPath(SpanPath spanPath)
        {
            for (int i = 0; i < this._spanList.Count; i++)
            {
                if (spanPath.IsSubPath(this._spanList[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal List<SpanPath> SpanList =>
            this._spanList;

        internal class SpanPath
        {
            public readonly List<string> Navigations;

            public SpanPath(List<string> navigations)
            {
                this.Navigations = navigations;
            }

            public bool IsSubPath(Span.SpanPath rhs)
            {
                if (this.Navigations.Count > rhs.Navigations.Count)
                {
                    return false;
                }
                for (int i = 0; i < this.Navigations.Count; i++)
                {
                    if (!this.Navigations[i].Equals(rhs.Navigations[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}

