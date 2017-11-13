namespace System.Globalization
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;

    internal class CultureTable : BaseInfoTable
    {
        private const CultureTypes CultureTypesMask = ~(CultureTypes.FrameworkCultures | CultureTypes.WindowsOnlyCultures | CultureTypes.ReplacementCultures | CultureTypes.UserCustomCulture | CultureTypes.AllCultures);
        private Hashtable hashByLcid;
        private Hashtable hashByName;
        private Hashtable hashByRegionName;
        internal const int ILANGUAGE = 0;
        private static CultureTable m_defaultInstance = new CultureTable("culture.nlp", true);
        private unsafe IDOffsetItem* m_pCultureIDIndex;
        private unsafe CultureNameOffsetItem* m_pCultureNameIndex;
        private unsafe RegionNameOffsetItem* m_pRegionNameIndex;
        internal const string NewSimplifiedChineseCultureName = "zh-Hans";
        internal const string NewTraditionalChineseCultureName = "zh-Hant";
        internal const string SimplifiedChineseCultureName = "zh-CHS";
        private const uint sizeofNameOffsetItem = 8;
        internal const string TraditionalChineseCultureName = "zh-CHT";
        internal const string TypeLoadExceptionMessage = "Failure has occurred while loading a type.";

        internal unsafe CultureTable(string fileName, bool fromAssembly) : base(fileName, fromAssembly)
        {
            if (base.IsValid)
            {
                this.hashByName = Hashtable.Synchronized(new Hashtable());
                this.hashByLcid = Hashtable.Synchronized(new Hashtable());
                this.hashByRegionName = Hashtable.Synchronized(new Hashtable());
                this.m_pCultureNameIndex = (CultureNameOffsetItem*) (base.m_pDataFileStart + base.m_pCultureHeader.cultureNameTableOffset);
                this.m_pRegionNameIndex = (RegionNameOffsetItem*) (base.m_pDataFileStart + base.m_pCultureHeader.regionNameTableOffset);
                this.m_pCultureIDIndex = (IDOffsetItem*) (base.m_pDataFileStart + base.m_pCultureHeader.cultureIDTableOffset);
            }
        }

        private static unsafe string CheckAndGetTheString(ushort* pDataPool, uint offsetInPool, int poolSize)
        {
            if ((offsetInPool + 2) > poolSize)
            {
                return null;
            }
            char* chPtr = (char*) (pDataPool + offsetInPool);
            int length = chPtr[0];
            if (((offsetInPool + length) + ((ulong) 2L)) > poolSize)
            {
                return null;
            }
            return new string(chPtr + 1, 0, length);
        }

        internal unsafe CultureInfo[] GetCultures(CultureTypes types)
        {
            if ((types <= 0) || ((types & ~(CultureTypes.FrameworkCultures | CultureTypes.WindowsOnlyCultures | CultureTypes.ReplacementCultures | CultureTypes.UserCustomCulture | CultureTypes.AllCultures)) != 0))
            {
                throw new ArgumentOutOfRangeException("types", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { CultureTypes.NeutralCultures, CultureTypes.FrameworkCultures }));
            }
            ArrayList list = new ArrayList();
            bool flag = (types & CultureTypes.SpecificCultures) != 0;
            bool flag2 = (types & CultureTypes.NeutralCultures) != 0;
            bool flag3 = (types & CultureTypes.InstalledWin32Cultures) != 0;
            bool flag4 = (types & CultureTypes.UserCustomCulture) != 0;
            bool flag5 = (types & CultureTypes.ReplacementCultures) != 0;
            bool flag6 = (types & CultureTypes.FrameworkCultures) != 0;
            bool flag7 = (types & CultureTypes.WindowsOnlyCultures) != 0;
            StringBuilder builder = new StringBuilder(260);
            builder.Append(Environment.InternalWindowsDirectory);
            builder.Append(@"\Globalization\");
            string path = builder.ToString();
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Assert();
            try
            {
                if (Directory.Exists(path))
                {
                    DirectoryInfo info = new DirectoryInfo(path);
                    foreach (FileInfo info2 in info.GetFiles("*.nlp"))
                    {
                        if (info2.Name.Length > 4)
                        {
                            try
                            {
                                CultureInfo ci = new CultureInfo(info2.Name.Substring(0, info2.Name.Length - 4), true);
                                CultureTypes cultureTypes = ci.CultureTypes;
                                if (!IsNewNeutralChineseCulture(ci) && ((((flag4 && ((cultureTypes & CultureTypes.UserCustomCulture) != 0)) || (flag5 && ((cultureTypes & CultureTypes.ReplacementCultures) != 0))) || ((flag && ((cultureTypes & CultureTypes.SpecificCultures) != 0)) || (flag2 && ((cultureTypes & CultureTypes.NeutralCultures) != 0)))) || (((flag6 && ((cultureTypes & CultureTypes.FrameworkCultures) != 0)) || (flag3 && ((cultureTypes & CultureTypes.InstalledWin32Cultures) != 0))) || (flag7 && ((cultureTypes & CultureTypes.WindowsOnlyCultures) != 0)))))
                                {
                                    list.Add(ci);
                                }
                            }
                            catch (ArgumentException)
                            {
                            }
                        }
                    }
                }
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            if ((flag2 || flag) || (flag6 || flag3))
            {
                for (int i = 0; i < base.m_pCultureHeader.numCultureNames; i++)
                {
                    int actualCultureID = this.m_pCultureIDIndex[i].actualCultureID;
                    if ((CultureInfo.GetSortID(actualCultureID) == 0) && (actualCultureID != 0x40a))
                    {
                        CultureInfo info4 = new CultureInfo(actualCultureID);
                        CultureTypes types3 = info4.CultureTypes;
                        if (((types3 & CultureTypes.ReplacementCultures) == 0) && (((flag6 || ((flag && (info4.Name.Length > 0)) && ((types3 & CultureTypes.SpecificCultures) != 0))) || (flag2 && (((types3 & CultureTypes.NeutralCultures) != 0) || (info4.Name.Length == 0)))) || (flag3 && ((types3 & CultureTypes.InstalledWin32Cultures) != 0))))
                        {
                            list.Add(info4);
                        }
                    }
                    switch (actualCultureID)
                    {
                        case 4:
                        case 0x7c04:
                            i++;
                            break;
                    }
                }
            }
            if ((flag7 || flag) || flag3)
            {
                CultureTableRecord.InitSyntheticMapping();
                foreach (int num3 in CultureTableRecord.SyntheticLcidToNameCache.Keys)
                {
                    if (CultureInfo.GetSortID(num3) == 0)
                    {
                        CultureInfo info5 = new CultureInfo(num3);
                        if ((info5.CultureTypes & CultureTypes.ReplacementCultures) == 0)
                        {
                            list.Add(info5);
                        }
                    }
                }
            }
            CultureInfo[] array = new CultureInfo[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        internal unsafe int GetDataItemFromCultureID(int cultureID, out string actualName)
        {
            CultureTableItem item = (CultureTableItem) this.hashByLcid[cultureID];
            if ((item != null) && (item.culture != 0))
            {
                actualName = item.name;
                return item.dataItem;
            }
            int num = 0;
            int num2 = base.m_pCultureHeader.numCultureNames - 1;
            while (num <= num2)
            {
                int index = (num + num2) / 2;
                int num4 = cultureID - this.m_pCultureIDIndex[index].actualCultureID;
                if (num4 == 0)
                {
                    item = new CultureTableItem();
                    int num5 = item.dataItem = this.m_pCultureIDIndex[index].dataItemIndex;
                    item.culture = cultureID;
                    actualName = item.name = base.GetStringPoolString(this.m_pCultureIDIndex[index].strOffset);
                    this.hashByLcid[cultureID] = item;
                    return num5;
                }
                if (num4 < 0)
                {
                    num2 = index - 1;
                }
                else
                {
                    num = index + 1;
                }
            }
            actualName = "";
            return -1;
        }

        internal unsafe int GetDataItemFromCultureName(string name, out int culture, out string actualName)
        {
            culture = -1;
            actualName = "";
            CultureTableItem item = (CultureTableItem) this.hashByName[name];
            if ((item != null) && (item.culture != 0))
            {
                culture = item.culture;
                actualName = item.name;
                return item.dataItem;
            }
            int num = 0;
            int num2 = base.m_pCultureHeader.numCultureNames - 1;
            while (num <= num2)
            {
                int index = (num + num2) / 2;
                int num4 = base.CompareStringToStringPoolStringBinary(name, this.m_pCultureNameIndex[index].strOffset);
                if (num4 == 0)
                {
                    item = new CultureTableItem();
                    int num5 = item.dataItem = this.m_pCultureNameIndex[index].dataItemIndex;
                    culture = item.culture = this.m_pCultureNameIndex[index].actualCultureID;
                    actualName = item.name = base.GetStringPoolString(this.m_pCultureNameIndex[index].strOffset);
                    this.hashByName[name] = item;
                    return num5;
                }
                if (num4 < 0)
                {
                    num2 = index - 1;
                }
                else
                {
                    num = index + 1;
                }
            }
            culture = -1;
            return -1;
        }

        internal unsafe int GetDataItemFromRegionName(string name)
        {
            object obj2 = this.hashByRegionName[name];
            if (obj2 != null)
            {
                return (int) obj2;
            }
            int num = 0;
            int num2 = base.m_pCultureHeader.numRegionNames - 1;
            while (num <= num2)
            {
                int index = (num + num2) / 2;
                int num4 = base.CompareStringToStringPoolStringBinary(name, this.m_pRegionNameIndex[index].strOffset);
                if (num4 == 0)
                {
                    int dataItemIndex = this.m_pRegionNameIndex[index].dataItemIndex;
                    this.hashByRegionName[name] = dataItemIndex;
                    return dataItemIndex;
                }
                if (num4 < 0)
                {
                    num2 = index - 1;
                }
                else
                {
                    num = index + 1;
                }
            }
            return -1;
        }

        internal bool IsExistingCulture(int lcid)
        {
            string str;
            if (lcid == 0)
            {
                return false;
            }
            return (this.GetDataItemFromCultureID(lcid, out str) >= 0);
        }

        internal static bool IsInstalledLCID(int cultureID)
        {
            if ((Environment.OSInfo & Environment.OSName.Win9x) != Environment.OSName.Invalid)
            {
                return CultureInfo.IsWin9xInstalledCulture(string.Format(CultureInfo.InvariantCulture, "{0,8:X08}", new object[] { cultureID }), cultureID);
            }
            return CultureInfo.IsValidLCID(cultureID, 1);
        }

        internal static bool IsNewNeutralChineseCulture(CultureInfo ci)
        {
            if (((ci.LCID != 0x7c04) || !ci.Name.Equals("zh-Hant")) && ((ci.LCID != 4) || !ci.Name.Equals("zh-Hans")))
            {
                return false;
            }
            return true;
        }

        internal static bool IsOldNeutralChineseCulture(CultureInfo ci)
        {
            if (((ci.LCID != 0x7c04) || !ci.Name.Equals("zh-CHT")) && ((ci.LCID != 4) || !ci.Name.Equals("zh-CHS")))
            {
                return false;
            }
            return true;
        }

        private static bool IsValidLcid(int lcid, bool canBeCustomLcid)
        {
            if (canBeCustomLcid && CultureTableRecord.IsCustomCultureId(lcid))
            {
                return true;
            }
            if (Default.IsExistingCulture(lcid))
            {
                return true;
            }
            CultureTableRecord.InitSyntheticMapping();
            return (CultureTableRecord.SyntheticLcidToNameCache[lcid] != null);
        }

        internal override unsafe void SetDataItemPointers()
        {
            if (this.Validate())
            {
                base.m_itemSize = base.m_pCultureHeader.sizeCultureItem;
                base.m_numItem = base.m_pCultureHeader.numCultureItems;
                base.m_pDataPool = (ushort*) (base.m_pDataFileStart + base.m_pCultureHeader.offsetToDataPool);
                base.m_pItemData = base.m_pDataFileStart + ((byte*) base.m_pCultureHeader.offsetToCultureItemData);
            }
            else
            {
                base.m_valid = false;
            }
        }

        internal unsafe bool Validate()
        {
            if (base.memoryMapFile != null)
            {
                long fileSize = base.memoryMapFile.FileSize;
                if ((((sizeof(EndianessHeader) + sizeof(CultureTableHeader)) + sizeof(CultureTableData)) + 8) > fileSize)
                {
                    return false;
                }
                if (base.m_pDataFileStart.leOffset > fileSize)
                {
                    return false;
                }
                if ((base.m_pCultureHeader.offsetToCultureItemData + base.m_pCultureHeader.sizeCultureItem) > fileSize)
                {
                    return false;
                }
                if (base.m_pCultureHeader.cultureIDTableOffset > fileSize)
                {
                    return false;
                }
                if ((base.m_pCultureHeader.cultureNameTableOffset + 8) > fileSize)
                {
                    return false;
                }
                if (base.m_pCultureHeader.regionNameTableOffset > fileSize)
                {
                    return false;
                }
                if ((base.m_pCultureHeader.offsetToCalendarItemData + base.m_pCultureHeader.sizeCalendarItem) > fileSize)
                {
                    return false;
                }
                if (base.m_pCultureHeader.offsetToDataPool > fileSize)
                {
                    return false;
                }
                ushort* pDataPool = (ushort*) (base.m_pDataFileStart + base.m_pCultureHeader.offsetToDataPool);
                int poolSize = ((int) (((ulong) fileSize) - (((ulong) pDataPool) - ((ulong) base.m_pDataFileStart)))) / 2;
                if (poolSize <= 0)
                {
                    return false;
                }
                uint offsetInPool = base.m_pDataFileStart[base.m_pCultureHeader.cultureNameTableOffset];
                CultureTableData* dataPtr = (CultureTableData*) (base.m_pDataFileStart + base.m_pCultureHeader.offsetToCultureItemData);
                if ((dataPtr->iLanguage == 0x7f) || !IsValidLcid(dataPtr->iLanguage, true))
                {
                    return false;
                }
                string str = CheckAndGetTheString(pDataPool, dataPtr->sName, poolSize);
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                if ((offsetInPool != dataPtr->sName) && !str.Equals(CheckAndGetTheString(pDataPool, offsetInPool, poolSize)))
                {
                    return false;
                }
                string str2 = CheckAndGetTheString(pDataPool, dataPtr->sParent, poolSize);
                if ((str2 == null) || str2.Equals(str, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                if (!IsValidLcid(dataPtr->iTextInfo, false) || !IsValidLcid((int) dataPtr->iCompareInfo, false))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->waGrouping, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->waMonetaryGrouping, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sListSeparator, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sDecimalSeparator, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sThousandSeparator, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sCurrency, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sMonetaryDecimal, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sMonetaryThousand, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sPositiveSign, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sNegativeSign, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sAM1159, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sPM2359, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saNativeDigits, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saTimeFormat, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saShortDate, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saLongDate, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saYearMonth, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saDuration, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->waCalendars, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sAbbrevLang, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sISO639Language, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sEnglishLanguage, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sNativeLanguage, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sEnglishCountry, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sNativeCountry, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sAbbrevCountry, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sISO3166CountryName, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sIntlMonetarySymbol, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sEnglishCurrency, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sNativeCurrency, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->waFontSignature, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sISO639Language2, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sISO3166CountryName2, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saDayNames, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saAbbrevDayNames, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saMonthNames, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saAbbrevMonthNames, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saMonthGenitiveNames, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saAbbrevMonthGenitiveNames, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saNativeCalendarNames, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saAltSortID, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sEnglishDisplayName, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sNativeDisplayName, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sPercent, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sNaN, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sPositiveInfinity, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sNegativeInfinity, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sMonthDay, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sAdEra, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sAbbrevAdEra, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sRegionName, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sConsoleFallbackName, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saShortTime, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saSuperShortDayNames, poolSize))
                {
                    return false;
                }
                if (!ValidateStringArray(pDataPool, dataPtr->saDateWords, poolSize))
                {
                    return false;
                }
                if (!ValidateString(pDataPool, dataPtr->sSpecificCulture, poolSize))
                {
                    return false;
                }
            }
            return true;
        }

        private static unsafe bool ValidateString(ushort* pDataPool, uint offsetInPool, int poolSize)
        {
            if ((offsetInPool + 2) > poolSize)
            {
                return false;
            }
            char* chPtr = (char*) (pDataPool + offsetInPool);
            int num = chPtr[0];
            if (((offsetInPool + num) + ((ulong) 2L)) > poolSize)
            {
                return false;
            }
            return true;
        }

        private static unsafe bool ValidateStringArray(ushort* pDataPool, uint offsetInPool, int poolSize)
        {
            if (!ValidateUintArray(pDataPool, offsetInPool, poolSize))
            {
                return false;
            }
            ushort* numPtr = pDataPool + ((ushort*) offsetInPool);
            int num = numPtr[0];
            if (num != 0)
            {
                uint* numPtr2 = (uint*) (numPtr + 1);
                for (int i = 0; i < num; i++)
                {
                    if (!ValidateString(pDataPool, numPtr2[i], poolSize))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static unsafe bool ValidateUintArray(ushort* pDataPool, uint offsetInPool, int poolSize)
        {
            if (offsetInPool != 0)
            {
                if ((offsetInPool + 2) > poolSize)
                {
                    return false;
                }
                ushort* numPtr = pDataPool + ((ushort*) offsetInPool);
                if ((((int) numPtr) & 2) != 2)
                {
                    return false;
                }
                int num = numPtr[0];
                if (((offsetInPool + (num * 2)) + ((ulong) 2L)) > poolSize)
                {
                    return false;
                }
            }
            return true;
        }

        internal static CultureTable Default
        {
            get
            {
                if (m_defaultInstance == null)
                {
                    throw new TypeLoadException("Failure has occurred while loading a type.");
                }
                return m_defaultInstance;
            }
        }
    }
}

