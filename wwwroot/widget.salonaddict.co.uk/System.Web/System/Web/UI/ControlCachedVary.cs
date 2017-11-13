namespace System.Web.UI
{
    using System;
    using System.Web.Util;

    internal class ControlCachedVary
    {
        internal readonly string[] _varyByControls;
        internal readonly string _varyByCustom;
        internal readonly string[] _varyByParams;

        internal ControlCachedVary(string[] varyByParams, string[] varyByControls, string varyByCustom)
        {
            this._varyByParams = varyByParams;
            this._varyByControls = varyByControls;
            this._varyByCustom = varyByCustom;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ControlCachedVary))
            {
                return false;
            }
            ControlCachedVary vary = (ControlCachedVary) obj;
            return (((this._varyByCustom == vary._varyByCustom) && StringUtil.StringArrayEquals(this._varyByParams, vary._varyByParams)) && StringUtil.StringArrayEquals(this._varyByControls, vary._varyByControls));
        }

        public override int GetHashCode()
        {
            HashCodeCombiner combiner = new HashCodeCombiner();
            combiner.AddObject(this._varyByCustom);
            combiner.AddArray(this._varyByParams);
            combiner.AddArray(this._varyByControls);
            return combiner.CombinedHash32;
        }
    }
}

