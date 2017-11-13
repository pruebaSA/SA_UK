namespace System.Globalization
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    internal class CultureTableRecord
    {
        private static Hashtable CultureTableRecordCache;
        private static Hashtable CultureTableRecordRegionCache;
        private const int INT32TABLE_EVERETT_CULTURE_DATA_ITEM_MAPPINGS = 1;
        private const int INT32TABLE_EVERETT_DATA_ITEM_TO_LCID_MAPPINGS = 2;
        private const int INT32TABLE_EVERETT_REGION_DATA_ITEM_MAPPINGS = 0;
        private const int INT32TABLE_EVERETT_REGION_DATA_ITEM_TO_LCID_MAPPINGS = 3;
        private const int LOCALE_ICALENDARTYPE = 0x1009;
        private const int LOCALE_SCOUNTRY = 6;
        private const int LOCALE_SLANGUAGE = 2;
        private const int LOCALE_SNATIVECTRYNAME = 8;
        private const int LOCALE_SNATIVELANGNAME = 4;
        private int m_ActualCultureID;
        private string m_ActualName;
        private bool m_bUseUserOverride;
        private int m_CultureID;
        private string m_CultureName;
        private CultureTable m_CultureTable;
        private unsafe static int* m_EverettCultureDataItemMappings = null;
        private static int m_EverettCultureDataItemMappingsSize = 0;
        private unsafe static int* m_EverettDataItemToLCIDMappings = null;
        private static int m_EverettDataItemToLCIDMappingsSize = 0;
        private unsafe static int* m_EverettRegionDataItemMappings = null;
        private static int m_EverettRegionDataItemMappingsSize = 0;
        private unsafe static int* m_EverettRegionInfoDataItemToLCIDMappings = null;
        private static int m_EverettRegionInfoDataItemToLCIDMappingsSize = 0;
        private unsafe CultureTableData* m_pData;
        private unsafe ushort* m_pPool;
        private bool m_synthetic;
        private string m_windowsPath;
        private const int MAXSIZE_FULLTAGNAME = 0x54;
        private const int MAXSIZE_LANGUAGE = 8;
        private const int MAXSIZE_REGION = 8;
        private const int MAXSIZE_SUFFIX = 0x40;
        private AgileSafeNativeMemoryHandle nativeMemoryHandle;
        private static AdjustedSyntheticCultureName[] s_adjustedSyntheticNames = null;
        private static object s_InternalSyncObject;
        private const int SPANISH_INTERNATIONAL_SORT = 0xc0a;
        internal const int SPANISH_TRADITIONAL_SORT = 0x40a;
        private static Hashtable SyntheticDataCache;
        internal static Hashtable SyntheticLcidToNameCache;
        internal static Hashtable SyntheticNameToLcidCache;

        internal unsafe CultureTableRecord(int cultureId, bool useUserOverride)
        {
            this.m_bUseUserOverride = useUserOverride;
            int dataItemFromCultureID = CultureTable.Default.GetDataItemFromCultureID(cultureId, out this.m_ActualName);
            if (dataItemFromCultureID < 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_CultureNotSupported"), new object[] { cultureId }), "culture");
            }
            this.m_ActualCultureID = cultureId;
            this.m_CultureTable = CultureTable.Default;
            this.m_pData = (CultureTableData*) (this.m_CultureTable.m_pItemData + (this.m_CultureTable.m_itemSize * dataItemFromCultureID));
            this.m_pPool = this.m_CultureTable.m_pDataPool;
            this.m_CultureName = this.SNAME;
            this.m_CultureID = (cultureId == 0x40a) ? cultureId : this.ILANGUAGE;
        }

        private unsafe CultureTableRecord(string cultureName, bool useUserOverride)
        {
            int cultureID = 0;
            if (cultureName.Length == 0)
            {
                useUserOverride = false;
                cultureID = 0x7f;
            }
            this.m_bUseUserOverride = useUserOverride;
            int dataItemFromCultureID = -1;
            if (cultureName.Length > 0)
            {
                string str;
                int num3;
                string name = cultureName;
                int num4 = CultureTable.Default.GetDataItemFromCultureName(name, out num3, out str);
                if ((num4 >= 0) && ((CultureInfo.GetSortID(num3) > 0) || (num3 == 0x40a)))
                {
                    string str3;
                    int langID;
                    if (num3 == 0x40a)
                    {
                        langID = 0xc0a;
                    }
                    else
                    {
                        langID = CultureInfo.GetLangID(num3);
                    }
                    if (CultureTable.Default.GetDataItemFromCultureID(langID, out str3) >= 0)
                    {
                        name = ValidateCulturePieceToLower(str3, "cultureName", 0x54);
                    }
                }
                if (!Environment.GetCompatibilityFlag(CompatibilityFlag.DisableReplacementCustomCulture) || IsCustomCultureId(num3))
                {
                    this.m_CultureTable = this.GetCustomCultureTable(name);
                }
                if (this.m_CultureTable != null)
                {
                    dataItemFromCultureID = this.m_CultureTable.GetDataItemFromCultureName(name, out this.m_ActualCultureID, out this.m_ActualName);
                    if (num4 >= 0)
                    {
                        this.m_ActualCultureID = num3;
                        this.m_ActualName = str;
                    }
                }
                if ((dataItemFromCultureID < 0) && (num4 >= 0))
                {
                    this.m_CultureTable = CultureTable.Default;
                    this.m_ActualCultureID = num3;
                    this.m_ActualName = str;
                    dataItemFromCultureID = num4;
                }
                if (dataItemFromCultureID < 0)
                {
                    InitSyntheticMapping();
                    if (SyntheticNameToLcidCache[name] != null)
                    {
                        cultureID = (int) SyntheticNameToLcidCache[name];
                    }
                }
            }
            if ((dataItemFromCultureID < 0) && (cultureID > 0))
            {
                if (cultureID == 0x7f)
                {
                    dataItemFromCultureID = CultureTable.Default.GetDataItemFromCultureID(cultureID, out this.m_ActualName);
                    if (dataItemFromCultureID > 0)
                    {
                        this.m_ActualCultureID = cultureID;
                        this.m_CultureTable = CultureTable.Default;
                    }
                }
                else
                {
                    CultureTable table = null;
                    string actualName = null;
                    if (CultureInfo.GetSortID(cultureID) > 0)
                    {
                        dataItemFromCultureID = CultureTable.Default.GetDataItemFromCultureID(CultureInfo.GetLangID(cultureID), out actualName);
                    }
                    if (dataItemFromCultureID < 0)
                    {
                        actualName = (string) SyntheticLcidToNameCache[CultureInfo.GetLangID(cultureID)];
                    }
                    string str5 = (string) SyntheticLcidToNameCache[cultureID];
                    int dataItem = -1;
                    if (((str5 != null) && (actualName != null)) && !Environment.GetCompatibilityFlag(CompatibilityFlag.DisableReplacementCustomCulture))
                    {
                        table = this.TryCreateReplacementCulture(actualName, out dataItem);
                    }
                    if (table == null)
                    {
                        if (dataItemFromCultureID <= 0)
                        {
                            if (this.GetSyntheticCulture(cultureID))
                            {
                                return;
                            }
                        }
                        else
                        {
                            this.m_CultureTable = CultureTable.Default;
                            this.m_ActualCultureID = cultureID;
                            this.m_synthetic = true;
                            this.m_ActualName = CultureInfo.nativeGetCultureName(cultureID, true, false);
                        }
                    }
                    else
                    {
                        this.m_CultureTable = table;
                        dataItemFromCultureID = dataItem;
                        this.m_ActualName = CultureInfo.nativeGetCultureName(cultureID, true, false);
                        this.m_ActualCultureID = cultureID;
                    }
                }
            }
            if (dataItemFromCultureID >= 0)
            {
                this.m_pData = (CultureTableData*) (this.m_CultureTable.m_pItemData + (this.m_CultureTable.m_itemSize * dataItemFromCultureID));
                this.m_pPool = this.m_CultureTable.m_pDataPool;
                this.m_CultureName = this.SNAME;
                this.m_CultureID = (this.m_ActualCultureID == 0x40a) ? this.m_ActualCultureID : this.ILANGUAGE;
                this.CheckCustomSynthetic();
            }
            else
            {
                if (cultureName != null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidCultureName"), new object[] { cultureName }), "name");
                }
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_CultureNotSupported"), new object[] { cultureID }), "culture");
            }
        }

        private unsafe CultureTableRecord(string regionName, int dataItem, bool useUserOverride)
        {
            this.m_bUseUserOverride = useUserOverride;
            this.m_CultureName = regionName;
            this.m_CultureTable = CultureTable.Default;
            this.m_pData = (CultureTableData*) (this.m_CultureTable.m_pItemData + (this.m_CultureTable.m_itemSize * dataItem));
            this.m_pPool = this.m_CultureTable.m_pDataPool;
            this.m_CultureID = this.ILANGUAGE;
        }

        private static void AdjustSyntheticCalendars(ref CultureData data, ref CompositeCultureData compositeData)
        {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            ushort num2 = data.waCalendars[0];
            builder.Append((char) num2);
            for (int i = 1; i < data.waCalendars.Length; i++)
            {
                builder.Append((char) data.waCalendars[i]);
                if (data.waCalendars[i] == ((ushort) data.iDefaultCalender))
                {
                    num = i;
                }
                if (data.waCalendars[i] > num2)
                {
                    num2 = data.waCalendars[i];
                }
            }
            if (num2 > 1)
            {
                string[] strArray = new string[num2];
                for (int j = 0; j < strArray.Length; j++)
                {
                    strArray[j] = string.Empty;
                }
                for (int k = 0; k < data.waCalendars.Length; k++)
                {
                    strArray[data.waCalendars[k] - 1] = data.saNativeCalendarNames[k];
                }
                data.saNativeCalendarNames = strArray;
            }
            if (num > 0)
            {
                char ch = builder[num];
                builder[num] = builder[0];
                builder[0] = ch;
            }
            compositeData.waCalendars = builder.ToString();
        }

        internal static string AnsiToLower(string testString)
        {
            StringBuilder builder = new StringBuilder(testString.Length);
            for (int i = 0; i < testString.Length; i++)
            {
                char ch = testString[i];
                builder.Append(((ch <= 'Z') && (ch >= 'A')) ? ((char) ((ch - 'A') + 0x61)) : ch);
            }
            return builder.ToString();
        }

        internal static void CacheSyntheticNameLcidMapping()
        {
            Hashtable lcidToName = new Hashtable();
            Hashtable nameToLcid = new Hashtable();
            int[] localesArray = null;
            bool flag = false;
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(typeof(CultureTableRecord), ref tookLock);
                flag = CultureInfo.nativeEnumSystemLocales(out localesArray);
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(typeof(CultureTableRecord));
                }
            }
            if (flag && !GetCultureNamesUsingSNameLCType(localesArray, lcidToName, nameToLcid))
            {
                Hashtable namesHashtable = GetNamesHashtable();
                StringBuilder helper = new StringBuilder();
                for (int i = 0; i < localesArray.Length; i++)
                {
                    int lcid = localesArray[i];
                    if (!IsBuiltInCulture(lcid) && !IsCustomCultureId(lcid))
                    {
                        string sName;
                        AdjustedSyntheticCultureName name;
                        GetAdjustedNames(lcid, out name);
                        if (name != null)
                        {
                            sName = name.sName;
                        }
                        else
                        {
                            sName = CultureInfo.nativeGetCultureName(lcid, false, false);
                        }
                        if (sName != null)
                        {
                            string qualifiedName;
                            sName = ValidateCulturePieceToLower(sName, "cultureName", sName.Length);
                            if (namesHashtable[sName] != null)
                            {
                                if (GetScriptTag(lcid, out qualifiedName))
                                {
                                    qualifiedName = GetQualifiedName(Concatenate(helper, new string[] { sName, "-", qualifiedName }));
                                    nameToLcid[qualifiedName] = lcid;
                                    lcidToName[lcid] = qualifiedName;
                                }
                            }
                            else if (nameToLcid[sName] == null)
                            {
                                nameToLcid[sName] = lcid;
                                lcidToName[lcid] = sName;
                            }
                            else
                            {
                                int key = (int) nameToLcid[sName];
                                nameToLcid.Remove(sName);
                                lcidToName.Remove(key);
                                namesHashtable[sName] = "";
                                if (GetScriptTag(key, out qualifiedName))
                                {
                                    qualifiedName = GetQualifiedName(Concatenate(helper, new string[] { sName, "-", qualifiedName }));
                                    nameToLcid[qualifiedName] = key;
                                    lcidToName[key] = qualifiedName;
                                }
                                if (GetScriptTag(lcid, out qualifiedName))
                                {
                                    qualifiedName = GetQualifiedName(Concatenate(helper, new string[] { sName, "-", qualifiedName }));
                                    nameToLcid[qualifiedName] = lcid;
                                    lcidToName[lcid] = qualifiedName;
                                }
                            }
                        }
                    }
                }
            }
            lock (InternalSyncObject)
            {
                SyntheticLcidToNameCache = lcidToName;
                SyntheticNameToLcidCache = nameToLcid;
            }
        }

        private void CheckCustomSynthetic()
        {
            if (this.IsCustomCulture)
            {
                InitSyntheticMapping();
                if (IsCustomCultureId(this.m_CultureID))
                {
                    string str = ValidateCulturePieceToLower(this.m_CultureName, "CultureName", 0x54);
                    if (SyntheticNameToLcidCache[str] != null)
                    {
                        this.m_synthetic = true;
                        this.m_ActualCultureID = this.m_CultureID = (int) SyntheticNameToLcidCache[str];
                    }
                }
                else if (SyntheticLcidToNameCache[this.m_CultureID] != null)
                {
                    this.m_synthetic = true;
                    this.m_ActualCultureID = this.m_CultureID;
                }
                else if ((this.m_CultureID != this.m_ActualCultureID) && (SyntheticLcidToNameCache[this.m_ActualCultureID] != null))
                {
                    this.m_synthetic = true;
                }
            }
        }

        internal CultureTableRecord CloneWithUserOverride(bool userOverride)
        {
            if (this.m_bUseUserOverride == userOverride)
            {
                return this;
            }
            CultureTableRecord record = (CultureTableRecord) base.MemberwiseClone();
            record.m_bUseUserOverride = userOverride;
            return record;
        }

        internal static string Concatenate(StringBuilder helper, params string[] stringsToConcat)
        {
            if (helper.Length > 0)
            {
                helper.Remove(0, helper.Length);
            }
            for (int i = 0; i < stringsToConcat.Length; i++)
            {
                helper.Append(stringsToConcat[i]);
            }
            return helper.ToString();
        }

        private ushort ConvertFirstDayOfWeekMonToSun(int iTemp)
        {
            if ((iTemp < 0) || (iTemp > 6))
            {
                iTemp = 1;
            }
            else if (iTemp == 6)
            {
                iTemp = 0;
            }
            else
            {
                iTemp++;
            }
            return (ushort) iTemp;
        }

        private static int[] ConvertWin32GroupString(string win32Str)
        {
            int[] numArray;
            if (((win32Str == null) || (win32Str.Length == 0)) || (win32Str[0] == '0'))
            {
                return new int[] { 3 };
            }
            if (win32Str[win32Str.Length - 1] == '0')
            {
                numArray = new int[win32Str.Length / 2];
            }
            else
            {
                numArray = new int[(win32Str.Length / 2) + 2];
                numArray[numArray.Length - 1] = 0;
            }
            int num = 0;
            for (int i = 0; (num < win32Str.Length) && (i < numArray.Length); i++)
            {
                if ((win32Str[num] < '1') || (win32Str[num] > '9'))
                {
                    return new int[] { 3 };
                }
                numArray[i] = win32Str[num] - '0';
                num += 2;
            }
            return numArray;
        }

        public override unsafe bool Equals(object value)
        {
            CultureTableRecord record = value as CultureTableRecord;
            if (record == null)
            {
                return false;
            }
            return ((((this.m_pData == record.m_pData) && (this.m_bUseUserOverride == record.m_bUseUserOverride)) && ((this.m_CultureID == record.m_CultureID) && (CultureInfo.InvariantCulture.CompareInfo.Compare(this.m_CultureName, record.m_CultureName, CompareOptions.IgnoreCase) == 0))) && this.m_CultureTable.Equals(record.m_CultureTable));
        }

        internal unsafe int EverettDataItem()
        {
            if (!this.IsCustomCulture)
            {
                InitEverettCultureDataItemMapping();
                int num = 0;
                int num2 = (m_EverettCultureDataItemMappingsSize / 2) - 1;
                while (num <= num2)
                {
                    int num3 = (num + num2) / 2;
                    int num4 = this.m_CultureID - m_EverettCultureDataItemMappings[num3 * 2];
                    if (num4 == 0)
                    {
                        return m_EverettCultureDataItemMappings[(num3 * 2) + 1];
                    }
                    if (num4 < 0)
                    {
                        num2 = num3 - 1;
                    }
                    else
                    {
                        num = num3 + 1;
                    }
                }
            }
            return 0;
        }

        internal unsafe int EverettRegionDataItem()
        {
            if (!this.IsCustomCulture)
            {
                InitEverettRegionDataItemMapping();
                int num = 0;
                int num2 = (m_EverettRegionDataItemMappingsSize / 2) - 1;
                while (num <= num2)
                {
                    int num3 = (num + num2) / 2;
                    int num4 = this.m_CultureID - m_EverettRegionDataItemMappings[num3 * 2];
                    if (num4 == 0)
                    {
                        return m_EverettRegionDataItemMappings[(num3 * 2) + 1];
                    }
                    if (num4 < 0)
                    {
                        num2 = num3 - 1;
                    }
                    else
                    {
                        num = num3 + 1;
                    }
                }
            }
            return 0;
        }

        private unsafe uint FillCultureDataMemory(int cultureID, ref CultureData data, ref CompositeCultureData compositeData)
        {
            uint index = 0;
            Hashtable offsetTable = new Hashtable(30);
            this.m_pPool[index] = 0;
            index++;
            this.SetPoolString("", offsetTable, ref index);
            offsetTable[""] = 0;
            this.m_pData.iLanguage = (ushort) cultureID;
            this.m_pData.sName = (ushort) this.SetPoolString(compositeData.sname, offsetTable, ref index);
            this.m_pData.iDigits = (ushort) data.iDigits;
            this.m_pData.iNegativeNumber = (ushort) data.iNegativeNumber;
            this.m_pData.iCurrencyDigits = (ushort) data.iCurrencyDigits;
            this.m_pData.iCurrency = (ushort) data.iCurrency;
            this.m_pData.iNegativeCurrency = (ushort) data.iNegativeCurrency;
            this.m_pData.iLeadingZeros = (ushort) data.iLeadingZeros;
            this.m_pData.iFlags = 1;
            this.m_pData.iFirstDayOfWeek = this.ConvertFirstDayOfWeekMonToSun(data.iFirstDayOfWeek);
            this.m_pData.iFirstWeekOfYear = (ushort) data.iFirstWeekOfYear;
            this.m_pData.iCountry = (ushort) data.iCountry;
            this.m_pData.iMeasure = (ushort) data.iMeasure;
            this.m_pData.iDigitSubstitution = (ushort) data.iDigitSubstitution;
            this.m_pData.waGrouping = (ushort) this.SetPoolString(data.waGrouping, offsetTable, ref index);
            this.m_pData.waMonetaryGrouping = (ushort) this.SetPoolString(data.waMonetaryGrouping, offsetTable, ref index);
            this.m_pData.sListSeparator = (ushort) this.SetPoolString(data.sListSeparator, offsetTable, ref index);
            this.m_pData.sDecimalSeparator = (ushort) this.SetPoolString(data.sDecimalSeparator, offsetTable, ref index);
            this.m_pData.sThousandSeparator = (ushort) this.SetPoolString(data.sThousandSeparator, offsetTable, ref index);
            this.m_pData.sCurrency = (ushort) this.SetPoolString(data.sCurrency, offsetTable, ref index);
            this.m_pData.sMonetaryDecimal = (ushort) this.SetPoolString(data.sMonetaryDecimal, offsetTable, ref index);
            this.m_pData.sMonetaryThousand = (ushort) this.SetPoolString(data.sMonetaryThousand, offsetTable, ref index);
            this.m_pData.sPositiveSign = (ushort) this.SetPoolString(data.sPositiveSign, offsetTable, ref index);
            this.m_pData.sNegativeSign = (ushort) this.SetPoolString(data.sNegativeSign, offsetTable, ref index);
            this.m_pData.sAM1159 = (ushort) this.SetPoolString(data.sAM1159, offsetTable, ref index);
            this.m_pData.sPM2359 = (ushort) this.SetPoolString(data.sPM2359, offsetTable, ref index);
            this.m_pData.saNativeDigits = (ushort) this.SetPoolStringArrayFromSingleString(data.saNativeDigits, offsetTable, ref index);
            this.m_pData.saTimeFormat = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saTimeFormat);
            this.m_pData.saShortDate = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saShortDate);
            this.m_pData.saLongDate = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saLongDate);
            this.m_pData.saYearMonth = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saYearMonth);
            this.m_pData.saDuration = (ushort) this.SetPoolStringArray(offsetTable, ref index, new string[] { "" });
            this.m_pData.iDefaultLanguage = this.m_pData.iLanguage;
            this.m_pData.iDefaultAnsiCodePage = (ushort) data.iDefaultAnsiCodePage;
            this.m_pData.iDefaultOemCodePage = (ushort) data.iDefaultOemCodePage;
            this.m_pData.iDefaultMacCodePage = (ushort) data.iDefaultMacCodePage;
            this.m_pData.iDefaultEbcdicCodePage = (ushort) data.iDefaultEbcdicCodePage;
            this.m_pData.iGeoId = (ushort) data.iGeoId;
            this.m_pData.iPaperSize = (ushort) data.iPaperSize;
            this.m_pData.iIntlCurrencyDigits = (ushort) data.iIntlCurrencyDigits;
            this.m_pData.iParent = (ushort) compositeData.parentLcid;
            this.m_pData.waCalendars = (ushort) this.SetPoolString(compositeData.waCalendars, offsetTable, ref index);
            this.m_pData.sAbbrevLang = (ushort) this.SetPoolString(data.sAbbrevLang, offsetTable, ref index);
            this.m_pData.sISO639Language = (ushort) this.SetPoolString(data.sIso639Language, offsetTable, ref index);
            this.m_pData.sEnglishLanguage = (ushort) this.SetPoolString(data.sEnglishLanguage, offsetTable, ref index);
            this.m_pData.sNativeLanguage = (ushort) this.SetPoolString(data.sNativeLanguage, offsetTable, ref index);
            this.m_pData.sEnglishCountry = (ushort) this.SetPoolString(data.sEnglishCountry, offsetTable, ref index);
            this.m_pData.sNativeCountry = (ushort) this.SetPoolString(data.sNativeCountry, offsetTable, ref index);
            this.m_pData.sAbbrevCountry = (ushort) this.SetPoolString(data.sAbbrevCountry, offsetTable, ref index);
            this.m_pData.sISO3166CountryName = (ushort) this.SetPoolString(data.sIso3166CountryName, offsetTable, ref index);
            this.m_pData.sIntlMonetarySymbol = (ushort) this.SetPoolString(data.sIntlMonetarySymbol, offsetTable, ref index);
            this.m_pData.sEnglishCurrency = (ushort) this.SetPoolString(data.sEnglishCurrency, offsetTable, ref index);
            this.m_pData.sNativeCurrency = (ushort) this.SetPoolString(data.sNativeCurrency, offsetTable, ref index);
            this.m_pData.waFontSignature = (ushort) this.SetPoolString(data.waFontSignature, offsetTable, ref index);
            this.m_pData.sISO639Language2 = (ushort) this.SetPoolString(data.sISO639Language2, offsetTable, ref index);
            this.m_pData.sISO3166CountryName2 = (ushort) this.SetPoolString(data.sISO3166CountryName2, offsetTable, ref index);
            this.m_pData.sParent = (ushort) this.SetPoolString(compositeData.parentName, offsetTable, ref index);
            this.m_pData.saDayNames = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saDayNames);
            this.m_pData.saAbbrevDayNames = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saAbbrevDayNames);
            this.m_pData.saMonthNames = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saMonthNames);
            this.m_pData.saAbbrevMonthNames = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saAbbrevMonthNames);
            this.m_pData.saMonthGenitiveNames = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saGenitiveMonthNames);
            this.m_pData.saAbbrevMonthGenitiveNames = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saAbbrevGenitiveMonthNames);
            this.m_pData.saNativeCalendarNames = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saNativeCalendarNames);
            this.m_pData.saAltSortID = (ushort) this.SetPoolStringArray(offsetTable, ref index, new string[] { "" });
            this.m_pData.iNegativePercent = (ushort) CultureInfo.InvariantCulture.NumberFormat.PercentNegativePattern;
            this.m_pData.iPositivePercent = (ushort) CultureInfo.InvariantCulture.NumberFormat.PercentPositivePattern;
            this.m_pData.iFormatFlags = 0;
            this.m_pData.iLineOrientations = 0;
            this.m_pData.iTextInfo = this.m_pData.iLanguage;
            this.m_pData.iInputLanguageHandle = this.m_pData.iLanguage;
            this.m_pData.iCompareInfo = this.m_pData.iLanguage;
            this.m_pData.sEnglishDisplayName = (ushort) this.SetPoolString(compositeData.englishDisplayName, offsetTable, ref index);
            this.m_pData.sNativeDisplayName = (ushort) this.SetPoolString(compositeData.sNativeDisplayName, offsetTable, ref index);
            this.m_pData.sPercent = (ushort) this.SetPoolString(CultureInfo.InvariantCulture.NumberFormat.PercentSymbol, offsetTable, ref index);
            this.m_pData.sNaN = (ushort) this.SetPoolString(data.sNaN, offsetTable, ref index);
            this.m_pData.sPositiveInfinity = (ushort) this.SetPoolString(data.sPositiveInfinity, offsetTable, ref index);
            this.m_pData.sNegativeInfinity = (ushort) this.SetPoolString(data.sNegativeInfinity, offsetTable, ref index);
            this.m_pData.sMonthDay = (ushort) this.SetPoolString(CultureInfo.InvariantCulture.DateTimeFormat.MonthDayPattern, offsetTable, ref index);
            this.m_pData.sAdEra = (ushort) this.SetPoolString(CultureInfo.InvariantCulture.DateTimeFormat.GetEraName(0), offsetTable, ref index);
            this.m_pData.sAbbrevAdEra = (ushort) this.SetPoolString(CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedEraName(0), offsetTable, ref index);
            this.m_pData.sRegionName = this.m_pData.sISO3166CountryName;
            this.m_pData.sConsoleFallbackName = (ushort) this.SetPoolString(compositeData.consoleFallbackName, offsetTable, ref index);
            this.m_pData.saShortTime = this.m_pData.saTimeFormat;
            this.m_pData.saSuperShortDayNames = (ushort) this.SetPoolStringArray(offsetTable, ref index, data.saSuperShortDayNames);
            this.m_pData.saDateWords = this.m_pData.saDuration;
            this.m_pData.sSpecificCulture = this.m_pData.sName;
            this.m_pData.sScripts = 0;
            return (2 * index);
        }

        internal static void GetAdjustedNames(int lcid, out AdjustedSyntheticCultureName adjustedNames)
        {
            for (int i = 0; i < AdjustedSyntheticNames.Length; i++)
            {
                if (AdjustedSyntheticNames[i].lcid == lcid)
                {
                    adjustedNames = AdjustedSyntheticNames[i];
                    return;
                }
            }
            adjustedNames = null;
        }

        private static string GetCasedName(string name)
        {
            StringBuilder builder = new StringBuilder(name.Length);
            int num = 0;
            while ((num < name.Length) && (name[num] != '-'))
            {
                builder.Append(name[num]);
                num++;
            }
            builder.Append("-");
            num++;
            char ch = char.ToUpper(name[num], CultureInfo.InvariantCulture);
            builder.Append(ch);
            num++;
            while ((num < name.Length) && (name[num] != '-'))
            {
                builder.Append(name[num]);
                num++;
            }
            builder.Append("-");
            num++;
            while (num < name.Length)
            {
                ch = char.ToUpper(name[num], CultureInfo.InvariantCulture);
                builder.Append(ch);
                num++;
            }
            return builder.ToString();
        }

        private int GetCultureDataSize(int cultureID, ref CultureData data, ref CompositeCultureData compositeData)
        {
            int num = sizeof(CultureTableData);
            Hashtable offsetTable = new Hashtable(30);
            num += 2;
            num += this.GetPoolStringSize("", offsetTable);
            compositeData.sname = CultureInfo.nativeGetCultureName(cultureID, true, false);
            if (compositeData.sname == null)
            {
                AdjustedSyntheticCultureName name;
                GetAdjustedNames(cultureID, out name);
                if (name != null)
                {
                    data.sIso639Language = name.isoLanguage;
                    data.sIso3166CountryName = name.isoCountry;
                    compositeData.sname = name.sName;
                }
                else
                {
                    string tempName = (string) SyntheticLcidToNameCache[cultureID];
                    if (this.NameHasScriptTag(tempName))
                    {
                        compositeData.sname = GetCasedName(tempName);
                    }
                    else
                    {
                        compositeData.sname = data.sIso639Language + "-" + data.sIso3166CountryName;
                    }
                }
            }
            compositeData.englishDisplayName = data.sEnglishLanguage + " (" + data.sEnglishCountry + ")";
            compositeData.sNativeDisplayName = data.sNativeLanguage + " (" + data.sNativeCountry + ")";
            AdjustSyntheticCalendars(ref data, ref compositeData);
            num += this.GetPoolStringSize(compositeData.sname, offsetTable);
            num += this.GetPoolStringSize(compositeData.englishDisplayName, offsetTable);
            num += this.GetPoolStringSize(compositeData.sNativeDisplayName, offsetTable);
            num += this.GetPoolStringSize(compositeData.waCalendars, offsetTable);
            GetSyntheticParentData(ref data, ref compositeData);
            num += this.GetPoolStringSize(compositeData.parentName, offsetTable);
            num += this.GetPoolStringSize(data.sIso639Language, offsetTable);
            num += this.GetPoolStringSize(data.sListSeparator, offsetTable);
            num += this.GetPoolStringSize(data.sDecimalSeparator, offsetTable);
            num += this.GetPoolStringSize(data.sThousandSeparator, offsetTable);
            num += this.GetPoolStringSize(data.sCurrency, offsetTable);
            num += this.GetPoolStringSize(data.sMonetaryDecimal, offsetTable);
            num += this.GetPoolStringSize(data.sMonetaryThousand, offsetTable);
            num += this.GetPoolStringSize(data.sPositiveSign, offsetTable);
            num += this.GetPoolStringSize(data.sNegativeSign, offsetTable);
            num += this.GetPoolStringSize(data.sAM1159, offsetTable);
            num += this.GetPoolStringSize(data.sPM2359, offsetTable);
            num += this.GetPoolStringSize(data.sAbbrevLang, offsetTable);
            num += this.GetPoolStringSize(data.sEnglishLanguage, offsetTable);
            num += this.GetPoolStringSize(data.sNativeLanguage, offsetTable);
            num += this.GetPoolStringSize(data.sEnglishCountry, offsetTable);
            num += this.GetPoolStringSize(data.sNativeCountry, offsetTable);
            num += this.GetPoolStringSize(data.sAbbrevCountry, offsetTable);
            num += this.GetPoolStringSize(data.sIso3166CountryName, offsetTable);
            num += this.GetPoolStringSize(data.sIntlMonetarySymbol, offsetTable);
            num += this.GetPoolStringSize(data.sEnglishCurrency, offsetTable);
            num += this.GetPoolStringSize(data.sNativeCurrency, offsetTable);
            num += this.GetPoolStringSize(CultureInfo.InvariantCulture.NumberFormat.PercentSymbol, offsetTable);
            if (data.sNaN == null)
            {
                data.sNaN = CultureInfo.InvariantCulture.NumberFormat.NaNSymbol;
            }
            num += this.GetPoolStringSize(data.sNaN, offsetTable);
            if (data.sPositiveInfinity == null)
            {
                data.sPositiveInfinity = CultureInfo.InvariantCulture.NumberFormat.PositiveInfinitySymbol;
            }
            num += this.GetPoolStringSize(data.sPositiveInfinity, offsetTable);
            if (data.sNegativeInfinity == null)
            {
                data.sNegativeInfinity = CultureInfo.InvariantCulture.NumberFormat.NegativeInfinitySymbol;
            }
            num += this.GetPoolStringSize(data.sNegativeInfinity, offsetTable);
            num += this.GetPoolStringSize(CultureInfo.InvariantCulture.DateTimeFormat.MonthDayPattern, offsetTable);
            num += this.GetPoolStringSize(CultureInfo.InvariantCulture.DateTimeFormat.GetEraName(0), offsetTable);
            num += this.GetPoolStringSize(CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedEraName(0), offsetTable);
            GetSyntheticConsoleFallback(ref data, ref compositeData);
            num += this.GetPoolStringSize(compositeData.consoleFallbackName, offsetTable);
            num += this.GetPoolStringArraySize(offsetTable, data.saMonthNames);
            num += this.GetPoolStringArraySize(offsetTable, data.saDayNames);
            num += this.GetPoolStringArraySize(offsetTable, data.saAbbrevDayNames);
            num += this.GetPoolStringArraySize(offsetTable, data.saAbbrevMonthNames);
            data.saGenitiveMonthNames[12] = data.saMonthNames[12];
            num += this.GetPoolStringArraySize(offsetTable, data.saGenitiveMonthNames);
            data.saAbbrevGenitiveMonthNames[12] = data.saAbbrevMonthNames[12];
            num += this.GetPoolStringArraySize(offsetTable, data.saAbbrevGenitiveMonthNames);
            num += this.GetPoolStringArraySize(offsetTable, data.saNativeCalendarNames);
            num += this.GetPoolStringArraySize(offsetTable, data.saTimeFormat);
            num += this.GetPoolStringArraySize(offsetTable, data.saShortDate);
            num += this.GetPoolStringArraySize(offsetTable, data.saLongDate);
            num += this.GetPoolStringArraySize(offsetTable, data.saYearMonth);
            num += this.GetPoolStringArraySize(offsetTable, new string[] { "" });
            num += this.GetPoolStringArraySize(offsetTable, new string[] { "" });
            data.waGrouping = this.GroupSizesConstruction(data.waGrouping);
            num += this.GetPoolStringSize(data.waGrouping, offsetTable);
            data.waMonetaryGrouping = this.GroupSizesConstruction(data.waMonetaryGrouping);
            num += this.GetPoolStringSize(data.waMonetaryGrouping, offsetTable);
            num += this.GetPoolStringArraySize(data.saNativeDigits, offsetTable);
            num += this.GetPoolStringSize(data.waFontSignature, offsetTable);
            if (data.sISO3166CountryName2 == null)
            {
                data.sISO3166CountryName2 = data.sIso3166CountryName;
            }
            num += this.GetPoolStringSize(data.sISO3166CountryName2, offsetTable);
            if (data.sISO639Language2 == null)
            {
                data.sISO639Language2 = data.sIso639Language;
            }
            num += this.GetPoolStringSize(data.sISO639Language2, offsetTable);
            if (data.saSuperShortDayNames == null)
            {
                data.saSuperShortDayNames = data.saAbbrevDayNames;
            }
            return (num + this.GetPoolStringArraySize(offsetTable, data.saSuperShortDayNames));
        }

        internal static bool GetCultureNamesUsingSNameLCType(int[] lcidArray, Hashtable lcidToName, Hashtable nameToLcid)
        {
            string testString = CultureInfo.nativeGetCultureName(lcidArray[0], true, false);
            if (testString == null)
            {
                return false;
            }
            if (!IsBuiltInCulture(lcidArray[0]) && !IsCustomCultureId(lcidArray[0]))
            {
                testString = ValidateCulturePieceToLower(testString, "cultureName", testString.Length);
                nameToLcid[testString] = lcidArray[0];
                lcidToName[lcidArray[0]] = testString;
            }
            for (int i = 1; i < lcidArray.Length; i++)
            {
                if (!IsBuiltInCulture(lcidArray[i]) || IsCustomCultureId(lcidArray[0]))
                {
                    testString = CultureInfo.nativeGetCultureName(lcidArray[i], true, false);
                    if (testString != null)
                    {
                        testString = ValidateCulturePieceToLower(testString, "cultureName", testString.Length);
                        nameToLcid[testString] = lcidArray[i];
                        lcidToName[lcidArray[i]] = testString;
                    }
                }
            }
            return true;
        }

        internal static CultureTableRecord GetCultureTableRecord(int cultureId, bool useUserOverride)
        {
            if (cultureId == 0x7f)
            {
                return GetCultureTableRecord("", false);
            }
            string actualName = null;
            if ((CultureTable.Default.GetDataItemFromCultureID(cultureId, out actualName) < 0) && CultureInfo.IsValidLCID(cultureId, 1))
            {
                InitSyntheticMapping();
                actualName = (string) SyntheticLcidToNameCache[cultureId];
            }
            if ((actualName == null) || (actualName.Length <= 0))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_CultureNotSupported"), new object[] { cultureId }), "culture");
            }
            return GetCultureTableRecord(actualName, useUserOverride);
        }

        internal static CultureTableRecord GetCultureTableRecord(string name, bool useUserOverride)
        {
            if (CultureTableRecordCache == null)
            {
                if (name.Length == 0)
                {
                    return new CultureTableRecord(name, useUserOverride);
                }
                lock (InternalSyncObject)
                {
                    if (CultureTableRecordCache == null)
                    {
                        CultureTableRecordCache = new Hashtable();
                    }
                }
            }
            name = ValidateCulturePieceToLower(name, "name", 0x54);
            CultureTableRecord[] recordArray = (CultureTableRecord[]) CultureTableRecordCache[name];
            if (recordArray != null)
            {
                int index = useUserOverride ? 0 : 1;
                if (recordArray[index] == null)
                {
                    int num2 = (index == 0) ? 1 : 0;
                    recordArray[index] = recordArray[num2].CloneWithUserOverride(useUserOverride);
                }
                return recordArray[index];
            }
            CultureTableRecord record = new CultureTableRecord(name, useUserOverride);
            lock (InternalSyncObject)
            {
                if (CultureTableRecordCache[name] == null)
                {
                    recordArray = new CultureTableRecord[2];
                    recordArray[useUserOverride ? 0 : 1] = record;
                    CultureTableRecordCache[name] = recordArray;
                }
            }
            return record;
        }

        internal static CultureTableRecord GetCultureTableRecordForRegion(string regionName, bool useUserOverride)
        {
            if (CultureTableRecordRegionCache == null)
            {
                lock (InternalSyncObject)
                {
                    if (CultureTableRecordRegionCache == null)
                    {
                        CultureTableRecordRegionCache = new Hashtable();
                    }
                }
            }
            regionName = ValidateCulturePieceToLower(regionName, "regionName", 0x54);
            CultureTableRecord[] recordArray = (CultureTableRecord[]) CultureTableRecordRegionCache[regionName];
            if (recordArray != null)
            {
                int index = useUserOverride ? 0 : 1;
                if (recordArray[index] == null)
                {
                    recordArray[index] = recordArray[(index == 0) ? 1 : 0].CloneWithUserOverride(useUserOverride);
                }
                return recordArray[index];
            }
            int dataItemFromRegionName = CultureTable.Default.GetDataItemFromRegionName(regionName);
            CultureTableRecord cultureTableRecord = null;
            if (dataItemFromRegionName > 0)
            {
                cultureTableRecord = new CultureTableRecord(regionName, dataItemFromRegionName, useUserOverride);
            }
            else
            {
                try
                {
                    cultureTableRecord = GetCultureTableRecord(regionName, useUserOverride);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidRegionName"), new object[] { regionName }), "name");
                }
            }
            lock (InternalSyncObject)
            {
                if (CultureTableRecordRegionCache[regionName] == null)
                {
                    recordArray = new CultureTableRecord[2];
                    recordArray[useUserOverride ? 0 : 1] = cultureTableRecord.CloneWithUserOverride(useUserOverride);
                    CultureTableRecordRegionCache[regionName] = recordArray;
                }
            }
            return cultureTableRecord;
        }

        private string GetCustomCultureFile(string name)
        {
            StringBuilder builder = new StringBuilder(this.WindowsPath);
            builder.Append(@"\Globalization\");
            builder.Append(name);
            builder.Append(".nlp");
            string fileName = builder.ToString();
            if (CultureInfo.nativeFileExists(fileName))
            {
                return fileName;
            }
            return null;
        }

        private CultureTable GetCustomCultureTable(string name)
        {
            CultureTable table = null;
            string customCultureFile = this.GetCustomCultureFile(name);
            if (customCultureFile == null)
            {
                return null;
            }
            try
            {
                string str2;
                int num;
                table = new CultureTable(customCultureFile, false);
                if (table.IsValid)
                {
                    return table;
                }
                if (CultureTable.Default.GetDataItemFromCultureName(name, out num, out str2) < 0)
                {
                    InitSyntheticMapping();
                    if (SyntheticNameToLcidCache[name] == null)
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_CorruptedCustomCultureFile"), new object[] { name }));
                    }
                }
                return null;
            }
            catch (FileNotFoundException)
            {
                table = null;
            }
            return table;
        }

        private static string GetDateSeparator(string format)
        {
            string str = string.Empty;
            int num = 0;
            int start = -1;
            for (num = 0; num < format.Length; num++)
            {
                if (((format[num] == 'd') || (format[num] == 'y')) || (format[num] == 'M'))
                {
                    char ch = format[num];
                    num++;
                    while ((num < format.Length) && (format[num] == ch))
                    {
                        num++;
                    }
                    if (num < format.Length)
                    {
                        start = num;
                    }
                    break;
                }
                if (format[num] == '\'')
                {
                    num++;
                    while ((num < format.Length) && (format[num] != '\''))
                    {
                        num++;
                    }
                }
            }
            if (start != -1)
            {
                for (num = start; num < format.Length; num++)
                {
                    if (((format[num] == 'y') || (format[num] == 'M')) || (format[num] == 'd'))
                    {
                        return UnescapeWin32String(format, start, num - 1);
                    }
                    if (format[num] == '\'')
                    {
                        num++;
                        while ((num < format.Length) && (format[num] != '\''))
                        {
                            num++;
                        }
                    }
                }
            }
            return str;
        }

        internal void GetDTFIOverrideValues(ref DTFIUserOverrideValues values)
        {
            bool flag = false;
            if (this.UseGetLocaleInfo)
            {
                flag = CultureInfo.nativeGetDTFIUserValues(this.InteropLCID, ref values);
            }
            if (flag)
            {
                values.firstDayOfWeek = this.ConvertFirstDayOfWeekMonToSun(values.firstDayOfWeek);
                values.shortDatePattern = ReescapeWin32String(values.shortDatePattern);
                values.longDatePattern = ReescapeWin32String(values.longDatePattern);
                values.longTimePattern = ReescapeWin32String(values.longTimePattern);
                values.yearMonthPattern = ReescapeWin32String(values.yearMonthPattern);
            }
            else
            {
                values.firstDayOfWeek = this.IFIRSTDAYOFWEEK;
                values.calendarWeekRule = this.IFIRSTWEEKOFYEAR;
                values.shortDatePattern = this.SSHORTDATE;
                values.longDatePattern = this.SLONGDATE;
                values.yearMonthPattern = this.SYEARMONTH;
                values.amDesignator = this.S1159;
                values.pmDesignator = this.S2359;
                values.longTimePattern = this.STIMEFORMAT;
            }
        }

        public override int GetHashCode()
        {
            if (!IsCustomCultureId(this.m_CultureID))
            {
                return this.m_CultureID;
            }
            return this.m_CultureName.GetHashCode();
        }

        internal static Hashtable GetNamesHashtable() => 
            new Hashtable { 
                ["bs-ba"] = "",
                ["tg-tj"] = "",
                ["mn-cn"] = "",
                ["iu-ca"] = ""
            };

        internal void GetNFIOverrideValues(NumberFormatInfo nfi)
        {
            bool flag = false;
            if (this.UseGetLocaleInfo)
            {
                flag = CultureInfo.nativeGetNFIUserValues(this.InteropLCID, nfi);
            }
            if (!flag)
            {
                nfi.numberDecimalDigits = this.IDIGITS;
                nfi.numberNegativePattern = this.INEGNUMBER;
                nfi.currencyDecimalDigits = this.ICURRDIGITS;
                nfi.currencyPositivePattern = this.ICURRENCY;
                nfi.currencyNegativePattern = this.INEGCURR;
                nfi.negativeSign = this.SNEGATIVESIGN;
                nfi.numberDecimalSeparator = this.SDECIMAL;
                nfi.numberGroupSeparator = this.STHOUSAND;
                nfi.positiveSign = this.SPOSITIVESIGN;
                nfi.currencyDecimalSeparator = this.SMONDECIMALSEP;
                nfi.currencySymbol = this.SCURRENCY;
                nfi.currencyGroupSeparator = this.SMONTHOUSANDSEP;
                nfi.nativeDigits = this.SNATIVEDIGITS;
                nfi.digitSubstitution = this.IDIGITSUBSTITUTION;
            }
            else if (-1 == nfi.digitSubstitution)
            {
                nfi.digitSubstitution = this.IDIGITSUBSTITUTION;
            }
            nfi.numberGroupSizes = this.SGROUPING;
            nfi.currencyGroupSizes = this.SMONGROUPING;
            nfi.percentDecimalDigits = nfi.numberDecimalDigits;
            nfi.percentDecimalSeparator = nfi.numberDecimalSeparator;
            nfi.percentGroupSizes = nfi.numberGroupSizes;
            nfi.percentGroupSeparator = nfi.numberGroupSeparator;
            nfi.percentNegativePattern = this.INEGATIVEPERCENT;
            nfi.percentPositivePattern = this.IPOSITIVEPERCENT;
            nfi.percentSymbol = this.SPERCENT;
            if ((nfi.positiveSign == null) || (nfi.positiveSign.Length == 0))
            {
                nfi.positiveSign = "+";
            }
            if (nfi.currencyDecimalSeparator.Length == 0)
            {
                nfi.currencyDecimalSeparator = this.SMONDECIMALSEP;
            }
        }

        private int[] GetOverrideGrouping(uint iData, int iWindowsFlag)
        {
            if (this.UseGetLocaleInfo)
            {
                string str = CultureInfo.nativeGetLocaleInfo(this.InteropLCID, iWindowsFlag);
                if ((str != null) && (str.Length > 0))
                {
                    int[] numArray = ConvertWin32GroupString(str);
                    if (numArray != null)
                    {
                        return numArray;
                    }
                }
            }
            return this.GetWordArray(iData);
        }

        private string GetOverrideString(uint iOffset, int iWindowsFlag)
        {
            if (this.UseGetLocaleInfo)
            {
                string str = CultureInfo.nativeGetLocaleInfo(this.InteropLCID, iWindowsFlag);
                if ((str != null) && (str.Length > 0))
                {
                    return str;
                }
            }
            return this.GetString(iOffset);
        }

        private string GetOverrideStringArrayDefault(uint iOffset, int iWindowsFlag)
        {
            if (this.UseGetLocaleInfo)
            {
                string str = CultureInfo.nativeGetLocaleInfo(this.InteropLCID, iWindowsFlag);
                if ((str != null) && (str.Length > 0))
                {
                    return str;
                }
            }
            return this.GetStringArrayDefault(iOffset);
        }

        private ushort GetOverrideUSHORT(ushort iData, int iWindowsFlag)
        {
            if (this.UseGetLocaleInfo)
            {
                short num;
                string s = CultureInfo.nativeGetLocaleInfo(this.InteropLCID, iWindowsFlag);
                if (((s != null) && (s.Length > 0)) && short.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out num))
                {
                    return (ushort) num;
                }
            }
            return iData;
        }

        private int GetPoolStringArraySize(Hashtable offsetTable, params string[] array)
        {
            int num = 0;
            for (int i = 0; i < array.Length; i++)
            {
                num += this.GetPoolStringSize(array[i], offsetTable);
            }
            return (num + (2 * (((array.Length * 2) + 1) + 1)));
        }

        private int GetPoolStringArraySize(string s, Hashtable offsetTable)
        {
            string[] array = new string[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                array[i] = s.Substring(i, 1);
            }
            return this.GetPoolStringArraySize(offsetTable, array);
        }

        private int GetPoolStringSize(string s, Hashtable offsetTable)
        {
            int num = 0;
            if (offsetTable[s] == null)
            {
                offsetTable[s] = "";
                num = 2 * ((s.Length + 1) + (1 - (s.Length & 1)));
            }
            return num;
        }

        private static string GetQualifiedName(string name)
        {
            StringBuilder builder = new StringBuilder(name.Length);
            int num = 0;
            while ((num < name.Length) && (name[num] != '-'))
            {
                builder.Append(name[num]);
                num++;
            }
            builder.Append("--");
            num++;
            int index = num;
            while ((num < name.Length) && (name[num] != '-'))
            {
                builder.Append(name[num]);
                num++;
            }
            num++;
            while (num < name.Length)
            {
                builder.Insert(index, name[num]);
                index++;
                num++;
            }
            return builder.ToString();
        }

        private static bool GetScriptTag(int lcid, out string script)
        {
            script = null;
            string source = CultureInfo.nativeGetCultureName(lcid, false, true);
            if (source != null)
            {
                byte[] buffer;
                for (int i = 0; i < source.Length; i++)
                {
                    if ((source[i] >= 'ᐁ') && (source[i] <= 'ᙶ'))
                    {
                        script = "cans";
                        return true;
                    }
                    if ((source[i] >= 'ሀ') && (source[i] <= '፼'))
                    {
                        script = "ethi";
                        return true;
                    }
                    if ((source[i] >= '᠀') && (source[i] <= '᠙'))
                    {
                        script = "mong";
                        return true;
                    }
                    if ((source[i] >= 0xa000) && (source[i] <= 0xa4c6))
                    {
                        script = "yiii";
                        return true;
                    }
                    if ((source[i] >= 'Ꭰ') && (source[i] <= 'Ᏼ'))
                    {
                        script = "cher";
                        return true;
                    }
                    if ((source[i] >= 'ក') && (source[i] <= '៹'))
                    {
                        script = "khmr";
                        return true;
                    }
                }
                int num2 = CultureInfo.GetNativeSortKey(lcid, 0, source, source.Length, out buffer);
                if (num2 == 0)
                {
                    return false;
                }
                for (int j = 0; (j < num2) && (buffer[j] != 1); j += 2)
                {
                    switch (buffer[j])
                    {
                        case 14:
                            script = "latn";
                            return true;

                        case 15:
                            script = "grek";
                            return true;

                        case 0x10:
                            script = "cyrl";
                            return true;

                        case 0x11:
                            script = "armn";
                            return true;

                        case 0x12:
                            script = "hebr";
                            return true;

                        case 0x13:
                            script = "arab";
                            return true;

                        case 20:
                            script = "deva";
                            return true;

                        case 0x15:
                            script = "beng";
                            return true;

                        case 0x16:
                            script = "guru";
                            return true;

                        case 0x17:
                            script = "gujr";
                            return true;

                        case 0x18:
                            script = "orya";
                            return true;

                        case 0x19:
                            script = "taml";
                            return true;

                        case 0x1a:
                            script = "telu";
                            return true;

                        case 0x1b:
                            script = "knda";
                            return true;

                        case 0x1c:
                            script = "mlym";
                            return true;

                        case 0x1d:
                            script = "sinh";
                            return true;

                        case 30:
                            script = "thai";
                            return true;

                        case 0x1f:
                            script = "laoo";
                            return true;

                        case 0x20:
                            script = "tibt";
                            return true;

                        case 0x21:
                            script = "geor";
                            return true;

                        case 0x22:
                            script = "kana";
                            return true;

                        case 0x23:
                            script = "bopo";
                            return true;

                        case 0x24:
                            script = "hang";
                            return true;

                        case 0x80:
                            script = "hani";
                            return true;
                    }
                }
            }
            return false;
        }

        private unsafe string GetString(uint iOffset)
        {
            char* chPtr = (char*) (this.m_pPool + iOffset);
            if (chPtr[1] == '\0')
            {
                return string.Empty;
            }
            return new string(chPtr + 1, 0, chPtr[0]);
        }

        private unsafe string[] GetStringArray(uint iOffset)
        {
            if (iOffset == 0)
            {
                return new string[0];
            }
            ushort* numPtr = this.m_pPool + ((ushort*) iOffset);
            int num = numPtr[0];
            string[] strArray = new string[num];
            uint* numPtr2 = (uint*) (numPtr + 1);
            for (int i = 0; i < num; i++)
            {
                strArray[i] = this.GetString(numPtr2[i]);
            }
            return strArray;
        }

        private unsafe string GetStringArrayDefault(uint iOffset)
        {
            if (iOffset == 0)
            {
                return string.Empty;
            }
            ushort* numPtr = this.m_pPool + ((ushort*) iOffset);
            uint* numPtr2 = (uint*) (numPtr + 1);
            return this.GetString(numPtr2[0]);
        }

        private static void GetSyntheticConsoleFallback(ref CultureData data, ref CompositeCultureData compositeData)
        {
            compositeData.consoleFallbackName = CultureInfo.InvariantCulture.GetConsoleFallbackUICulture().Name;
            if (data.sConsoleFallbackName != null)
            {
                string str;
                int num;
                string name = ValidateCulturePieceToLower(data.sConsoleFallbackName, "ConsoleFallbackName", 0x54);
                if (CultureTable.Default.GetDataItemFromCultureName(name, out num, out str) >= 0)
                {
                    compositeData.consoleFallbackName = str;
                }
                else if (SyntheticNameToLcidCache[name] != null)
                {
                    compositeData.consoleFallbackName = data.sConsoleFallbackName;
                }
            }
        }

        private unsafe bool GetSyntheticCulture(int cultureID)
        {
            if ((SyntheticLcidToNameCache == null) || (SyntheticNameToLcidCache == null))
            {
                CacheSyntheticNameLcidMapping();
            }
            if (SyntheticLcidToNameCache[cultureID] == null)
            {
                return false;
            }
            if (SyntheticDataCache == null)
            {
                SyntheticDataCache = new Hashtable();
            }
            else
            {
                this.nativeMemoryHandle = (AgileSafeNativeMemoryHandle) SyntheticDataCache[cultureID];
            }
            if (this.nativeMemoryHandle != null)
            {
                this.m_pData = (CultureTableData*) this.nativeMemoryHandle.DangerousGetHandle();
                this.m_pPool = (ushort*) (this.m_pData + 1);
                this.m_CultureTable = CultureTable.Default;
                this.m_CultureName = this.SNAME;
                this.m_CultureID = cultureID;
                this.m_synthetic = true;
                this.m_ActualCultureID = cultureID;
                this.m_ActualName = this.m_CultureName;
                return true;
            }
            CultureData cultureData = new CultureData();
            bool flag = false;
            bool tookLock = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Monitor.ReliableEnter(typeof(CultureTableRecord), ref tookLock);
                flag = CultureInfo.nativeGetCultureData(cultureID, ref cultureData);
            }
            finally
            {
                if (tookLock)
                {
                    Monitor.Exit(typeof(CultureTableRecord));
                }
            }
            if (!flag)
            {
                return false;
            }
            CompositeCultureData compositeData = new CompositeCultureData();
            int cb = this.GetCultureDataSize(cultureID, ref cultureData, ref compositeData);
            IntPtr zero = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    zero = Marshal.AllocHGlobal(cb);
                    if (zero != IntPtr.Zero)
                    {
                        this.nativeMemoryHandle = new AgileSafeNativeMemoryHandle(zero, true);
                    }
                }
            }
            finally
            {
                if ((this.nativeMemoryHandle == null) && (zero != IntPtr.Zero))
                {
                    Marshal.FreeHGlobal(zero);
                    zero = IntPtr.Zero;
                }
            }
            if (zero == IntPtr.Zero)
            {
                throw new OutOfMemoryException(Environment.GetResourceString("OutOfMemory_MemFailPoint"));
            }
            this.m_pData = (CultureTableData*) this.nativeMemoryHandle.DangerousGetHandle();
            this.m_pPool = (ushort*) (this.m_pData + 1);
            this.FillCultureDataMemory(cultureID, ref cultureData, ref compositeData);
            this.m_CultureTable = CultureTable.Default;
            this.m_CultureName = this.SNAME;
            this.m_CultureID = cultureID;
            this.m_synthetic = true;
            this.m_ActualCultureID = cultureID;
            this.m_ActualName = this.m_CultureName;
            lock (SyntheticDataCache)
            {
                if (SyntheticDataCache[cultureID] == null)
                {
                    SyntheticDataCache[cultureID] = this.nativeMemoryHandle;
                }
            }
            return true;
        }

        private static void GetSyntheticParentData(ref CultureData data, ref CompositeCultureData compositeData)
        {
            compositeData.parentLcid = CultureInfo.InvariantCulture.LCID;
            compositeData.parentName = CultureInfo.InvariantCulture.Name;
            if (data.sParentName != null)
            {
                string str;
                int num;
                string name = ValidateCulturePieceToLower(data.sParentName, "ParentName", 0x54);
                if (CultureTable.Default.GetDataItemFromCultureName(name, out num, out str) >= 0)
                {
                    compositeData.parentLcid = num;
                    compositeData.parentName = str;
                }
                else if (SyntheticNameToLcidCache[name] != null)
                {
                    compositeData.parentLcid = (int) SyntheticNameToLcidCache[name];
                    compositeData.parentName = data.sParentName;
                }
            }
        }

        private static string GetTimeSeparator(string format)
        {
            string str = string.Empty;
            int num = 0;
            int start = -1;
            for (num = 0; num < format.Length; num++)
            {
                if (((format[num] == 'H') || (format[num] == 'h')) || ((format[num] == 'm') || (format[num] == 's')))
                {
                    char ch = format[num];
                    num++;
                    while ((num < format.Length) && (format[num] == ch))
                    {
                        num++;
                    }
                    if (num < format.Length)
                    {
                        start = num;
                    }
                    break;
                }
                if (format[num] == '\'')
                {
                    num++;
                    while ((num < format.Length) && (format[num] != '\''))
                    {
                        num++;
                    }
                }
            }
            if (start != -1)
            {
                for (num = start; num < format.Length; num++)
                {
                    if (((format[num] == 'H') || (format[num] == 'h')) || ((format[num] == 'm') || (format[num] == 's')))
                    {
                        return UnescapeWin32String(format, start, num - 1);
                    }
                    if (format[num] == '\'')
                    {
                        num++;
                        while ((num < format.Length) && (format[num] != '\''))
                        {
                            num++;
                        }
                    }
                }
            }
            return str;
        }

        private unsafe int[] GetWordArray(uint iData)
        {
            if (iData == 0)
            {
                return new int[0];
            }
            ushort* numPtr = this.m_pPool + ((ushort*) iData);
            int num = numPtr[0];
            int[] numArray = new int[num];
            numPtr++;
            for (int i = 0; i < num; i++)
            {
                numArray[i] = numPtr[i];
            }
            return numArray;
        }

        private string GroupSizesConstruction(string rawGroupSize)
        {
            int length = rawGroupSize.Length;
            if (rawGroupSize[length - 1] == '0')
            {
                length--;
            }
            int num2 = 0;
            StringBuilder builder = new StringBuilder();
            while (num2 < length)
            {
                builder.Append((char) (rawGroupSize[num2] - '0'));
                num2++;
                if (num2 < length)
                {
                    num2++;
                }
            }
            if (length == rawGroupSize.Length)
            {
                builder.Append('\0');
            }
            return builder.ToString();
        }

        internal static unsafe int IdFromEverettDataItem(int iDataItem)
        {
            InitEverettDataItemToLCIDMappings();
            if ((iDataItem < 0) || (iDataItem >= m_EverettDataItemToLCIDMappingsSize))
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_InvalidFieldState"));
            }
            return m_EverettDataItemToLCIDMappings[iDataItem];
        }

        internal static unsafe int IdFromEverettRegionInfoDataItem(int iDataItem)
        {
            InitEverettRegionDataItemToLCIDMappings();
            if ((iDataItem < 0) || (iDataItem >= m_EverettRegionInfoDataItemToLCIDMappingsSize))
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_InvalidFieldState"));
            }
            return m_EverettRegionInfoDataItemToLCIDMappings[iDataItem];
        }

        private static unsafe void InitEverettCultureDataItemMapping()
        {
            if (m_EverettCultureDataItemMappings == null)
            {
                m_EverettCultureDataItemMappings = CultureInfo.nativeGetStaticInt32DataTable(1, out m_EverettCultureDataItemMappingsSize);
            }
        }

        private static unsafe void InitEverettDataItemToLCIDMappings()
        {
            if (m_EverettDataItemToLCIDMappings == null)
            {
                m_EverettDataItemToLCIDMappings = CultureInfo.nativeGetStaticInt32DataTable(2, out m_EverettDataItemToLCIDMappingsSize);
            }
        }

        private static unsafe void InitEverettRegionDataItemMapping()
        {
            if (m_EverettRegionDataItemMappings == null)
            {
                m_EverettRegionDataItemMappings = CultureInfo.nativeGetStaticInt32DataTable(0, out m_EverettRegionDataItemMappingsSize);
            }
        }

        private static unsafe void InitEverettRegionDataItemToLCIDMappings()
        {
            if (m_EverettRegionInfoDataItemToLCIDMappings == null)
            {
                m_EverettRegionInfoDataItemToLCIDMappings = CultureInfo.nativeGetStaticInt32DataTable(3, out m_EverettRegionInfoDataItemToLCIDMappingsSize);
            }
        }

        internal static void InitSyntheticMapping()
        {
            if ((SyntheticLcidToNameCache == null) || (SyntheticNameToLcidCache == null))
            {
                CacheSyntheticNameLcidMapping();
            }
        }

        private static bool IsBuiltInCulture(int lcid) => 
            CultureTable.Default.IsExistingCulture(lcid);

        internal static bool IsCustomCultureId(int cultureId)
        {
            if ((cultureId != 0xc00) && (cultureId != 0x1000))
            {
                return false;
            }
            return true;
        }

        private bool IsOptionalCalendar(int calendarId)
        {
            for (int i = 0; i < this.IOPTIONALCALENDARS.Length; i++)
            {
                if (this.IOPTIONALCALENDARS[i] == calendarId)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsValidSortID(int sortID)
        {
            if ((sortID != 0) && (((this.SALTSORTID == null) || (this.SALTSORTID.Length < sortID)) || (this.SALTSORTID[sortID - 1].Length == 0)))
            {
                return false;
            }
            return true;
        }

        private bool NameHasScriptTag(string tempName)
        {
            int num = 0;
            for (int i = 0; (i < tempName.Length) && (num < 2); i++)
            {
                if (tempName[i] == '-')
                {
                    num++;
                }
            }
            return (num > 1);
        }

        private static string ReescapeWin32String(string str)
        {
            if (str == null)
            {
                return null;
            }
            StringBuilder builder = null;
            bool flag = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\'')
                {
                    if (flag)
                    {
                        if (((i + 1) < str.Length) && (str[i + 1] == '\''))
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(str, 0, i, str.Length * 2);
                            }
                            builder.Append(@"\'");
                            i++;
                            continue;
                        }
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else if (str[i] == '\\')
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(str, 0, i, str.Length * 2);
                    }
                    builder.Append(@"\\");
                    continue;
                }
                if (builder != null)
                {
                    builder.Append(str[i]);
                }
            }
            return builder?.ToString();
        }

        private static string[] ReescapeWin32Strings(string[] array)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = ReescapeWin32String(array[i]);
                }
            }
            return array;
        }

        internal static void ResetCustomCulturesCache()
        {
            CultureTableRecordCache = null;
            CultureTableRecordRegionCache = null;
        }

        private unsafe uint SetPoolString(string s, Hashtable offsetTable, ref uint currentOffset)
        {
            uint num = currentOffset;
            if (offsetTable[s] != null)
            {
                return (uint) offsetTable[s];
            }
            offsetTable[s] = (uint) currentOffset;
            this.m_pPool[currentOffset] = (ushort) s.Length;
            currentOffset++;
            for (int i = 0; i < s.Length; i++)
            {
                this.m_pPool[currentOffset] = s[i];
                currentOffset++;
            }
            if ((currentOffset & 1) == 0)
            {
                this.m_pPool[currentOffset] = 0;
                currentOffset++;
            }
            return num;
        }

        private unsafe uint SetPoolStringArray(Hashtable offsetTable, ref uint currentOffset, params string[] array)
        {
            uint[] numArray = new uint[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                numArray[i] = this.SetPoolString(array[i], offsetTable, ref currentOffset);
            }
            uint num2 = currentOffset;
            this.m_pPool[currentOffset] = (ushort) numArray.Length;
            currentOffset++;
            uint* numPtr = (uint*) (this.m_pPool + currentOffset);
            for (int j = 0; j < numArray.Length; j++)
            {
                numPtr[j] = numArray[j];
                currentOffset += 2;
            }
            if ((currentOffset & 1) == 0)
            {
                this.m_pPool[currentOffset] = 0;
                currentOffset++;
            }
            return num2;
        }

        private uint SetPoolStringArrayFromSingleString(string s, Hashtable offsetTable, ref uint currentOffset)
        {
            string[] array = new string[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                array[i] = s.Substring(i, 1);
            }
            return this.SetPoolStringArray(offsetTable, ref currentOffset, array);
        }

        internal CultureTable TryCreateReplacementCulture(string replacementCultureName, out int dataItem)
        {
            int num;
            string str2;
            string name = ValidateCulturePieceToLower(replacementCultureName, "cultureName", 0x54);
            CultureTable customCultureTable = this.GetCustomCultureTable(name);
            if (customCultureTable == null)
            {
                dataItem = -1;
                return null;
            }
            dataItem = customCultureTable.GetDataItemFromCultureName(name, out num, out str2);
            if (dataItem < 0)
            {
                return null;
            }
            return customCultureTable;
        }

        private static string UnescapeWin32String(string str, int start, int end)
        {
            StringBuilder builder = null;
            bool flag = false;
            for (int i = start; (i < str.Length) && (i <= end); i++)
            {
                if (str[i] == '\'')
                {
                    if (flag)
                    {
                        if (((i + 1) < str.Length) && (str[i + 1] == '\''))
                        {
                            builder.Append('\'');
                            i++;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = true;
                        if (builder == null)
                        {
                            builder = new StringBuilder(str, start, i - start, str.Length);
                        }
                    }
                }
                else if (builder != null)
                {
                    builder.Append(str[i]);
                }
            }
            return builder?.ToString();
        }

        internal bool UseCurrentCalendar(int calID) => 
            (this.UseGetLocaleInfo && (CultureInfo.nativeGetCurrentCalendar() == calID));

        private static string ValidateCulturePieceToLower(string testString, string paramName, int maxLength)
        {
            if (testString.Length > maxLength)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_NameTooLong"), new object[] { testString, maxLength }), paramName);
            }
            StringBuilder builder = new StringBuilder(testString.Length);
            for (int i = 0; i < testString.Length; i++)
            {
                char ch = testString[i];
                if ((ch <= 'Z') && (ch >= 'A'))
                {
                    builder.Append((char) ((ch - 'A') + 0x61));
                }
                else
                {
                    if ((((ch > 'z') || (ch < 'a')) && ((ch > '9') || (ch < '0'))) && ((ch != '_') && (ch != '-')))
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_NameContainsInvalidCharacters"), new object[] { testString }), paramName);
                    }
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }

        internal int ActualCultureID
        {
            get
            {
                if (this.m_ActualCultureID == 0)
                {
                    this.m_ActualCultureID = this.ILANGUAGE;
                }
                return this.m_ActualCultureID;
            }
        }

        internal string ActualName
        {
            get
            {
                if (this.m_ActualName == null)
                {
                    this.m_ActualName = this.SNAME;
                }
                return this.m_ActualName;
            }
        }

        private static AdjustedSyntheticCultureName[] AdjustedSyntheticNames
        {
            get
            {
                if (s_adjustedSyntheticNames == null)
                {
                    s_adjustedSyntheticNames = new AdjustedSyntheticCultureName[] { new AdjustedSyntheticCultureName(0x141a, "bs", "BA", "bs-Latn-BA"), new AdjustedSyntheticCultureName(0x243b, "smn", "FI", "smn-FI"), new AdjustedSyntheticCultureName(0x103b, "smj", "NO", "smj-NO"), new AdjustedSyntheticCultureName(0x143b, "smj", "SE", "smj-SE"), new AdjustedSyntheticCultureName(0x203b, "sms", "FI", "sms-FI"), new AdjustedSyntheticCultureName(0x183b, "sma", "NO", "sma-NO"), new AdjustedSyntheticCultureName(0x1c3b, "sma", "SE", "sma-SE"), new AdjustedSyntheticCultureName(0x46b, "quz", "BO", "quz-BO"), new AdjustedSyntheticCultureName(0x86b, "quz", "EC", "quz-EC"), new AdjustedSyntheticCultureName(0xc6b, "quz", "PE", "quz-PE") };
                }
                return s_adjustedSyntheticNames;
            }
        }

        internal int CultureID =>
            this.m_CultureID;

        internal string CultureName
        {
            get => 
                this.m_CultureName;
            set
            {
                this.m_CultureName = value;
            }
        }

        internal string CultureNativeDisplayName
        {
            get
            {
                int num;
                CultureInfo.nativeGetUserDefaultUILanguage(&num);
                if (CultureInfo.GetLangID(num) == CultureInfo.GetLangID(CultureInfo.CurrentUICulture.LCID))
                {
                    string str = CultureInfo.nativeGetLocaleInfo(this.m_ActualCultureID, 2);
                    if (str != null)
                    {
                        if (str[str.Length - 1] == '\0')
                        {
                            return str.Substring(0, str.Length - 1);
                        }
                        return str;
                    }
                }
                return this.SNATIVEDISPLAYNAME;
            }
        }

        internal ushort ICALENDARTYPE
        {
            get
            {
                if (this.m_bUseUserOverride)
                {
                    short num;
                    string s = CultureInfo.nativeGetLocaleInfo(this.ActualCultureID, 0x1009);
                    if (((s != null) && (s.Length > 0)) && (short.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out num) && this.IsOptionalCalendar(num)))
                    {
                        return (ushort) num;
                    }
                }
                return (ushort) this.IOPTIONALCALENDARS[0];
            }
        }

        internal uint ICOMPAREINFO =>
            this.m_pData.iCompareInfo;

        internal ushort ICURRDIGITS =>
            this.m_pData.iCurrencyDigits;

        internal ushort ICURRENCY =>
            this.m_pData.iCurrency;

        internal ushort IDEFAULTANSICODEPAGE =>
            this.m_pData.iDefaultAnsiCodePage;

        internal ushort IDEFAULTEBCDICCODEPAGE =>
            this.m_pData.iDefaultEbcdicCodePage;

        internal ushort IDEFAULTMACCODEPAGE =>
            this.m_pData.iDefaultMacCodePage;

        internal ushort IDEFAULTOEMCODEPAGE =>
            this.m_pData.iDefaultOemCodePage;

        internal ushort IDIGITS =>
            this.m_pData.iDigits;

        internal ushort IDIGITSUBSTITUTION =>
            this.GetOverrideUSHORT(this.m_pData.iDigitSubstitution, 0x1014);

        internal ushort IFIRSTDAYOFWEEK =>
            this.m_pData.iFirstDayOfWeek;

        internal ushort IFIRSTWEEKOFYEAR =>
            this.GetOverrideUSHORT(this.m_pData.iFirstWeekOfYear, 0x100d);

        internal uint IFLAGS =>
            this.m_pData.iFlags;

        internal DateTimeFormatFlags IFORMATFLAGS =>
            ((DateTimeFormatFlags) this.m_pData.iFormatFlags);

        internal ushort IGEOID =>
            this.m_pData.iGeoId;

        internal ushort IINPUTLANGUAGEHANDLE =>
            this.m_pData.iInputLanguageHandle;

        internal ushort ILANGUAGE =>
            this.m_pData.iLanguage;

        internal ushort ILINEORIENTATIONS =>
            this.m_pData.iLineOrientations;

        internal ushort IMEASURE =>
            this.GetOverrideUSHORT(this.m_pData.iMeasure, 13);

        internal ushort INEGATIVEPERCENT =>
            this.m_pData.iNegativePercent;

        internal ushort INEGCURR =>
            this.m_pData.iNegativeCurrency;

        internal ushort INEGNUMBER =>
            this.m_pData.iNegativeNumber;

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        private int InteropLCID
        {
            get
            {
                if (this.ActualCultureID != 0x1000)
                {
                    return this.ActualCultureID;
                }
                return 0xc00;
            }
        }

        internal int[] IOPTIONALCALENDARS =>
            this.GetWordArray(this.m_pData.waCalendars);

        internal ushort IPARENT =>
            this.m_pData.iParent;

        internal ushort IPOSITIVEPERCENT =>
            this.m_pData.iPositivePercent;

        internal bool IsCustomCulture =>
            !this.m_CultureTable.fromAssembly;

        internal bool IsNeutralCulture =>
            ((this.IFLAGS & 1) == 0);

        internal bool IsReplacementCulture =>
            (this.IsCustomCulture && !IsCustomCultureId(this.m_CultureID));

        internal bool IsSynthetic =>
            this.m_synthetic;

        internal ushort ITEXTINFO
        {
            get
            {
                ushort iTextInfo = this.m_pData.iTextInfo;
                if (this.CultureID == 0x40a)
                {
                    iTextInfo = 0x40a;
                }
                if ((iTextInfo != 0xc00) && (iTextInfo != 0))
                {
                    return iTextInfo;
                }
                return 0x7f;
            }
        }

        internal string RegionNativeDisplayName
        {
            get
            {
                int num;
                CultureInfo.nativeGetUserDefaultUILanguage(&num);
                if (CultureInfo.GetLangID(num) == CultureInfo.GetLangID(CultureInfo.CurrentUICulture.LCID))
                {
                    string str = CultureInfo.nativeGetLocaleInfo(this.m_ActualCultureID, 6);
                    if (str != null)
                    {
                        if (str[str.Length - 1] == '\0')
                        {
                            return str.Substring(0, str.Length - 1);
                        }
                        return str;
                    }
                }
                return this.SNATIVECOUNTRY;
            }
        }

        internal string S1159 =>
            this.GetString(this.m_pData.sAM1159);

        internal string S2359 =>
            this.GetString(this.m_pData.sPM2359);

        internal string SABBREVADERA =>
            this.GetString(this.m_pData.sAbbrevAdEra);

        internal string SABBREVCTRYNAME =>
            this.GetString(this.m_pData.sAbbrevCountry);

        internal string[] SABBREVDAYNAMES =>
            this.GetStringArray(this.m_pData.saAbbrevDayNames);

        internal string SABBREVLANGNAME =>
            this.GetString(this.m_pData.sAbbrevLang);

        internal string[] SABBREVMONTHGENITIVENAMES =>
            this.GetStringArray(this.m_pData.saAbbrevMonthGenitiveNames);

        internal string[] SABBREVMONTHNAMES =>
            this.GetStringArray(this.m_pData.saAbbrevMonthNames);

        internal string SADERA =>
            this.GetString(this.m_pData.sAdEra);

        internal string[] SALTSORTID =>
            this.GetStringArray(this.m_pData.saAltSortID);

        internal string SCONSOLEFALLBACKNAME =>
            this.GetString(this.m_pData.sConsoleFallbackName);

        internal string SCURRENCY =>
            this.GetString(this.m_pData.sCurrency);

        internal string SDATE =>
            GetDateSeparator(this.GetOverrideStringArrayDefault(this.m_pData.saShortDate, 0x1f));

        internal string[] SDATEWORDS =>
            this.GetStringArray(this.m_pData.saDateWords);

        internal string[] SDAYNAMES =>
            this.GetStringArray(this.m_pData.saDayNames);

        internal string SDECIMAL =>
            this.GetString(this.m_pData.sDecimalSeparator);

        internal string SENGCOUNTRY =>
            this.GetString(this.m_pData.sEnglishCountry);

        internal string SENGDISPLAYNAME =>
            this.GetString(this.m_pData.sEnglishDisplayName);

        internal string SENGLISHCURRENCY =>
            this.GetString(this.m_pData.sEnglishCurrency);

        internal int[] SGROUPING =>
            this.GetOverrideGrouping(this.m_pData.waGrouping, 0x10);

        internal string SINTLSYMBOL =>
            this.GetString(this.m_pData.sIntlMonetarySymbol);

        internal string SISO3166CTRYNAME =>
            this.GetString(this.m_pData.sISO3166CountryName);

        internal string SISO3166CTRYNAME2 =>
            this.GetString(this.m_pData.sISO3166CountryName2);

        internal string SISO639LANGNAME =>
            this.GetString(this.m_pData.sISO639Language);

        internal string SISO639LANGNAME2 =>
            this.GetString(this.m_pData.sISO639Language2);

        internal string SLIST =>
            this.GetOverrideString(this.m_pData.sListSeparator, 12);

        internal string SLONGDATE =>
            ReescapeWin32String(this.GetStringArrayDefault(this.m_pData.saLongDate));

        internal string[] SLONGDATES =>
            ReescapeWin32Strings(this.GetStringArray(this.m_pData.saLongDate));

        internal string SMONDECIMALSEP =>
            this.GetString(this.m_pData.sMonetaryDecimal);

        internal int[] SMONGROUPING =>
            this.GetOverrideGrouping(this.m_pData.waMonetaryGrouping, 0x18);

        internal string SMONTHDAY =>
            ReescapeWin32String(this.GetString(this.m_pData.sMonthDay));

        internal string[] SMONTHGENITIVENAMES =>
            this.GetStringArray(this.m_pData.saMonthGenitiveNames);

        internal string[] SMONTHNAMES =>
            this.GetStringArray(this.m_pData.saMonthNames);

        internal string SMONTHOUSANDSEP =>
            this.GetString(this.m_pData.sMonetaryThousand);

        internal string SNAME =>
            this.GetString(this.m_pData.sName);

        internal string SNAN =>
            this.GetString(this.m_pData.sNaN);

        internal string[] SNATIVECALNAMES =>
            this.GetStringArray(this.m_pData.saNativeCalendarNames);

        internal string SNATIVECOUNTRY =>
            this.GetString(this.m_pData.sNativeCountry);

        internal string SNATIVECURRENCY =>
            this.GetString(this.m_pData.sNativeCurrency);

        internal string[] SNATIVEDIGITS
        {
            get
            {
                string str;
                if ((!this.m_bUseUserOverride || (this.CultureID == 0xc00)) || (((str = CultureInfo.nativeGetLocaleInfo(this.ActualCultureID, 0x13)) == null) || (str.Length != 10)))
                {
                    return this.GetStringArray(this.m_pData.saNativeDigits);
                }
                string[] strArray = new string[10];
                for (int i = 0; i < str.Length; i++)
                {
                    strArray[i] = str[i].ToString(CultureInfo.InvariantCulture);
                }
                return strArray;
            }
        }

        internal string SNATIVEDISPLAYNAME
        {
            get
            {
                if (((CultureInfo.GetLangID(this.ActualCultureID) == 0x404) && (CultureInfo.GetLangID(CultureInfo.InstalledUICulture.LCID) == 0x404)) && !this.IsCustomCulture)
                {
                    return (CultureInfo.nativeGetLocaleInfo(0x404, 4) + " (" + CultureInfo.nativeGetLocaleInfo(0x404, 8) + ")");
                }
                return this.GetString(this.m_pData.sNativeDisplayName);
            }
        }

        internal string SNEGATIVESIGN =>
            this.GetString(this.m_pData.sNegativeSign);

        internal string SNEGINFINITY =>
            this.GetString(this.m_pData.sNegativeInfinity);

        internal string SPARENT =>
            this.GetString(this.m_pData.sParent);

        internal string SPERCENT =>
            this.GetString(this.m_pData.sPercent);

        internal string SPOSINFINITY =>
            this.GetString(this.m_pData.sPositiveInfinity);

        internal string SPOSITIVESIGN
        {
            get
            {
                string str = this.GetString(this.m_pData.sPositiveSign);
                if ((str != null) && (str.Length != 0))
                {
                    return str;
                }
                return "+";
            }
        }

        internal string SREGIONNAME =>
            this.GetString(this.m_pData.sRegionName);

        internal string SSHORTDATE =>
            ReescapeWin32String(this.GetStringArrayDefault(this.m_pData.saShortDate));

        internal string[] SSHORTDATES =>
            ReescapeWin32Strings(this.GetStringArray(this.m_pData.saShortDate));

        internal string SSHORTTIME =>
            ReescapeWin32String(this.GetStringArrayDefault(this.m_pData.saShortTime));

        internal string[] SSHORTTIMES =>
            ReescapeWin32Strings(this.GetStringArray(this.m_pData.saShortTime));

        internal string SSPECIFICCULTURE =>
            this.GetString(this.m_pData.sSpecificCulture);

        internal string[] SSUPERSHORTDAYNAMES =>
            this.GetStringArray(this.m_pData.saSuperShortDayNames);

        internal string STHOUSAND =>
            this.GetString(this.m_pData.sThousandSeparator);

        internal string STIME =>
            GetTimeSeparator(this.GetOverrideStringArrayDefault(this.m_pData.saTimeFormat, 0x1003));

        internal string STIMEFORMAT =>
            ReescapeWin32String(this.GetStringArrayDefault(this.m_pData.saTimeFormat));

        internal string[] STIMEFORMATS =>
            ReescapeWin32Strings(this.GetStringArray(this.m_pData.saTimeFormat));

        internal string SYEARMONTH =>
            ReescapeWin32String(this.GetStringArrayDefault(this.m_pData.saYearMonth));

        internal string[] SYEARMONTHS =>
            ReescapeWin32Strings(this.GetStringArray(this.m_pData.saYearMonth));

        internal bool UseGetLocaleInfo
        {
            get
            {
                int num;
                if (!this.m_bUseUserOverride)
                {
                    return false;
                }
                CultureInfo.nativeGetUserDefaultLCID(&num, 0x400);
                if ((this.ActualCultureID != 0x1000) || (num != 0xc00))
                {
                    return (this.ActualCultureID == num);
                }
                return this.SNAME.Equals(CultureInfo.nativeGetCultureName(num, true, false));
            }
        }

        internal bool UseUserOverride =>
            this.m_bUseUserOverride;

        private string WindowsPath
        {
            get
            {
                if (this.m_windowsPath == null)
                {
                    this.m_windowsPath = CultureInfo.nativeGetWindowsDirectory();
                }
                return this.m_windowsPath;
            }
        }

        internal class AdjustedSyntheticCultureName
        {
            internal string isoCountry;
            internal string isoLanguage;
            internal int lcid;
            internal string sName;

            internal AdjustedSyntheticCultureName(int lcid, string isoLanguage, string isoCountry, string sName)
            {
                this.lcid = lcid;
                this.isoLanguage = isoLanguage;
                this.isoCountry = isoCountry;
                this.sName = sName;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CompositeCultureData
        {
            internal string sname;
            internal string englishDisplayName;
            internal string sNativeDisplayName;
            internal string waCalendars;
            internal string consoleFallbackName;
            internal string parentName;
            internal int parentLcid;
        }
    }
}

