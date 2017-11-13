namespace System
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    public class UriTemplateTable
    {
        private Uri baseAddress;
        private string basePath;
        private Dictionary<string, FastPathInfo> fastPathTable;
        private bool noTemplateHasQueryPart;
        private int numSegmentsInBaseAddress;
        private Uri originalUncanonicalizedBaseAddress;
        private UriTemplateTrieNode rootNode;
        private UriTemplatesCollection templates;
        private object thisLock;

        public UriTemplateTable() : this(null, null)
        {
        }

        public UriTemplateTable(IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs) : this(null, keyValuePairs)
        {
        }

        public UriTemplateTable(Uri baseAddress) : this(baseAddress, null)
        {
        }

        public UriTemplateTable(Uri baseAddress, IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs)
        {
            if ((baseAddress != null) && !baseAddress.IsAbsoluteUri)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("baseAddress", SR2.GetString(SR2.UTTMustBeAbsolute, new object[0]));
            }
            this.originalUncanonicalizedBaseAddress = baseAddress;
            if (keyValuePairs != null)
            {
                this.templates = new UriTemplatesCollection(keyValuePairs);
            }
            else
            {
                this.templates = new UriTemplatesCollection();
            }
            this.thisLock = new object();
            this.baseAddress = baseAddress;
            this.NormalizeBaseAddress();
        }

        private static bool AllEquivalent(IList<UriTemplateTableMatchCandidate> list, int a, int b)
        {
            for (int i = a; i < (b - 1); i++)
            {
                UriTemplateTableMatchCandidate candidate = list[i];
                UriTemplateTableMatchCandidate candidate2 = list[i + 1];
                UriTemplateTableMatchCandidate candidate3 = list[i];
                if (!candidate.Template.IsPathPartiallyEquivalentAt(candidate2.Template, candidate3.SegmentsCount))
                {
                    return false;
                }
                UriTemplateTableMatchCandidate candidate4 = list[i];
                UriTemplateTableMatchCandidate candidate5 = list[i + 1];
                if (!candidate4.Template.IsQueryEquivalent(candidate5.Template))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AtLeastOneCandidateHasQueryPart(IList<UriTemplateTableMatchCandidate> candidates)
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                UriTemplateTableMatchCandidate candidate = candidates[i];
                if (!UriTemplateHelpers.CanMatchQueryTrivially(candidate.Template))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ComputeRelativeSegmentsAndLookup(Uri uri, ICollection<string> relativePathSegments, ICollection<UriTemplateTableMatchCandidate> candidates)
        {
            string[] segments = uri.Segments;
            int num = segments.Length - this.numSegmentsInBaseAddress;
            UriTemplateLiteralPathSegment[] wireData = new UriTemplateLiteralPathSegment[num];
            for (int i = 0; i < num; i++)
            {
                string str = segments[i + this.numSegmentsInBaseAddress];
                UriTemplateLiteralPathSegment segment = UriTemplateLiteralPathSegment.CreateFromWireData(str);
                wireData[i] = segment;
                string item = Uri.UnescapeDataString(str);
                if (segment.EndsWithSlash)
                {
                    item = item.Substring(0, item.Length - 1);
                }
                relativePathSegments.Add(item);
            }
            return this.rootNode.Match(wireData, candidates);
        }

        private void ConstructFastPathTable()
        {
            this.noTemplateHasQueryPart = true;
            foreach (KeyValuePair<UriTemplate, object> pair in this.templates)
            {
                UriTemplate key = pair.Key;
                if (!UriTemplateHelpers.CanMatchQueryTrivially(key))
                {
                    this.noTemplateHasQueryPart = false;
                }
                if (key.HasNoVariables && !key.HasWildcard)
                {
                    if (this.fastPathTable == null)
                    {
                        this.fastPathTable = new Dictionary<string, FastPathInfo>();
                    }
                    Uri uri = key.BindByPosition(this.originalUncanonicalizedBaseAddress, new string[0]);
                    string uriPath = UriTemplateHelpers.GetUriPath(uri);
                    if (!this.fastPathTable.ContainsKey(uriPath))
                    {
                        FastPathInfo info = new FastPathInfo();
                        if (this.ComputeRelativeSegmentsAndLookup(uri, info.RelativePathSegments, info.Candidates))
                        {
                            info.Freeze();
                            this.fastPathTable.Add(uriPath, info);
                        }
                    }
                }
            }
        }

        private bool FastComputeRelativeSegmentsAndLookup(Uri uri, out Collection<string> relativePathSegments, out IList<UriTemplateTableMatchCandidate> candidates)
        {
            string uriPath = UriTemplateHelpers.GetUriPath(uri);
            FastPathInfo info = null;
            if ((this.fastPathTable != null) && this.fastPathTable.TryGetValue(uriPath, out info))
            {
                relativePathSegments = info.RelativePathSegments;
                candidates = info.Candidates;
                return true;
            }
            relativePathSegments = new Collection<string>();
            candidates = new Collection<UriTemplateTableMatchCandidate>();
            return this.SlowComputeRelativeSegmentsAndLookup(uri, uriPath, relativePathSegments, candidates);
        }

        public void MakeReadOnly(bool allowDuplicateEquivalentUriTemplates)
        {
            lock (this.thisLock)
            {
                if (!this.IsReadOnly)
                {
                    this.templates.Freeze();
                    this.Validate(allowDuplicateEquivalentUriTemplates);
                    this.ConstructFastPathTable();
                }
            }
        }

        public Collection<UriTemplateMatch> Match(Uri uri)
        {
            Collection<string> collection;
            IList<UriTemplateTableMatchCandidate> list;
            if (uri == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("uri");
            }
            if (!uri.IsAbsoluteUri)
            {
                return None();
            }
            this.MakeReadOnly(true);
            if (!this.FastComputeRelativeSegmentsAndLookup(uri, out collection, out list))
            {
                return None();
            }
            NameValueCollection query = null;
            if (!this.noTemplateHasQueryPart && AtLeastOneCandidateHasQueryPart(list))
            {
                Collection<UriTemplateTableMatchCandidate> collection2 = new Collection<UriTemplateTableMatchCandidate>();
                query = UriTemplateHelpers.ParseQueryString(uri.Query);
                bool mustBeEspeciallyInteresting = NoCandidateHasQueryLiteralRequirementsAndThereIsAnEmptyFallback(list);
                for (int j = 0; j < list.Count; j++)
                {
                    UriTemplateTableMatchCandidate candidate3 = list[j];
                    if (UriTemplateHelpers.CanMatchQueryInterestingly(candidate3.Template, query, mustBeEspeciallyInteresting))
                    {
                        collection2.Add(list[j]);
                    }
                }
                int count = collection2.Count;
                if (collection2.Count == 0)
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        UriTemplateTableMatchCandidate candidate4 = list[k];
                        if (UriTemplateHelpers.CanMatchQueryTrivially(candidate4.Template))
                        {
                            collection2.Add(list[k]);
                        }
                    }
                }
                if (collection2.Count == 0)
                {
                    return None();
                }
                int num6 = collection2.Count;
                list = collection2;
            }
            if (NotAllCandidatesArePathFullyEquivalent(list))
            {
                Collection<UriTemplateTableMatchCandidate> collection3 = new Collection<UriTemplateTableMatchCandidate>();
                int num3 = -1;
                for (int m = 0; m < list.Count; m++)
                {
                    UriTemplateTableMatchCandidate item = list[m];
                    if (num3 == -1)
                    {
                        num3 = item.Template.segments.Count;
                        collection3.Add(item);
                    }
                    else if (item.Template.segments.Count < num3)
                    {
                        num3 = item.Template.segments.Count;
                        collection3.Clear();
                        collection3.Add(item);
                    }
                    else if (item.Template.segments.Count == num3)
                    {
                        collection3.Add(item);
                    }
                }
                list = collection3;
            }
            Collection<UriTemplateMatch> collection4 = new Collection<UriTemplateMatch>();
            for (int i = 0; i < list.Count; i++)
            {
                UriTemplateTableMatchCandidate candidate2 = list[i];
                UriTemplateMatch match = candidate2.Template.CreateUriTemplateMatch(this.originalUncanonicalizedBaseAddress, uri, candidate2.Data, candidate2.SegmentsCount, collection, query);
                collection4.Add(match);
            }
            return collection4;
        }

        public UriTemplateMatch MatchSingle(Uri uri)
        {
            Collection<UriTemplateMatch> collection = this.Match(uri);
            if (collection.Count == 0)
            {
                return null;
            }
            if (collection.Count != 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new UriTemplateMatchException(SR2.GetString(SR2.UTTMultipleMatches, new object[0])));
            }
            return collection[0];
        }

        private static bool NoCandidateHasQueryLiteralRequirementsAndThereIsAnEmptyFallback(IList<UriTemplateTableMatchCandidate> candidates)
        {
            bool flag = false;
            for (int i = 0; i < candidates.Count; i++)
            {
                UriTemplateTableMatchCandidate candidate = candidates[i];
                if (UriTemplateHelpers.HasQueryLiteralRequirements(candidate.Template))
                {
                    return false;
                }
                UriTemplateTableMatchCandidate candidate2 = candidates[i];
                if (candidate2.Template.queries.Count == 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        private static Collection<UriTemplateMatch> None() => 
            new Collection<UriTemplateMatch>();

        private void NormalizeBaseAddress()
        {
            if (this.baseAddress != null)
            {
                UriBuilder builder = new UriBuilder(this.baseAddress);
                if (!builder.Path.EndsWith("/", StringComparison.Ordinal))
                {
                    builder.Path = builder.Path + "/";
                }
                builder.Host = "localhost";
                builder.Port = -1;
                builder.UserName = null;
                builder.Password = null;
                builder.Path = builder.Path.ToUpperInvariant();
                builder.Scheme = Uri.UriSchemeHttp;
                this.baseAddress = builder.Uri;
                this.basePath = UriTemplateHelpers.GetUriPath(this.baseAddress);
            }
        }

        private static bool NotAllCandidatesArePathFullyEquivalent(IList<UriTemplateTableMatchCandidate> candidates)
        {
            if (candidates.Count > 1)
            {
                int count = -1;
                for (int i = 0; i < candidates.Count; i++)
                {
                    if (count == -1)
                    {
                        UriTemplateTableMatchCandidate candidate = candidates[i];
                        count = candidate.Template.segments.Count;
                    }
                    else
                    {
                        UriTemplateTableMatchCandidate candidate2 = candidates[i];
                        if (count != candidate2.Template.segments.Count)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool SlowComputeRelativeSegmentsAndLookup(Uri uri, string uriPath, Collection<string> relativePathSegments, ICollection<UriTemplateTableMatchCandidate> candidates)
        {
            if (uriPath.Length < this.basePath.Length)
            {
                return false;
            }
            if (!uriPath.StartsWith(this.basePath, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return this.ComputeRelativeSegmentsAndLookup(uri, relativePathSegments, candidates);
        }

        private void Validate(bool allowDuplicateEquivalentUriTemplates)
        {
            if (this.baseAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UTTBaseAddressNotSet, new object[0])));
            }
            this.numSegmentsInBaseAddress = this.baseAddress.Segments.Length;
            if (this.templates.Count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UTTEmptyKeyValuePairs, new object[0])));
            }
            this.rootNode = UriTemplateTrieNode.Make(this.templates, allowDuplicateEquivalentUriTemplates);
        }

        [Conditional("DEBUG")]
        private void VerifyThatFastPathAndSlowPathHaveSameResults(Uri uri, Collection<string> fastPathRelativePathSegments, IList<UriTemplateTableMatchCandidate> fastPathCandidates)
        {
            Collection<string> relativePathSegments = new Collection<string>();
            List<UriTemplateTableMatchCandidate> candidates = new List<UriTemplateTableMatchCandidate>();
            this.SlowComputeRelativeSegmentsAndLookup(uri, UriTemplateHelpers.GetUriPath(uri), relativePathSegments, candidates);
            int count = relativePathSegments.Count;
            int num3 = fastPathRelativePathSegments.Count;
            for (int i = 0; i < fastPathRelativePathSegments.Count; i++)
            {
                bool flag1 = fastPathRelativePathSegments[i] != relativePathSegments[i];
            }
            int num4 = candidates.Count;
            int num5 = fastPathCandidates.Count;
            for (int j = 0; j < fastPathCandidates.Count; j++)
            {
                candidates.Contains(fastPathCandidates[j]);
            }
        }

        public Uri BaseAddress
        {
            get => 
                this.baseAddress;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                lock (this.thisLock)
                {
                    if (this.IsReadOnly)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UTTCannotChangeBaseAddress, new object[0])));
                    }
                    if (!value.IsAbsoluteUri)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR2.GetString(SR2.UTTBaseAddressMustBeAbsolute, new object[0]));
                    }
                    this.originalUncanonicalizedBaseAddress = value;
                    this.baseAddress = value;
                    this.NormalizeBaseAddress();
                }
            }
        }

        public bool IsReadOnly =>
            this.templates.IsFrozen;

        public IList<KeyValuePair<UriTemplate, object>> KeyValuePairs =>
            this.templates;

        private class FastPathInfo
        {
            private FreezableCollection<UriTemplateTableMatchCandidate> candidates = new FreezableCollection<UriTemplateTableMatchCandidate>();
            private FreezableCollection<string> relativePathSegments = new FreezableCollection<string>();

            public void Freeze()
            {
                this.relativePathSegments.Freeze();
                this.candidates.Freeze();
            }

            public Collection<UriTemplateTableMatchCandidate> Candidates =>
                this.candidates;

            public Collection<string> RelativePathSegments =>
                this.relativePathSegments;
        }

        private class UriTemplatesCollection : FreezableCollection<KeyValuePair<UriTemplate, object>>
        {
            public UriTemplatesCollection()
            {
            }

            public UriTemplatesCollection(IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs)
            {
                foreach (KeyValuePair<UriTemplate, object> pair in keyValuePairs)
                {
                    ThrowIfInvalid(pair.Key, "keyValuePairs");
                    base.Add(pair);
                }
            }

            protected override void InsertItem(int index, KeyValuePair<UriTemplate, object> item)
            {
                ThrowIfInvalid(item.Key, "item");
                base.InsertItem(index, item);
            }

            protected override void SetItem(int index, KeyValuePair<UriTemplate, object> item)
            {
                ThrowIfInvalid(item.Key, "item");
                base.SetItem(index, item);
            }

            private static void ThrowIfInvalid(UriTemplate template, string argName)
            {
                if (template == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(argName, SR2.GetString(SR2.UTTNullTemplateKey, new object[0]));
                }
                if (template.IgnoreTrailingSlash)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(argName, SR2.GetString(SR2.UTTInvalidTemplateKey, new object[] { template }));
                }
            }
        }
    }
}

