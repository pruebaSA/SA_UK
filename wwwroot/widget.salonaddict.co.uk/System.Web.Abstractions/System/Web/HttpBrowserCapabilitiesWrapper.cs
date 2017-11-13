namespace System.Web
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpBrowserCapabilitiesWrapper : HttpBrowserCapabilitiesBase
    {
        private HttpBrowserCapabilities _browser;

        public HttpBrowserCapabilitiesWrapper(HttpBrowserCapabilities httpBrowserCapabilities)
        {
            if (httpBrowserCapabilities == null)
            {
                throw new ArgumentNullException("httpBrowserCapabilities");
            }
            this._browser = httpBrowserCapabilities;
        }

        public override void AddBrowser(string browserName)
        {
            this._browser.AddBrowser(browserName);
        }

        public override int CompareFilters(string filter1, string filter2) => 
            ((IFilterResolutionService) this._browser).CompareFilters(filter1, filter2);

        public override System.Web.UI.HtmlTextWriter CreateHtmlTextWriter(TextWriter w) => 
            this._browser.CreateHtmlTextWriter(w);

        public override void DisableOptimizedCacheKey()
        {
            this._browser.DisableOptimizedCacheKey();
        }

        public override bool EvaluateFilter(string filterName) => 
            ((IFilterResolutionService) this._browser).EvaluateFilter(filterName);

        public override System.Version[] GetClrVersions() => 
            this._browser.GetClrVersions();

        public override bool IsBrowser(string browserName) => 
            this._browser.IsBrowser(browserName);

        public override bool ActiveXControls =>
            this._browser.ActiveXControls;

        public override IDictionary Adapters =>
            this._browser.Adapters;

        public override bool AOL =>
            this._browser.AOL;

        public override bool BackgroundSounds =>
            this._browser.BackgroundSounds;

        public override bool Beta =>
            this._browser.Beta;

        public override string Browser =>
            this._browser.Browser;

        public override ArrayList Browsers =>
            this._browser.Browsers;

        public override bool CanCombineFormsInDeck =>
            this._browser.CanCombineFormsInDeck;

        public override bool CanInitiateVoiceCall =>
            this._browser.CanInitiateVoiceCall;

        public override bool CanRenderAfterInputOrSelectElement =>
            this._browser.CanRenderAfterInputOrSelectElement;

        public override bool CanRenderEmptySelects =>
            this._browser.CanRenderEmptySelects;

        public override bool CanRenderInputAndSelectElementsTogether =>
            this._browser.CanRenderInputAndSelectElementsTogether;

        public override bool CanRenderMixedSelects =>
            this._browser.CanRenderMixedSelects;

        public override bool CanRenderOneventAndPrevElementsTogether =>
            this._browser.CanRenderOneventAndPrevElementsTogether;

        public override bool CanRenderPostBackCards =>
            this._browser.CanRenderPostBackCards;

        public override bool CanRenderSetvarZeroWithMultiSelectionList =>
            this._browser.CanRenderSetvarZeroWithMultiSelectionList;

        public override bool CanSendMail =>
            this._browser.CanSendMail;

        public override IDictionary Capabilities
        {
            get => 
                this._browser.Capabilities;
            set
            {
                this._browser.Capabilities = value;
            }
        }

        public override bool CDF =>
            this._browser.CDF;

        public override System.Version ClrVersion =>
            this._browser.ClrVersion;

        public override bool Cookies =>
            this._browser.Cookies;

        public override bool Crawler =>
            this._browser.Crawler;

        public override int DefaultSubmitButtonLimit =>
            this._browser.DefaultSubmitButtonLimit;

        public override System.Version EcmaScriptVersion =>
            this._browser.EcmaScriptVersion;

        public override bool Frames =>
            this._browser.Frames;

        public override int GatewayMajorVersion =>
            this._browser.GatewayMajorVersion;

        public override double GatewayMinorVersion =>
            this._browser.GatewayMinorVersion;

        public override string GatewayVersion =>
            this._browser.GatewayVersion;

        public override bool HasBackButton =>
            this._browser.HasBackButton;

        public override bool HidesRightAlignedMultiselectScrollbars =>
            this._browser.HidesRightAlignedMultiselectScrollbars;

        public override string HtmlTextWriter
        {
            get => 
                this._browser.HtmlTextWriter;
            set
            {
                this._browser.HtmlTextWriter = value;
            }
        }

        public override string Id =>
            this._browser.Id;

        public override string InputType =>
            this._browser.InputType;

        public override bool IsColor =>
            this._browser.IsColor;

        public override bool IsMobileDevice =>
            this._browser.IsMobileDevice;

        public override string this[string key] =>
            this._browser[key];

        public override bool JavaApplets =>
            this._browser.JavaApplets;

        public override System.Version JScriptVersion =>
            this._browser.JScriptVersion;

        public override int MajorVersion =>
            this._browser.MajorVersion;

        public override int MaximumHrefLength =>
            this._browser.MaximumHrefLength;

        public override int MaximumRenderedPageSize =>
            this._browser.MaximumRenderedPageSize;

        public override int MaximumSoftkeyLabelLength =>
            this._browser.MaximumSoftkeyLabelLength;

        public override double MinorVersion =>
            this._browser.MinorVersion;

        public override string MinorVersionString =>
            this._browser.MinorVersionString;

        public override string MobileDeviceManufacturer =>
            this._browser.MobileDeviceManufacturer;

        public override string MobileDeviceModel =>
            this._browser.MobileDeviceModel;

        public override System.Version MSDomVersion =>
            this._browser.MSDomVersion;

        public override int NumberOfSoftkeys =>
            this._browser.NumberOfSoftkeys;

        public override string Platform =>
            this._browser.Platform;

        public override string PreferredImageMime =>
            this._browser.PreferredImageMime;

        public override string PreferredRenderingMime =>
            this._browser.PreferredRenderingMime;

        public override string PreferredRenderingType =>
            this._browser.PreferredRenderingType;

        public override string PreferredRequestEncoding =>
            this._browser.PreferredRequestEncoding;

        public override string PreferredResponseEncoding =>
            this._browser.PreferredResponseEncoding;

        public override bool RendersBreakBeforeWmlSelectAndInput =>
            this._browser.RendersBreakBeforeWmlSelectAndInput;

        public override bool RendersBreaksAfterHtmlLists =>
            this._browser.RendersBreaksAfterHtmlLists;

        public override bool RendersBreaksAfterWmlAnchor =>
            this._browser.RendersBreaksAfterWmlAnchor;

        public override bool RendersBreaksAfterWmlInput =>
            this._browser.RendersBreaksAfterWmlInput;

        public override bool RendersWmlDoAcceptsInline =>
            this._browser.RendersWmlDoAcceptsInline;

        public override bool RendersWmlSelectsAsMenuCards =>
            this._browser.RendersWmlSelectsAsMenuCards;

        public override string RequiredMetaTagNameValue =>
            this._browser.RequiredMetaTagNameValue;

        public override bool RequiresAttributeColonSubstitution =>
            this._browser.RequiresAttributeColonSubstitution;

        public override bool RequiresContentTypeMetaTag =>
            this._browser.RequiresContentTypeMetaTag;

        public override bool RequiresControlStateInSession =>
            this._browser.RequiresControlStateInSession;

        public override bool RequiresDBCSCharacter =>
            this._browser.RequiresDBCSCharacter;

        public override bool RequiresHtmlAdaptiveErrorReporting =>
            this._browser.RequiresHtmlAdaptiveErrorReporting;

        public override bool RequiresLeadingPageBreak =>
            this._browser.RequiresLeadingPageBreak;

        public override bool RequiresNoBreakInFormatting =>
            this._browser.RequiresNoBreakInFormatting;

        public override bool RequiresOutputOptimization =>
            this._browser.RequiresOutputOptimization;

        public override bool RequiresPhoneNumbersAsPlainText =>
            this._browser.RequiresPhoneNumbersAsPlainText;

        public override bool RequiresSpecialViewStateEncoding =>
            this._browser.RequiresSpecialViewStateEncoding;

        public override bool RequiresUniqueFilePathSuffix =>
            this._browser.RequiresUniqueFilePathSuffix;

        public override bool RequiresUniqueHtmlCheckboxNames =>
            this._browser.RequiresUniqueHtmlCheckboxNames;

        public override bool RequiresUniqueHtmlInputNames =>
            this._browser.RequiresUniqueHtmlInputNames;

        public override bool RequiresUrlEncodedPostfieldValues =>
            this._browser.RequiresUrlEncodedPostfieldValues;

        public override int ScreenBitDepth =>
            this._browser.ScreenBitDepth;

        public override int ScreenCharactersHeight =>
            this._browser.ScreenCharactersHeight;

        public override int ScreenCharactersWidth =>
            this._browser.ScreenCharactersWidth;

        public override int ScreenPixelsHeight =>
            this._browser.ScreenPixelsHeight;

        public override int ScreenPixelsWidth =>
            this._browser.ScreenPixelsWidth;

        public override bool SupportsAccesskeyAttribute =>
            this._browser.SupportsAccesskeyAttribute;

        public override bool SupportsBodyColor =>
            this._browser.SupportsBodyColor;

        public override bool SupportsBold =>
            this._browser.SupportsBold;

        public override bool SupportsCacheControlMetaTag =>
            this._browser.SupportsCacheControlMetaTag;

        public override bool SupportsCallback =>
            this._browser.SupportsCallback;

        public override bool SupportsCss =>
            this._browser.SupportsCss;

        public override bool SupportsDivAlign =>
            this._browser.SupportsDivAlign;

        public override bool SupportsDivNoWrap =>
            this._browser.SupportsDivNoWrap;

        public override bool SupportsEmptyStringInCookieValue =>
            this._browser.SupportsEmptyStringInCookieValue;

        public override bool SupportsFontColor =>
            this._browser.SupportsFontColor;

        public override bool SupportsFontName =>
            this._browser.SupportsFontName;

        public override bool SupportsFontSize =>
            this._browser.SupportsFontSize;

        public override bool SupportsImageSubmit =>
            this._browser.SupportsImageSubmit;

        public override bool SupportsIModeSymbols =>
            this._browser.SupportsIModeSymbols;

        public override bool SupportsInputIStyle =>
            this._browser.SupportsInputIStyle;

        public override bool SupportsInputMode =>
            this._browser.SupportsInputMode;

        public override bool SupportsItalic =>
            this._browser.SupportsItalic;

        public override bool SupportsJPhoneMultiMediaAttributes =>
            this._browser.SupportsJPhoneMultiMediaAttributes;

        public override bool SupportsJPhoneSymbols =>
            this._browser.SupportsJPhoneSymbols;

        public override bool SupportsQueryStringInFormAction =>
            this._browser.SupportsQueryStringInFormAction;

        public override bool SupportsRedirectWithCookie =>
            this._browser.SupportsRedirectWithCookie;

        public override bool SupportsSelectMultiple =>
            this._browser.SupportsSelectMultiple;

        public override bool SupportsUncheck =>
            this._browser.SupportsUncheck;

        public override bool SupportsXmlHttp =>
            this._browser.SupportsXmlHttp;

        public override bool Tables =>
            this._browser.Tables;

        public override System.Type TagWriter =>
            this._browser.TagWriter;

        public override string Type =>
            this._browser.Type;

        public override bool UseOptimizedCacheKey =>
            this._browser.UseOptimizedCacheKey;

        public override bool VBScript =>
            this._browser.VBScript;

        public override string Version =>
            this._browser.Version;

        public override System.Version W3CDomVersion =>
            this._browser.W3CDomVersion;

        public override bool Win16 =>
            this._browser.Win16;

        public override bool Win32 =>
            this._browser.Win32;
    }
}

