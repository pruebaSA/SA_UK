namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class RegionInfo
    {
        [OptionalField(VersionAdded=2)]
        private int m_cultureId;
        [NonSerialized]
        internal CultureTableRecord m_cultureTableRecord;
        internal static RegionInfo m_currentRegionInfo;
        internal int m_dataItem;
        internal string m_name;

        internal RegionInfo(CultureTableRecord table)
        {
            this.m_cultureTableRecord = table;
            this.m_name = this.m_cultureTableRecord.SREGIONNAME;
        }

        public RegionInfo(int culture)
        {
            if (culture == 0x7f)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NoRegionInvariantCulture"));
            }
            if (CultureTableRecord.IsCustomCultureId(culture))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_CustomCultureCannotBePassedByNumber", new object[] { "culture" }));
            }
            if (CultureInfo.GetSubLangID(culture) == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_CultureIsNeutral", new object[] { culture }), "culture");
            }
            this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(culture, true);
            if (this.m_cultureTableRecord.IsNeutralCulture)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_CultureIsNeutral", new object[] { culture }), "culture");
            }
            this.m_name = this.m_cultureTableRecord.SREGIONNAME;
            this.m_cultureId = culture;
        }

        public RegionInfo(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidRegionName", new object[] { name }), "name");
            }
            this.m_name = name.ToUpper(CultureInfo.InvariantCulture);
            this.m_cultureId = 0;
            this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecordForRegion(name, true);
            if (this.m_cultureTableRecord.IsNeutralCulture)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidNeutralRegionName", new object[] { name }), "name");
            }
        }

        public override bool Equals(object value)
        {
            RegionInfo info = value as RegionInfo;
            return ((info != null) && this.Name.Equals(info.Name));
        }

        public override int GetHashCode() => 
            this.Name.GetHashCode();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            if (this.m_name == null)
            {
                this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(CultureTableRecord.IdFromEverettRegionInfoDataItem(this.m_dataItem), true);
                this.m_name = this.m_cultureTableRecord.SREGIONNAME;
            }
            else if (this.m_cultureId != 0)
            {
                this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(this.m_cultureId, true);
            }
            else
            {
                this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecordForRegion(this.m_name, true);
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctx)
        {
            this.m_dataItem = this.m_cultureTableRecord.EverettRegionDataItem();
        }

        public override string ToString() => 
            this.Name;

        [ComVisible(false)]
        public virtual string CurrencyEnglishName =>
            this.m_cultureTableRecord.SENGLISHCURRENCY;

        [ComVisible(false)]
        public virtual string CurrencyNativeName =>
            this.m_cultureTableRecord.SNATIVECURRENCY;

        public virtual string CurrencySymbol =>
            this.m_cultureTableRecord.SCURRENCY;

        public static RegionInfo CurrentRegion
        {
            get
            {
                RegionInfo currentRegionInfo = m_currentRegionInfo;
                if (currentRegionInfo == null)
                {
                    currentRegionInfo = new RegionInfo(CultureInfo.CurrentCulture.m_cultureTableRecord);
                    if (currentRegionInfo.m_cultureTableRecord.IsCustomCulture)
                    {
                        currentRegionInfo.m_name = currentRegionInfo.m_cultureTableRecord.SNAME;
                    }
                    m_currentRegionInfo = currentRegionInfo;
                }
                return currentRegionInfo;
            }
        }

        public virtual string DisplayName
        {
            get
            {
                if (this.m_cultureTableRecord.IsCustomCulture)
                {
                    if (!this.m_cultureTableRecord.IsReplacementCulture)
                    {
                        return this.m_cultureTableRecord.SNATIVECOUNTRY;
                    }
                    if (this.m_cultureTableRecord.IsSynthetic)
                    {
                        return this.m_cultureTableRecord.RegionNativeDisplayName;
                    }
                    return Environment.GetResourceString("Globalization.ri_" + this.m_cultureTableRecord.SREGIONNAME);
                }
                if (this.m_cultureTableRecord.IsSynthetic)
                {
                    return this.m_cultureTableRecord.RegionNativeDisplayName;
                }
                return Environment.GetResourceString("Globalization.ri_" + this.m_cultureTableRecord.SREGIONNAME);
            }
        }

        public virtual string EnglishName =>
            this.m_cultureTableRecord.SENGCOUNTRY;

        [ComVisible(false)]
        public virtual int GeoId =>
            this.m_cultureTableRecord.IGEOID;

        public virtual bool IsMetric =>
            (this.m_cultureTableRecord.IMEASURE == 0);

        public virtual string ISOCurrencySymbol =>
            this.m_cultureTableRecord.SINTLSYMBOL;

        public virtual string Name
        {
            get
            {
                if (this.m_name == null)
                {
                    this.m_name = this.m_cultureTableRecord.SREGIONNAME;
                }
                return this.m_name;
            }
        }

        [ComVisible(false)]
        public virtual string NativeName =>
            this.m_cultureTableRecord.SNATIVECOUNTRY;

        public virtual string ThreeLetterISORegionName =>
            this.m_cultureTableRecord.SISO3166CTRYNAME2;

        public virtual string ThreeLetterWindowsRegionName =>
            this.m_cultureTableRecord.SABBREVCTRYNAME;

        public virtual string TwoLetterISORegionName =>
            this.m_cultureTableRecord.SISO3166CTRYNAME;
    }
}

