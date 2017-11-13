namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct CultureData
    {
        internal string sIso639Language;
        internal string sIso3166CountryName;
        internal string sListSeparator;
        internal string sDecimalSeparator;
        internal string sThousandSeparator;
        internal string sCurrency;
        internal string sMonetaryDecimal;
        internal string sMonetaryThousand;
        internal string sNegativeSign;
        internal string sAM1159;
        internal string sPM2359;
        internal string sAbbrevLang;
        internal string sEnglishLanguage;
        internal string sEnglishCountry;
        internal string sNativeLanguage;
        internal string sNativeCountry;
        internal string sAbbrevCountry;
        internal string sIntlMonetarySymbol;
        internal string sEnglishCurrency;
        internal string sNativeCurrency;
        internal string saAltSortID;
        internal string sParentName;
        internal string sConsoleFallbackName;
        internal string sPositiveSign;
        internal string saNativeDigits;
        internal string waGrouping;
        internal string waMonetaryGrouping;
        internal string waFontSignature;
        internal string sNaN;
        internal string sPositiveInfinity;
        internal string sNegativeInfinity;
        internal string sISO3166CountryName2;
        internal string sISO639Language2;
        internal string[] saSuperShortDayNames;
        internal string[] saTimeFormat;
        internal string[] saShortDate;
        internal string[] saLongDate;
        internal string[] saYearMonth;
        internal string[] saMonthNames;
        internal string[] saDayNames;
        internal string[] saAbbrevDayNames;
        internal string[] saAbbrevMonthNames;
        internal string[] saNativeCalendarNames;
        internal string[] saGenitiveMonthNames;
        internal string[] saAbbrevGenitiveMonthNames;
        internal ushort[] waCalendars;
        internal int iFirstDayOfWeek;
        internal int iDigits;
        internal int iNegativeNumber;
        internal int iCurrencyDigits;
        internal int iCurrency;
        internal int iNegativeCurrency;
        internal int iFirstWeekOfYear;
        internal int iMeasure;
        internal int iDigitSubstitution;
        internal int iDefaultAnsiCodePage;
        internal int iDefaultOemCodePage;
        internal int iDefaultMacCodePage;
        internal int iDefaultEbcdicCodePage;
        internal int iCountry;
        internal int iPaperSize;
        internal int iLeadingZeros;
        internal int iIntlCurrencyDigits;
        internal int iGeoId;
        internal int iDefaultCalender;
    }
}

