﻿namespace System.Web.Caching
{
    using System;
    using System.Web.Util;

    internal class CachedVary
    {
        internal readonly string[] _contentEncodings;
        internal readonly string[] _headers;
        internal readonly string[] _params;
        internal readonly bool _varyByAllParams;
        internal readonly string _varyByCustom;

        internal CachedVary(string[] contentEncodings, string[] headers, string[] parameters, bool varyByAllParams, string varyByCustom)
        {
            this._contentEncodings = contentEncodings;
            this._headers = headers;
            this._params = parameters;
            this._varyByAllParams = varyByAllParams;
            this._varyByCustom = varyByCustom;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CachedVary))
            {
                return false;
            }
            CachedVary vary = (CachedVary) obj;
            return ((((this._varyByAllParams == vary._varyByAllParams) && (this._varyByCustom == vary._varyByCustom)) && (StringUtil.StringArrayEquals(this._contentEncodings, vary._contentEncodings) && StringUtil.StringArrayEquals(this._headers, vary._headers))) && StringUtil.StringArrayEquals(this._params, vary._params));
        }

        public override int GetHashCode()
        {
            HashCodeCombiner combiner = new HashCodeCombiner();
            combiner.AddObject(this._varyByAllParams);
            combiner.AddObject(this._varyByCustom);
            combiner.AddArray(this._contentEncodings);
            combiner.AddArray(this._headers);
            combiner.AddArray(this._params);
            return combiner.CombinedHash32;
        }
    }
}

