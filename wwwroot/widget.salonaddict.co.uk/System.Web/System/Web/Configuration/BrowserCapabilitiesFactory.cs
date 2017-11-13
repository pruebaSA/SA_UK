namespace System.Web.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class BrowserCapabilitiesFactory : BrowserCapabilitiesFactoryBase
    {
        private bool A500Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["HTTP_X_WAP_PROFILE"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"\/SPH-A500\/"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SPH-A500";
            capabilities["maximumRenderedPageSize"] = "8850";
            capabilities["preferredREnderingType"] = "xhtml-basic";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsTitleElement"] = "true";
            browserCaps.AddBrowser("a500");
            this.A500ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.A500ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void A500ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void A500ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool AlavProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "ALAV"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Alcatel";
            capabilities["mobileDeviceModel"] = "OneTouch";
            browserCaps.AddBrowser("Alav");
            this.AlavProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.AlavProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void AlavProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void AlavProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool AlazProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "ALAZ"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Alcatel";
            capabilities["mobileDeviceModel"] = "OneTouch";
            browserCaps.AddBrowser("Alaz");
            this.AlazProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.AlazProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void AlazProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void AlazProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Alcatelbe3Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Alcatel-BE3"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Alcatel";
            capabilities["mobileDeviceModel"] = "OneTouchDB";
            browserCaps.AddBrowser("AlcatelBe3");
            this.Alcatelbe3ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Alcatelbe3ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Alcatelbe3ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Alcatelbe3ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Alcatelbe4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Alcatel-BE4"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Alcatel";
            capabilities["mobileDeviceModel"] = "301";
            browserCaps.AddBrowser("AlcatelBe4");
            this.Alcatelbe4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Alcatelbe4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Alcatelbe4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Alcatelbe4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Alcatelbe5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Alcatel-BE5"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Alcatel";
            capabilities["mobileDeviceModel"] = "501, 701";
            browserCaps.AddBrowser("AlcatelBe5");
            this.Alcatelbe5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Alcatelbe5v2Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Alcatelbe5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Alcatelbe5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Alcatelbe5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Alcatelbe5v2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"2\.0"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "1900";
            capabilities["maximumSoftkeyLabelLength"] = "0";
            capabilities["mobileDeviceModel"] = "Alcatel One Touch 501";
            capabilities["numberOfSoftkeys"] = "10";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["requiresNoSoftkeyLabels"] = "true";
            capabilities["screenBitDepth"] = "0";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "14";
            capabilities["screenPixelsHeight"] = "60";
            capabilities["screenPixelsWidth"] = "91";
            capabilities["supportsBold"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("AlcatelBe5v2");
            this.Alcatelbe5v2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Alcatelbe5v2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Alcatelbe5v2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Alcatelbe5v2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Alcatelbf3Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Alcatel-BF3"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "1900";
            capabilities["maximumSoftkeyLabelLength"] = "13";
            capabilities["mobileDeviceManufacturer"] = "Alcatel";
            capabilities["mobileDeviceModel"] = "Alcatel One Touch 311";
            capabilities["numberOfSoftkeys"] = "10";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "11";
            capabilities["screenPixelsHeight"] = "65";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("AlcatelBf3");
            this.Alcatelbf3ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Alcatelbf3ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Alcatelbf3ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Alcatelbf3ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Alcatelbf4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Alcatel-BF4"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "1900";
            capabilities["maximumSoftkeyLabelLength"] = "13";
            capabilities["mobileDeviceManufacturer"] = "Alcatel";
            capabilities["mobileDeviceModel"] = "Alcatel One Touch 511";
            capabilities["numberOfSoftkeys"] = "10";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "11";
            capabilities["screenPixelsHeight"] = "60";
            capabilities["screenPixelsWidth"] = "89";
            browserCaps.AddBrowser("AlcatelBf4");
            this.Alcatelbf4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Alcatelbf4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Alcatelbf4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Alcatelbf4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool AumicProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"AU-MIC/(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*).*"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "AU MIC";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "true";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "17";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsSelectFollowingTable"] = "false";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = worker["AU MIC ${browserMajorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.AddBrowser("AuMic");
            this.AumicProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Aumicv2Process(headers, browserCaps) && !this.A500Process(headers, browserCaps)) && !this.N400Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.AumicProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void AumicProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void AumicProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Aumicv2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "2"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "8600";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["supportsBold"] = "false";
            browserCaps.AddBrowser("AuMicV2");
            this.Aumicv2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Aumicv2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Aumicv2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Aumicv2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool AuspalmProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "AUS PALM WAPPER"))
            {
                return false;
            }
            capabilities["browser"] = "AU-System Demo WAP Browser";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isMobileDevice"] = "true";
            capabilities["mobileDeviceManufacturer"] = "PalmOS-licensee";
            capabilities["optimumPageWeight"] = "900";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml11";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "36";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsCharacterEntityEncoding"] = "true";
            capabilities["type"] = "AU-System";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("AusPalm");
            this.AuspalmProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.AuspalmProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void AuspalmProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void AuspalmProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool AvantgoProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"\(compatible; AvantGo .*\)"))
            {
                return false;
            }
            worker.ProcessRegex(headers["X-AVANTGO-VERSION"], @"(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*)");
            capabilities["browser"] = "AvantGo";
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "true";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "12";
            capabilities["defaultScreenPixelsHeight"] = "72";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["javascript"] = "false";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "2560";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiredMetaTagNameValue"] = "HandheldFriendly";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "13";
            capabilities["screenCharactersWidth"] = "30";
            capabilities["screenPixelsHeight"] = "150";
            capabilities["screenPixelsWidth"] = "150";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "false";
            capabilities["tables"] = "true";
            capabilities["type"] = worker["AvantGo${browserMajorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("AvantGo");
            this.AvantgoProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.TmobilesidekickProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.AvantgoProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void AvantgoProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void AvantgoProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool BenqathenaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^BENQ\-Athena"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Openwave";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceManufacturer"] = "BenQ";
            capabilities["mobileDeviceModel"] = "S830c";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "0";
            capabilities["screenPixelsWidth"] = "0";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsNoWrapStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("BenQAthena");
            this.BenqathenaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.BenqathenaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void BenqathenaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void BenqathenaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool BlazerProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Blazer"))
            {
                return false;
            }
            capabilities["browser"] = "Handspring Blazer";
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "12";
            capabilities["defaultScreenPixelsHeight"] = "72";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["maximumRenderedPageSize"] = "7000";
            capabilities["mobileDeviceManufacturer"] = "PalmOS-licensee";
            capabilities["numberOfSoftkeys"] = "0";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["rendersWmlSelectsAsMenuCards"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "36";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsBold"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["type"] = "Handspring Blazer";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            this.BlazerProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Blazerupg1Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.BlazerProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void BlazerProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void BlazerProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Blazerupg1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"UPG1 UP/\S* \(compatible; Blazer (?'browserMajorVersion'\d+)(?'browserMinorVersion'\.\d+)"))
            {
                return false;
            }
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            this.Blazerupg1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Blazerupg1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Blazerupg1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Blazerupg1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C201hProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HI01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hitachi";
            capabilities["mobileDeviceModel"] = "C201H";
            browserCaps.AddBrowser("C201h");
            this.C201hProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C201hProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C201hProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C201hProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C202deProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "DN01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Casio";
            capabilities["mobileDeviceModel"] = "C202DE";
            browserCaps.AddBrowser("C202de");
            this.C202deProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C202deProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C202deProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C202deProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C302hProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HI11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hitachi";
            capabilities["mobileDeviceModel"] = "C302H";
            browserCaps.AddBrowser("C302h");
            this.C302hProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C302hProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C302hProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C302hProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C303caProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "CA11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Casio";
            capabilities["mobileDeviceModel"] = "C303CA";
            browserCaps.AddBrowser("C303ca");
            this.C303caProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C303caProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C303caProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C303caProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C304saProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Sanyo-C304SA/"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "C304SA";
            browserCaps.AddBrowser("C304sa");
            this.C304saProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C304saProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C304saProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C304saProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C309hProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HI12"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hitachi";
            capabilities["mobileDeviceModel"] = "C309H";
            browserCaps.AddBrowser("C309h");
            this.C309hProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C309hProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C309hProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C309hProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C311caProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "CA12"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Casio";
            capabilities["mobileDeviceModel"] = "C311CA";
            browserCaps.AddBrowser("C311ca");
            this.C311caProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C311caProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C311caProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C311caProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C402deProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "DN11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Denso";
            capabilities["mobileDeviceModel"] = "C402DE";
            browserCaps.AddBrowser("C402de");
            this.C402deProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C402deProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C402deProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C402deProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C407hProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HI13"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Hitachi";
            capabilities["mobileDeviceModel"] = "C407H";
            browserCaps.AddBrowser("C407h");
            this.C407hProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C407hProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C407hProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C407hProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C409caProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "CA13"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Casio";
            capabilities["mobileDeviceModel"] = "C409CA";
            browserCaps.AddBrowser("C409ca");
            this.C409caProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C409caProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C409caProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C409caProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool C451hProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HI14"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hitachi";
            capabilities["mobileDeviceModel"] = "C451H";
            browserCaps.AddBrowser("C451h");
            this.C451hProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.C451hProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void C451hProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void C451hProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Casioa5302Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KDDI-CA22"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "9000";
            capabilities["mobileDeviceManufacturer"] = "Casio";
            capabilities["mobileDeviceModel"] = "A5302CA";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenPixelsHeight"] = "147";
            capabilities["screenPixelsWidth"] = "132";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("CasioA5302");
            this.Casioa5302ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Casioa5302ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Casioa5302ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Casioa5302ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool CasiopeiaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "CASSIOPEIA BE"))
            {
                return false;
            }
            capabilities["browser"] = "CASSIOPEIA";
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "false";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "12";
            capabilities["defaultScreenPixelsHeight"] = "72";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["ecmascriptversion"] = "1.3";
            capabilities["frames"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isMobileDevice"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["mobileDeviceManufacturer"] = "Casio";
            capabilities["mobileDeviceModel"] = "Casio BE-500";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresContentTypeMetaTag"] = "true";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["requiresNoBreakInFormatting"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "50";
            capabilities["screenCharactersWidth"] = "38";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "240";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCharacterEntityEncoding"] = "false";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivNoWrap"] = "true";
            capabilities["supportsFileUpload"] = "false";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "false";
            capabilities["supportsItalic"] = "false";
            capabilities["tables"] = "true";
            capabilities["type"] = "CASSIOPEIA";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("Casiopeia");
            this.CasiopeiaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.CasiopeiaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void CasiopeiaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void CasiopeiaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Cdm135Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HD-MMD1010"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Audiovox";
            capabilities["mobileDeviceModel"] = "CDM-135";
            browserCaps.AddBrowser("Cdm135");
            this.Cdm135ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Cdm135ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Cdm135ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Cdm135ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Cdm9000Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TSCA"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Audiovox";
            capabilities["mobileDeviceModel"] = "CDM-9000";
            browserCaps.AddBrowser("Cdm9000");
            this.Cdm9000ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Cdm9000ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Cdm9000ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Cdm9000ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Cdm9100Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "AUDIOVOX-CDM9100"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Audiovox";
            capabilities["mobileDeviceModel"] = "CDM-9100";
            browserCaps.AddBrowser("Cdm9100");
            this.Cdm9100ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Cdm9100ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Cdm9100ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Cdm9100ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool CharsetProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = headers["X-UP-DEVCAP-CHARSET"];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            str = headers["X-UP-DEVCAP-CHARSET"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(str, "(?i)^Shift_JIS$"))
            {
                return false;
            }
            str = browserCaps[string.Empty];
            if (!worker.ProcessRegex(str, @"(UP/\S* UP\.Browser/3\.\[3-9]d*)|(UP\.Browser/3\.\[3-9]d*)|(UP\.Browser/3\.\[3-9]d*)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["canSendMail"] = "true";
            this.CharsetProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.CharsetProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void CharsetProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void CharsetProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool ColorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = headers["UA-COLOR"];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            str = headers["UA-COLOR"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(str, @"color(?'colorDepth'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["isColor"] = "true";
            capabilities["screenBitDepth"] = worker["${colorDepth}"];
            this.ColorProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.ColorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void ColorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void ColorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        public override void ConfigureBrowserCapabilities(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            this.DefaultProcess(headers, browserCaps);
            if (base.IsBrowserUnknown(browserCaps))
            {
                this.DefaultDefaultProcess(headers, browserCaps);
            }
        }

        private bool CrawlerProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "crawler|Crawler|Googlebot|msnbot"))
            {
                return false;
            }
            capabilities["crawler"] = "true";
            this.CrawlerProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.CrawlerProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void CrawlerProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void CrawlerProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool D2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D2"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["mobileDeviceModel"] = "D2";
            browserCaps.AddBrowser("D2");
            this.D2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.D2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void D2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void D2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool D303kProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCC1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "D303K";
            browserCaps.AddBrowser("D303k");
            this.D303kProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.D303kProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void D303kProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void D303kProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool D304kProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCC2"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "D304K";
            browserCaps.AddBrowser("D304k");
            this.D304kProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.D304kProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void D304kProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void D304kProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool D512Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG22"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "D-512";
            browserCaps.AddBrowser("D512");
            this.D512ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.D512ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void D512ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void D512ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Db520Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LGE-DB520"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "3072";
            capabilities["mobileDeviceManufacturer"] = "Sprint";
            capabilities["mobileDeviceModel"] = "TP5200";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/wnd.wap.wml";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Db520");
            this.Db520ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Db520ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Db520ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Db520ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool DefaultDefaultProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            capabilities["ecmascriptversion"] = "0.0";
            capabilities["javascript"] = "false";
            capabilities["jscriptversion"] = "0.0";
            bool ignoreApplicationBrowsers = true;
            if (!this.DefaultWmlProcess(headers, browserCaps) && !this.DefaultXhtmlmpProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.DefaultDefaultProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void DefaultDefaultProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool DefaultProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            capabilities["activexcontrols"] = "false";
            capabilities["ak"] = "false";
            capabilities["aol"] = "false";
            capabilities["authenticodeupdate"] = "false";
            capabilities["backgroundsounds"] = "false";
            capabilities["beta"] = "false";
            capabilities["browser"] = "Unknown";
            capabilities["cachesAllResponsesWithExpires"] = "false";
            capabilities["canCombineFormsInDeck"] = "true";
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["canRenderAfterInputOrSelectElement"] = "true";
            capabilities["canRenderEmptySelects"] = "true";
            capabilities["canRenderInputAndSelectElementsTogether"] = "true";
            capabilities["canRenderMixedSelects"] = "true";
            capabilities["canRenderOneventAndPrevElementsTogether"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["canRenderSetvarZeroWithMultiSelectionList"] = "true";
            capabilities["canSendMail"] = "true";
            capabilities["cdf"] = "false";
            capabilities["cookies"] = "true";
            capabilities["crawler"] = "false";
            capabilities["css1"] = "false";
            capabilities["css2"] = "false";
            capabilities["defaultCharacterHeight"] = "12";
            capabilities["defaultCharacterWidth"] = "8";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "12";
            capabilities["defaultScreenPixelsHeight"] = "72";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["defaultSubmitButtonLimit"] = "1";
            capabilities["ecmascriptversion"] = "0.0";
            capabilities["frames"] = "false";
            capabilities["gatewayMajorVersion"] = "0";
            capabilities["gatewayMinorVersion"] = "0";
            capabilities["gatewayVersion"] = "None";
            capabilities["gold"] = "false";
            capabilities["hasBackButton"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "false";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "false";
            capabilities["javaapplets"] = "false";
            capabilities["javascript"] = "false";
            capabilities["jscriptversion"] = "0.0";
            capabilities["majorversion"] = "0";
            capabilities["maximumHrefLength"] = "10000";
            capabilities["maximumRenderedPageSize"] = "2000";
            capabilities["maximumSoftkeyLabelLength"] = "5";
            capabilities["minorversion"] = "0";
            capabilities["mobileDeviceManufacturer"] = "Unknown";
            capabilities["mobileDeviceModel"] = "Unknown";
            capabilities["msdomversion"] = "0.0";
            capabilities["numberOfSoftkeys"] = "0";
            capabilities["platform"] = "Unknown";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["rendersBreaksAfterHtmlLists"] = "true";
            capabilities["rendersBreaksAfterWmlAnchor"] = "false";
            capabilities["rendersBreaksAfterWmlInput"] = "false";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["rendersWmlSelectsAsMenuCards"] = "false";
            capabilities["requiredMetaTagNameValue"] = "";
            capabilities["requiresAdaptiveErrorReporting"] = "false";
            capabilities["requiresAttributeColonSubstitution"] = "false";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["requiresDBCSCharacter"] = "false";
            capabilities["requiresFullyQualifiedRedirectUrl"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["requiresLeadingPageBreak"] = "false";
            capabilities["requiresNoBreakInFormatting"] = "false";
            capabilities["requiresNoescapedPostUrl"] = "true";
            capabilities["requiresNoSoftkeyLabels"] = "false";
            capabilities["requiresOutputOptimization"] = "false";
            capabilities["requiresPhoneNumbersAsPlainText"] = "false";
            capabilities["requiresPostRedirectionHandling"] = "false";
            capabilities["requiresSpecialViewStateEncoding"] = "false";
            capabilities["requiresUniqueFilePathSuffix"] = "false";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "false";
            capabilities["requiresUniqueHtmlInputNames"] = "false";
            capabilities["requiresUrlEncodedPostfieldValues"] = "false";
            capabilities["screenBitDepth"] = "1";
            capabilities["sk"] = "false";
            capabilities["supportsAccesskeyAttribute"] = "false";
            capabilities["supportsBodyColor"] = "true";
            capabilities["supportsBold"] = "false";
            capabilities["supportsCacheControlMetaTag"] = "true";
            capabilities["supportsCharacterEntityEncoding"] = "true";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivAlign"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsEmptyStringInCookieValue"] = "true";
            capabilities["supportsFileUpload"] = "false";
            capabilities["supportsFontColor"] = "true";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["supportsImageSubmit"] = "false";
            capabilities["supportsIModeSymbols"] = "false";
            capabilities["supportsInputIStyle"] = "false";
            capabilities["supportsInputMode"] = "false";
            capabilities["supportsItalic"] = "false";
            capabilities["supportsJPhoneMultiMediaAttributes"] = "false";
            capabilities["supportsJPhoneSymbols"] = "false";
            capabilities["supportsMaintainScrollPositionOnPostback"] = "false";
            capabilities["supportsMultilineTextBoxDisplay"] = "false";
            capabilities["supportsQueryStringInFormAction"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectMultiple"] = "true";
            capabilities["supportsUncheck"] = "true";
            capabilities["supportsVCard"] = "false";
            capabilities["tables"] = "false";
            capabilities["tagwriter"] = "System.Web.UI.Html32TextWriter";
            capabilities["type"] = "Unknown";
            capabilities["vbscript"] = "false";
            capabilities["version"] = "0.0";
            capabilities["w3cdomversion"] = "0.0";
            capabilities["win16"] = "false";
            capabilities["win32"] = "false";
            capabilities["xml"] = "false";
            browserCaps.AddBrowser("Default");
            this.DefaultProcessGateways(headers, browserCaps);
            this.NokiagatewayProcess(headers, browserCaps);
            this.UpgatewayProcess(headers, browserCaps);
            this.CrawlerProcess(headers, browserCaps);
            this.ColorProcess(headers, browserCaps);
            this.MonoProcess(headers, browserCaps);
            this.PixelsProcess(headers, browserCaps);
            this.VoiceProcess(headers, browserCaps);
            this.CharsetProcess(headers, browserCaps);
            this.PlatformProcess(headers, browserCaps);
            this.WinProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (((((!this.MozillaProcess(headers, browserCaps) && !this.DocomoProcess(headers, browserCaps)) && (!this.Ericssonr380Process(headers, browserCaps) && !this.EricssonProcess(headers, browserCaps))) && ((!this.EzwapProcess(headers, browserCaps) && !this.GenericdownlevelProcess(headers, browserCaps)) && (!this.JataayuProcess(headers, browserCaps) && !this.JphoneProcess(headers, browserCaps)))) && (((!this.LegendProcess(headers, browserCaps) && !this.MmeProcess(headers, browserCaps)) && (!this.NokiaProcess(headers, browserCaps) && !this.NokiamobilebrowserrainbowProcess(headers, browserCaps))) && ((!this.Nokiaepoc32wtlProcess(headers, browserCaps) && !this.UpProcess(headers, browserCaps)) && (!this.OperaProcess(headers, browserCaps) && !this.PalmscapeProcess(headers, browserCaps))))) && (((!this.AuspalmProcess(headers, browserCaps) && !this.SharppdaProcess(headers, browserCaps)) && (!this.PanasonicProcess(headers, browserCaps) && !this.Mspie06Process(headers, browserCaps))) && ((!this.SktdevicesProcess(headers, browserCaps) && !this.WinwapProcess(headers, browserCaps)) && !this.XiinoProcess(headers, browserCaps))))
            {
                ignoreApplicationBrowsers = false;
            }
            this.DefaultProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void DefaultProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void DefaultProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool DefaultWmlProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["Accept"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"text/vnd\.wap\.wml|text/hdml"))
            {
                return false;
            }
            target = headers["Accept"];
            if (worker.ProcessRegex(target, @"application/xhtml\+xml; profile|application/vnd\.wap\.xhtml\+xml"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml11";
            bool ignoreApplicationBrowsers = false;
            this.DefaultWmlProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void DefaultWmlProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool DefaultXhtmlmpProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["Accept"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"application/xhtml\+xml; profile|application/vnd\.wap\.xhtml\+xml"))
            {
                return false;
            }
            target = headers["Accept"];
            if (worker.ProcessRegex(target, "text/hdml"))
            {
                return false;
            }
            target = headers["Accept"];
            if (worker.ProcessRegex(target, @"text/vnd\.wap\.wml"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            browserCaps.HtmlTextWriter = "System.Web.UI.XhtmlTextWriter";
            bool ignoreApplicationBrowsers = false;
            this.DefaultXhtmlmpProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void DefaultXhtmlmpProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Dm110Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG05"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "DM-110";
            browserCaps.AddBrowser("Dm110");
            this.Dm110ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Dm110ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Dm110ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Dm110ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomod209iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D209i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "90";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoD209i");
            this.Docomod209iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomod209iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomod209iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomod209iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomod210iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D210i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "91";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoD210i");
            this.Docomod210iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomod210iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomod210iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomod210iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomod211iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D211i"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "91";
            capabilities["screenPixelsWidth"] = "100";
            browserCaps.AddBrowser("DocomoD211i");
            this.Docomod211iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomod211iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomod211iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomod211iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomod501iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D501i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoD501i");
            this.Docomod501iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomod501iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomod501iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomod501iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomod502iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D502i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "90";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoD502i");
            this.Docomod502iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomod502iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomod502iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomod502iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomod503iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D503i$"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "126";
            capabilities["screenPixelsWidth"] = "132";
            browserCaps.AddBrowser("DocomoD503i");
            this.Docomod503iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomod503iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomod503iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomod503iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomod503isProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D503iS$"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "126";
            capabilities["screenPixelsWidth"] = "132";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoD503is");
            this.Docomod503isProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomod503isProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomod503isProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomod503isProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomod505iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "D505i"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["screenBitDepth"] = "18";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "270";
            capabilities["screenPixelsWidth"] = "240";
            browserCaps.AddBrowser("DocomoD505i");
            this.Docomod505iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomod505iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomod505iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomod505iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool DocomodefaultrenderingsizeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["maximumRenderedPageSize"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^(0|00|000)$"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "800";
            this.DocomodefaultrenderingsizeProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.DocomodefaultrenderingsizeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void DocomodefaultrenderingsizeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void DocomodefaultrenderingsizeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoer209iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "ER209i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Ericsson";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "120";
            browserCaps.AddBrowser("DocomoEr209i");
            this.Docomoer209iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoer209iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoer209iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoer209iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof209iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F209i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "91";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoF209i");
            this.Docomof209iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof209iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof209iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof209iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof210iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F210i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "113";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoF210i");
            this.Docomof210iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof210iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof210iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof210iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof211iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F211i$"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "113";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoF211i");
            this.Docomof211iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof211iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof211iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof211iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof212iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F212i"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenPixelsHeight"] = "136";
            capabilities["screenPixelsWidth"] = "132";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoF212i");
            this.Docomof212iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof212iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof212iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof212iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof501iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F501i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "84";
            capabilities["screenPixelsWidth"] = "112";
            browserCaps.AddBrowser("DocomoF501i");
            this.Docomof501iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof501iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof501iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof501iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof502iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F502i$"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "91";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoF502i");
            this.Docomof502iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof502iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof502iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof502iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof502itProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F502it"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "91";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoF502it");
            this.Docomof502itProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof502itProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof502itProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof502itProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof503iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F503i$"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "120";
            browserCaps.AddBrowser("DocomoF503i");
            this.Docomof503iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof503iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof503iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof503iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof503isProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F503iS"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "24";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "120";
            browserCaps.AddBrowser("DocomoF503is");
            this.Docomof503isProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof503isProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof503isProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof503isProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof504iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F504i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "136";
            capabilities["screenPixelsWidth"] = "132";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoF504i");
            this.Docomof504iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof504iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof504iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof504iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomof671iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "F671i$"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Fujitsu";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "126";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoF671i");
            this.Docomof671iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomof671iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomof671iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomof671iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoisim60Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "ISIM60"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "true";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NTT DoCoMo";
            capabilities["mobileDeviceModel"] = "i-mode HTML Simulator";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "180";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsEmptyStringInCookieValue"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectMultiple"] = "false";
            browserCaps.AddBrowser("DocomoISIM60");
            this.Docomoisim60ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoisim60ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoisim60ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoisim60ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoko209iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KO209i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Kokusai";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "96";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoKo209i");
            this.Docomoko209iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoko209iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoko209iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoko209iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoko210iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KO210i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Kokusai";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "96";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoKo210i");
            this.Docomoko210iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoko210iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoko210iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoko210iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon2001Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N2001"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoN2001");
            this.Docomon2001ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon2001ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon2001ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon2001ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon2002Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N2002"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoN2002");
            this.Docomon2002ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon2002ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon2002ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon2002ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon209iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N209i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "82";
            capabilities["screenPixelsWidth"] = "108";
            browserCaps.AddBrowser("DocomoN209i");
            this.Docomon209iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon209iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon209iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon209iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon210iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N210i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "113";
            capabilities["screenPixelsWidth"] = "118";
            browserCaps.AddBrowser("DocomoN210i");
            this.Docomon210iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon210iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon210iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon210iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon211iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N211i"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoN211i");
            this.Docomon211iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon211iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon211iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon211iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon251iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N251i"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "22";
            capabilities["screenPixelsHeight"] = "140";
            capabilities["screenPixelsWidth"] = "132";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoN251i");
            this.Docomon251iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Docomon251isProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Docomon251iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon251iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon251iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon251isProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            string target = (string) browserCaps.Capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N251iS"))
            {
                return false;
            }
            browserCaps.AddBrowser("DocomoN251iS");
            this.Docomon251isProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon251isProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon251isProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon251isProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon501iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N501i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            browserCaps.AddBrowser("DocomoN501i");
            this.Docomon501iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon501iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon501iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon501iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon502iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N502i$"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            browserCaps.AddBrowser("DocomoN502i");
            this.Docomon502iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon502iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon502iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon502iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon502itProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N502it"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            browserCaps.AddBrowser("DocomoN502it");
            this.Docomon502itProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon502itProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon502itProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon502itProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon503iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N503i$"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoN503i");
            this.Docomon503iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon503iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon503iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon503iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon503isProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N503iS"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoN503is");
            this.Docomon503isProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon503isProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon503isProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon503isProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon504iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N504i"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "180";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoN504i");
            this.Docomon504iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon504iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon504iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon504iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon505iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N505i"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "NEC";
            browserCaps.AddBrowser("DocomoN505i");
            this.Docomon505iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon505iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon505iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon505iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomon821iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "N821i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "NEC";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "119";
            browserCaps.AddBrowser("DocomoN821i");
            this.Docomon821iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomon821iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomon821iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomon821iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomonm502iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "NM502i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Nokia";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "106";
            capabilities["screenPixelsWidth"] = "111";
            browserCaps.AddBrowser("DocomoNm502i");
            this.Docomonm502iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomonm502iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomonm502iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomonm502iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop209iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P209i$"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "87";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoP209i");
            this.Docomop209iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop209iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop209iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop209iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop209isProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P209iS"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "87";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoP209is");
            this.Docomop209isProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop209isProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop209isProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop209isProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop2101vProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P2101V"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["screenBitDepth"] = "18";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "182";
            capabilities["screenPixelsWidth"] = "163";
            browserCaps.AddBrowser("DocomoP2101v");
            this.Docomop2101vProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop2101vProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop2101vProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop2101vProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop2102vProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P2102V"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            browserCaps.AddBrowser("DocomoP2102v");
            this.Docomop2102vProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop2102vProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop2102vProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop2102vProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop210iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P210i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "91";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoP210i");
            this.Docomop210iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop210iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop210iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop210iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop211iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P211i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["MobileDeviceManufacturer"] = "Panasonic";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoP211i");
            this.Docomop211iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop211iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop211iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop211iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop501iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P501i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "120";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoP501i");
            this.Docomop501iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop501iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop501iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop501iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop502iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P502i"))
            {
                return false;
            }
            capabilities["canRenderEmptySelects"] = "false";
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "117";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoP502i");
            this.Docomop502iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop502iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop502iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop502iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop503iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P503i$"))
            {
                return false;
            }
            capabilities["canRenderEmptySelects"] = "false";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["rendersBreaksAfterHtmlLists"] = "false";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoP503i");
            this.Docomop503iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop503iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop503iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop503iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop503isProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P503iS"))
            {
                return false;
            }
            capabilities["canRenderEmptySelects"] = "false";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoP503is");
            this.Docomop503isProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop503isProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop503isProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop503isProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop504iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P504i"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["screenCharactersWidth"] = "22";
            capabilities["screenPixelsHeight"] = "144";
            capabilities["screenPixelsWidth"] = "132";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoP504i");
            this.Docomop504iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop504iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop504iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop504iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop505iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P505i"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            browserCaps.AddBrowser("DocomoP505i");
            this.Docomop505iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop505iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop505iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop505iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomop821iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P821i"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["maximumRenderedPageSize"] = "5000";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "118";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoP821i");
            this.Docomop821iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomop821iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomop821iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomop821iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool DocomoProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^DoCoMo/"))
            {
                return false;
            }
            worker.ProcessRegex(browserCaps[string.Empty], @"^DoCoMo/(?'httpVersion'[^/ ]*)[/ ](?'deviceID'[^/\x28]*)");
            capabilities["browser"] = "i-mode";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["cookies"] = "false";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "16";
            capabilities["defaultScreenPixelsHeight"] = "70";
            capabilities["defaultScreenPixelsWidth"] = "90";
            capabilities["deviceID"] = worker["${deviceID}"];
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["javaapplets"] = "false";
            capabilities["javascript"] = "false";
            capabilities["maximumHrefLength"] = "524";
            capabilities["maximumRenderedPageSize"] = "5120";
            capabilities["mobileDeviceModel"] = worker["${deviceID}"];
            capabilities["optimumPageWeight"] = "700";
            capabilities["preferredRenderingType"] = "chtml10";
            capabilities["preferredRequestEncoding"] = "shift_jis";
            capabilities["preferredResponseEncoding"] = "shift_jis";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresFullyQualifiedRedirectUrl"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["supportsAccesskeyAttribute"] = "true";
            capabilities["supportsCharacterEntityEncoding"] = "false";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsIModeSymbols"] = "true";
            capabilities["supportsInputIStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["tables"] = "false";
            capabilities["type"] = "i-mode";
            capabilities["vbscript"] = "false";
            browserCaps.HtmlTextWriter = "System.Web.UI.ChtmlTextWriter";
            browserCaps.AddBrowser("Docomo");
            this.DocomoProcessGateways(headers, browserCaps);
            this.DocomorenderingsizeProcess(headers, browserCaps);
            this.DocomodefaultrenderingsizeProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((((((!this.Docomosh251iProcess(headers, browserCaps) && !this.Docomon251iProcess(headers, browserCaps)) && (!this.Docomop211iProcess(headers, browserCaps) && !this.Docomof212iProcess(headers, browserCaps))) && ((!this.Docomod501iProcess(headers, browserCaps) && !this.Docomof501iProcess(headers, browserCaps)) && (!this.Docomon501iProcess(headers, browserCaps) && !this.Docomop501iProcess(headers, browserCaps)))) && (((!this.Docomod502iProcess(headers, browserCaps) && !this.Docomof502iProcess(headers, browserCaps)) && (!this.Docomon502iProcess(headers, browserCaps) && !this.Docomop502iProcess(headers, browserCaps))) && ((!this.Docomonm502iProcess(headers, browserCaps) && !this.Docomoso502iProcess(headers, browserCaps)) && (!this.Docomof502itProcess(headers, browserCaps) && !this.Docomon502itProcess(headers, browserCaps))))) && ((((!this.Docomoso502iwmProcess(headers, browserCaps) && !this.Docomof504iProcess(headers, browserCaps)) && (!this.Docomon504iProcess(headers, browserCaps) && !this.Docomop504iProcess(headers, browserCaps))) && ((!this.Docomon821iProcess(headers, browserCaps) && !this.Docomop821iProcess(headers, browserCaps)) && (!this.Docomod209iProcess(headers, browserCaps) && !this.Docomoer209iProcess(headers, browserCaps)))) && (((!this.Docomof209iProcess(headers, browserCaps) && !this.Docomoko209iProcess(headers, browserCaps)) && (!this.Docomon209iProcess(headers, browserCaps) && !this.Docomop209iProcess(headers, browserCaps))) && ((!this.Docomop209isProcess(headers, browserCaps) && !this.Docomor209iProcess(headers, browserCaps)) && (!this.Docomor691iProcess(headers, browserCaps) && !this.Docomof503iProcess(headers, browserCaps)))))) && (((((!this.Docomof503isProcess(headers, browserCaps) && !this.Docomod503iProcess(headers, browserCaps)) && (!this.Docomod503isProcess(headers, browserCaps) && !this.Docomod210iProcess(headers, browserCaps))) && ((!this.Docomof210iProcess(headers, browserCaps) && !this.Docomon210iProcess(headers, browserCaps)) && (!this.Docomon2001Process(headers, browserCaps) && !this.Docomod211iProcess(headers, browserCaps)))) && (((!this.Docomon211iProcess(headers, browserCaps) && !this.Docomop210iProcess(headers, browserCaps)) && (!this.Docomoko210iProcess(headers, browserCaps) && !this.Docomop2101vProcess(headers, browserCaps))) && ((!this.Docomop2102vProcess(headers, browserCaps) && !this.Docomof211iProcess(headers, browserCaps)) && (!this.Docomof671iProcess(headers, browserCaps) && !this.Docomon503isProcess(headers, browserCaps))))) && ((((!this.Docomon503iProcess(headers, browserCaps) && !this.Docomoso503iProcess(headers, browserCaps)) && (!this.Docomop503isProcess(headers, browserCaps) && !this.Docomop503iProcess(headers, browserCaps))) && ((!this.Docomoso210iProcess(headers, browserCaps) && !this.Docomoso503isProcess(headers, browserCaps)) && (!this.Docomosh821iProcess(headers, browserCaps) && !this.Docomon2002Process(headers, browserCaps)))) && (((!this.Docomoso505iProcess(headers, browserCaps) && !this.Docomop505iProcess(headers, browserCaps)) && (!this.Docomon505iProcess(headers, browserCaps) && !this.Docomod505iProcess(headers, browserCaps))) && !this.Docomoisim60Process(headers, browserCaps)))))
            {
                ignoreApplicationBrowsers = false;
            }
            this.DocomoProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void DocomoProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void DocomoProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomor209iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R209i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "JRC";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoR209i");
            this.Docomor209iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomor209iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomor209iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomor209iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomor691iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R691i"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "JRC";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("DocomoR691i");
            this.Docomor691iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomor691iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomor691iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomor691iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool DocomorenderingsizeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^DoCoMo/([^/ ]*)[/ ]([^/\x28]*)([/\x28]c(?'cacheSize'\d+))"))
            {
                return false;
            }
            target = (string) capabilities["maximumRenderedPageSize"];
            if (!worker.ProcessRegex(target, "^5120$"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = worker["${cacheSize}000"];
            this.DocomorenderingsizeProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.DocomorenderingsizeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void DocomorenderingsizeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void DocomorenderingsizeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomosh251iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SH251i"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Sharp";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoSH251i");
            this.Docomosh251iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Docomosh251isProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Docomosh251iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomosh251iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomosh251iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomosh251isProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SH251iS"))
            {
                return false;
            }
            capabilities["requiresLeadingPageBreak"] = "false";
            capabilities["screenCharactersHeight"] = "11";
            capabilities["screenCharactersWidth"] = "22";
            capabilities["screenPixelsHeight"] = "220";
            capabilities["screenPixelsWidth"] = "176";
            browserCaps.AddBrowser("DocomoSH251iS");
            this.Docomosh251isProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomosh251isProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomosh251isProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomosh251isProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomosh821iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SH821i"))
            {
                return false;
            }
            capabilities["canRenderEmptySelects"] = "false";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Sharp";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "78";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoSh821i");
            this.Docomosh821iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomosh821iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomosh821iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomosh821iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoso210iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SO210i$"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "17";
            capabilities["screenPixelsHeight"] = "113";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoSo210i");
            this.Docomoso210iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoso210iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoso210iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoso210iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoso502iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SO502i$"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "120";
            capabilities["screenPixelsWidth"] = "120";
            browserCaps.AddBrowser("DocomoSo502i");
            this.Docomoso502iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoso502iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoso502iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoso502iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoso502iwmProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SO502iWM"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "113";
            capabilities["screenPixelsWidth"] = "120";
            browserCaps.AddBrowser("DocomoSo502iwm");
            this.Docomoso502iwmProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoso502iwmProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoso502iwmProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoso502iwmProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoso503iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SO503i$"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "113";
            capabilities["screenPixelsWidth"] = "120";
            browserCaps.AddBrowser("DocomoSo503i");
            this.Docomoso503iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoso503iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoso503iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoso503iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoso503isProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SO503iS"))
            {
                return false;
            }
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "17";
            capabilities["screenPixelsHeight"] = "113";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("DocomoSo503is");
            this.Docomoso503isProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoso503isProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoso503isProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoso503isProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Docomoso505iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SO505i"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "SonyEricsson";
            capabilities["screenBitDepth"] = "18";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "240";
            capabilities["screenPixelsWidth"] = "256";
            browserCaps.AddBrowser("DocomoSo505i");
            this.Docomoso505iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Docomoso505iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Docomoso505iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Docomoso505iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ds10Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "DS10"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Denso";
            capabilities["mobileDeviceModel"] = "Eagle 10";
            browserCaps.AddBrowser("Ds10");
            this.Ds10ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ds10ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ds10ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ds10ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ds15Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "DS15"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Denso";
            capabilities["mobileDeviceModel"] = "Touchpoint DS15";
            browserCaps.AddBrowser("Ds15");
            this.Ds15ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ds15ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ds15ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ds15ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericsson301aProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R301A"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canSendMail"] = "true";
            capabilities["displaysAccessKeysAutomatically"] = "true";
            capabilities["maximumRenderedPageSize"] = "2800";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresNewLineSuppression"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsBold"] = "false";
            capabilities["supportsDivAlign"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "false";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["usePOverDiv"] = "true";
            browserCaps.AddBrowser("Ericsson301A");
            this.Ericsson301aProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericsson301aProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericsson301aProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericsson301aProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssona2628Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "A2628"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "1600";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "54";
            capabilities["screenPixelsWidth"] = "101";
            browserCaps.AddBrowser("EricssonA2628");
            this.Ericssona2628ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssona2628ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssona2628ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssona2628ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssonp800Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P800"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Sony Ericsson";
            capabilities["cookies"] = "true";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Sony Ericsson";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "16";
            capabilities["screenCharactersWidth"] = "28";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "208";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("EricssonP800");
            this.Ericssonp800ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Ericssonp800r101Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Ericssonp800ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssonp800ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssonp800ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssonp800r101Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R101"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "10000";
            browserCaps.AddBrowser("EricssonP800R101");
            this.Ericssonp800r101ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssonp800r101ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssonp800r101ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssonp800r101ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool EricssonProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Ericsson(?'deviceID'[^/]+)/(?'deviceVer'.*)"))
            {
                return false;
            }
            capabilities["browser"] = "Ericsson";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["cookies"] = "false";
            capabilities["defaultScreenCharactersHeight"] = "4";
            capabilities["defaultScreenCharactersWidth"] = "20";
            capabilities["defaultScreenPixelsHeight"] = "52";
            capabilities["defaultScreenPixelsWidth"] = "101";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["maximumRenderedPageSize"] = "1600";
            capabilities["mobileDeviceManufacturer"] = "Ericsson";
            capabilities["mobileDeviceModel"] = worker["${deviceID}"];
            capabilities["mobileDeviceVersion"] = worker["${deviceVer}"];
            capabilities["numberOfSoftkeys"] = "2";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml11";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["type"] = worker["Ericsson ${deviceID}"];
            browserCaps.HtmlTextWriter = "System.Web.UI.XhtmlTextWriter";
            browserCaps.AddBrowser("Ericsson");
            this.EricssonProcessGateways(headers, browserCaps);
            this.SonyericssonProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((((!this.Ericssonr320Process(headers, browserCaps) && !this.Ericssont20Process(headers, browserCaps)) && (!this.Ericssont65Process(headers, browserCaps) && !this.Ericssont68Process(headers, browserCaps))) && ((!this.Ericssont300Process(headers, browserCaps) && !this.Ericssonp800Process(headers, browserCaps)) && (!this.Ericssont61Process(headers, browserCaps) && !this.Ericssont31Process(headers, browserCaps)))) && ((!this.Ericssonr520Process(headers, browserCaps) && !this.Ericssona2628Process(headers, browserCaps)) && !this.Ericssont39Process(headers, browserCaps)))
            {
                ignoreApplicationBrowsers = false;
            }
            this.EricssonProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void EricssonProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void EricssonProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssonr320Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R320"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "3000";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "52";
            capabilities["screenPixelsWidth"] = "101";
            browserCaps.AddBrowser("EricssonR320");
            this.Ericssonr320ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssonr320ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssonr320ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssonr320ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssonr380Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"R380 (?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*) WAP1\.1"))
            {
                return false;
            }
            capabilities["browser"] = "Ericsson";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["cookies"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "3000";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceManufacturer"] = "Ericsson";
            capabilities["mobileDeviceModel"] = "R380";
            capabilities["requiresNoescapedPostUrl"] = "false";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml11";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenPixelsHeight"] = "100";
            capabilities["screenPixelsWidth"] = "310";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["type"] = "Ericsson R380";
            capabilities["version"] = worker["${browserMajorVersion}.${browserMinorVersion}"];
            browserCaps.AddBrowser("EricssonR380");
            this.Ericssonr380ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssonr380ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssonr380ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssonr380ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssonr520Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R520"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "1600";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "67";
            capabilities["screenPixelsWidth"] = "101";
            browserCaps.AddBrowser("EricssonR520");
            this.Ericssonr520ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssonr520ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssonr520ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssonr520ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont20Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "T20"))
            {
                return false;
            }
            capabilities["canSendMail"] = "false";
            capabilities["maximumRenderedPageSize"] = "1400";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["mobileDeviceModel"] = "T20, T20e, T29s";
            capabilities["numberOfSoftkeys"] = "1";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenCharactersHeight"] = "3";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "33";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("EricssonT20");
            this.Ericssont20ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont20ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont20ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont20ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont300Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "T300"))
            {
                return false;
            }
            capabilities["breaksOnBlockElements"] = "false";
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["cookies"] = "true";
            capabilities["displaysAccessKeysAutomatically"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "2800";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsDivAlign"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "false";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["supportsWtai"] = "true";
            capabilities["tables"] = "false";
            browserCaps.AddBrowser("EricssonT300");
            this.Ericssont300ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont300ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont300ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont300ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont31Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "T310|T312|T316"))
            {
                return false;
            }
            capabilities["breaksOnBlockElements"] = "false";
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["cookies"] = "true";
            capabilities["displaysAccessKeysAutomatically"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "2800";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["screenPixelsHeight"] = "80";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsDivAlign"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "false";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            browserCaps.AddBrowser("EricssonT31");
            this.Ericssont31ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont31ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont31ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont31ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont39Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "T39"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["maximumRenderedPageSize"] = "3000";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["mobileDeviceManufacturer"] = "Ericsson";
            capabilities["mobileDeviceModel"] = "Ericsson T39";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "3";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "54";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("EricssonT39");
            this.Ericssont39ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont39ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont39ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont39ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont61Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "T610|T616|T618"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["cookies"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "9800";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "true";
            capabilities["requiredOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "128";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsSelectFollowingTable"] = "false";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            browserCaps.AddBrowser("EricssonT61");
            this.Ericssont61ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont61ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont61ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont61ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont65Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "T65"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "3000";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["mobileDeviceModel"] = "Ericsson T65";
            capabilities["numberOfSoftkeys"] = "1";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "67";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("EricssonT65");
            this.Ericssont65ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont65ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont65ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont65ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont68Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "T68"))
            {
                return false;
            }
            capabilities["canSendMail"] = "false";
            capabilities["isColor"] = "true";
            capabilities["numberOfSoftkeys"] = "1";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "false";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "80";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            browserCaps.AddBrowser("EricssonT68");
            this.Ericssont68ProcessGateways(headers, browserCaps);
            this.Ericssont68upgatewayProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Ericsson301aProcess(headers, browserCaps) && !this.Ericssont68r1aProcess(headers, browserCaps)) && (!this.Ericssont68r101Process(headers, browserCaps) && !this.Ericssont68r201aProcess(headers, browserCaps)))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Ericssont68ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont68ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont68ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont68r101Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R101"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2900";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["requiresNoSoftkeyLabels"] = "true";
            browserCaps.AddBrowser("EricssonT68R101");
            this.Ericssont68r101ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont68r101ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont68r101ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont68r101ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont68r1aProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R1A"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["maximumRenderedPageSize"] = "5000";
            capabilities["maximumSoftkeyLabelLength"] = "14";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            browserCaps.AddBrowser("EricssonT68R1A");
            this.Ericssont68r1aProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont68r1aProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont68r1aProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont68r1aProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont68r201aProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "R201A"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["maximumRenderedPageSize"] = "2900";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["screenBitDepth"] = "24";
            browserCaps.AddBrowser("EricssonT68R201A");
            this.Ericssont68r201aProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont68r201aProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont68r201aProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont68r201aProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ericssont68upgatewayProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"UP\.Link"))
            {
                return false;
            }
            capabilities["cookies"] = "true";
            this.Ericssont68upgatewayProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ericssont68upgatewayProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ericssont68upgatewayProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ericssont68upgatewayProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Eudoraweb21plusProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"([3-9]\.\d+)|(2\.[1-9]\d*)"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "true";
            capabilities["screenCharactersHeight"] = "11";
            capabilities["screenCharactersWidth"] = "30";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("Eudoraweb21Plus");
            this.Eudoraweb21plusProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Eudoraweb21plusProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Eudoraweb21plusProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Eudoraweb21plusProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool EudorawebmsieProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"MSIE (?'version'(?'msMajorVersion'\d+)(?'msMinorVersion'\.\d+)(?'letters'\w*))(?'extra'[^)]*)"))
            {
                return false;
            }
            capabilities["activexcontrols"] = "true";
            capabilities["backgroundsounds"] = "true";
            capabilities["ecmaScriptVersion"] = "1.2";
            capabilities["frames"] = "true";
            capabilities["msdomversion"] = worker["${msMajorVersion}${msMinorVersion}"];
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["w3cdomversion"] = "1.0";
            this.EudorawebmsieProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.EudorawebmsieProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void EudorawebmsieProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void EudorawebmsieProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool EudorawebProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"EudoraWeb (?'browserMajorVersion'\d+)(?'browserMinorVersion'\.\d+)"))
            {
                return false;
            }
            capabilities["browser"] = "EudoraWeb";
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["cookies"] = "true";
            capabilities["isColor"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isMobileDevice"] = "true";
            capabilities["javaapplets"] = "false";
            capabilities["javascript"] = "false";
            capabilities["maximumRenderedPageSize"] = "30000";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceManufacturer"] = "PalmOS-licensee";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "36";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsImageSubmit"] = "false";
            capabilities["supportsItalic"] = "true";
            capabilities["tables"] = "false";
            capabilities["type"] = "EudoraWeb";
            capabilities["vbscript"] = "false";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("Eudoraweb");
            this.EudorawebProcessGateways(headers, browserCaps);
            this.EudorawebmsieProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.PdqbrowserProcess(headers, browserCaps) && !this.Eudoraweb21plusProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.EudorawebProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void EudorawebProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void EudorawebProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool EzwapProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"EzWAP (?'browserMajorVersion'\d+)(?'browserMinorVersion'\.\d+)"))
            {
                return false;
            }
            capabilities["browser"] = "EzWAP";
            capabilities["canSendMail"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresXhtmlCssSuppression"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "33";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "240";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsDivAlign"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsStyleElement"] = "false";
            capabilities["supportsUrlAttributeEncoding"] = "false";
            capabilities["tables"] = "true";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.AddBrowser("EzWAP");
            this.EzwapProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.EzwapProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void EzwapProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void EzwapProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GatablefalseProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-GA-TABLES"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "(?i:FALSE)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["tables"] = "false";
            this.GatablefalseProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.GatablefalseProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GatablefalseProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GatablefalseProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GatableProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = headers["X-GA-TABLES"];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            this.GatableProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.GatablefalseProcess(headers, browserCaps) && !this.GatabletrueProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.GatableProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GatableProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GatableProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GatabletrueProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-GA-TABLES"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "(?i:TRUE)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["tables"] = "true";
            this.GatabletrueProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.GatabletrueProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GatabletrueProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GatabletrueProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GeckoProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Gecko"))
            {
                return false;
            }
            capabilities["browser"] = "Mozilla";
            capabilities["css1"] = "true";
            capabilities["css2"] = "true";
            capabilities["cookies"] = "true";
            capabilities["ecmascriptversion"] = "1.5";
            capabilities["frames"] = "true";
            capabilities["isColor"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["maximumRenderedPageSize"] = "20000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["screenBitDepth"] = "32";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFileUpload"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "desktop";
            capabilities["version"] = worker["${version}"];
            capabilities["w3cdomversion"] = "1.0";
            browserCaps.AddBrowser("Gecko");
            this.GeckoProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.MozillarvProcess(headers, browserCaps) && !this.SafariProcess(headers, browserCaps)) && !this.Netscape5Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.GeckoProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GeckoProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GeckoProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GenericdownlevelProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^Generic Downlevel$"))
            {
                return false;
            }
            capabilities["cookies"] = "false";
            capabilities["ecmascriptversion"] = "1.0";
            capabilities["tables"] = "true";
            capabilities["type"] = "Downlevel";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("GenericDownlevel");
            this.GenericdownlevelProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.GenericdownlevelProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GenericdownlevelProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GenericdownlevelProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Gm832Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "GM832"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Telit";
            capabilities["mobileDeviceModel"] = "GM832";
            browserCaps.AddBrowser("Gm832");
            this.Gm832ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Gm832ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Gm832ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Gm832ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Gm910iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Telit-GM910i"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Telit";
            capabilities["mobileDeviceModel"] = "GM910i";
            browserCaps.AddBrowser("Gm910i");
            this.Gm910iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Gm910iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Gm910iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Gm910iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Goamerica7to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^[7-9]$"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["requiresAttributeColonSubstitution"] = "false";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenCharactersHeight"] = "16";
            capabilities["screenCharactersWidth"] = "31";
            capabilities["supportsUncheck"] = "false";
            browserCaps.AddBrowser("GoAmerica7to9");
            this.Goamerica7to9ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Goamerica7to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Goamerica7to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Goamerica7to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GoamericanonuprimProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (worker.ProcessRegex(target, @"UP\.Browser"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.1";
            capabilities["frames"] = "true";
            this.GoamericanonuprimProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.GoamericanonuprimProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GoamericanonuprimProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GoamericanonuprimProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GoamericapalmProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Palm"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "PalmOS-licensee";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "36";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsUncheck"] = "false";
            browserCaps.AddBrowser("GoAmericaPalm");
            this.GoamericapalmProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.GoamericapalmProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GoamericapalmProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GoamericapalmProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GoamericaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Go\.Web/(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*)"))
            {
                return false;
            }
            capabilities["BackgroundSounds"] = "true";
            capabilities["browser"] = "Go.Web";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "true";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "12";
            capabilities["defaultScreenPixelsHeight"] = "72";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["isMobileDevice"] = "true";
            capabilities["javaapplets"] = "false";
            capabilities["javascript"] = "false";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "6000";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["rendersBreaksAfterHtmlLists"] = "false";
            capabilities["requiredMetaTagNameValue"] = "HandheldFriendly";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresNoBreakInFormatting"] = "true";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "true";
            capabilities["screenBitDepth"] = "2";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivAlign"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["SupportsDivNoWrap"] = "false";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "false";
            capabilities["supportsSelectMultiple"] = "false";
            capabilities["type"] = "Go.Web";
            capabilities["vbscript"] = "false";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("GoAmerica");
            this.GoamericaProcessGateways(headers, browserCaps);
            this.GoamericaupProcess(headers, browserCaps);
            this.GatableProcess(headers, browserCaps);
            this.MaxpagesizeProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.GoamericawinceProcess(headers, browserCaps) && !this.GoamericapalmProcess(headers, browserCaps)) && !this.GoamericarimProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.GoamericaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GoamericaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GoamericaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Goamericarim850Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "RIM850"))
            {
                return false;
            }
            capabilities["mobileDeviceModel"] = "850";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "25";
            capabilities["screenPixelsHeight"] = "64";
            capabilities["screenPixelsWidth"] = "132";
            browserCaps.AddBrowser("GoAmericaRIM850");
            this.Goamericarim850ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Goamericarim850ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Goamericarim850ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Goamericarim850ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Goamericarim857major6minor2to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"6\.[2-9]"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["requiresAttributeColonSubstitution"] = "false";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenCharactersHeight"] = "16";
            capabilities["screenCharactersWidth"] = "31";
            capabilities["supportsUncheck"] = "false";
            browserCaps.AddBrowser("GoAmericaRIM857major6minor2to9");
            this.Goamericarim857major6minor2to9ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Goamericarim857major6minor2to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Goamericarim857major6minor2to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Goamericarim857major6minor2to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Goamericarim857major6Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"6\."))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.1";
            capabilities["frames"] = "true";
            browserCaps.AddBrowser("GoAmericaRIM857major6");
            this.Goamericarim857major6ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Goamericarim857major6minor2to9Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Goamericarim857major6ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Goamericarim857major6ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Goamericarim857major6ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Goamericarim857Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "RIM857"))
            {
                return false;
            }
            capabilities["BackgroundSounds"] = "false";
            capabilities["ecmascriptversion"] = "0.0";
            capabilities["frames"] = "false";
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["mobileDeviceModel"] = "857";
            capabilities["screenCharactersHeight"] = "15";
            capabilities["screenCharactersWidth"] = "32";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "160";
            browserCaps.AddBrowser("GoAmericaRIM857");
            this.Goamericarim857ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Goamericarim857major6Process(headers, browserCaps) && !this.Goamerica7to9Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Goamericarim857ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Goamericarim857ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Goamericarim857ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Goamericarim950Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "RIM950"))
            {
                return false;
            }
            capabilities["BackgroundSounds"] = "false";
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["mobileDeviceModel"] = "950";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "25";
            capabilities["screenPixelsHeight"] = "64";
            capabilities["screenPixelsWidth"] = "132";
            browserCaps.AddBrowser("GoAmericaRIM950");
            this.Goamericarim950ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Goamericarim950ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Goamericarim950ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Goamericarim950ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Goamericarim957major6minor2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"6\.2"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["maximumRenderedPageSize"] = "7168";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "true";
            capabilities["screenCharactersHeight"] = "13";
            capabilities["screenCharactersWidth"] = "30";
            capabilities["supportsSelectMultiple"] = "true";
            capabilities["supportsUncheck"] = "true";
            browserCaps.AddBrowser("GoAmericaRIM957major6minor2");
            this.Goamericarim957major6minor2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Goamericarim957major6minor2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Goamericarim957major6minor2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Goamericarim957major6minor2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Goamericarim957Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "RIM957"))
            {
                return false;
            }
            capabilities["BackgroundSounds"] = "false";
            capabilities["mobileDeviceModel"] = "957";
            capabilities["screenCharactersHeight"] = "15";
            capabilities["screenCharactersWidth"] = "32";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "160";
            browserCaps.AddBrowser("GoAmericaRIM957");
            this.Goamericarim957ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Goamericarim957major6minor2Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Goamericarim957ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Goamericarim957ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Goamericarim957ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GoamericarimProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "RIM"))
            {
                return false;
            }
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "false";
            capabilities["mobileDeviceManufacturer"] = "RIM";
            capabilities["screenBitDepth"] = "1";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivAlign"] = "false";
            capabilities["supportsDivWrap"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["supportsFontItalic"] = "false";
            browserCaps.AddBrowser("GoAmericaRIM");
            this.GoamericarimProcessGateways(headers, browserCaps);
            this.GoamericanonuprimProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Goamericarim950Process(headers, browserCaps) && !this.Goamericarim850Process(headers, browserCaps)) && (!this.Goamericarim957Process(headers, browserCaps) && !this.Goamericarim857Process(headers, browserCaps)))
            {
                ignoreApplicationBrowsers = false;
            }
            this.GoamericarimProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GoamericarimProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GoamericarimProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GoamericaupProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"UP\.Browser"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "0.0";
            capabilities["frames"] = "false";
            this.GoamericaupProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.GoamericaupProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GoamericaupProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GoamericaupProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool GoamericawinceProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "WinCE"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["defaultScreenCharactersHeight"] = "14";
            capabilities["defaultScreenCharactersWidth"] = "30";
            capabilities["defaultScreenPixelsHeight"] = "320";
            capabilities["defaultScreenPixelsWidth"] = "240";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["mobileDeviceModel"] = "Pocket PC";
            capabilities["platform"] = "WinCE";
            capabilities["screenBitDepth"] = "16";
            capabilities["supportsDivAlign"] = "true";
            capabilities["supportsFontColor"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsSelectMultiple"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("GoAmericaWinCE");
            this.GoamericawinceProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.GoamericawinceProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void GoamericawinceProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void GoamericawinceProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Hitachip300Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Hitachi-P300"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceManufacturer"] = "Hitachi";
            capabilities["mobileDeviceModel"] = "SH-P300";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "17";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("HitachiP300");
            this.Hitachip300ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Hitachip300ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Hitachip300ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Hitachip300ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie1minor5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^1\.5"))
            {
                return false;
            }
            capabilities["cookies"] = "true";
            capabilities["tables"] = "true";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("IE1minor5");
            this.Ie1minor5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie1minor5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie1minor5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie1minor5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^2"))
            {
                return false;
            }
            capabilities["backgroundsounds"] = "true";
            capabilities["cookies"] = "true";
            capabilities["tables"] = "true";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("IE2");
            this.Ie2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.WebtvProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Ie2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie3akProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["extra"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "; AK;"))
            {
                return false;
            }
            capabilities["ak"] = "true";
            this.Ie3akProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie3akProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie3akProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie3akProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie3macProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "PPC Mac|Macintosh.*(68K|PPC)|Mac_(PowerPC|PPC|68(K|000))"))
            {
                return false;
            }
            capabilities["activexcontrols"] = "false";
            capabilities["vbscript"] = "false";
            browserCaps.AddBrowser("IE3Mac");
            this.Ie3macProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie3macProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie3macProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie3macProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie3Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^3"))
            {
                return false;
            }
            capabilities["activexcontrols"] = "true";
            capabilities["backgroundsounds"] = "true";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "true";
            capabilities["ecmascriptversion"] = "1.0";
            capabilities["frames"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["jscriptversion"] = "1.0";
            capabilities["supportsMultilineTextBoxDisplay"] = "false";
            capabilities["tables"] = "true";
            capabilities["vbscript"] = "true";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("IE3");
            this.Ie3ProcessGateways(headers, browserCaps);
            this.Ie3akProcess(headers, browserCaps);
            this.Ie3skProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Ie3win16Process(headers, browserCaps) && !this.Ie3macProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Ie3ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie3ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie3ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie3skProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["extra"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "; SK;"))
            {
                return false;
            }
            capabilities["sk"] = "true";
            this.Ie3skProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie3skProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie3skProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie3skProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie3win16aProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["extra"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^a"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            capabilities["javascript"] = "false";
            capabilities["vbscript"] = "false";
            browserCaps.AddBrowser("IE3win16a");
            this.Ie3win16aProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie3win16aProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie3win16aProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie3win16aProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie3win16Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"16bit|Win(dows 3\.1|16)"))
            {
                return false;
            }
            capabilities["activexcontrols"] = "false";
            capabilities["javaapplets"] = "false";
            browserCaps.AddBrowser("IE3win16");
            this.Ie3win16ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Ie3win16aProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Ie3win16ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie3win16ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie3win16ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MSIE 4"))
            {
                return false;
            }
            capabilities["activexcontrols"] = "true";
            capabilities["backgroundsounds"] = "true";
            capabilities["cdf"] = "true";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "true";
            capabilities["ecmascriptversion"] = "1.2";
            capabilities["frames"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["jscriptversion"] = "3.0";
            capabilities["msdomversion"] = "4.0";
            capabilities["supportsFileUpload"] = "true";
            capabilities["supportsMultilineTextBoxDisplay"] = "false";
            capabilities["supportsMaintainScrollPositionOnPostback"] = "true";
            capabilities["tables"] = "true";
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["vbscript"] = "true";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("IE4");
            this.Ie4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie50Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            string target = (string) browserCaps.Capabilities["minorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^\.0"))
            {
                return false;
            }
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("IE50");
            this.Ie50ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie50ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie50ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie50ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie55Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["minorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^\.5"))
            {
                return false;
            }
            capabilities["jscriptversion"] = "5.5";
            capabilities["ExchangeOmaSupported"] = "true";
            browserCaps.AddBrowser("IE55");
            this.Ie55ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie55ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie55ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie55ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            string target = (string) browserCaps.Capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^5$"))
            {
                return false;
            }
            browserCaps.AddBrowser("IE5");
            this.Ie5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Ie50Process(headers, browserCaps) && !this.Ie55Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Ie5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie5to9macProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            string target = (string) browserCaps.Capabilities["platform"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "(MacPPC|Mac68K)"))
            {
                return false;
            }
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("IE5to9Mac");
            this.Ie5to9macProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ie5to9macProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie5to9macProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie5to9macProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie5to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^[5-9]|[1-9]\d+"))
            {
                return false;
            }
            capabilities["activexcontrols"] = "true";
            capabilities["backgroundsounds"] = "true";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "true";
            capabilities["css2"] = "true";
            capabilities["ecmascriptversion"] = "1.2";
            capabilities["frames"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["jscriptversion"] = "5.0";
            capabilities["msdomversion"] = worker["${majorversion}${minorversion}"];
            capabilities["supportsCallback"] = "true";
            capabilities["supportsFileUpload"] = "true";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["supportsMaintainScrollPositionOnPostback"] = "true";
            capabilities["supportsVCard"] = "true";
            capabilities["supportsXmlHttp"] = "true";
            capabilities["tables"] = "true";
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["vbscript"] = "true";
            capabilities["w3cdomversion"] = "1.0";
            capabilities["xml"] = "true";
            browserCaps.AddBrowser("IE5to9");
            this.Ie5to9ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Ie6to9Process(headers, browserCaps) && !this.Ie5Process(headers, browserCaps)) && !this.Ie5to9macProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Ie5to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie5to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie5to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ie6to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"[6-9]|[1-9]\d+"))
            {
                return false;
            }
            capabilities["jscriptversion"] = "5.6";
            capabilities["ExchangeOmaSupported"] = "true";
            browserCaps.AddBrowser("IE6to9");
            this.Ie6to9ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Treo600Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Ie6to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ie6to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ie6to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool IeaolProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["extra"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "; AOL"))
            {
                return false;
            }
            capabilities["aol"] = "true";
            capabilities["frames"] = "true";
            this.IeaolProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.IeaolProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void IeaolProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void IeaolProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool IebetaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["letters"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^([bB]|ab)"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            this.IebetaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.IebetaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void IebetaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void IebetaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool IeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Mozilla[^(]*\([C|c]ompatible;\s*MSIE (?'version'(?'major'\d+)(?'minor'\.\d+)(?'letters'\w*))(?'extra'[^)]*)"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, @"Opera|Go\.Web|Windows CE|EudoraWeb"))
            {
                return false;
            }
            capabilities["browser"] = "IE";
            capabilities["extra"] = worker["${extra}"];
            capabilities["isColor"] = "true";
            capabilities["letters"] = worker["${letters}"];
            capabilities["majorversion"] = worker["${major}"];
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["screenBitDepth"] = "8";
            capabilities["type"] = worker["IE${major}"];
            capabilities["version"] = worker["${version}"];
            browserCaps.AddBrowser("IE");
            this.IeProcessGateways(headers, browserCaps);
            this.IeaolProcess(headers, browserCaps);
            this.IebetaProcess(headers, browserCaps);
            this.IeupdateProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (((!this.Ie5to9Process(headers, browserCaps) && !this.Ie4Process(headers, browserCaps)) && (!this.Ie3Process(headers, browserCaps) && !this.Ie2Process(headers, browserCaps))) && !this.Ie1minor5Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.IeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void IeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void IeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool IeupdateProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["extra"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "; Update a;"))
            {
                return false;
            }
            capabilities["authenticodeupdate"] = "true";
            this.IeupdateProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.IeupdateProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void IeupdateProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void IeupdateProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ig01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "IG01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "NeoPoint";
            capabilities["mobileDeviceModel"] = "NP1000";
            browserCaps.AddBrowser("Ig01");
            this.Ig01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ig01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ig01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ig01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ig02Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "IG02"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "NeoPoint";
            capabilities["mobileDeviceModel"] = "NP1660";
            browserCaps.AddBrowser("Ig02");
            this.Ig02ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ig02ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ig02ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ig02ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ig03Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "IG03"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "NeoPoint";
            capabilities["mobileDeviceModel"] = "NP2000";
            browserCaps.AddBrowser("Ig03");
            this.Ig03ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ig03ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ig03ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ig03ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Im1kProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "IM1K"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "iDEN";
            browserCaps.AddBrowser("Im1k");
            this.Im1kProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Im1kProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Im1kProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Im1kProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool IscolorfalseProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-ISCOLOR"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "0"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["isColor"] = "false";
            this.IscolorfalseProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.IscolorfalseProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void IscolorfalseProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void IscolorfalseProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool IscolorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-ISCOLOR"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, ".+"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            this.IscolorProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.IscolortrueProcess(headers, browserCaps) && !this.IscolorfalseProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.IscolorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void IscolorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void IscolorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool IscolortrueProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-ISCOLOR"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "1"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["isColor"] = "true";
            this.IscolortrueProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.IscolortrueProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void IscolortrueProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void IscolortrueProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JataayuppcProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "(PPC)"))
            {
                return false;
            }
            capabilities["cookies"] = "true";
            capabilities["screenCharactersHeight"] = "14";
            capabilities["screenCharactersWidth"] = "31";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "240";
            capabilities["supportsStyleElement"] = "true";
            browserCaps.AddBrowser("JataayuPPC");
            this.JataayuppcProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JataayuppcProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JataayuppcProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JataayuppcProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JataayuProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^jBrowser"))
            {
                return false;
            }
            capabilities["browser"] = "Jataayu jBrowser";
            capabilities["canSendMail"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceManufacturer"] = "Jataayu";
            capabilities["mobileDeviceModel"] = "jBrowser";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "wml20";
            capabilities["requiresCommentInStyleElement"] = "true";
            capabilities["requiresHiddenFieldValues"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "true";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "17";
            capabilities["screenCharactersWidth"] = "42";
            capabilities["screenPixelsHeight"] = "265";
            capabilities["screenPixelsWidth"] = "248";
            capabilities["supportsBodyClassAttribute"] = "false";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["tables"] = "true";
            capabilities["type"] = "Jataayu jBrowser";
            browserCaps.AddBrowser("Jataayu");
            this.JataayuProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.JataayuppcProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.JataayuProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JataayuProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JataayuProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphone16bitcolorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["bitDepth"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^65536$"))
            {
                return false;
            }
            capabilities["screenBitDepth"] = "16";
            this.Jphone16bitcolorProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphone16bitcolorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphone16bitcolorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphone16bitcolorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphone2bitcolorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["bitDepth"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^4$"))
            {
                return false;
            }
            capabilities["screenBitDepth"] = "2";
            this.Jphone2bitcolorProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphone2bitcolorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphone2bitcolorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphone2bitcolorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphone4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "4"))
            {
                return false;
            }
            capabilities["supportsQueryStringInFormAction"] = "true";
            this.Jphone4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphone4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphone4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphone4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphone8bitcolorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["bitDepth"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^256$"))
            {
                return false;
            }
            capabilities["screenBitDepth"] = "8";
            this.Jphone8bitcolorProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphone8bitcolorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphone8bitcolorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphone8bitcolorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonecoloriscolorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["colorIndicator"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "C"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            this.JphonecoloriscolorProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JphonecoloriscolorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonecoloriscolorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonecoloriscolorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonecolorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-JPHONE-COLOR"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'colorIndicator'[CG])(?'bitDepth'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["bitDepth"] = worker["${bitDepth}"];
            capabilities["colorIndicator"] = worker["${colorIndicator}"];
            this.JphonecolorProcessGateways(headers, browserCaps);
            this.JphonecoloriscolorProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Jphone16bitcolorProcess(headers, browserCaps) && !this.Jphone8bitcolorProcess(headers, browserCaps)) && !this.Jphone2bitcolorProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.JphonecolorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonecolorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonecolorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonedensoProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"J-DN\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Denso";
            browserCaps.AddBrowser("JphoneDenso");
            this.JphonedensoProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JphonedensoProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonedensoProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonedensoProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonedisplayProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-JPHONE-DISPLAY"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'screenWidth'\d+)\*(?'screenHeight'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            this.JphonedisplayProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JphonedisplayProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonedisplayProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonedisplayProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonekenwoodProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"J-K\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kenwood";
            browserCaps.AddBrowser("JphoneKenwood");
            this.JphonekenwoodProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JphonekenwoodProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonekenwoodProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonekenwoodProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonemitsubishiProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^J-D\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            browserCaps.AddBrowser("JphoneMitsubishi");
            this.JphonemitsubishiProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JphonemitsubishiProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonemitsubishiProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonemitsubishiProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonenecn51Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "J-N51"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["mobileDeviceModel"] = "J-N51";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["supportsCharacterEntityEncoding"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsInputIStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneNecN51");
            this.Jphonenecn51ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonenecn51ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonenecn51ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonenecn51ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonenecProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"J-N\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "NEC";
            browserCaps.AddBrowser("JphoneNec");
            this.JphonenecProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Jphonenecn51Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.JphonenecProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonenecProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonenecProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonepanasonicProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"J-P\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            browserCaps.AddBrowser("JphonePanasonic");
            this.JphonepanasonicProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JphonepanasonicProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonepanasonicProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonepanasonicProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonepioneerProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"J-PE\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Pioneer";
            browserCaps.AddBrowser("JphonePioneer");
            this.JphonepioneerProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JphonepioneerProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonepioneerProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonepioneerProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphoneProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^J-PHONE/"))
            {
                return false;
            }
            worker.ProcessRegex(browserCaps[string.Empty], @"J-PHONE/(?'majorVersion'\d+)(?'minorVersion'\.\d+)/(?'deviceModel'.*)");
            capabilities["browser"] = "J-Phone";
            capabilities["cookies"] = "false";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["defaultCharacterHeight"] = "12";
            capabilities["defaultCharacterWidth"] = "12";
            capabilities["defaultScreenCharactersHeight"] = "7";
            capabilities["defaultScreenCharactersWidth"] = "16";
            capabilities["defaultScreenPixelsHeight"] = "84";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["javaapplets"] = "false";
            capabilities["javascript"] = "false";
            capabilities["majorVersion"] = worker["${majorVersion}"];
            capabilities["maximumRenderedPageSize"] = "6000";
            capabilities["minorVersion"] = worker["${minorVersion}"];
            capabilities["mobileDeviceModel"] = worker["${deviceModel}"];
            capabilities["optimumPageWeight"] = "700";
            capabilities["preferredImageMime"] = "image/png";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["preferredRequestEncoding"] = "shift_jis";
            capabilities["preferredResponseEncoding"] = "shift_jis";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresContentTypeMetaTag"] = "true";
            capabilities["requiresFullyQualifiedRedirectUrl"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["screenBitDepth"] = "2";
            capabilities["supportsAccesskeyAttribute"] = "true";
            capabilities["supportsBodyColor"] = "true";
            capabilities["supportsBold"] = "false";
            capabilities["supportsCharacterEntityEncoding"] = "false";
            capabilities["supportsDivAlign"] = "true";
            capabilities["supportsDivNoWrap"] = "true";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsFontColor"] = "true";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["supportsInputMode"] = "true";
            capabilities["supportsItalic"] = "false";
            capabilities["supportsJPhoneMultiMediaAttributes"] = "true";
            capabilities["supportsJPhoneSymbols"] = "true";
            capabilities["supportsQueryStringInFormAction"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["tables"] = "true";
            capabilities["type"] = "J-Phone";
            capabilities["vbscript"] = "false";
            capabilities["version"] = worker["${majorVersion}${minorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.ChtmlTextWriter";
            browserCaps.AddBrowser("Jphone");
            this.JphoneProcessGateways(headers, browserCaps);
            this.Jphone4Process(headers, browserCaps);
            this.JphonecolorProcess(headers, browserCaps);
            this.JphonedisplayProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((((!this.JphonemitsubishiProcess(headers, browserCaps) && !this.JphonedensoProcess(headers, browserCaps)) && (!this.JphonekenwoodProcess(headers, browserCaps) && !this.JphonenecProcess(headers, browserCaps))) && ((!this.JphonepanasonicProcess(headers, browserCaps) && !this.JphonepioneerProcess(headers, browserCaps)) && (!this.JphonesanyoProcess(headers, browserCaps) && !this.JphonesharpProcess(headers, browserCaps)))) && !this.JphonetoshibaProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.JphoneProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphoneProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphoneProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonesa51Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^J-SA51\D*"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["mobileDeviceModel"] = "J-SA51";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenCharactersWidth"] = "22";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsInputIStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneSA51");
            this.Jphonesa51ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonesa51ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonesa51ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonesa51ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonesanyoProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"J-SA\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            browserCaps.AddBrowser("JphoneSanyo");
            this.JphonesanyoProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Jphonesa51Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.JphonesanyoProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonesanyoProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonesanyoProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonesharpProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"J-SH\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sharp";
            browserCaps.AddBrowser("JphoneSharp");
            this.JphonesharpProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (((!this.Jphonesharpsh53Process(headers, browserCaps) && !this.Jphonesharpsh07Process(headers, browserCaps)) && (!this.Jphonesharpsh08Process(headers, browserCaps) && !this.Jphonesharpsh51Process(headers, browserCaps))) && !this.Jphonesharpsh52Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.JphonesharpProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonesharpProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonesharpProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonesharpsh07Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^J-SH07"))
            {
                return false;
            }
            capabilities["canRenderEmptySelects"] = "false";
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["requiresLeadingPageBreak"] = "false";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneSharpSh07");
            this.Jphonesharpsh07ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonesharpsh07ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonesharpsh07ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonesharpsh07ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonesharpsh08Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^J-SH08"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["supportsInputIStyle"] = "false";
            capabilities["requiresLeadingPageBreak"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "117";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneSharpSh08");
            this.Jphonesharpsh08ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonesharpsh08ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonesharpsh08ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonesharpsh08ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonesharpsh51Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^J-SH51"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["supportsInputIStyle"] = "true";
            capabilities["requiresLeadingPageBreak"] = "false";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneSharpSh51");
            this.Jphonesharpsh51ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonesharpsh51ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonesharpsh51ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonesharpsh51ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonesharpsh52Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^J-SH52\D*"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["mobileDeviceModel"] = "J-SH52";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsInputIStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneSharpSh52");
            this.Jphonesharpsh52ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonesharpsh52ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonesharpsh52ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonesharpsh52ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonesharpsh53Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "J-SH53"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["mobileDeviceModel"] = "J-SH53";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["screenBitDepth"] = "18";
            capabilities["screenCharactersHeight"] = "13";
            capabilities["screenCharactersWidth"] = "24";
            capabilities["supportsCharacterEntityEncoding"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsInputIStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneSharpSh53");
            this.Jphonesharpsh53ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonesharpsh53ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonesharpsh53ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonesharpsh53ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JphonetoshibaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"J-T\d"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            browserCaps.AddBrowser("JphoneToshiba");
            this.JphonetoshibaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Jphonetoshibat06aProcess(headers, browserCaps) && !this.Jphonetoshibat08Process(headers, browserCaps)) && !this.Jphonetoshibat51Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.JphonetoshibaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JphonetoshibaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JphonetoshibaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonetoshibat06aProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^J-T06_a"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["mobileDeviceModel"] = "J-T06";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneToshibaT06a");
            this.Jphonetoshibat06aProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonetoshibat06aProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonetoshibat06aProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonetoshibat06aProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonetoshibat08Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^J-T08\D*"))
            {
                return false;
            }
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "22";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneToshibaT08");
            this.Jphonetoshibat08ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonetoshibat08ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonetoshibat08ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonetoshibat08ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jphonetoshibat51Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^J-T51"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["requiresLeadingPageBreak"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "24";
            capabilities["screenPixelsHeight"] = "144";
            capabilities["screenPixelsWidth"] = "144";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsInputIStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            browserCaps.AddBrowser("JphoneToshibaT51");
            this.Jphonetoshibat51ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Jphonetoshibat51ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jphonetoshibat51ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jphonetoshibat51ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Jtel01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "01"))
            {
                return false;
            }
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["mobileDeviceModel"] = "Cellvic XG";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "12";
            browserCaps.AddBrowser("JTEL01");
            this.Jtel01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.JtelnateProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Jtel01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Jtel01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Jtel01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool JtelnateProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["browserType"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "03"))
            {
                return false;
            }
            capabilities["browser"] = "NATE";
            capabilities["cookies"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontColor"] = "false";
            capabilities["tables"] = "true";
            capabilities["type"] = "NATE 1";
            browserCaps.AddBrowser("JTELNate");
            this.JtelnateProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.JtelnateProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void JtelnateProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void JtelnateProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Kddica21Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^KDDI-CA21$"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "9000";
            capabilities["mobileDeviceManufacturer"] = "Casio";
            capabilities["mobileDeviceModel"] = "A3012CA";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "147";
            capabilities["screenPixelsWidth"] = "125";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("KDDICA21");
            this.Kddica21ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Kddica21ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Kddica21ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Kddica21ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Kddisa21Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KDDI-SA21"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "9000";
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "A3011SA";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "176";
            capabilities["screenPixelsWidth"] = "132";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsItalic"] = "false";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("KDDISA21");
            this.Kddisa21ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Kddisa21ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Kddisa21ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Kddisa21ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Kddits21Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KDDI-TS21"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "9000";
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "C5001T";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "135";
            capabilities["screenPixelsWidth"] = "144";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("KDDITS21");
            this.Kddits21ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Kddits21ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Kddits21ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Kddits21ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Kddits24Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KDDI-TS24"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Openwave";
            capabilities["canSendMail"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "9000";
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "A5304T";
            capabilities["PreferredImageMime"] = "image/jpeg";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenPixelsHeight"] = "176";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsNoWrapStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsTitleElement"] = "false";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("KDDITS24");
            this.Kddits24ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Kddits24ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Kddits24ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Kddits24ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Km100Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KM100"))
            {
                return false;
            }
            capabilities["cookies"] = "false";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "2900";
            capabilities["maximumSoftkeyLabelLength"] = "12";
            capabilities["mobileDeviceManufacturer"] = "OKWap";
            capabilities["mobileDeviceModel"] = "i108";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["rendersWmlSelectsAsMenuCards"] = "false";
            capabilities["requiresNoSoftkeyLabels"] = "true";
            capabilities["requiresPhoneNumbersAsPlainText"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("KM100");
            this.Km100ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Km100ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Km100ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Km100ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Kyocera702gProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCI1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "702G";
            browserCaps.AddBrowser("Kyocera702g");
            this.Kyocera702gProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Kyocera702gProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Kyocera702gProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Kyocera702gProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Kyocera703gProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCI2"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "703G";
            browserCaps.AddBrowser("Kyocera703g");
            this.Kyocera703gProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Kyocera703gProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Kyocera703gProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Kyocera703gProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Kyocerac307kProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KC11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "C307K";
            browserCaps.AddBrowser("KyoceraC307k");
            this.Kyocerac307kProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Kyocerac307kProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Kyocerac307kProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Kyocerac307kProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool L430v03j02Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LGE-L430V03J02"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2900";
            capabilities["maximumSoftkeyLabelLength"] = "10";
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "LG-LP9000";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["requiresNoSoftkeyLabels"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "19";
            capabilities["screenPixelsHeight"] = "133";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsBold"] = "true";
            browserCaps.AddBrowser("L430V03J02");
            this.L430v03j02ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.L430v03j02ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void L430v03j02ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void L430v03j02ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool LegendProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^(?'deviceID'LG\S*) AU\/(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*).*"))
            {
                return false;
            }
            capabilities["browser"] = "AU-System";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "false";
            capabilities["hasBackButton"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["numberOfSoftkeys"] = "2";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "false";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["type"] = worker["AU ${browserMajorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.AddBrowser("Legend");
            this.LegendProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Lgg5200Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.LegendProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void LegendProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void LegendProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lg13Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG13"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "DM-510";
            browserCaps.AddBrowser("Lg13");
            this.Lg13ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lg13ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lg13ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lg13ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lgc840fProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG09"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "LGC-840F";
            browserCaps.AddBrowser("Lgc840f");
            this.Lgc840fProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lgc840fProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lgc840fProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lgc840fProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lgc875fProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG07"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "LGC-875F";
            browserCaps.AddBrowser("Lgc875f");
            this.Lgc875fProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lgc875fProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lgc875fProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lgc875fProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lgelx5350Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LGE-LX5350"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "LX5350";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "108";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("LGELX5350");
            this.Lgelx5350ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lgelx5350ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lgelx5350ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lgelx5350ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lgg5200Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^LG-G5200"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "3500";
            capabilities["maximumSoftkeyLabelLength"] = "6";
            capabilities["mobileDeviceManufacturer"] = "LEGEND";
            capabilities["mobileDeviceModel"] = "G808";
            capabilities["screenBitDepth"] = "4";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "17";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "128";
            browserCaps.AddBrowser("LGG5200");
            this.Lgg5200ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lgg5200ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lgg5200ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lgg5200ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lgi2100Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG02"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "LGI-2100";
            browserCaps.AddBrowser("Lgi2100");
            this.Lgi2100ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lgi2100ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lgi2100ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lgi2100ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lgp680fProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG03"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "LGP-6800F";
            browserCaps.AddBrowser("Lgp680f");
            this.Lgp680fProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lgp680fProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lgp680fProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lgp680fProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lgp7300fProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "LGP-7300F";
            browserCaps.AddBrowser("Lgp7300f");
            this.Lgp7300fProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lgp7300fProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lgp7300fProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lgp7300fProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Lgp7800fProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG04"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "LGP-7800F";
            browserCaps.AddBrowser("Lgp7800f");
            this.Lgp7800fProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Lgp7800fProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Lgp7800fProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Lgp7800fProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ma112Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MA1[12]"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["mobileDeviceModel"] = "C308P";
            browserCaps.AddBrowser("Ma112");
            this.Ma112ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ma112ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ma112ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ma112ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ma13Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MA13"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["mobileDeviceModel"] = "C408P";
            browserCaps.AddBrowser("Ma13");
            this.Ma13ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ma13ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ma13ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ma13ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mac1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MAC1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["mobileDeviceModel"] = "D305P";
            browserCaps.AddBrowser("Mac1");
            this.Mac1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mac1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mac1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mac1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mai12Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MAI[12]"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["mobileDeviceModel"] = "704G";
            browserCaps.AddBrowser("Mai12");
            this.Mai12ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mai12ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mai12ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mai12ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mat1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MAT1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["mobileDeviceModel"] = "TP01";
            browserCaps.AddBrowser("Mat1");
            this.Mat1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mat1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mat1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mat1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MaxpagesizeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-GA-MAX-TRANSFER"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'maxPageSize'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["maximumRenderedPageSize"] = worker["${maxPageSize}"];
            this.MaxpagesizeProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MaxpagesizeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MaxpagesizeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MaxpagesizeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mc01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MC01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "StarTac ST786x, Talkabout T816x, Timeport P816x";
            browserCaps.AddBrowser("Mc01");
            this.Mc01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mc01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mc01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mc01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mcc9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MCC9"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Talkabout V8162";
            browserCaps.AddBrowser("Mcc9");
            this.Mcc9ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mcc9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mcc9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mcc9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MccaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MCCA"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Timeport 8767/ST7868";
            browserCaps.AddBrowser("Mcca");
            this.MccaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MccaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MccaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MccaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool McccProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MCCC"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Talkabout V2267";
            browserCaps.AddBrowser("Mccc");
            this.McccProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.McccProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void McccProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void McccProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MmebenefonqProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Benefon Q"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Benefon";
            capabilities["mobileDeviceModel"] = "Q";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "48";
            capabilities["screenPixelsWidth"] = "100";
            browserCaps.AddBrowser("MMEBenefonQ");
            this.MmebenefonqProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MmebenefonqProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MmebenefonqProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MmebenefonqProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MmecellphoneProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Mozilla/.*\(compatible; MMEF(?'majorVersion'\d)(?'minorVersion'\d); Cell[pP]hone(([;,] (?'deviceID'[^;]*))(;(?'buildInfo'.*))*)*\)"))
            {
                return false;
            }
            capabilities["canCombineFormsInDeck"] = "false";
            capabilities["canRenderPostBackCards"] = "false";
            capabilities["deviceID"] = worker["${deviceID}"];
            capabilities["majorVersion"] = worker["${majorVersion}"];
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["minorVersion"] = worker[".${minorVersion}"];
            capabilities["version"] = worker["${majorVersion}.${minorVersion}"];
            browserCaps.AddBrowser("MMECellphone");
            this.MmecellphoneProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (((!this.MmebenefonqProcess(headers, browserCaps) && !this.Mmesonycmdz5Process(headers, browserCaps)) && (!this.Mmesonycmdj5Process(headers, browserCaps) && !this.Mmesonycmdj7Process(headers, browserCaps))) && ((!this.MmegenericsmallProcess(headers, browserCaps) && !this.MmegenericlargeProcess(headers, browserCaps)) && (!this.MmegenericflipProcess(headers, browserCaps) && !this.Mmegeneric3dProcess(headers, browserCaps))))
            {
                ignoreApplicationBrowsers = false;
            }
            this.MmecellphoneProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MmecellphoneProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MmecellphoneProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mmef20Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MMEF20"))
            {
                return false;
            }
            capabilities["canRenderSetvarZeroWithMultiSelectionList"] = "false";
            capabilities["defaultCharacterHeight"] = "15";
            capabilities["defaultCharacterWidth"] = "5";
            capabilities["defaultScreenPixelsHeight"] = "160";
            capabilities["defaultScreenPixelsWidth"] = "120";
            capabilities["isColor"] = "false";
            capabilities["maximumRenderedPageSize"] = "4000";
            capabilities["mobileDeviceModel"] = "Simulator";
            capabilities["numberOfSoftkeys"] = "2";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml11";
            capabilities["screenBitDepth"] = "1";
            browserCaps.AddBrowser("MMEF20");
            this.Mmef20ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.MmecellphoneProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Mmef20ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mmef20ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mmef20ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mmegeneric3dProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Generic3D"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Microsoft";
            capabilities["mobileDeviceModel"] = "Generic 3D Skin";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "128";
            browserCaps.AddBrowser("MMEGeneric3D");
            this.Mmegeneric3dProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mmegeneric3dProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mmegeneric3dProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mmegeneric3dProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MmegenericflipProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "GenericFlip"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Microsoft";
            capabilities["mobileDeviceModel"] = "Generic Flip Skin";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenPixelsHeight"] = "200";
            capabilities["screenPixelsWidth"] = "160";
            browserCaps.AddBrowser("MMEGenericFlip");
            this.MmegenericflipProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MmegenericflipProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MmegenericflipProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MmegenericflipProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MmegenericlargeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "GenericLarge"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Microsoft";
            capabilities["mobileDeviceModel"] = "Generic Large Skin";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenPixelsHeight"] = "240";
            capabilities["screenPixelsWidth"] = "160";
            browserCaps.AddBrowser("MMEGenericLarge");
            this.MmegenericlargeProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MmegenericlargeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MmegenericlargeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MmegenericlargeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MmegenericsmallProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "GenericSmall"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Microsoft";
            capabilities["mobileDeviceModel"] = "Generic Small Skin";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenPixelsHeight"] = "60";
            capabilities["screenPixelsWidth"] = "100";
            browserCaps.AddBrowser("MMEGenericSmall");
            this.MmegenericsmallProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MmegenericsmallProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MmegenericsmallProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MmegenericsmallProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MmemobileexplorerProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^MobileExplorer/(?'majorVersion'\d*)(?'minorVersion'\.\d*) \(Mozilla/1\.22; compatible; MMEF\d+; (?'manufacturer'[^;]*); (?'model'[^;)]*)(; (?'deviceID'[^)]*))?"))
            {
                return false;
            }
            capabilities["canRenderSetvarZeroWithMultiSelectionList"] = "false";
            capabilities["defaultCharacterHeight"] = "15";
            capabilities["defaultCharacterWidth"] = "5";
            capabilities["defaultScreenPixelsHeight"] = "160";
            capabilities["defaultScreenPixelsWidth"] = "120";
            capabilities["deviceID"] = worker["${deviceID}"];
            capabilities["isColor"] = "true";
            capabilities["majorVersion"] = worker["${majorVersion}"];
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["minorVersion"] = worker["${minorVersion}"];
            capabilities["mobileDeviceManufacturer"] = worker["${manufacturer}"];
            capabilities["mobileDeviceModel"] = worker["${model}"];
            capabilities["numberOfSoftkeys"] = "2";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["screenBitDepth"] = "8";
            capabilities["supportsBodyColor"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsDivAlign"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFontColor"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["version"] = worker["${majorVersion}${minorVersion}"];
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("MMEMobileExplorer");
            this.MmemobileexplorerProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MmemobileexplorerProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MmemobileexplorerProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MmemobileexplorerProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MmeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MMEF"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (!worker.ProcessRegex(target, @"Mozilla/(?'version'(?'major'\d+)(?'minor'\.\d+)\w*).*"))
            {
                return false;
            }
            capabilities["browser"] = "Microsoft Mobile Explorer";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorversion"] = worker["${major}"];
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["mobileDeviceManufacturer"] = "Microsoft";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["type"] = "Microsoft Mobile Explorer";
            capabilities["version"] = worker["${version}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("MME");
            this.MmeProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Mmef20Process(headers, browserCaps) && !this.MmemobileexplorerProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.MmeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MmeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MmeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mmesonycmdj5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Sony CMD-J5"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "CMD-J5";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "65";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("MMESonyCMDJ5");
            this.Mmesonycmdj5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mmesonycmdj5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mmesonycmdj5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mmesonycmdj5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mmesonycmdj7Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Sony CMD-J7/J70"))
            {
                return false;
            }
            capabilities["canCombineFormsInDeck"] = "true";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["canRenderSetvarZeroWithMultiSelectionList"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "2900";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["mobileDeviceManufacturer"] = "Ericsson";
            capabilities["mobileDeviceModel"] = "T68";
            capabilities["numberOfSoftkeys"] = "1";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "false";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "80";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            browserCaps.AddBrowser("MMESonyCMDJ7");
            this.Mmesonycmdj7ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mmesonycmdj7ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mmesonycmdj7ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mmesonycmdj7ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mmesonycmdz5pj020eProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Pj020e"))
            {
                return false;
            }
            capabilities["screenPixelsHeight"] = "65";
            browserCaps.AddBrowser("MMESonyCMDZ5Pj020e");
            this.Mmesonycmdz5pj020eProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mmesonycmdz5pj020eProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mmesonycmdz5pj020eProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mmesonycmdz5pj020eProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mmesonycmdz5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Sony CMD-Z5"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "CMD-Z5";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "60";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("MMESonyCMDZ5");
            this.Mmesonycmdz5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Mmesonycmdz5pj020eProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Mmesonycmdz5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mmesonycmdz5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mmesonycmdz5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mo01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MO01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "i500+, i700+, i1000+";
            browserCaps.AddBrowser("Mo01");
            this.Mo01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mo01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mo01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mo01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mo02Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MO02"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "i2000+";
            browserCaps.AddBrowser("Mo02");
            this.Mo02ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mo02ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mo02ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mo02ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MonoProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = headers["UA-COLOR"];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            str = headers["UA-COLOR"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(str, @"mono(?'colorDepth'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["isColor"] = "false";
            capabilities["screenBitDepth"] = worker["${colorDepth}"];
            this.MonoProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MonoProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MonoProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MonoProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mot2000Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-2000"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "V60c";
            browserCaps.AddBrowser("Mot2000");
            this.Mot2000ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mot2000ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mot2000ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mot2000ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mot2001Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-2001"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "1946";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Timeport 270c";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["requiresUrlEncodedPostfieldValues"] = "true";
            capabilities["screenCharactersWidth"] = "19";
            browserCaps.AddBrowser("Mot2001");
            this.Mot2001ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mot2001ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mot2001ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mot2001ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mot28Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-28"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "i700+, i1000+";
            browserCaps.AddBrowser("Mot28");
            this.Mot28ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mot28ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mot28ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mot28ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mot32Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-32"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "i85s, i50sx";
            browserCaps.AddBrowser("Mot32");
            this.Mot32ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mot32ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mot32ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mot32ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mot72Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-72"))
            {
                return false;
            }
            capabilities["hasBackButton"] = "false";
            capabilities["maximumRenderedPageSize"] = "2900";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Motorola i80s";
            capabilities["numberOfSoftkeys"] = "4";
            capabilities["rendersBreaksAfterWmlAnchor"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["requiresUrlEncodedPostfieldValues"] = "true";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "13";
            browserCaps.AddBrowser("Mot72");
            this.Mot72ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mot72ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mot72ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mot72ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mot76Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-76"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2969";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Motorola i90c";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["screenCharactersWidth"] = "14";
            browserCaps.AddBrowser("Mot76");
            this.Mot76ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mot76ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mot76ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mot76ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motaf418Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["DeviceVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"4\.1\.8"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["maximumRenderedPageSize"] = "1900";
            capabilities["maximumSoftkeyLabelLength"] = "5";
            capabilities["mobileDeviceModel"] = "Timeport 260";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "64";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsCacheControlMetaTag"] = "false";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("MotAf418");
            this.Motaf418ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motaf418ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motaf418ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motaf418ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MotafProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-AF"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Timeport 260/P7382i/P7389i";
            capabilities["screenCharactersHeight"] = "4";
            browserCaps.AddBrowser("MotAf");
            this.MotafProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Motaf418Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.MotafProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MotafProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MotafProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MotbcProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-BC"))
            {
                return false;
            }
            capabilities["inputType"] = "keyboard";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Accompli 009";
            browserCaps.AddBrowser("MotBc");
            this.MotbcProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MotbcProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MotbcProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MotbcProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motc2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-C2"))
            {
                return false;
            }
            capabilities["inputType"] = "keyboard";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "V100, V.Box";
            browserCaps.AddBrowser("MotC2");
            this.Motc2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motc2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motc2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motc2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motc4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-C4"))
            {
                return false;
            }
            capabilities["canRenderMixedSelects"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "V2288, V2282";
            capabilities["supportsCacheControlMetaTag"] = "false";
            browserCaps.AddBrowser("MotC4");
            this.Motc4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motc4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motc4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motc4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MotcbProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-CB"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Timeport P7389";
            capabilities["numberOfSoftkeys"] = "1";
            browserCaps.AddBrowser("MotCb");
            this.MotcbProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MotcbProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MotcbProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MotcbProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MotcfProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-CF"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Accompli 6188";
            browserCaps.AddBrowser("MotCf");
            this.MotcfProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MotcfProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MotcfProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MotcfProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motd5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-D5"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2000";
            capabilities["maximumSoftkeyLabelLength"] = "6";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Motorola Talkabout 191/192/193";
            capabilities["numberOfSoftkeys"] = "3";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "13";
            capabilities["screenPixelsHeight"] = "51";
            capabilities["screenPixelsWidth"] = "91";
            browserCaps.AddBrowser("MotD5");
            this.Motd5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motd5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motd5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motd5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motd8Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-D8"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Timeport 250/P7689";
            browserCaps.AddBrowser("MotD8");
            this.Motd8ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motd8ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motd8ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motd8ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MotdcProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-DC"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "V3682, V50";
            browserCaps.AddBrowser("MotDc");
            this.MotdcProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MotdcProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MotdcProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MotdcProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motf0Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-F0"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2000";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Motorola v50";
            capabilities["numberOfSoftkeys"] = "3";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["requiresUrlEncodedPostfieldValues"] = "true";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "40";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("MotF0");
            this.Motf0ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motf0ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motf0ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motf0ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motf5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-F5"))
            {
                return false;
            }
            capabilities["cookies"] = "false";
            capabilities["maximumRenderedPageSize"] = "1900";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Talkabout 192";
            capabilities["numberOfSoftkeys"] = "3";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "40";
            capabilities["screenPixelsWidth"] = "96";
            browserCaps.AddBrowser("MotF5");
            this.Motf5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motf5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motf5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motf5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motf6Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-F6"))
            {
                return false;
            }
            capabilities["cookies"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["maximumRenderedPageSize"] = "5000";
            capabilities["maximumSoftkeyLabelLength"] = "6";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Accompli 008/6288";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlSelectsAsMenuCards"] = "false";
            capabilities["screenBitDepth"] = "4";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "240";
            browserCaps.AddBrowser("MotF6");
            this.Motf6ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motf6ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motf6ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motf6ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motorolae360Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Motorola-T33"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["hasBackButton"] = "false";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "3500";
            capabilities["maximumSoftkeyLabelLength"] = "8";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "E360";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "27";
            capabilities["screenPixelsHeight"] = "96";
            capabilities["screenPixelsWidth"] = "128";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("MotorolaE360");
            this.Motorolae360ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motorolae360ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motorolae360ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motorolae360ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motorolav60gProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-PHX4_"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2000";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "V60G";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["requiresUrlEncodedPostfieldValues"] = "true";
            capabilities["screenCharactersHeight"] = "3";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "36";
            capabilities["screenPixelsWidth"] = "90";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("MotorolaV60G");
            this.Motorolav60gProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motorolav60gProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motorolav60gProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motorolav60gProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motorolav708aProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-V708A"))
            {
                return false;
            }
            capabilities["canRenderInputAndSelectElementsTogether"] = "false";
            capabilities["hasBackButton"] = "false";
            capabilities["maximumRenderedPageSize"] = "2000";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "V70";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["screenCharactersHeight"] = "3";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "94";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("MotorolaV708A");
            this.Motorolav708aProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motorolav708aProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motorolav708aProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motorolav708aProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motorolav708Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-V708_"))
            {
                return false;
            }
            capabilities["isColor"] = "false";
            capabilities["maximumRenderedPageSize"] = "1900";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "V70";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "2";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["screenPixelsHeight"] = "34";
            capabilities["screenPixelsWidth"] = "90";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("MotorolaV708");
            this.Motorolav708ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motorolav708ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motorolav708ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motorolav708ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motp2kcProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-P2K-C"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "V120c";
            browserCaps.AddBrowser("MotP2kC");
            this.Motp2kcProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motp2kcProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motp2kcProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motp2kcProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MotpancProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-PAN-C"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Timeport 270c";
            browserCaps.AddBrowser("MotPanC");
            this.MotpancProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MotpancProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MotpancProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MotpancProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Motv200Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MOT-v200"))
            {
                return false;
            }
            capabilities["hasBackButton"] = "false";
            capabilities["inputType"] = "keyboard";
            capabilities["maximumRenderedPageSize"] = "2000";
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            capabilities["mobileDeviceModel"] = "Motorola v200";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["requiresUrlEncodedPostfieldValues"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Motv200");
            this.Motv200ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Motv200ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Motv200ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Motv200ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MozillabetaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Mozilla/\d+\.\d+b"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            this.MozillabetaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MozillabetaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MozillabetaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MozillabetaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MozillafirebirdProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Gecko\/\d+ Firebird\/(?'version'(?'major'\d+)(?'minor'\.[.\d]*))"))
            {
                return false;
            }
            capabilities["browser"] = "Firebird";
            capabilities["majorversion"] = worker["${major}"];
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["version"] = worker["${version}"];
            capabilities["type"] = worker["Firebird${version}"];
            browserCaps.AddBrowser("MozillaFirebird");
            this.MozillafirebirdProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MozillafirebirdProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MozillafirebirdProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MozillafirebirdProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MozillafirefoxProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Gecko\/\d+ Firefox\/(?'version'(?'major'\d+)(?'minor'\.[.\d]*))"))
            {
                return false;
            }
            capabilities["browser"] = "Firefox";
            capabilities["majorversion"] = worker["${major}"];
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["version"] = worker["${version}"];
            capabilities["type"] = worker["Firefox${version}"];
            browserCaps.AddBrowser("MozillaFirefox");
            this.MozillafirefoxProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MozillafirefoxProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MozillafirefoxProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MozillafirefoxProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MozillagoldProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Mozilla/\d+\.\d+\w*Gold"))
            {
                return false;
            }
            capabilities["gold"] = "true";
            this.MozillagoldProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.MozillagoldProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MozillagoldProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MozillagoldProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MozillaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Mozilla"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, "MME|Opera"))
            {
                return false;
            }
            worker.ProcessRegex(browserCaps[string.Empty], @"Mozilla/(?'version'(?'major'\d+)(?'minor'\.\d+)\w*)");
            worker.ProcessRegex(browserCaps[string.Empty], @" (?'screenWidth'\d*)x(?'screenHeight'\d*)");
            capabilities["browser"] = "Mozilla";
            capabilities["cookies"] = "false";
            capabilities["defaultScreenCharactersHeight"] = "40";
            capabilities["defaultScreenCharactersWidth"] = "80";
            capabilities["defaultScreenPixelsHeight"] = "480";
            capabilities["defaultScreenPixelsWidth"] = "640";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "false";
            capabilities["majorversion"] = worker["${major}"];
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["screenBitDepth"] = "8";
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsDivNoWrap"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["type"] = "Mozilla";
            capabilities["version"] = worker["${version}"];
            browserCaps.AddBrowser("Mozilla");
            this.MozillaProcessGateways(headers, browserCaps);
            this.MozillabetaProcess(headers, browserCaps);
            this.MozillagoldProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((((!this.IeProcess(headers, browserCaps) && !this.PowerbrowserProcess(headers, browserCaps)) && (!this.GeckoProcess(headers, browserCaps) && !this.AvantgoProcess(headers, browserCaps))) && ((!this.GoamericaProcess(headers, browserCaps) && !this.Netscape3Process(headers, browserCaps)) && (!this.Netscape4Process(headers, browserCaps) && !this.MypalmProcess(headers, browserCaps)))) && ((!this.EudorawebProcess(headers, browserCaps) && !this.WinceProcess(headers, browserCaps)) && !this.MspieProcess(headers, browserCaps)))
            {
                ignoreApplicationBrowsers = false;
            }
            this.MozillaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MozillaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MozillaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MozillarvProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"rv\:(?'version'(?'major'\d+)(?'minor'\.[.\d]*))"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, "Netscape"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.4";
            capabilities["majorversion"] = worker["${major}"];
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["supportsCallback"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsMaintainScrollPositionOnPostback"] = "true";
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["type"] = worker["Mozilla${major}"];
            capabilities["version"] = worker["${version}"];
            capabilities["w3cdomversion"] = "1.0";
            browserCaps.AddBrowser("MozillaRV");
            this.MozillarvProcessGateways(headers, browserCaps);
            this.Mozillav14plusProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.MozillafirebirdProcess(headers, browserCaps) && !this.MozillafirefoxProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.MozillarvProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MozillarvProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MozillarvProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mozillav14plusProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^1$"))
            {
                return false;
            }
            target = (string) capabilities["minorversion"];
            if (!worker.ProcessRegex(target, @"^\.[4-9]"))
            {
                return false;
            }
            capabilities["backgroundsounds"] = "true";
            capabilities["css1"] = "true";
            capabilities["css2"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["maximumRenderedPageSize"] = "2000000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["screenBitDepth"] = "32";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["type"] = worker["Mozilla${version}"];
            capabilities["xml"] = "true";
            this.Mozillav14plusProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mozillav14plusProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mozillav14plusProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mozillav14plusProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mspie06Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Microsoft Pocket Internet Explorer/0\.6"))
            {
                return false;
            }
            capabilities["backgroundsounds"] = "true";
            capabilities["browser"] = "PIE";
            capabilities["cookies"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorversion"] = "1";
            capabilities["minorversion"] = "0";
            capabilities["platform"] = "WinCE";
            capabilities["tables"] = "true";
            capabilities["type"] = "PIE";
            capabilities["version"] = "1.0";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("MSPIE06");
            this.Mspie06ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mspie06ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mspie06ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mspie06ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mspie2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"2\."))
            {
                return false;
            }
            capabilities["frames"] = "true";
            browserCaps.AddBrowser("MSPIE2");
            this.Mspie2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mspie2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mspie2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mspie2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MspieProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Mozilla[^(]*\(compatible; MSPIE (?'version'(?'major'\d+)(?'minor'\.\d+)(?'letters'\w*))(?'extra'.*)"))
            {
                return false;
            }
            capabilities["backgroundsounds"] = "true";
            capabilities["browser"] = "PIE";
            capabilities["cookies"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorversion"] = worker["${major}"];
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["tables"] = "true";
            capabilities["type"] = worker["PIE${major}"];
            capabilities["version"] = worker["${version}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("MSPIE");
            this.MspieProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Mspie2Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.MspieProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MspieProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MspieProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Mypalm1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["gatewayMajorVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^1"))
            {
                return false;
            }
            target = (string) capabilities["gatewayMinorVersion"];
            if (!worker.ProcessRegex(target, @"\.0$"))
            {
                return false;
            }
            capabilities["browser"] = "EarthLink";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "false";
            capabilities["maximumRenderedPageSize"] = "7000";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["rendersBreaksAfterHtmlLists"] = "false";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "true";
            capabilities["requiresUniqueHtmlInputNames"] = "true";
            capabilities["screenBitDepth"] = "4";
            capabilities["screenCharactersHeight"] = "13";
            capabilities["screenCharactersWidth"] = "30";
            capabilities["supportsFontSize"] = "false";
            capabilities["tables"] = "true";
            capabilities["type"] = "EarthLink";
            browserCaps.AddBrowser("MyPalm1");
            this.Mypalm1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Mypalm1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Mypalm1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Mypalm1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool MypalmProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Mozilla/2\.0 \(compatible; Elaine/(?'gatewayMajorVersion'\w*)(?'gatewayMinorVersion'\.\w*)\)"))
            {
                return false;
            }
            capabilities["browser"] = "MyPalm";
            capabilities["cookies"] = "false";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "12";
            capabilities["defaultScreenPixelsHeight"] = "72";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["ecmascriptversion"] = "1.1";
            capabilities["frames"] = "true";
            capabilities["gatewayMajorVersion"] = worker["${gatewayMajorVersion}"];
            capabilities["gatewayMinorVersion"] = worker["${gatewayMinorVersion}"];
            capabilities["gatewayVersion"] = worker["${gatewayMajorVersion}${gatewayMinorVersion}"];
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["javaapplets"] = "false";
            capabilities["javascript"] = "false";
            capabilities["mobileDeviceManufacturer"] = "PalmOS-licensee";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiredMetaTagNameValue"] = "PalmComputingPlatform";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "36";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsImageSubmit"] = "false";
            capabilities["tables"] = "false";
            capabilities["type"] = "MyPalm";
            capabilities["vbscript"] = "false";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("Mypalm");
            this.MypalmProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Mypalm1Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.MypalmProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void MypalmProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void MypalmProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool N400Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["HTTP_X_WAP_PROFILE"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"\/SPH-N400\/"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "N400";
            capabilities["maximumRenderedPageSize"] = "46750";
            capabilities["preferredREnderingType"] = "xhtml-basic";
            capabilities["screenBitDepth"] = "24";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsTitleElement"] = "false";
            browserCaps.AddBrowser("n400");
            this.N400ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.N400ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void N400ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void N400ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool NetfrontProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"\((?'deviceID'.*)\) NetFront\/(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*).*"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Compact NetFront";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isMobileDevice"] = "true";
            capabilities["javascript"] = "false";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresFullyQualifiedRedirectUrl"] = "true";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFileUpload"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsImageSubmit"] = "false";
            capabilities["supportsItalic"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["type"] = worker["Compact NetFront ${browserMajorVersion}"];
            browserCaps.AddBrowser("NetFront");
            this.NetfrontProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Slb500Process(headers, browserCaps) && !this.VrnaProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.NetfrontProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void NetfrontProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void NetfrontProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Netscape3Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^Mozilla/3"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, "Opera"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, "AvantGo"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, "MSIE"))
            {
                return false;
            }
            capabilities["browser"] = "Netscape";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "true";
            capabilities["ecmascriptversion"] = "1.1";
            capabilities["frames"] = "true";
            capabilities["isColor"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["supportsCss"] = "false";
            capabilities["supportsFileUpload"] = "true";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "Netscape3";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("Netscape3");
            this.Netscape3ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Netscape3ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Netscape3ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Netscape3ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Netscape4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^Mozilla/4"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, "Opera"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, "MSIE"))
            {
                return false;
            }
            capabilities["browser"] = "Netscape";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "true";
            capabilities["ecmascriptversion"] = "1.3";
            capabilities["frames"] = "true";
            capabilities["isColor"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["supportsCss"] = "false";
            capabilities["supportsFileUpload"] = "true";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "Netscape4";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("Netscape4");
            this.Netscape4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.CasiopeiaProcess(headers, browserCaps) && !this.PalmwebproProcess(headers, browserCaps)) && !this.NetfrontProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Netscape4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Netscape4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Netscape4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Netscape5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Mozilla/5\.0 \([^)]*\) (Gecko/[-\d]+ )?Netscape\d?/(?'version'(?'major'\d+)(?'minor'\.\d+)(?'letters'\w*))"))
            {
                return false;
            }
            capabilities["browser"] = "Netscape";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "true";
            capabilities["css2"] = "true";
            capabilities["ecmascriptversion"] = "1.5";
            capabilities["frames"] = "true";
            capabilities["isColor"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["letters"] = worker["${letters}"];
            capabilities["majorversion"] = worker["${major}"];
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["screenBitDepth"] = "8";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["supportsVCard"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = worker["Netscape${major}"];
            capabilities["version"] = worker["${version}"];
            capabilities["w3cdomversion"] = "1.0";
            capabilities["xml"] = "true";
            browserCaps.AddBrowser("Netscape5");
            this.Netscape5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Netscape6to9Process(headers, browserCaps) && !this.NetscapebetaProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Netscape5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Netscape5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Netscape5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Netscape6to9betaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["letters"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^b"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            browserCaps.AddBrowser("Netscape6to9Beta");
            this.Netscape6to9betaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Netscape6to9betaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Netscape6to9betaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Netscape6to9betaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Netscape6to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"[6-9]\."))
            {
                return false;
            }
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsXmlHttp"] = "true";
            capabilities["supportsCallback"] = "true";
            capabilities["supportsMaintainScrollPositionOnPostback"] = "true";
            browserCaps.AddBrowser("Netscape6to9");
            this.Netscape6to9ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Netscape6to9betaProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Netscape6to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Netscape6to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Netscape6to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool NetscapebetaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["letters"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^b"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            browserCaps.AddBrowser("NetscapeBeta");
            this.NetscapebetaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.NetscapebetaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void NetscapebetaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void NetscapebetaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nk00Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "NK00"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2252";
            capabilities["maximumSoftkeyLabelLength"] = "6";
            capabilities["mobileDeviceManufacturer"] = "Nokia";
            capabilities["mobileDeviceModel"] = "nokia 3285";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["supportsBold"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Nk00");
            this.Nk00ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nk00ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nk00ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nk00ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia3330Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia3330/1\.0 \((?'versionString'(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*).*)\)"))
            {
                return false;
            }
            capabilities["hasBackButton"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "2800";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "3330";
            capabilities["screenCharactersHeight"] = "3";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "39";
            capabilities["screenPixelsWidth"] = "78";
            capabilities["type"] = "Nokia 3330";
            capabilities["version"] = worker["${versionString}"];
            browserCaps.AddBrowser("Nokia3330");
            this.Nokia3330ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia3330ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia3330ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia3330ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia3560Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Nokia3560"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "28";
            capabilities["screenPixelsHeight"] = "176";
            capabilities["screenPixelsWidth"] = "208";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("Nokia3560");
            this.Nokia3560ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia3560ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia3560ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia3560ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia3590Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Nokia3590/(?'version'(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*)"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "3200";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "3590";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "true";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "Nokia 3590";
            capabilities["version"] = worker["${version}"];
            browserCaps.AddBrowser("Nokia3590");
            this.Nokia3590ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Nokia3590v1Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Nokia3590ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia3590ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia3590ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia3590v1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia3590/1\.0\(7\."))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "8020";
            capabilities["preferredRequestEncoding"] = "iso-8859-1";
            capabilities["preferredResponseEncoding"] = "utf-8";
            capabilities["screenPixelsHeight"] = "65";
            capabilities["supportsCss"] = "true";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsTitleElement"] = "true";
            capabilities["tables"] = "false";
            browserCaps.AddBrowser("Nokia3590V1");
            this.Nokia3590v1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia3590v1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia3590v1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia3590v1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia3595Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia3595/(?'version'(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*)"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "15700";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "3595";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["preferredRequestEncoding"] = "iso-8859-1";
            capabilities["preferredResponseEncoding"] = "utf-8";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersWidth"] = "17";
            capabilities["screenPixelsHeight"] = "132";
            capabilities["screenPixelsWidth"] = "176";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "Nokia 3595";
            capabilities["version"] = worker["${version}"];
            browserCaps.AddBrowser("Nokia3595");
            this.Nokia3595ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia3595ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia3595ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia3595ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia3650p12plusProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobilePlatformVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"[^01]\.|1\.[^01]"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "28";
            capabilities["screenPixelsHeight"] = "176";
            capabilities["screenPixelsWidth"] = "208";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            capabilities["tagwriter"] = "System.Web.UI.XhtmlTextWriter";
            browserCaps.AddBrowser("Nokia3650P12Plus");
            this.Nokia3650p12plusProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia3650p12plusProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia3650p12plusProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia3650p12plusProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia3650Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia3650/(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*).* Series60/(?'platformVersion'\S*)"))
            {
                return false;
            }
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "3650";
            capabilities["mobilePlatformVersion"] = worker["${platformVersion}"];
            capabilities["type"] = "Nokia 3650";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.AddBrowser("Nokia3650");
            this.Nokia3650ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Nokia3650p12plusProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Nokia3650ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia3650ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia3650ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia5100Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia5100/(?'version'(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*)"))
            {
                return false;
            }
            capabilities["canRenderOnEventAndPrevElementsTogether"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["isColor"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "3500";
            capabilities["maximumSoftkeyLabelLength"] = "14";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "5100";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["rendersWmlSelectsAsMenuCards"] = "true";
            capabilities["requiresPhoneNumbersAsPlainText"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "128";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["type"] = "Nokia 5100";
            capabilities["version"] = worker["${version}"];
            browserCaps.AddBrowser("Nokia5100");
            this.Nokia5100ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia5100ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia5100ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia5100ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia6200Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia6200/(?'version'(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*)"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "6200";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["preferredRequestEncoding"] = "iso-8859-1";
            capabilities["preferredResponseEncoding"] = "utf-8";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "19";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "128";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsNoWrapStyle"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "Nokia 6200";
            capabilities["version"] = worker["${version}"];
            browserCaps.AddBrowser("Nokia6200");
            this.Nokia6200ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia6200ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia6200ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia6200ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia6220Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia6210/1\.0 \((?'versionString'(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*).*)\)"))
            {
                return false;
            }
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "6210";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "22";
            capabilities["screenPixelsHeight"] = "41";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["type"] = "Nokia 6210";
            capabilities["version"] = worker["${versionString}"];
            browserCaps.AddBrowser("Nokia6220");
            this.Nokia6220ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia6220ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia6220ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia6220ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia6250Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia6250/1\.0 \((?'versionString'(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*).*)\)"))
            {
                return false;
            }
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "6250";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "22";
            capabilities["screenPixelsHeight"] = "41";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["type"] = "Nokia 6250";
            capabilities["version"] = worker["${versionString}"];
            browserCaps.AddBrowser("Nokia6250");
            this.Nokia6250ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia6250ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia6250ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia6250ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia6310Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia6310/1\.0 \((?'versionString'(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*).*)\)"))
            {
                return false;
            }
            capabilities["canRenderOneventAndPrevElementsTogether"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["cookies"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "2800";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "6310";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreaksAfterWmlAnchor"] = "false";
            capabilities["rendersBreaksAfterWmlInput"] = "false";
            capabilities["requiresPhoneNumbersAsPlainText"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "45";
            capabilities["screenPixelsWidth"] = "92";
            capabilities["type"] = "Nokia 6310";
            capabilities["version"] = worker["${versionString}"];
            browserCaps.AddBrowser("Nokia6310");
            this.Nokia6310ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia6310ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia6310ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia6310ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia6510Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia6510/1\.0 \((?'versionString'(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*).*)\)"))
            {
                return false;
            }
            capabilities["canRenderOnEventAndPrevElementsTogether"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["cookies"] = "true";
            capabilities["hasBackButton"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "2800";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "6510";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["requiresPhoneNumbersAsPlainText"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "45";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["type"] = "Nokia 6510";
            capabilities["version"] = worker["${versionString}"];
            browserCaps.AddBrowser("Nokia6510");
            this.Nokia6510ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia6510ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia6510ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia6510ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia6590Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Nokia6590/(?'versionString'(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*) "))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "9800";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "6590";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "true";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "Nokia 6590";
            capabilities["version"] = worker["${versionString}"];
            browserCaps.AddBrowser("Nokia6590");
            this.Nokia6590ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia6590ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia6590ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia6590ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia6800Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia6800/(?'version'(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*)"))
            {
                return false;
            }
            capabilities["canRenderOnEventAndPrevElementsTogether"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["hasBackButton"] = "true";
            capabilities["isColor"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "3500";
            capabilities["maximumSoftkeyLabelLength"] = "14";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "6800";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["requiresPhoneNumbersAsPlainText"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "128";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["type"] = "Nokia 6800";
            capabilities["version"] = worker["${version}"];
            browserCaps.AddBrowser("Nokia6800");
            this.Nokia6800ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia6800ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia6800ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia6800ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia7110Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia7110/1\.0 \((?'versionString'(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*).*)\)"))
            {
                return false;
            }
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "7110";
            capabilities["optimumPageWeight"] = "800";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "22";
            capabilities["screenPixelsHeight"] = "44";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["type"] = "Nokia 7110";
            capabilities["version"] = worker["${versionString}"];
            browserCaps.AddBrowser("Nokia7110");
            this.Nokia7110ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia7110ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia7110ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia7110ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia7650Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Nokia7650/(?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*).*"))
            {
                return false;
            }
            capabilities["canRenderOnEventAndPrevElementsTogether"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["cookies"] = "true";
            capabilities["hasBackButton"] = "true";
            capabilities["isColor"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "3500";
            capabilities["maximumSoftkeyLabelLength"] = "18";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "7650";
            capabilities["numberOfSoftkeys"] = "3";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["requiresPhoneNumbersAsPlainText"] = "false";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "28";
            capabilities["screenPixelsHeight"] = "208";
            capabilities["screenPixelsWidth"] = "176";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["type"] = "Nokia 7650";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.AddBrowser("Nokia7650");
            this.Nokia7650ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia7650ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia7650ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia7650ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia8310Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia8310/1\.0 \((?'versionString'(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*).*)\)"))
            {
                return false;
            }
            capabilities["canRenderOneventAndPrevElementsTogether"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "2700";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "8310";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreaksAfterWmlAnchor"] = "false";
            capabilities["rendersBreaksAfterWmlInput"] = "false";
            capabilities["requiresPhoneNumbersAsPlainText"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "3";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "39";
            capabilities["screenPixelsWidth"] = "78";
            capabilities["type"] = "Nokia 8310";
            capabilities["version"] = worker["${versionString}"];
            browserCaps.AddBrowser("Nokia8310");
            this.Nokia8310ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia8310ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia8310ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia8310ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia9110iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia9110/1\.0"))
            {
                return false;
            }
            capabilities["cookies"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["maximumRenderedPageSize"] = "8192";
            capabilities["mobileDeviceModel"] = "9110i Communicator";
            capabilities["rendersBreaksAfterWmlAnchor"] = "false";
            capabilities["screenBitDepth"] = "4";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["screenCharactersWidth"] = "60";
            capabilities["screenPixelsHeight"] = "180";
            capabilities["screenPixelsWidth"] = "400";
            capabilities["type"] = "Nokia 9110";
            browserCaps.AddBrowser("Nokia9110i");
            this.Nokia9110iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia9110iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia9110iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia9110iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia9110Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Nokia-9110"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["maximumRenderedPageSize"] = "150000";
            capabilities["mobileDeviceModel"] = "Nokia 9110";
            capabilities["numberOfSoftkeys"] = "0";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["rendersBreaksAfterHtmlLists"] = "false";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "11";
            capabilities["screenCharactersWidth"] = "57";
            capabilities["screenPixelsHeight"] = "200";
            capabilities["screenPixelsWidth"] = "540";
            capabilities["supportsAccesskeyAttribute"] = "true";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsSelectMultiple"] = "false";
            capabilities["tables"] = "true";
            capabilities["type"] = "Nokia 9110";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("Nokia9110");
            this.Nokia9110ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia9110ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia9110ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia9110ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia9210htmlProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"EPOC32-WTL/2\.2 Crystal/6\.0 STNC-WTL/"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            browserCaps.AddBrowser("Nokia9210HTML");
            this.Nokia9210htmlProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia9210htmlProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia9210htmlProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia9210htmlProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokia9210Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia9210/1\.0"))
            {
                return false;
            }
            capabilities["cookies"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "8192";
            capabilities["mobileDeviceModel"] = "9210 Communicator";
            capabilities["rendersBreaksAfterWmlAnchor"] = "false";
            capabilities["rendersBreaksAfterWmlInput"] = "false";
            capabilities["requiresNoSoftkeyLabels"] = "true";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["screenBitDepth"] = "12";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "75";
            capabilities["screenPixelsHeight"] = "165";
            capabilities["screenPixelsWidth"] = "490";
            capabilities["supportsCacheControlMetaTag"] = "false";
            capabilities["type"] = "Nokia 9210";
            browserCaps.AddBrowser("Nokia9210");
            this.Nokia9210ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokia9210ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokia9210ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokia9210ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool NokiablueprintProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia\-WAP\-Toolkit\/(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*)"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["cookies"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "65536";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "Blueprint Simulator";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreaksAfterWmlAnchor"] = "false";
            capabilities["type"] = "Nokia WAP Toolkit";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.AddBrowser("NokiaBlueprint");
            this.NokiablueprintProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.NokiablueprintProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void NokiablueprintProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void NokiablueprintProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokiaepoc32wtl20Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"2\.0"))
            {
                return false;
            }
            capabilities["canRenderEmptySelects"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "false";
            capabilities["maximumRenderedPageSize"] = "7168";
            capabilities["mobileDeviceManufacturer"] = "Psion";
            capabilities["mobileDeviceModel"] = "Series 7";
            capabilities["rendersBreaksAfterHtmlLists"] = "true";
            capabilities["requiresAttributeColonSubstitution"] = "false";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "true";
            capabilities["screenCharactersHeight"] = "31";
            capabilities["screenCharactersWidth"] = "69";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["SupportsEmptyStringInCookieValue"] = "true";
            browserCaps.AddBrowser("NokiaEpoc32wtl20");
            this.Nokiaepoc32wtl20ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nokiaepoc32wtl20ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokiaepoc32wtl20ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokiaepoc32wtl20ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nokiaepoc32wtlProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"EPOC32-WTL/(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*)"))
            {
                return false;
            }
            capabilities["browser"] = "EPOC";
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["maximumRenderedPageSize"] = "150000";
            capabilities["mobileDeviceManufacturer"] = "Nokia";
            capabilities["mobileDeviceModel"] = "Nokia 9210";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["rendersBreaksAfterHtmlLists"] = "false";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "54";
            capabilities["screenPixelsHeight"] = "170";
            capabilities["screenPixelsWidth"] = "478";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsSelectMultiple"] = "false";
            capabilities["supportsEmptyStringInCookieValue"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "Nokia Epoc";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("NokiaEpoc32wtl");
            this.Nokiaepoc32wtlProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Nokiaepoc32wtl20Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Nokiaepoc32wtlProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nokiaepoc32wtlProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nokiaepoc32wtlProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool NokiagatewayProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = headers["VIA"];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            str = headers["VIA"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(str, @"(?'nokiaVersion'Nokia\D*(?'gatewayMajorVersion'\d+)(?'gatewayMinorVersion'\.\d+)[^,]*)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["gatewayMajorVersion"] = worker["${gatewayMajorVersion}"];
            capabilities["gatewayMinorVersion"] = worker["${gatewayMinorVersion}"];
            capabilities["gatewayVersion"] = worker["${nokiaVersion}"];
            this.NokiagatewayProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.NokiagatewayProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void NokiagatewayProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void NokiagatewayProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool NokiamobilebrowserProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia Mobile Browser (?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*)"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "25000";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = worker["Mobile Browser ${browserMajorVersion}${browserMinorVersion}"];
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "application/vnd.wap.xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "true";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresPostRedirectionHandling"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "14";
            capabilities["screenCharactersWidth"] = "24";
            capabilities["screenPixelsHeight"] = "255";
            capabilities["screenPixelsWidth"] = "180";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["supportsStyleElement"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = worker["Nokia Mobile Browser ${browserMajorVersion}${browserMinorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.HtmlTextWriter = "System.Web.UI.XhtmlTextWriter";
            browserCaps.AddBrowser("NokiaMobileBrowser");
            this.NokiamobilebrowserProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.NokiamobilebrowserProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void NokiamobilebrowserProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void NokiamobilebrowserProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool NokiamobilebrowserrainbowProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Rainbow/(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*)"))
            {
                return false;
            }
            capabilities["browser"] = "Nokia";
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["canSendMail"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["javascript"] = "false";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "25000";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceManufacturer"] = "Nokia";
            capabilities["mobileDeviceModel"] = worker["Mobile Browser ${browserMajorVersion}${browserMinorVersion}"];
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "application/vnd.wap.xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "true";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresInputTypeAttribute"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "14";
            capabilities["screenCharactersWidth"] = "24";
            capabilities["screenPixelsHeight"] = "255";
            capabilities["screenPixelsWidth"] = "180";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyColor"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsDivAlign"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFontColor"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsQueryStringInFormAction"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["supportsStyleElement"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = worker["Nokia Mobile Browser ${browserMajorVersion}${browserMinorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.XhtmlTextWriter";
            browserCaps.AddBrowser("NokiaMobileBrowserRainbow");
            this.NokiamobilebrowserrainbowProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.NokiamobilebrowserrainbowProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void NokiamobilebrowserrainbowProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void NokiamobilebrowserrainbowProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool NokiaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Nokia"))
            {
                return false;
            }
            capabilities["browser"] = "Nokia";
            capabilities["cookies"] = "false";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canRenderOneventAndPrevElementsTogether"] = "false";
            capabilities["canRenderPostBackCards"] = "false";
            capabilities["canSendMail"] = "false";
            capabilities["defaultScreenCharactersHeight"] = "4";
            capabilities["defaultScreenCharactersWidth"] = "20";
            capabilities["defaultScreenPixelsHeight"] = "40";
            capabilities["defaultScreenPixelsWidth"] = "90";
            capabilities["hasBackButton"] = "false";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["maximumRenderedPageSize"] = "1397";
            capabilities["mobileDeviceManufacturer"] = "Nokia";
            capabilities["numberOfSoftkeys"] = "2";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml11";
            capabilities["rendersBreaksAfterWmlAnchor"] = "true";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "false";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresPhoneNumbersAsPlainText"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["type"] = "Nokia";
            browserCaps.AddBrowser("Nokia");
            this.NokiaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (((((!this.NokiablueprintProcess(headers, browserCaps) && !this.NokiawapsimulatorProcess(headers, browserCaps)) && (!this.NokiamobilebrowserProcess(headers, browserCaps) && !this.Nokia7110Process(headers, browserCaps))) && ((!this.Nokia6220Process(headers, browserCaps) && !this.Nokia6250Process(headers, browserCaps)) && (!this.Nokia6310Process(headers, browserCaps) && !this.Nokia6510Process(headers, browserCaps)))) && (((!this.Nokia8310Process(headers, browserCaps) && !this.Nokia9110iProcess(headers, browserCaps)) && (!this.Nokia9110Process(headers, browserCaps) && !this.Nokia3330Process(headers, browserCaps))) && ((!this.Nokia9210Process(headers, browserCaps) && !this.Nokia9210htmlProcess(headers, browserCaps)) && (!this.Nokia3590Process(headers, browserCaps) && !this.Nokia3595Process(headers, browserCaps))))) && (((!this.Nokia3560Process(headers, browserCaps) && !this.Nokia3650Process(headers, browserCaps)) && (!this.Nokia5100Process(headers, browserCaps) && !this.Nokia6200Process(headers, browserCaps))) && ((!this.Nokia6590Process(headers, browserCaps) && !this.Nokia6800Process(headers, browserCaps)) && !this.Nokia7650Process(headers, browserCaps))))
            {
                ignoreApplicationBrowsers = false;
            }
            this.NokiaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void NokiaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void NokiaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool NokiawapsimulatorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Nokia\-MIT\-Browser\/(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*)"))
            {
                return false;
            }
            capabilities["canRenderOnEventAndPrevElementsTogether"] = "true";
            capabilities["canRenderPostBackCards"] = "true";
            capabilities["cookies"] = "true";
            capabilities["hasBackButton"] = "true";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "3584";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceModel"] = "WAP Simulator";
            capabilities["rendersBreaksAfterWmlAnchor"] = "false";
            capabilities["requiresPhoneNumbersAsPlainText"] = "false";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "25";
            capabilities["screenCharactersWidth"] = "32";
            capabilities["screenPixelsHeight"] = "512";
            capabilities["screenPixelsWidth"] = "384";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["type"] = "Nokia Mobile Internet Toolkit";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.AddBrowser("NokiaWapSimulator");
            this.NokiawapsimulatorProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.NokiawapsimulatorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void NokiawapsimulatorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void NokiawapsimulatorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Nt95Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "NT95"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "cdmaOne";
            browserCaps.AddBrowser("Nt95");
            this.Nt95ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Nt95ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Nt95ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Nt95ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opera1to3betaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["letters"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^b"))
            {
                return false;
            }
            target = (string) capabilities["majorversion"];
            if (!worker.ProcessRegex(target, "[1-3]"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            browserCaps.AddBrowser("Opera1to3beta");
            this.Opera1to3betaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Opera1to3betaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opera1to3betaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opera1to3betaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opera4betaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["letters"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^b"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            browserCaps.AddBrowser("Opera4beta");
            this.Opera4betaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Opera4betaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opera4betaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opera4betaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opera4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "4"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.3";
            capabilities["supportsFileUpload"] = "true";
            capabilities["xml"] = "true";
            browserCaps.AddBrowser("Opera4");
            this.Opera4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Opera4betaProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Opera4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opera4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opera4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opera5to9betaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["letters"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^b"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            this.Opera5to9betaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Opera5to9betaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opera5to9betaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opera5to9betaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opera5to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "[5-9]"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.3";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "7000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["screenBitDepth"] = "24";
            capabilities["supportsFileUpload"] = "true";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsItalic"] = "false";
            capabilities["w3cdomversion"] = "1.0";
            capabilities["xml"] = "true";
            browserCaps.AddBrowser("Opera5to9");
            this.Opera5to9ProcessGateways(headers, browserCaps);
            this.Opera5to9betaProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Opera6to9Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Opera5to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opera5to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opera5to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opera6to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "[6-9]"))
            {
                return false;
            }
            capabilities["backgroundsounds"] = "true";
            capabilities["css1"] = "true";
            capabilities["css2"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["javaapplets"] = "true";
            capabilities["maximumRenderedPageSize"] = "2000000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["screenBitDepth"] = "32";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["xml"] = "false";
            browserCaps.Adapters["System.Web.UI.WebControls.CheckBox, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "";
            browserCaps.Adapters["System.Web.UI.WebControls.RadioButton, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "";
            browserCaps.Adapters["System.Web.UI.WebControls.TextBox, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "";
            browserCaps.HtmlTextWriter = "";
            browserCaps.AddBrowser("Opera6to9");
            this.Opera6to9ProcessGateways(headers, browserCaps);
            this.OperamobilebrowserProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Opera7to9Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Opera6to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opera6to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opera6to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opera7to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "[7-9]"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.4";
            capabilities["supportsMaintainScrollPositionOnPostback"] = "true";
            browserCaps.AddBrowser("Opera7to9");
            this.Opera7to9ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Opera8to9Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Opera7to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opera7to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opera7to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opera8to9Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "[8-9]"))
            {
                return false;
            }
            capabilities["supportsCallback"] = "true";
            browserCaps.AddBrowser("Opera8to9");
            this.Opera8to9ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Opera8to9ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opera8to9ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opera8to9ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool OperamobilebrowserProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"; (?'screenWidth'\d+)x(?'screenHeight'\d+)\)"))
            {
                return false;
            }
            capabilities["backgroundSounds"] = "false";
            capabilities["canSendMail"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["JavaApplets"] = "false";
            capabilities["maximumRenderedPageSize"] = "200000";
            capabilities["msDomVersion"] = "0.0";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["requiredMetaTagNameValue"] = "HandheldFriendly";
            capabilities["requiresPragmaNoCacheHeader"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsItalic"] = "true";
            capabilities["tagwriter"] = "System.Web.UI.Html32TextWriter";
            this.OperamobilebrowserProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.OperamobilebrowserProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void OperamobilebrowserProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void OperamobilebrowserProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool OperamobileProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Linux \S+embedix"))
            {
                return false;
            }
            capabilities["isMobileDevice"] = "true";
            this.OperamobileProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.OperamobileProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void OperamobileProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void OperamobileProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool OperaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Opera[ /](?'version'(?'major'\d+)(?'minor'\.\d+)(?'letters'\w*))"))
            {
                return false;
            }
            worker.ProcessRegex(browserCaps[string.Empty], @" (?'screenWidth'\d*)x(?'screenHeight'\d*)");
            capabilities["browser"] = "Opera";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "true";
            capabilities["css2"] = "true";
            capabilities["defaultScreenCharactersHeight"] = "40";
            capabilities["defaultScreenCharactersWidth"] = "80";
            capabilities["defaultScreenPixelsHeight"] = "480";
            capabilities["defaultScreenPixelsWidth"] = "640";
            capabilities["ecmascriptversion"] = "1.1";
            capabilities["frames"] = "true";
            capabilities["javascript"] = "true";
            capabilities["letters"] = worker["${letters}"];
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "false";
            capabilities["majorversion"] = worker["${major}"];
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["screenBitDepth"] = "8";
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsDivNoWrap"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["tables"] = "true";
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["type"] = worker["Opera${major}"];
            capabilities["version"] = worker["${version}"];
            browserCaps.Adapters["System.Web.UI.WebControls.CheckBox, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.HideDisabledControlAdapter";
            browserCaps.Adapters["System.Web.UI.WebControls.RadioButton, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.HideDisabledControlAdapter";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("Opera");
            this.OperaProcessGateways(headers, browserCaps);
            this.OperamobileProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Opera1to3betaProcess(headers, browserCaps) && !this.Opera4Process(headers, browserCaps)) && (!this.Opera5to9Process(headers, browserCaps) && !this.OperapsionProcess(headers, browserCaps)))
            {
                ignoreApplicationBrowsers = false;
            }
            this.OperaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void OperaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void OperaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool OperapsionProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Mozilla/.* \(compatible; Opera .*; EPOC; (?'screenWidth'\d*)x(?'screenHeight'\d*)\)"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["canRenderEmptySelects"] = "false";
            capabilities["canSendMail"] = "false";
            capabilities["css1"] = "false";
            capabilities["css2"] = "false";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["maximumRenderedPageSize"] = "2560";
            capabilities["mobileDeviceManufacturer"] = "Psion";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "2";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "50";
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFontColor"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsSelectMultiple"] = "false";
            capabilities["tagwriter"] = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("OperaPsion");
            this.OperapsionProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.OperapsionProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void OperapsionProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void OperapsionProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opwv1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "OPWV1"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["inputType"] = "keyboard";
            capabilities["maximumRenderedPageSize"] = "3584";
            capabilities["maximumSoftkeyLabelLength"] = "9";
            capabilities["mobileDeviceManufacturer"] = "Openwave";
            capabilities["mobileDeviceModel"] = "Openwave 5.0 emulator";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "19";
            capabilities["screenPixelsHeight"] = "188";
            capabilities["screenPixelsWidth"] = "144";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            browserCaps.AddBrowser("Opwv1");
            this.Opwv1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Opwv1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opwv1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opwv1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opwvsdk6plusProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^OPWV-SDK/(?'modelVersion'[7-9]|6[^01])"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Openwave";
            capabilities["canSendMail"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["maximumRenderedPageSize"] = "66000";
            capabilities["mobileDeviceModel"] = worker["Mobile Browser ${version}"];
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresPragmaNoCacheHeader"] = "true";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsNoWrapStyle"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            browserCaps.AddBrowser("OPWVSDK6Plus");
            this.Opwvsdk6plusProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Opwvsdk6plusProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opwvsdk6plusProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opwvsdk6plusProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Opwvsdk6Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^OPWV-SDK/61"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "UP.Browser";
            capabilities["canSendMail"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["mobileDeviceModel"] = "WAP Simulator 6.1";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "6";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["screenPixelsHeight"] = "108";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            browserCaps.AddBrowser("OPWVSDK6");
            this.Opwvsdk6ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Opwvsdk6ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Opwvsdk6ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Opwvsdk6ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool OpwvsdkProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^OPWV-SDK"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceManufacturer"] = "Openwave";
            capabilities["mobileDeviceModel"] = "Mobile Browser";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["screenCharactersWidth"] = "19";
            capabilities["screenPixelsHeight"] = "188";
            capabilities["screenPixelsWidth"] = "144";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("OPWVSDK");
            this.OpwvsdkProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Opwvsdk6Process(headers, browserCaps) && !this.Opwvsdk6plusProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.OpwvsdkProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void OpwvsdkProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void OpwvsdkProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool P100Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "P-100";
            browserCaps.AddBrowser("P100");
            this.P100ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.P100ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void P100ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void P100ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool P21Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HD02"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hyundai";
            capabilities["mobileDeviceModel"] = "P-21";
            browserCaps.AddBrowser("P21");
            this.P21ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.P21ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void P21ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void P21ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PalmscapeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Palmscape/.*\(v. (?'version'[^;]+);"))
            {
                return false;
            }
            capabilities["browser"] = "Palmscape";
            capabilities["cookies"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isMobileDevice"] = "true";
            capabilities["mobileDeviceManufacturer"] = "PalmOS-licensee";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "36";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsCharacterEntityEncodign"] = "false";
            capabilities["type"] = "Palmscape";
            capabilities["version"] = worker["${version}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("Palmscape");
            this.PalmscapeProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.PalmscapeversionProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.PalmscapeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PalmscapeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PalmscapeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PalmscapeversionProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'browserMajorVersion'\d+)(?'browserMinorVersion'\.\d+)"))
            {
                return false;
            }
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            browserCaps.AddBrowser("PalmscapeVersion");
            this.PalmscapeversionProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PalmscapeversionProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PalmscapeversionProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PalmscapeversionProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Palmwebpro3Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"WebPro3\."))
            {
                return false;
            }
            capabilities["CanSendMail"] = "true";
            capabilities["InputType"] = "keyboard";
            capabilities["JavaScript"] = "true";
            capabilities["MaximumRenderedPageSize"] = "520000";
            capabilities["maximumHrefLength"] = "2000";
            capabilities["preferredRequestEncoding"] = "ISO-8859-1";
            capabilities["preferredResponseEncoding"] = "ISO-8859-1";
            capabilities["RequiresControlStateInSession"] = "false";
            capabilities["RequiresOutputOptimization"] = "false";
            capabilities["RequiresPragmaNoCacheHeader"] = "false";
            capabilities["RequiresUniqueFilePathSuffix"] = "false";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["Tables"] = "true";
            browserCaps.AddBrowser("PalmWebPro3");
            this.Palmwebpro3ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Palmwebpro3ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Palmwebpro3ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Palmwebpro3ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PalmwebproProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "PalmOS.*WebPro"))
            {
                return false;
            }
            capabilities["browser"] = "Palm WebPro";
            capabilities["canSendMail"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["javascript"] = "false";
            capabilities["maximumRenderedPageSize"] = "7000";
            capabilities["mobileDeviceManufacturer"] = "Palm";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiredMetaTagNameValue"] = "PalmComputingPlatform";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "30";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "320";
            capabilities["supportsBodyColor"] = "false";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivAlign"] = "false";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFileUpload"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["tables"] = "false";
            capabilities["type"] = "Palm WebPro";
            browserCaps.AddBrowser("PalmWebPro");
            this.PalmwebproProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Palmwebpro3Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.PalmwebproProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PalmwebproProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PalmwebproProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PanasonicexchangesupporteddeviceProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"\/(A3[89]|A[4-9]\d|A\d{3,})"))
            {
                return false;
            }
            capabilities["ExchangeOmaSupported"] = "true";
            this.PanasonicexchangesupporteddeviceProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PanasonicexchangesupporteddeviceProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PanasonicexchangesupporteddeviceProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PanasonicexchangesupporteddeviceProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Panasonicgad87a38Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"GAD87\/A38"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "12155";
            capabilities["mobileDeviceModel"] = "GU87";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            browserCaps.AddBrowser("PanasonicGAD87A38");
            this.Panasonicgad87a38ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Panasonicgad87a38ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Panasonicgad87a38ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Panasonicgad87a38ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Panasonicgad87a39Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"GAD87\/A39"))
            {
                return false;
            }
            capabilities["mobileDeviceModel"] = "GD87";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "14";
            capabilities["tables"] = "false";
            browserCaps.AddBrowser("PanasonicGAD87A39");
            this.Panasonicgad87a39ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Panasonicgad87a39ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Panasonicgad87a39ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Panasonicgad87a39ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Panasonicgad87Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "GAD87"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Panasonic";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenPixelsHeight"] = "176";
            capabilities["screenPixelsWidth"] = "132";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "false";
            capabilities["type"] = "Panasonic";
            browserCaps.AddBrowser("PanasonicGAD87");
            this.Panasonicgad87ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Panasonicgad87a39Process(headers, browserCaps) && !this.Panasonicgad87a38Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Panasonicgad87ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Panasonicgad87ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Panasonicgad87ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Panasonicgad95Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            string target = (string) browserCaps.Capabilities["mobileDeviceModel"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "GAD95"))
            {
                return false;
            }
            browserCaps.AddBrowser("PanasonicGAD95");
            this.Panasonicgad95ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Panasonicgad95ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Panasonicgad95ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Panasonicgad95ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PanasonicProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Panasonic-(?'deviceModel'.*)"))
            {
                return false;
            }
            capabilities["browser"] = "Panasonic";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["maximumSoftkeyLabelLength"] = "16";
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["mobileDeviceModel"] = worker["${deviceModel}"];
            capabilities["numberOfSoftkeys"] = "2";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersWmlDoAcceptsInline"] = "false";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "130";
            capabilities["screenPixelsWidth"] = "100";
            capabilities["supportsCacheControlMetaTag"] = "false";
            capabilities["tables"] = "false";
            capabilities["type"] = "Panasonic";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("Panasonic");
            this.PanasonicProcessGateways(headers, browserCaps);
            this.PanasonicexchangesupporteddeviceProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Panasonicgad95Process(headers, browserCaps) && !this.Panasonicgad87Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.PanasonicProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PanasonicProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PanasonicProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PdqbrowserProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "pdQbrowser"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "QCP 6035";
            capabilities["supportsFontSize"] = "false";
            browserCaps.AddBrowser("PdQbrowser");
            this.PdqbrowserProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PdqbrowserProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PdqbrowserProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PdqbrowserProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Philipsfisio820Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "PHILIPS-FISIO 820"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "3000";
            capabilities["maximumSoftkeyLabelLength"] = "6";
            capabilities["mobileDeviceManufacturer"] = "Philips";
            capabilities["mobileDeviceModel"] = "Fisio 820";
            capabilities["numberOfSoftkeys"] = "3";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["requiresNoSoftkeyLabels"] = "true";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "112";
            capabilities["screenPixelsWidth"] = "112";
            capabilities["supportsBold"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("PhilipsFisio820");
            this.Philipsfisio820ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Philipsfisio820ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Philipsfisio820ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Philipsfisio820ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Pie4ppcProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"PPC(; (?'screenWidth'\d+)x(?'screenHeight'\d+))?"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "MSIE";
            capabilities["ecmascriptversion"] = "1.0";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["maximumRenderedPageSize"] = "800000";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "16";
            capabilities["screenCharactersWidth"] = "32";
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsNoWrapStyle"] = "false";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "false";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["type"] = worker["MSIE ${majorVersion}"];
            browserCaps.AddBrowser("PIE4PPC");
            this.Pie4ppcProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Pie4ppcProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Pie4ppcProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Pie4ppcProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Pie4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"MSIE 4(\.\d*)"))
            {
                return false;
            }
            capabilities["activexcontrols"] = "true";
            capabilities["browser"] = "Pocket IE";
            capabilities["cdf"] = "true";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "12";
            capabilities["defaultScreenPixelsHeight"] = "72";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["ecmascriptversion"] = "1.2";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["maximumRenderedPageSize"] = "7000";
            capabilities["msdomversion"] = "4.0";
            capabilities["platform"] = "WinCE";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["screenCharactersWidth"] = "50";
            capabilities["screenPixelsHeight"] = "240";
            capabilities["screenPixelsWidth"] = "640";
            capabilities["supportsCss"] = "true";
            capabilities["supportsDivNoWrap"] = "true";
            capabilities["supportsFileUpload"] = "false";
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["type"] = "Pocket IE";
            capabilities["vbscript"] = "true";
            browserCaps.AddBrowser("PIE4");
            this.Pie4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Pie4ppcProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Pie4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Pie4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Pie4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Pie5plusProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"MSIE 5(\.\d*)"))
            {
                return false;
            }
            capabilities["browser"] = "IE";
            capabilities["ecmascriptversion"] = "1.2";
            capabilities["javaapplets"] = "true";
            capabilities["msdomversion"] = "5.5";
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["type"] = "Pocket IE";
            capabilities["vbscript"] = "true";
            capabilities["w3cdomversion"] = "1.0";
            capabilities["xml"] = "true";
            browserCaps.AddBrowser("PIE5Plus");
            this.Pie5plusProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Sigmarion3Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Pie5plusProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Pie5plusProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Pie5plusProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PienodeviceidProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^$"))
            {
                return false;
            }
            capabilities["supportsQueryStringInFormAction"] = "false";
            browserCaps.AddBrowser("PIEnoDeviceID");
            this.PienodeviceidProcessGateways(headers, browserCaps);
            this.PiescreenbitdepthProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PienodeviceidProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PienodeviceidProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PienodeviceidProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PieppcProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, " PPC;"))
            {
                return false;
            }
            capabilities["minorVersion"] = ".1";
            capabilities["version"] = "4.1";
            browserCaps.AddBrowser("PIEPPC");
            this.PieppcProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PieppcProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PieppcProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PieppcProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PieProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^3\.02$"))
            {
                return false;
            }
            capabilities["browser"] = "Pocket IE";
            capabilities["defaultCharacterHeight"] = "18";
            capabilities["defaultCharacterWidth"] = "7";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["javascript"] = "true";
            capabilities["majorVersion"] = "4";
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["minorVersion"] = ".0";
            capabilities["mobileDeviceModel"] = "Pocket PC";
            capabilities["optimumPageWeight"] = "4000";
            capabilities["requiresContentTypeMetaTag"] = "true";
            capabilities["requiresLeadingPageBreak"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["supportsBodyColor"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivAlign"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFontColor"] = "true";
            capabilities["supportsFontName"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["type"] = "Pocket IE";
            capabilities["version"] = "4.0";
            browserCaps.AddBrowser("PIE");
            this.PieProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.PieppcProcess(headers, browserCaps) && !this.PienodeviceidProcess(headers, browserCaps)) && !this.PiesmartphoneProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.PieProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PieProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PieProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PiescreenbitdepthProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["UA-COLOR"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "color32"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["screenBitDepth"] = "32";
            this.PiescreenbitdepthProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PiescreenbitdepthProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PiescreenbitdepthProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PiescreenbitdepthProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PiesmartphoneProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Smartphone"))
            {
                return false;
            }
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["frames"] = "false";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["minorVersion"] = ".1";
            capabilities["mobileDeviceModel"] = "Smartphone";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["screenCharactersHeight"] = "13";
            capabilities["screenCharactersWidth"] = "28";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsFontName"] = "false";
            capabilities["version"] = "4.1";
            browserCaps.AddBrowser("PIESmartphone");
            this.PiesmartphoneProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PiesmartphoneProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PiesmartphoneProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PiesmartphoneProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PixelsProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = headers["UA-PIXELS"];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            str = headers["UA-PIXELS"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(str, @"(?'screenWidth'\d+)x(?'screenHeight'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            this.PixelsProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PixelsProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PixelsProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PixelsProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Platformmac68kProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Mac(_68(000|K)|intosh.*68K)"))
            {
                return false;
            }
            capabilities["platform"] = "Mac68K";
            this.Platformmac68kProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Platformmac68kProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Platformmac68kProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Platformmac68kProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PlatformmacppcProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Mac(_PowerPC|intosh.*PPC|_PPC)|PPC Mac"))
            {
                return false;
            }
            capabilities["platform"] = "MacPPC";
            this.PlatformmacppcProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PlatformmacppcProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PlatformmacppcProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PlatformmacppcProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PlatformProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = browserCaps[string.Empty];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            this.PlatformProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((((!this.PlatformwinntProcess(headers, browserCaps) && !this.Platformwin2000bProcess(headers, browserCaps)) && (!this.Platformwin95Process(headers, browserCaps) && !this.Platformwin98Process(headers, browserCaps))) && ((!this.Platformwin16Process(headers, browserCaps) && !this.PlatformwinceProcess(headers, browserCaps)) && (!this.Platformmac68kProcess(headers, browserCaps) && !this.PlatformmacppcProcess(headers, browserCaps)))) && (!this.PlatformunixProcess(headers, browserCaps) && !this.PlatformwebtvProcess(headers, browserCaps)))
            {
                ignoreApplicationBrowsers = false;
            }
            this.PlatformProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PlatformProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PlatformProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PlatformunixProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "X11"))
            {
                return false;
            }
            capabilities["platform"] = "UNIX";
            this.PlatformunixProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PlatformunixProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PlatformunixProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PlatformunixProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PlatformwebtvProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "WebTV"))
            {
                return false;
            }
            capabilities["platform"] = "WebTV";
            this.PlatformwebtvProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PlatformwebtvProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PlatformwebtvProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PlatformwebtvProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Platformwin16Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Win(dows 3\.1|16)"))
            {
                return false;
            }
            capabilities["platform"] = "Win16";
            this.Platformwin16ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Platformwin16ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Platformwin16ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Platformwin16ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Platformwin2000aProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Windows NT 5\.0"))
            {
                return false;
            }
            capabilities["platform"] = "Win2000";
            this.Platformwin2000aProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Platformwin2000aProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Platformwin2000aProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Platformwin2000aProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Platformwin2000bProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Windows 2000"))
            {
                return false;
            }
            capabilities["platform"] = "Win2000";
            this.Platformwin2000bProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Platformwin2000bProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Platformwin2000bProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Platformwin2000bProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Platformwin95Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Win(dows )?95"))
            {
                return false;
            }
            capabilities["platform"] = "Win95";
            this.Platformwin95ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Platformwin95ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Platformwin95ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Platformwin95ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Platformwin98Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Win(dows )?98"))
            {
                return false;
            }
            capabilities["platform"] = "Win98";
            this.Platformwin98ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Platformwin98ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Platformwin98ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Platformwin98ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PlatformwinceProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Win(dows )?CE"))
            {
                return false;
            }
            capabilities["platform"] = "WinCE";
            this.PlatformwinceProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PlatformwinceProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PlatformwinceProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PlatformwinceProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PlatformwinntProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Windows NT|WinNT|Windows XP"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, "WinCE|Windows CE"))
            {
                return false;
            }
            capabilities["platform"] = "WinNT";
            this.PlatformwinntProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.PlatformwinxpProcess(headers, browserCaps) && !this.Platformwin2000aProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.PlatformwinntProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PlatformwinntProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PlatformwinntProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PlatformwinxpProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Windows (NT 5\.1|XP)"))
            {
                return false;
            }
            capabilities["platform"] = "WinXP";
            this.PlatformwinxpProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PlatformwinxpProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PlatformwinxpProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PlatformwinxpProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected override void PopulateBrowserElements(IDictionary dictionary)
        {
            base.PopulateBrowserElements(dictionary);
            dictionary["Default"] = new Triplet(null, string.Empty, 0);
            dictionary["Mozilla"] = new Triplet("Default", string.Empty, 1);
            dictionary["IE"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["IE5to9"] = new Triplet("Ie", string.Empty, 3);
            dictionary["IE6to9"] = new Triplet("Ie5to9", string.Empty, 4);
            dictionary["Treo600"] = new Triplet("Ie6to9", string.Empty, 5);
            dictionary["IE5"] = new Triplet("Ie5to9", string.Empty, 4);
            dictionary["IE50"] = new Triplet("Ie5", string.Empty, 5);
            dictionary["IE55"] = new Triplet("Ie5", string.Empty, 5);
            dictionary["IE5to9Mac"] = new Triplet("Ie5to9", string.Empty, 4);
            dictionary["IE4"] = new Triplet("Ie", string.Empty, 3);
            dictionary["IE3"] = new Triplet("Ie", string.Empty, 3);
            dictionary["IE3win16"] = new Triplet("Ie3", string.Empty, 4);
            dictionary["IE3win16a"] = new Triplet("Ie3win16", string.Empty, 5);
            dictionary["IE3Mac"] = new Triplet("Ie3", string.Empty, 4);
            dictionary["IE2"] = new Triplet("Ie", string.Empty, 3);
            dictionary["WebTV"] = new Triplet("Ie2", string.Empty, 4);
            dictionary["WebTV2"] = new Triplet("Webtv", string.Empty, 5);
            dictionary["IE1minor5"] = new Triplet("Ie", string.Empty, 3);
            dictionary["PowerBrowser"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["Gecko"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["MozillaRV"] = new Triplet("Gecko", string.Empty, 3);
            dictionary["MozillaFirebird"] = new Triplet("Mozillarv", string.Empty, 4);
            dictionary["MozillaFirefox"] = new Triplet("Mozillarv", string.Empty, 4);
            dictionary["Safari"] = new Triplet("Gecko", string.Empty, 3);
            dictionary["Safari60"] = new Triplet("Safari", string.Empty, 4);
            dictionary["Safari85"] = new Triplet("Safari", string.Empty, 4);
            dictionary["Safari1Plus"] = new Triplet("Safari", string.Empty, 4);
            dictionary["Netscape5"] = new Triplet("Gecko", string.Empty, 3);
            dictionary["Netscape6to9"] = new Triplet("Netscape5", string.Empty, 4);
            dictionary["Netscape6to9Beta"] = new Triplet("Netscape6to9", string.Empty, 5);
            dictionary["NetscapeBeta"] = new Triplet("Netscape5", string.Empty, 4);
            dictionary["AvantGo"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["TMobileSidekick"] = new Triplet("Avantgo", string.Empty, 3);
            dictionary["GoAmerica"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["GoAmericaWinCE"] = new Triplet("Goamerica", string.Empty, 3);
            dictionary["GoAmericaPalm"] = new Triplet("Goamerica", string.Empty, 3);
            dictionary["GoAmericaRIM"] = new Triplet("Goamerica", string.Empty, 3);
            dictionary["GoAmericaRIM950"] = new Triplet("Goamericarim", string.Empty, 4);
            dictionary["GoAmericaRIM850"] = new Triplet("Goamericarim", string.Empty, 4);
            dictionary["GoAmericaRIM957"] = new Triplet("Goamericarim", string.Empty, 4);
            dictionary["GoAmericaRIM957major6minor2"] = new Triplet("Goamericarim957", string.Empty, 5);
            dictionary["GoAmericaRIM857"] = new Triplet("Goamericarim", string.Empty, 4);
            dictionary["GoAmericaRIM857major6"] = new Triplet("Goamericarim857", string.Empty, 5);
            dictionary["GoAmericaRIM857major6minor2to9"] = new Triplet("Goamericarim857major6", string.Empty, 6);
            dictionary["GoAmerica7to9"] = new Triplet("Goamericarim857", string.Empty, 5);
            dictionary["Netscape3"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["Netscape4"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["Casiopeia"] = new Triplet("Netscape4", string.Empty, 3);
            dictionary["PalmWebPro"] = new Triplet("Netscape4", string.Empty, 3);
            dictionary["PalmWebPro3"] = new Triplet("Palmwebpro", string.Empty, 4);
            dictionary["NetFront"] = new Triplet("Netscape4", string.Empty, 3);
            dictionary["SLB500"] = new Triplet("Netfront", string.Empty, 4);
            dictionary["VRNA"] = new Triplet("Netfront", string.Empty, 4);
            dictionary["Mypalm"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["MyPalm1"] = new Triplet("Mypalm", string.Empty, 3);
            dictionary["Eudoraweb"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["PdQbrowser"] = new Triplet("Eudoraweb", string.Empty, 3);
            dictionary["Eudoraweb21Plus"] = new Triplet("Eudoraweb", string.Empty, 3);
            dictionary["WinCE"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["PIE"] = new Triplet("Wince", string.Empty, 3);
            dictionary["PIEPPC"] = new Triplet("Pie", string.Empty, 4);
            dictionary["PIEnoDeviceID"] = new Triplet("Pie", string.Empty, 4);
            dictionary["PIESmartphone"] = new Triplet("Pie", string.Empty, 4);
            dictionary["PIE4"] = new Triplet("Wince", string.Empty, 3);
            dictionary["PIE4PPC"] = new Triplet("Pie4", string.Empty, 4);
            dictionary["PIE5Plus"] = new Triplet("Wince", string.Empty, 3);
            dictionary["sigmarion3"] = new Triplet("Pie5plus", string.Empty, 4);
            dictionary["MSPIE"] = new Triplet("Mozilla", string.Empty, 2);
            dictionary["MSPIE2"] = new Triplet("Mspie", string.Empty, 3);
            dictionary["Docomo"] = new Triplet("Default", string.Empty, 1);
            dictionary["DocomoSH251i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoSH251iS"] = new Triplet("Docomosh251i", string.Empty, 3);
            dictionary["DocomoN251i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN251iS"] = new Triplet("Docomon251i", string.Empty, 3);
            dictionary["DocomoP211i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF212i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoD501i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF501i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN501i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP501i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoD502i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF502i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN502i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP502i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoNm502i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoSo502i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF502it"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN502it"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoSo502iwm"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF504i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN504i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP504i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN821i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP821i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoD209i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoEr209i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF209i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoKo209i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN209i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP209i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP209is"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoR209i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoR691i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF503i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF503is"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoD503i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoD503is"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoD210i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF210i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN210i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN2001"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoD211i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN211i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP210i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoKo210i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP2101v"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP2102v"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF211i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoF671i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN503is"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN503i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoSo503i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP503is"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP503i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoSo210i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoSo503is"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoSh821i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN2002"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoSo505i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoP505i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoN505i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoD505i"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["DocomoISIM60"] = new Triplet("Docomo", string.Empty, 2);
            dictionary["EricssonR380"] = new Triplet("Default", string.Empty, 1);
            dictionary["Ericsson"] = new Triplet("Default", string.Empty, 1);
            dictionary["EricssonR320"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonT20"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonT65"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonT68"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["Ericsson301A"] = new Triplet("Ericssont68", string.Empty, 3);
            dictionary["EricssonT68R1A"] = new Triplet("Ericssont68", string.Empty, 3);
            dictionary["EricssonT68R101"] = new Triplet("Ericssont68", string.Empty, 3);
            dictionary["EricssonT68R201A"] = new Triplet("Ericssont68", string.Empty, 3);
            dictionary["EricssonT300"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonP800"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonP800R101"] = new Triplet("Ericssonp800", string.Empty, 3);
            dictionary["EricssonT61"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonT31"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonR520"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonA2628"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EricssonT39"] = new Triplet("Ericsson", string.Empty, 2);
            dictionary["EzWAP"] = new Triplet("Default", string.Empty, 1);
            dictionary["GenericDownlevel"] = new Triplet("Default", string.Empty, 1);
            dictionary["Jataayu"] = new Triplet("Default", string.Empty, 1);
            dictionary["JataayuPPC"] = new Triplet("Jataayu", string.Empty, 2);
            dictionary["Jphone"] = new Triplet("Default", string.Empty, 1);
            dictionary["JphoneMitsubishi"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphoneDenso"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphoneKenwood"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphoneNec"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphoneNecN51"] = new Triplet("Jphonenec", string.Empty, 3);
            dictionary["JphonePanasonic"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphonePioneer"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphoneSanyo"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphoneSA51"] = new Triplet("Jphonesanyo", string.Empty, 3);
            dictionary["JphoneSharp"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphoneSharpSh53"] = new Triplet("Jphonesharp", string.Empty, 3);
            dictionary["JphoneSharpSh07"] = new Triplet("Jphonesharp", string.Empty, 3);
            dictionary["JphoneSharpSh08"] = new Triplet("Jphonesharp", string.Empty, 3);
            dictionary["JphoneSharpSh51"] = new Triplet("Jphonesharp", string.Empty, 3);
            dictionary["JphoneSharpSh52"] = new Triplet("Jphonesharp", string.Empty, 3);
            dictionary["JphoneToshiba"] = new Triplet("Jphone", string.Empty, 2);
            dictionary["JphoneToshibaT06a"] = new Triplet("Jphonetoshiba", string.Empty, 3);
            dictionary["JphoneToshibaT08"] = new Triplet("Jphonetoshiba", string.Empty, 3);
            dictionary["JphoneToshibaT51"] = new Triplet("Jphonetoshiba", string.Empty, 3);
            dictionary["Legend"] = new Triplet("Default", string.Empty, 1);
            dictionary["LGG5200"] = new Triplet("Legend", string.Empty, 2);
            dictionary["MME"] = new Triplet("Default", string.Empty, 1);
            dictionary["MMEF20"] = new Triplet("Mme", string.Empty, 2);
            dictionary["MMECellphone"] = new Triplet("Mmef20", string.Empty, 3);
            dictionary["MMEBenefonQ"] = new Triplet("Mmecellphone", string.Empty, 4);
            dictionary["MMESonyCMDZ5"] = new Triplet("Mmecellphone", string.Empty, 4);
            dictionary["MMESonyCMDZ5Pj020e"] = new Triplet("Mmesonycmdz5", string.Empty, 5);
            dictionary["MMESonyCMDJ5"] = new Triplet("Mmecellphone", string.Empty, 4);
            dictionary["MMESonyCMDJ7"] = new Triplet("Mmecellphone", string.Empty, 4);
            dictionary["MMEGenericSmall"] = new Triplet("Mmecellphone", string.Empty, 4);
            dictionary["MMEGenericLarge"] = new Triplet("Mmecellphone", string.Empty, 4);
            dictionary["MMEGenericFlip"] = new Triplet("Mmecellphone", string.Empty, 4);
            dictionary["MMEGeneric3D"] = new Triplet("Mmecellphone", string.Empty, 4);
            dictionary["MMEMobileExplorer"] = new Triplet("Mme", string.Empty, 2);
            dictionary["Nokia"] = new Triplet("Default", string.Empty, 1);
            dictionary["NokiaBlueprint"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["NokiaWapSimulator"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["NokiaMobileBrowser"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia7110"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia6220"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia6250"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia6310"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia6510"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia8310"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia9110i"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia9110"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia3330"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia9210"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia9210HTML"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia3590"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia3590V1"] = new Triplet("Nokia3590", string.Empty, 3);
            dictionary["Nokia3595"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia3560"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia3650"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia3650P12Plus"] = new Triplet("Nokia3650", string.Empty, 3);
            dictionary["Nokia5100"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia6200"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia6590"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia6800"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["Nokia7650"] = new Triplet("Nokia", string.Empty, 2);
            dictionary["NokiaMobileBrowserRainbow"] = new Triplet("Default", string.Empty, 1);
            dictionary["NokiaEpoc32wtl"] = new Triplet("Default", string.Empty, 1);
            dictionary["NokiaEpoc32wtl20"] = new Triplet("Nokiaepoc32wtl", string.Empty, 2);
            dictionary["Up"] = new Triplet("Default", string.Empty, 1);
            dictionary["AuMic"] = new Triplet("Up", string.Empty, 2);
            dictionary["AuMicV2"] = new Triplet("Aumic", string.Empty, 3);
            dictionary["a500"] = new Triplet("Aumic", string.Empty, 3);
            dictionary["n400"] = new Triplet("Aumic", string.Empty, 3);
            dictionary["AlcatelBe4"] = new Triplet("Up", string.Empty, 2);
            dictionary["AlcatelBe5"] = new Triplet("Up", string.Empty, 2);
            dictionary["AlcatelBe5v2"] = new Triplet("Alcatelbe5", string.Empty, 3);
            dictionary["AlcatelBe3"] = new Triplet("Up", string.Empty, 2);
            dictionary["AlcatelBf3"] = new Triplet("Up", string.Empty, 2);
            dictionary["AlcatelBf4"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotCb"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotF5"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotD8"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotCf"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotF6"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotBc"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotDc"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotPanC"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotC4"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mcca"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mot2000"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotP2kC"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotAf"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotAf418"] = new Triplet("Motaf", string.Empty, 3);
            dictionary["MotC2"] = new Triplet("Up", string.Empty, 2);
            dictionary["Xenium"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sagem959"] = new Triplet("Up", string.Empty, 2);
            dictionary["SghA300"] = new Triplet("Up", string.Empty, 2);
            dictionary["SghN100"] = new Triplet("Up", string.Empty, 2);
            dictionary["C304sa"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sy11"] = new Triplet("Up", string.Empty, 2);
            dictionary["St12"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sy14"] = new Triplet("Up", string.Empty, 2);
            dictionary["SieS40"] = new Triplet("Up", string.Empty, 2);
            dictionary["SieSl45"] = new Triplet("Up", string.Empty, 2);
            dictionary["SieS35"] = new Triplet("Up", string.Empty, 2);
            dictionary["SieMe45"] = new Triplet("Up", string.Empty, 2);
            dictionary["SieS45"] = new Triplet("Up", string.Empty, 2);
            dictionary["Gm832"] = new Triplet("Up", string.Empty, 2);
            dictionary["Gm910i"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mot32"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mot28"] = new Triplet("Up", string.Empty, 2);
            dictionary["D2"] = new Triplet("Up", string.Empty, 2);
            dictionary["PPat"] = new Triplet("Up", string.Empty, 2);
            dictionary["Alaz"] = new Triplet("Up", string.Empty, 2);
            dictionary["Cdm9100"] = new Triplet("Up", string.Empty, 2);
            dictionary["Cdm135"] = new Triplet("Up", string.Empty, 2);
            dictionary["Cdm9000"] = new Triplet("Up", string.Empty, 2);
            dictionary["C303ca"] = new Triplet("Up", string.Empty, 2);
            dictionary["C311ca"] = new Triplet("Up", string.Empty, 2);
            dictionary["C202de"] = new Triplet("Up", string.Empty, 2);
            dictionary["C409ca"] = new Triplet("Up", string.Empty, 2);
            dictionary["C402de"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ds15"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tp2200"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tp120"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ds10"] = new Triplet("Up", string.Empty, 2);
            dictionary["R280"] = new Triplet("Up", string.Empty, 2);
            dictionary["C201h"] = new Triplet("Up", string.Empty, 2);
            dictionary["S71"] = new Triplet("Up", string.Empty, 2);
            dictionary["C302h"] = new Triplet("Up", string.Empty, 2);
            dictionary["C309h"] = new Triplet("Up", string.Empty, 2);
            dictionary["C407h"] = new Triplet("Up", string.Empty, 2);
            dictionary["C451h"] = new Triplet("Up", string.Empty, 2);
            dictionary["R201"] = new Triplet("Up", string.Empty, 2);
            dictionary["P21"] = new Triplet("Up", string.Empty, 2);
            dictionary["Kyocera702g"] = new Triplet("Up", string.Empty, 2);
            dictionary["Kyocera703g"] = new Triplet("Up", string.Empty, 2);
            dictionary["KyoceraC307k"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tk01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tk02"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tk03"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tk04"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tk05"] = new Triplet("Up", string.Empty, 2);
            dictionary["D303k"] = new Triplet("Up", string.Empty, 2);
            dictionary["D304k"] = new Triplet("Up", string.Empty, 2);
            dictionary["Qcp2035"] = new Triplet("Up", string.Empty, 2);
            dictionary["Qcp3035"] = new Triplet("Up", string.Empty, 2);
            dictionary["D512"] = new Triplet("Up", string.Empty, 2);
            dictionary["Dm110"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tm510"] = new Triplet("Up", string.Empty, 2);
            dictionary["Lg13"] = new Triplet("Up", string.Empty, 2);
            dictionary["P100"] = new Triplet("Up", string.Empty, 2);
            dictionary["Lgc875f"] = new Triplet("Up", string.Empty, 2);
            dictionary["Lgp680f"] = new Triplet("Up", string.Empty, 2);
            dictionary["Lgp7800f"] = new Triplet("Up", string.Empty, 2);
            dictionary["Lgc840f"] = new Triplet("Up", string.Empty, 2);
            dictionary["Lgi2100"] = new Triplet("Up", string.Empty, 2);
            dictionary["Lgp7300f"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sd500"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tp1100"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tp3000"] = new Triplet("Up", string.Empty, 2);
            dictionary["T250"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mo01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mo02"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mc01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mccc"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mcc9"] = new Triplet("Up", string.Empty, 2);
            dictionary["Nk00"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mai12"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ma112"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ma13"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mac1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mat1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sc01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sc03"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sc02"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sc04"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sg08"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sc13"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sc11"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sec01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sc10"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sy12"] = new Triplet("Up", string.Empty, 2);
            dictionary["St11"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sy13"] = new Triplet("Up", string.Empty, 2);
            dictionary["Syc1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sy01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Syt1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sty2"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sy02"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sy03"] = new Triplet("Up", string.Empty, 2);
            dictionary["Si01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sni1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sn11"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sn12"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sn134"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sn156"] = new Triplet("Up", string.Empty, 2);
            dictionary["Snc1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tsc1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tsi1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ts11"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ts12"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ts13"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tst1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tst2"] = new Triplet("Up", string.Empty, 2);
            dictionary["Tst3"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ig01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ig02"] = new Triplet("Up", string.Empty, 2);
            dictionary["Ig03"] = new Triplet("Up", string.Empty, 2);
            dictionary["Qc31"] = new Triplet("Up", string.Empty, 2);
            dictionary["Qc12"] = new Triplet("Up", string.Empty, 2);
            dictionary["Qc32"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sp01"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sh"] = new Triplet("Up", string.Empty, 2);
            dictionary["Upg1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Opwv1"] = new Triplet("Up", string.Empty, 2);
            dictionary["Alav"] = new Triplet("Up", string.Empty, 2);
            dictionary["Im1k"] = new Triplet("Up", string.Empty, 2);
            dictionary["Nt95"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mot2001"] = new Triplet("Up", string.Empty, 2);
            dictionary["Motv200"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mot72"] = new Triplet("Up", string.Empty, 2);
            dictionary["Mot76"] = new Triplet("Up", string.Empty, 2);
            dictionary["Scp6000"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotD5"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotF0"] = new Triplet("Up", string.Empty, 2);
            dictionary["SghA400"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sec03"] = new Triplet("Up", string.Empty, 2);
            dictionary["SieC3i"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sn17"] = new Triplet("Up", string.Empty, 2);
            dictionary["Scp4700"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sec02"] = new Triplet("Up", string.Empty, 2);
            dictionary["Sy15"] = new Triplet("Up", string.Empty, 2);
            dictionary["Db520"] = new Triplet("Up", string.Empty, 2);
            dictionary["L430V03J02"] = new Triplet("Up", string.Empty, 2);
            dictionary["OPWVSDK"] = new Triplet("Up", string.Empty, 2);
            dictionary["OPWVSDK6"] = new Triplet("Opwvsdk", string.Empty, 3);
            dictionary["OPWVSDK6Plus"] = new Triplet("Opwvsdk", string.Empty, 3);
            dictionary["KDDICA21"] = new Triplet("Up", string.Empty, 2);
            dictionary["KDDITS21"] = new Triplet("Up", string.Empty, 2);
            dictionary["KDDISA21"] = new Triplet("Up", string.Empty, 2);
            dictionary["KM100"] = new Triplet("Up", string.Empty, 2);
            dictionary["LGELX5350"] = new Triplet("Up", string.Empty, 2);
            dictionary["HitachiP300"] = new Triplet("Up", string.Empty, 2);
            dictionary["SIES46"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotorolaV60G"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotorolaV708"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotorolaV708A"] = new Triplet("Up", string.Empty, 2);
            dictionary["MotorolaE360"] = new Triplet("Up", string.Empty, 2);
            dictionary["SonyericssonA1101S"] = new Triplet("Up", string.Empty, 2);
            dictionary["PhilipsFisio820"] = new Triplet("Up", string.Empty, 2);
            dictionary["CasioA5302"] = new Triplet("Up", string.Empty, 2);
            dictionary["TCLL668"] = new Triplet("Up", string.Empty, 2);
            dictionary["KDDITS24"] = new Triplet("Up", string.Empty, 2);
            dictionary["SIES55"] = new Triplet("Up", string.Empty, 2);
            dictionary["SHARPGx10"] = new Triplet("Up", string.Empty, 2);
            dictionary["BenQAthena"] = new Triplet("Up", string.Empty, 2);
            dictionary["Opera"] = new Triplet("Default", string.Empty, 1);
            dictionary["Opera1to3beta"] = new Triplet("Opera", string.Empty, 2);
            dictionary["Opera4"] = new Triplet("Opera", string.Empty, 2);
            dictionary["Opera4beta"] = new Triplet("Opera4", string.Empty, 3);
            dictionary["Opera5to9"] = new Triplet("Opera", string.Empty, 2);
            dictionary["Opera6to9"] = new Triplet("Opera5to9", string.Empty, 3);
            dictionary["Opera7to9"] = new Triplet("Opera6to9", string.Empty, 4);
            dictionary["Opera8to9"] = new Triplet("Opera7to9", string.Empty, 5);
            dictionary["OperaPsion"] = new Triplet("Opera", string.Empty, 2);
            dictionary["Palmscape"] = new Triplet("Default", string.Empty, 1);
            dictionary["PalmscapeVersion"] = new Triplet("Palmscape", string.Empty, 2);
            dictionary["AusPalm"] = new Triplet("Default", string.Empty, 1);
            dictionary["SharpPda"] = new Triplet("Default", string.Empty, 1);
            dictionary["ZaurusMiE1"] = new Triplet("Sharppda", string.Empty, 2);
            dictionary["ZaurusMiE21"] = new Triplet("Sharppda", string.Empty, 2);
            dictionary["ZaurusMiE25"] = new Triplet("Sharppda", string.Empty, 2);
            dictionary["Panasonic"] = new Triplet("Default", string.Empty, 1);
            dictionary["PanasonicGAD95"] = new Triplet("Panasonic", string.Empty, 2);
            dictionary["PanasonicGAD87"] = new Triplet("Panasonic", string.Empty, 2);
            dictionary["PanasonicGAD87A39"] = new Triplet("Panasonicgad87", string.Empty, 3);
            dictionary["PanasonicGAD87A38"] = new Triplet("Panasonicgad87", string.Empty, 3);
            dictionary["MSPIE06"] = new Triplet("Default", string.Empty, 1);
            dictionary["SKTDevices"] = new Triplet("Default", string.Empty, 1);
            dictionary["SKTDevicesHyundai"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["PSE200"] = new Triplet("Sktdeviceshyundai", string.Empty, 3);
            dictionary["SKTDevicesHanhwa"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["SKTDevicesJTEL"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["JTEL01"] = new Triplet("Sktdevicesjtel", string.Empty, 3);
            dictionary["JTELNate"] = new Triplet("Jtel01", string.Empty, 4);
            dictionary["SKTDevicesLG"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["SKTDevicesMotorola"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["SKTDevicesV730"] = new Triplet("Sktdevicesmotorola", string.Empty, 3);
            dictionary["SKTDevicesNokia"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["SKTDevicesSKTT"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["SKTDevicesSamSung"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["SCHE150"] = new Triplet("Sktdevicessamsung", string.Empty, 3);
            dictionary["SKTDevicesEricsson"] = new Triplet("Sktdevices", string.Empty, 2);
            dictionary["WinWap"] = new Triplet("Default", string.Empty, 1);
            dictionary["Xiino"] = new Triplet("Default", string.Empty, 1);
            dictionary["XiinoV2"] = new Triplet("Xiino", string.Empty, 2);
        }

        protected override void PopulateMatchedHeaders(IDictionary dictionary)
        {
            base.PopulateMatchedHeaders(dictionary);
            dictionary["X-UP-DEVCAP-NUMSOFTKEYS"] = null;
            dictionary["UA-COLOR"] = null;
            dictionary["UA-PIXELS"] = null;
            dictionary["HTTP_X_WAP_PROFILE"] = null;
            dictionary["X-UP-DEVCAP-SCREENDEPTH"] = null;
            dictionary["X-UP-DEVCAP-MSIZE"] = null;
            dictionary["X-UP-DEVCAP-MAX-PDU"] = null;
            dictionary["X-UP-DEVCAP-CHARSET"] = null;
            dictionary["X-GA-TABLES"] = null;
            dictionary["X-AVANTGO-VERSION"] = null;
            dictionary["X-UP-DEVCAP-SOFTKEYSIZE"] = null;
            dictionary["UA-VOICE"] = null;
            dictionary["X-UP-DEVCAP-ISCOLOR"] = null;
            dictionary["X-JPHONE-DISPLAY"] = null;
            dictionary["X-UP-DEVCAP-SCREENPIXELS"] = null;
            dictionary["X-JPHONE-COLOR"] = null;
            dictionary["X-UP-DEVCAP-SCREENCHARS"] = null;
            dictionary["Accept"] = null;
            dictionary[""] = null;
            dictionary["VIA"] = null;
            dictionary["X-GA-MAX-TRANSFER"] = null;
        }

        private bool PowerbrowserProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Mozilla/2\.01 \(Compatible\) Oracle\(tm\) PowerBrowser\(tm\)/1\.0a"))
            {
                return false;
            }
            capabilities["browser"] = "PowerBrowser";
            capabilities["cookies"] = "true";
            capabilities["ecmascriptversion"] = "1.0";
            capabilities["frames"] = "true";
            capabilities["javaapplets"] = "true";
            capabilities["javascript"] = "true";
            capabilities["majorversion"] = "1";
            capabilities["minorversion"] = ".5";
            capabilities["platform"] = "Win95";
            capabilities["tables"] = "true";
            capabilities["vbscript"] = "true";
            capabilities["version"] = "1.5";
            capabilities["type"] = "PowerBrowser1";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("PowerBrowser");
            this.PowerbrowserProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PowerbrowserProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PowerbrowserProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PowerbrowserProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool PpatProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "P-PAT"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Panasonic";
            capabilities["mobileDeviceModel"] = "P-PAT";
            browserCaps.AddBrowser("PPat");
            this.PpatProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.PpatProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void PpatProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void PpatProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Pse200Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "10"))
            {
                return false;
            }
            target = (string) capabilities["browserType"];
            if (!worker.ProcessRegex(target, "15"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Infraware";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceManufacturer"] = "Pantech&Curitel";
            capabilities["mobileDeviceModel"] = "PS-E200";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "true";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenPixelsHeight"] = "80";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsNoWrapStyle"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = worker["${browser} ${majorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.XhtmlTextWriter";
            browserCaps.AddBrowser("PSE200");
            this.Pse200ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Pse200ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Pse200ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Pse200ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Qc12Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "QC12"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Qualcomm";
            capabilities["mobileDeviceModel"] = "QCP-1900, QCP-2700";
            browserCaps.AddBrowser("Qc12");
            this.Qc12ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Qc12ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Qc12ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Qc12ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Qc31Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "QC31"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Qualcomm";
            capabilities["mobileDeviceModel"] = "QCP-860, QCP-1960";
            browserCaps.AddBrowser("Qc31");
            this.Qc31ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Qc31ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Qc31ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Qc31ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Qc32Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "QC32"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Qualcomm";
            capabilities["mobileDeviceModel"] = "QCP-2760";
            browserCaps.AddBrowser("Qc32");
            this.Qc32ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Qc32ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Qc32ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Qc32ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Qcp2035Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "QC06"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "QCP2035/2037";
            browserCaps.AddBrowser("Qcp2035");
            this.Qcp2035ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Qcp2035ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Qcp2035ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Qcp2035ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Qcp3035Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "QC07"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "QCP3035";
            browserCaps.AddBrowser("Qcp3035");
            this.Qcp3035ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Qcp3035ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Qcp3035ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Qcp3035ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool R201Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HD03"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hyundai";
            capabilities["mobileDeviceModel"] = "HGC-R201";
            browserCaps.AddBrowser("R201");
            this.R201ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.R201ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void R201ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void R201ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool R280Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "ERK0"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Ericsson";
            capabilities["mobileDeviceModel"] = "R280";
            browserCaps.AddBrowser("R280");
            this.R280ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.R280ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void R280ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void R280ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool S71Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HW01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hanwha";
            capabilities["mobileDeviceModel"] = "S71";
            browserCaps.AddBrowser("S71");
            this.S71ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.S71ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void S71ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void S71ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Safari1plusProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["appleWebTechnologyVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"\d\d\d"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.4";
            capabilities["w3cdomversion"] = "1.0";
            capabilities["supportsCallback"] = "true";
            browserCaps.AddBrowser("Safari1Plus");
            this.Safari1plusProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Safari1plusProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Safari1plusProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Safari1plusProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Safari60Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["appleWebTechnologyVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "60"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.0";
            browserCaps.AddBrowser("Safari60");
            this.Safari60ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Safari60ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Safari60ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Safari60ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Safari85Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["appleWebTechnologyVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "85"))
            {
                return false;
            }
            capabilities["ecmascriptversion"] = "1.4";
            browserCaps.AddBrowser("Safari85");
            this.Safari85ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Safari85ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Safari85ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Safari85ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SafariProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"AppleWebKit/(?'webversion'\d+)"))
            {
                return false;
            }
            capabilities["appleWebTechnologyVersion"] = worker["${webversion}"];
            capabilities["backgroundsounds"] = "true";
            capabilities["browser"] = "AppleMAC-Safari";
            capabilities["css1"] = "true";
            capabilities["css2"] = "true";
            capabilities["ecmascriptversion"] = "0.0";
            capabilities["futureBrowser"] = "Apple Safari";
            capabilities["screenBitDepth"] = "24";
            capabilities["tables"] = "true";
            capabilities["tagwriter"] = "System.Web.UI.HtmlTextWriter";
            capabilities["type"] = "Desktop";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("Safari");
            this.SafariProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Safari60Process(headers, browserCaps) && !this.Safari85Process(headers, browserCaps)) && !this.Safari1plusProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.SafariProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SafariProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SafariProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sagem959Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Sagem-959"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sagem";
            capabilities["mobileDeviceModel"] = "MW-959";
            browserCaps.AddBrowser("Sagem959");
            this.Sagem959ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sagem959ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sagem959ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sagem959ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sc01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SC01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SCH-3500";
            browserCaps.AddBrowser("Sc01");
            this.Sc01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sc01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sc01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sc01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sc02Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SC02"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SCH-8500";
            browserCaps.AddBrowser("Sc02");
            this.Sc02ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sc02ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sc02ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sc02ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sc03Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SC03"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SCH-6100";
            browserCaps.AddBrowser("Sc03");
            this.Sc03ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sc03ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sc03ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sc03ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sc04Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SC04"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SCH-850";
            browserCaps.AddBrowser("Sc04");
            this.Sc04ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sc04ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sc04ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sc04ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sc10Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SC10"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SCH-U02";
            browserCaps.AddBrowser("Sc10");
            this.Sc10ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sc10ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sc10ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sc10ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sc11Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SC11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SCH-N105";
            browserCaps.AddBrowser("Sc11");
            this.Sc11ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sc11ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sc11ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sc11ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sc13Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SC13"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "Uproar M100";
            browserCaps.AddBrowser("Sc13");
            this.Sc13ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sc13ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sc13ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sc13ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sche150Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["browserType"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "15"))
            {
                return false;
            }
            target = (string) capabilities["deviceID"];
            if (!worker.ProcessRegex(target, "50"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Infraware";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceModel"] = "SCH-E150";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenPixelsHeight"] = "80";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsNoWrapStyle"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsStyleElement"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = worker["${browser} ${majorVersion}"];
            browserCaps.AddBrowser("SCHE150");
            this.Sche150ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sche150ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sche150ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sche150ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Scp4700Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Sanyo-SCP4700"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "3072";
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "Sanyo SCP 4700";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["screenPixelsHeight"] = "32";
            capabilities["screenPixelsWidth"] = "91";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Scp4700");
            this.Scp4700ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Scp4700ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Scp4700ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Scp4700ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Scp6000Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Sanyo-SCP6000"))
            {
                return false;
            }
            capabilities["canRenderInputAndSelectElementsTogether"] = "false";
            capabilities["hasBackButton"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "Sanyo SCP-6000";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenPixelsHeight"] = "120";
            capabilities["screenPixelsWidth"] = "128";
            capabilities["supportsBold"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Scp6000");
            this.Scp6000ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Scp6000ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Scp6000ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Scp6000ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sd500Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG10"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "SD-500";
            browserCaps.AddBrowser("Sd500");
            this.Sd500ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sd500ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sd500ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sd500ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sec01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SEC01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SCH-U03";
            browserCaps.AddBrowser("Sec01");
            this.Sec01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sec01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sec01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sec01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sec02Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SEC02"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2867";
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "Samsung SPH-N200";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["rendersBreaksAfterWmlAnchor"] = "true";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "15";
            capabilities["screenPixelsHeight"] = "96";
            capabilities["screenPixelsWidth"] = "128";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Sec02");
            this.Sec02ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sec02ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sec02ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sec02ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sec03Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SEC03"))
            {
                return false;
            }
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "false";
            capabilities["maximumRenderedPageSize"] = "3000";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "Samsung SPH-i300";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "38";
            capabilities["screenPixelsHeight"] = "240";
            capabilities["screenPixelsWidth"] = "160";
            capabilities["supportsBold"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Sec03");
            this.Sec03ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sec03ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sec03ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sec03ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sg08Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SG08"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SGH-800";
            browserCaps.AddBrowser("Sg08");
            this.Sg08ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sg08ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sg08ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sg08ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sgha300Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SAMSUNG-SGH-A300"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2000";
            capabilities["maximumSoftkeyLabelLength"] = "19";
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SGH-A300";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "13";
            capabilities["screenPixelsHeight"] = "128";
            capabilities["screenPixelsWidth"] = "128";
            browserCaps.AddBrowser("SghA300");
            this.Sgha300ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sgha300ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sgha300ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sgha300ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sgha400Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SAMSUNG-SGH-A400"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2000";
            capabilities["maximumSoftkeyLabelLength"] = "6";
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "Samsung SGH-A400";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["requiresNoSoftkeyLabels"] = "true";
            capabilities["screenCharactersHeight"] = "3";
            capabilities["screenCharactersWidth"] = "13";
            capabilities["screenPixelsHeight"] = "96";
            capabilities["screenPixelsWidth"] = "128";
            browserCaps.AddBrowser("SghA400");
            this.Sgha400ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sgha400ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sgha400ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sgha400ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sghn100Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Samsung-SGH-N100/"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "SGH-N100";
            browserCaps.AddBrowser("SghN100");
            this.Sghn100ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sghn100ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sghn100ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sghn100ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sharpgx10Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^SHARP\-TQ\-GX10"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Openwave";
            capabilities["canSendMail"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceManufacturer"] = "Sharp";
            capabilities["mobileDeviceModel"] = "GX10";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["screenCharactersWidth"] = "18";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsNoWrapStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsTitleElement"] = "false";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("SHARPGx10");
            this.Sharpgx10ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sharpgx10ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sharpgx10ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sharpgx10ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SharppdaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"sharp pda browser/(?'browserMajorVersion'\d+)(?'browserMinorVersion'\.\d+)"))
            {
                return false;
            }
            capabilities["browser"] = "Sharp PDA Browser";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["mobileDeviceManufacturer"] = "Sharp";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["supportsCharacterEntityEncoding"] = "false";
            capabilities["type"] = "Sharp PDA Browser";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("SharpPda");
            this.SharppdaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.Zaurusmie1Process(headers, browserCaps) && !this.Zaurusmie21Process(headers, browserCaps)) && !this.Zaurusmie25Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.SharppdaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SharppdaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SharppdaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool ShProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SH"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            capabilities["mobileDeviceModel"] = "Duette";
            browserCaps.AddBrowser("Sh");
            this.ShProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.ShProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void ShProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void ShProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Si01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SI01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "S25";
            browserCaps.AddBrowser("Si01");
            this.Si01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Si01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Si01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Si01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Siec3iProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SIE-C3I"))
            {
                return false;
            }
            capabilities["canRenderMixedSelects"] = "false";
            capabilities["maximumRenderedPageSize"] = "1900";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "C35/M35";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "false";
            capabilities["requiresSpecialViewStateEncoding"] = "false";
            capabilities["requiresUrlEncodedPostfieldValues"] = "false";
            capabilities["screenCharactersHeight"] = "3";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "54";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsBold"] = "true";
            browserCaps.AddBrowser("SieC3i");
            this.Siec3iProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Siec3iProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Siec3iProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Siec3iProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sieme45Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SIE-ME45"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2800";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "ME45";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "65";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            browserCaps.AddBrowser("SieMe45");
            this.Sieme45ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sieme45ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sieme45ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sieme45ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sies35Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SIE-S35"))
            {
                return false;
            }
            capabilities["canRenderMixedSelects"] = "false";
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "S35";
            browserCaps.AddBrowser("SieS35");
            this.Sies35ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sies35ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sies35ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sies35ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sies40Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SIE-S40"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["maximumRenderedPageSize"] = "2048";
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "S40, S42";
            browserCaps.AddBrowser("SieS40");
            this.Sies40ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sies40ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sies40ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sies40ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sies45Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SIE-S45"))
            {
                return false;
            }
            capabilities["maximumRenderedPageSize"] = "2765";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "S45";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "true";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "16";
            capabilities["screenPixelsHeight"] = "64";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            browserCaps.AddBrowser("SieS45");
            this.Sies45ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sies45ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sies45ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sies45ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sies46Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SIE-S46"))
            {
                return false;
            }
            capabilities["hasBackButton"] = "false";
            capabilities["maximumRenderedPageSize"] = "2700";
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "S46";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "19";
            capabilities["screenPixelsHeight"] = "72";
            capabilities["screenPixelsWidth"] = "96";
            capabilities["supportsBold"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("SIES46");
            this.Sies46ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sies46ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sies46ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sies46ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sies55Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SIE-S55"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["browser"] = "Openwave";
            capabilities["canSendMail"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["isColor"] = "true";
            capabilities["maximumHrefLength"] = "980";
            capabilities["maximumRenderedPageSize"] = "10000";
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "S55";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            capabilities["preferredRenderingType"] = "xhtml-basic";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "4";
            capabilities["screenCharactersWidth"] = "19";
            capabilities["screenPixelsHeight"] = "80";
            capabilities["screenPixelsWidth"] = "101";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsFontSize"] = "false";
            capabilities["supportsNoWrapStyle"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsTitleElement"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("SIES55");
            this.Sies55ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sies55ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sies55ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sies55ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Siesl45Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SIE-SL45"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Siemens";
            capabilities["mobileDeviceModel"] = "SL-45";
            browserCaps.AddBrowser("SieSl45");
            this.Siesl45ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Siesl45ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Siesl45ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Siesl45ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sigmarion3Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "sigmarion3"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "64000";
            capabilities["mobileDeviceManufacturer"] = "NTT DoCoMo";
            capabilities["MobileDeviceModel"] = "Sigmarion III";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRequestEncoding"] = "shift_jis";
            capabilities["preferredResponseEncoding"] = "shift_jis";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "19";
            capabilities["screenCharactersWidth"] = "94";
            capabilities["screenPixelsHeight"] = "480";
            capabilities["screenPixelsWidth"] = "800";
            browserCaps.AddBrowser("sigmarion3");
            this.Sigmarion3ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sigmarion3ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sigmarion3ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sigmarion3ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicescolordepthProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["colorDepth"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^(\d[1-9])|([1-9]0)$"))
            {
                return false;
            }
            capabilities["screenBitDepth"] = worker["${colorDepth}"];
            this.SktdevicescolordepthProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.SktdevicesiscolorProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.SktdevicescolordepthProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicescolordepthProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicescolordepthProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesericssonProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["browserType"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "01"))
            {
                return false;
            }
            capabilities["browser"] = "Ericsson";
            browserCaps.AddBrowser("SKTDevicesEricsson");
            this.SktdevicesericssonProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdevicesericssonProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesericssonProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesericssonProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdeviceshanhwaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceManufacturer"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HH"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hanhwa";
            browserCaps.AddBrowser("SKTDevicesHanhwa");
            this.SktdeviceshanhwaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdeviceshanhwaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdeviceshanhwaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdeviceshanhwaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdeviceshyundaiProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceManufacturer"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "HD"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Hyundai";
            browserCaps.AddBrowser("SKTDevicesHyundai");
            this.SktdeviceshyundaiProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Pse200Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.SktdeviceshyundaiProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdeviceshyundaiProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdeviceshyundaiProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesiscolorProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["colorDepth"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^(0[5-9])|([1-9]\d)$"))
            {
                return false;
            }
            capabilities["isColor"] = "true";
            this.SktdevicesiscolorProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdevicesiscolorProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesiscolorProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesiscolorProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesjtelProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceManufacturer"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "JT"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "JTEL";
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.ChtmlTextWriter";
            browserCaps.AddBrowser("SKTDevicesJTEL");
            this.SktdevicesjtelProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Jtel01Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.SktdevicesjtelProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesjtelProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesjtelProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdeviceslgProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceManufacturer"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            browserCaps.AddBrowser("SKTDevicesLG");
            this.SktdeviceslgProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdeviceslgProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdeviceslgProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdeviceslgProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesmotorolaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceManufacturer"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MT"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Motorola";
            browserCaps.AddBrowser("SKTDevicesMotorola");
            this.SktdevicesmotorolaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Sktdevicesv730Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.SktdevicesmotorolaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesmotorolaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesmotorolaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesnokiaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceManufacturer"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "NO"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Nokia";
            browserCaps.AddBrowser("SKTDevicesNokia");
            this.SktdevicesnokiaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdevicesnokiaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesnokiaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesnokiaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^(?'carrier'\S{3})(?'serviceType'\d)(?'deviceType'\d)(?'deviceManufacturer'\S{2})(?'deviceID'\S{2})(?'browserType'\d{2})(?'majorVersion'\d)(?'minorVersion'\d)(?'screenWidth'\d{3})(?'screenHeight'\d{3})(?'screenColumn'\d{2})(?'screenRow'\d{2})(?'colorDepth'\d{2})(?'MINNumber'\d{8})"))
            {
                return false;
            }
            capabilities["browserType"] = worker["${browserType}"];
            capabilities["colorDepth"] = worker["${colorDepth}"];
            capabilities["cookies"] = "false";
            capabilities["deviceManufacturer"] = worker["${deviceManufacturer}"];
            capabilities["deviceID"] = worker["${deviceID}"];
            capabilities["deviceType"] = worker["${deviceType}"];
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${majorVersion}"];
            capabilities["minorVersion"] = worker["${minorVersion}"];
            capabilities["screenColumn"] = worker["${screenColumn}"];
            capabilities["screenHeight"] = worker["${screenHeight}"];
            capabilities["screenWidth"] = worker["${screenWidth}"];
            capabilities["screenRow"] = worker["${screenRow}"];
            capabilities["version"] = worker["${majorVersion}.${minorVersion}"];
            browserCaps.AddBrowser("SKTDevices");
            this.SktdevicesProcessGateways(headers, browserCaps);
            this.SktdevicescolordepthProcess(headers, browserCaps);
            this.SktdevicesscreenrowProcess(headers, browserCaps);
            this.SktdevicesscreencolumnProcess(headers, browserCaps);
            this.SktdevicesscreenheightProcess(headers, browserCaps);
            this.SktdevicesscreenwidthProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((((!this.SktdeviceshyundaiProcess(headers, browserCaps) && !this.SktdeviceshanhwaProcess(headers, browserCaps)) && (!this.SktdevicesjtelProcess(headers, browserCaps) && !this.SktdeviceslgProcess(headers, browserCaps))) && ((!this.SktdevicesmotorolaProcess(headers, browserCaps) && !this.SktdevicesnokiaProcess(headers, browserCaps)) && (!this.SktdevicesskttProcess(headers, browserCaps) && !this.SktdevicessamsungProcess(headers, browserCaps)))) && !this.SktdevicesericssonProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.SktdevicesProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicessamsungProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceManufacturer"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SS"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Samsung";
            browserCaps.AddBrowser("SKTDevicesSamSung");
            this.SktdevicessamsungProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Sche150Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.SktdevicessamsungProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicessamsungProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicessamsungProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesscreencolumnProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["screenColumn"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^(\d[1-9])|([1-9]0)$"))
            {
                return false;
            }
            capabilities["screenCharactersWidth"] = worker["${screenColumn}"];
            this.SktdevicesscreencolumnProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdevicesscreencolumnProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesscreencolumnProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesscreencolumnProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesscreenheightProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["screenHeight"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^(\d\d[1-9])|(((\d[1-9])|([1-9]0))0)$"))
            {
                return false;
            }
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            this.SktdevicesscreenheightProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdevicesscreenheightProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesscreenheightProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesscreenheightProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesscreenrowProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["screenRow"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^(\d[1-9])|([1-9]0)$"))
            {
                return false;
            }
            capabilities["screenCharactersHeight"] = worker["${screenRow}"];
            this.SktdevicesscreenrowProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdevicesscreenrowProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesscreenrowProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesscreenrowProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesscreenwidthProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["screenWidth"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^(\d\d[1-9])|(((\d[1-9])|([1-9]0))0)$"))
            {
                return false;
            }
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            this.SktdevicesscreenwidthProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdevicesscreenwidthProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesscreenwidthProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesscreenwidthProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SktdevicesskttProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceManufacturer"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SK"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "SKTT";
            browserCaps.AddBrowser("SKTDevicesSKTT");
            this.SktdevicesskttProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SktdevicesskttProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SktdevicesskttProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SktdevicesskttProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sktdevicesv730Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "07"))
            {
                return false;
            }
            target = (string) capabilities["browserType"];
            if (!worker.ProcessRegex(target, "00"))
            {
                return false;
            }
            capabilities["browser"] = "AU-System";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "false";
            capabilities["maximumRenderedPageSize"] = "2900";
            capabilities["mobileDeviceModel"] = "V730";
            capabilities["numberOfSoftkeys"] = "2";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml11";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "true";
            capabilities["rendersBreaksAfterWmlInput"] = "true";
            capabilities["rendersWmlSelectsAsMenuCards"] = "true";
            capabilities["requiresNoSoftkeyLabels"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["type"] = worker["${browser} ${majorVersion}"];
            browserCaps.AddBrowser("SKTDevicesV730");
            this.Sktdevicesv730ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sktdevicesv730ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sktdevicesv730ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sktdevicesv730ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Slb500Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SL-B500"))
            {
                return false;
            }
            capabilities["browser"] = "NetFront";
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "60000";
            capabilities["mobileDeviceManufacturer"] = "Sharp";
            capabilities["mobileDeviceModel"] = "SL-B500";
            capabilities["preferredImageMime"] = "image/gif";
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["requiresNoBreakInFormatting"] = "true";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "21";
            capabilities["screenCharactersWidth"] = "47";
            capabilities["screenPixelsHeight"] = "240";
            capabilities["screenPixelsWidth"] = "320";
            capabilities["supportsAccessKeyAttribute"] = "false";
            capabilities["supportsBold"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsMultilineTextboxDisplay"] = "true";
            browserCaps.AddBrowser("SLB500");
            this.Slb500ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Slb500ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Slb500ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Slb500ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sn11Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SN11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "C305SN";
            browserCaps.AddBrowser("Sn11");
            this.Sn11ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sn11ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sn11ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sn11ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sn12Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SN12"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "C404S";
            browserCaps.AddBrowser("Sn12");
            this.Sn12ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sn12ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sn12ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sn12ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sn134Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SN1[34]"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "C406S";
            browserCaps.AddBrowser("Sn134");
            this.Sn134ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sn134ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sn134ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sn134ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sn156Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SN1[56]"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "C413S";
            browserCaps.AddBrowser("Sn156");
            this.Sn156ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sn156ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sn156ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sn156ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sn17Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SN17"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["maximumRenderedPageSize"] = "12000";
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "C1002S";
            capabilities["numberOfSoftkeys"] = "3";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "20";
            capabilities["screenPixelsHeight"] = "120";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Sn17");
            this.Sn17ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sn17ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sn17ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sn17ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Snc1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SNC1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "D306S";
            browserCaps.AddBrowser("Snc1");
            this.Snc1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Snc1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Snc1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Snc1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sni1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SNI1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "705G";
            browserCaps.AddBrowser("Sni1");
            this.Sni1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sni1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sni1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sni1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sonyericssona1101sProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KDDI-SN22"))
            {
                return false;
            }
            capabilities["breaksOnInlineElements"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["maximumRenderedPageSize"] = "9000";
            capabilities["mobileDeviceManufacturer"] = "SonyEricsson";
            capabilities["mobileDeviceModel"] = "A1101S";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresAbsolutePostbackUrl"] = "false";
            capabilities["requiresCommentInStyleElement"] = "false";
            capabilities["requiresHiddenFieldValues"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOnEnterForwardForCheckboxLists"] = "false";
            capabilities["requiresXhtmlCssSuppression"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "9";
            capabilities["screenPixelsHeight"] = "123";
            capabilities["screenPixelsWidth"] = "120";
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyClassAttribute"] = "true";
            capabilities["supportsCss"] = "true";
            capabilities["supportsSelectFollowingTable"] = "true";
            capabilities["supportsUrlAttributeEncoding"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("SonyericssonA1101S");
            this.Sonyericssona1101sProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sonyericssona1101sProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sonyericssona1101sProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sonyericssona1101sProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool SonyericssonProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^SonyEricsson"))
            {
                return false;
            }
            capabilities["browser"] = "Sony Ericsson";
            capabilities["mobileDeviceManufacturer"] = "Sony Ericsson";
            capabilities["type"] = worker["Sony Ericsson ${mobileDeviceModel}"];
            this.SonyericssonProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.SonyericssonProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void SonyericssonProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void SonyericssonProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sp01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SP01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["mobileDeviceModel"] = "MA120";
            browserCaps.AddBrowser("Sp01");
            this.Sp01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sp01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sp01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sp01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool St11Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "ST11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "C403ST";
            browserCaps.AddBrowser("St11");
            this.St11ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.St11ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void St11ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void St11ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool St12Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "ST12"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "C411ST";
            browserCaps.AddBrowser("St12");
            this.St12ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.St12ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void St12ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void St12ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sty2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SYT2"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "TS02";
            browserCaps.AddBrowser("Sty2");
            this.Sty2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sty2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sty2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sty2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sy01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SY01"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "SCP-4000";
            browserCaps.AddBrowser("Sy01");
            this.Sy01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sy01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sy01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sy01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sy02Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SY02"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "SCP-4500";
            browserCaps.AddBrowser("Sy02");
            this.Sy02ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sy02ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sy02ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sy02ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sy03Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SY03"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "SCP-5000";
            browserCaps.AddBrowser("Sy03");
            this.Sy03ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sy03ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sy03ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sy03ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sy11Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SY11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "C304SA";
            browserCaps.AddBrowser("Sy11");
            this.Sy11ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sy11ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sy11ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sy11ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sy12Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SY12"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "C401SA";
            browserCaps.AddBrowser("Sy12");
            this.Sy12ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sy12ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sy12ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sy12ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sy13Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SY13"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "C405SA";
            browserCaps.AddBrowser("Sy13");
            this.Sy13ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sy13ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sy13ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sy13ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sy14Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SY14"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "C412SA";
            browserCaps.AddBrowser("Sy14");
            this.Sy14ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sy14ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sy14ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sy14ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Sy15Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SY15"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["maximumRenderedPageSize"] = "7500";
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "Sanyo C1001SA";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenCharactersHeight"] = "8";
            capabilities["supportsRedirectWithCookie"] = "true";
            browserCaps.AddBrowser("Sy15");
            this.Sy15ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Sy15ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Sy15ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Sy15ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Syc1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SYC1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "D301SA";
            browserCaps.AddBrowser("Syc1");
            this.Syc1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Syc1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Syc1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Syc1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Syt1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "SYT1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sanyo";
            capabilities["mobileDeviceModel"] = "TS01";
            browserCaps.AddBrowser("Syt1");
            this.Syt1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Syt1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Syt1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Syt1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool T250Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "T250"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Mitsubishi";
            capabilities["mobileDeviceModel"] = "T250";
            browserCaps.AddBrowser("T250");
            this.T250ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.T250ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void T250ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void T250ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tcll668Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Compal-Seville"))
            {
                return false;
            }
            capabilities["browser"] = "Openwave";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "3800";
            capabilities["maximumSoftkeyLabelLength"] = "7";
            capabilities["mobileDeviceManufacturer"] = "TCL";
            capabilities["mobileDeviceModel"] = "L668";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "false";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "7";
            capabilities["screenCharactersWidth"] = "14";
            capabilities["screenPixelsHeight"] = "160";
            capabilities["screenPixelsWidth"] = "128";
            capabilities["supportsBold"] = "true";
            capabilities["supportsEmptyStringInCookieValue"] = "false";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["type"] = worker["Openwave ${majorVersion}.x Browser"];
            browserCaps.AddBrowser("TCLL668");
            this.Tcll668ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tcll668ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tcll668ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tcll668ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tk01Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCT1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "TK01";
            browserCaps.AddBrowser("Tk01");
            this.Tk01ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tk01ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tk01ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tk01ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tk02Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCT2"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "TK02";
            browserCaps.AddBrowser("Tk02");
            this.Tk02ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tk02ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tk02ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tk02ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tk03Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCT4"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "TK03";
            browserCaps.AddBrowser("Tk03");
            this.Tk03ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tk03ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tk03ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tk03ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tk04Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCT5"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "TK04";
            browserCaps.AddBrowser("Tk04");
            this.Tk04ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tk04ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tk04ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tk04ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tk05Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "KCT6"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Kyocera";
            capabilities["mobileDeviceModel"] = "TK05";
            browserCaps.AddBrowser("Tk05");
            this.Tk05ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tk05ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tk05ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tk05ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tm510Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG21"))
            {
                return false;
            }
            capabilities["canRenderPostBackCards"] = "false";
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "TM-510";
            browserCaps.AddBrowser("Tm510");
            this.Tm510ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tm510ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tm510ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tm510ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool TmobilesidekickProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Danger hiptop"))
            {
                return false;
            }
            capabilities["canSendMail"] = "true";
            capabilities["css1"] = "true";
            capabilities["ecmaScriptVersion"] = "1.3";
            capabilities["frames"] = "true";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["javaapplets"] = "true";
            capabilities["majorVersion"] = "5";
            capabilities["maximumRenderedPageSize"] = "7000";
            capabilities["minorVersion"] = ".0";
            capabilities["mobileDeviceManufacturer"] = "T-Mobile";
            capabilities["mobileDeviceModel"] = "SideKick";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["requiresLeadingPageBreak"] = "false";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "11";
            capabilities["screenCharactersWidth"] = "57";
            capabilities["screenPixelsHeight"] = "136";
            capabilities["screenPixelsWidth"] = "236";
            capabilities["supportsCss"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["type"] = "AvantGo 3";
            capabilities["version"] = "5.0";
            browserCaps.AddBrowser("TMobileSidekick");
            this.TmobilesidekickProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.TmobilesidekickProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void TmobilesidekickProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void TmobilesidekickProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tp1100Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG06"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "Touchpoint TP1100";
            browserCaps.AddBrowser("Tp1100");
            this.Tp1100ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tp1100ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tp1100ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tp1100ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tp120Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "DS12"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Denso";
            capabilities["mobileDeviceModel"] = "TouchPoint TP120";
            browserCaps.AddBrowser("Tp120");
            this.Tp120ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tp120ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tp120ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tp120ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tp2200Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "DS1[34]"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Denso";
            capabilities["mobileDeviceModel"] = "TouchPoint TP2200";
            capabilities["screenCharactersHeight"] = "5";
            capabilities["screenCharactersWidth"] = "15";
            browserCaps.AddBrowser("Tp2200");
            this.Tp2200ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tp2200ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tp2200ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tp2200ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tp3000Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "LG08"))
            {
                return false;
            }
            capabilities["canRenderAfterInputOrSelectElement"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["mobileDeviceManufacturer"] = "LG";
            capabilities["mobileDeviceModel"] = "Touchpoint TP3000";
            browserCaps.AddBrowser("Tp3000");
            this.Tp3000ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tp3000ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tp3000ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tp3000ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Treo600Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "PalmSource; Blazer"))
            {
                return false;
            }
            worker.ProcessRegex(browserCaps[string.Empty], @"PalmSource; Blazer 3\.0\)\s\d+;(?'screenPixelsHeight'\d+)x(?'screenPixelsWidth'\d+)$");
            capabilities["browser"] = "Blazer 3.0";
            capabilities["cachesAllResponsesWithExpires"] = "false";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canRenderEmptySelects"] = "true";
            capabilities["canSendMail"] = "true";
            capabilities["cookies"] = "true";
            capabilities["ecmascriptversion"] = "1.1";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "false";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["javascript"] = "true";
            capabilities["jscriptversion"] = "0.0";
            capabilities["maximumHrefLength"] = "10000";
            capabilities["maximumRenderedPageSize"] = "300000";
            capabilities["mobileDeviceManufacturer"] = "";
            capabilities["mobileDeviceModel"] = "";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["preferredRequestEncoding"] = "utf-8";
            capabilities["preferredResponseEncoding"] = "utf-8";
            capabilities["rendersBreaksAfterHtmlLists"] = "true";
            capabilities["requiredMetaTagNameValue"] = "PalmComputingPlatform";
            capabilities["requiresAttributeColonSubstitution"] = "false";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["requiresControlStateInSession"] = "false";
            capabilities["requiresDBCSCharacter"] = "false";
            capabilities["requiresFullyQualifiedRedirectUrl"] = "false";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "false";
            capabilities["requiresLeadingPageBreak"] = "false";
            capabilities["requiresNoBreakInFormatting"] = "false";
            capabilities["requiresOutputOptimization"] = "false";
            capabilities["requiresPostRedirectionHandling"] = "false";
            capabilities["requiresPragmaNoCacheHeader"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["requiresUniqueHtmlCheckboxNames"] = "false";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "13";
            capabilities["screenCharactersWidth"] = "32";
            capabilities["screenPixelsHeight"] = worker["${screenPixelsHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenPixelsWidth}"];
            capabilities["supportsAccessKeyAttribute"] = "true";
            capabilities["supportsBodyColor"] = "true";
            capabilities["supportsBold"] = "true";
            capabilities["supportsCharacterEntityEncoding"] = "true";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivAlign"] = "true";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsEmptyStringInCookieValue"] = "true";
            capabilities["supportsFileUpload"] = "false";
            capabilities["supportsFontColor"] = "true";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsIModeSymbols"] = "false";
            capabilities["supportsInputIStyle"] = "false";
            capabilities["supportsInputMode"] = "false";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsJPhoneMultiMediaAttributes"] = "false";
            capabilities["supportsJPhoneSymbols"] = "false";
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["supportsQueryStringInFormAction"] = "true";
            capabilities["supportsRedirectWithCookie"] = "true";
            capabilities["supportsSelectMultiple"] = "true";
            capabilities["supportsUncheck"] = "true";
            capabilities["tables"] = "true";
            capabilities["type"] = "Handspring Treo 600";
            browserCaps.AddBrowser("Treo600");
            this.Treo600ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Treo600ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Treo600ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Treo600ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ts11Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TS11"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "C301T";
            browserCaps.AddBrowser("Ts11");
            this.Ts11ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ts11ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ts11ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ts11ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ts12Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TS12"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "C310T";
            browserCaps.AddBrowser("Ts12");
            this.Ts12ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ts12ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ts12ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ts12ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Ts13Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TS13"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "C410T";
            browserCaps.AddBrowser("Ts13");
            this.Ts13ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Ts13ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Ts13ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Ts13ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tsc1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TSC1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "D302T";
            browserCaps.AddBrowser("Tsc1");
            this.Tsc1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tsc1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tsc1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tsc1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tsi1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TSI1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "701G";
            browserCaps.AddBrowser("Tsi1");
            this.Tsi1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tsi1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tsi1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tsi1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tst1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TST1"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "TT01";
            browserCaps.AddBrowser("Tst1");
            this.Tst1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tst1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tst1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tst1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tst2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TST2"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "TT02";
            browserCaps.AddBrowser("Tst2");
            this.Tst2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tst2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tst2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tst2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Tst3Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "TST3"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Toshiba";
            capabilities["mobileDeviceModel"] = "TT03";
            browserCaps.AddBrowser("Tst3");
            this.Tst3ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Tst3ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Tst3ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Tst3ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Up3Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "3"))
            {
                return false;
            }
            capabilities["canRenderInputAndSelectElementsTogether"] = "false";
            capabilities["preferredImageMime"] = "image/bmp";
            capabilities["requiresAbsolutePostbackUrl"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["requiresUrlEncodedPostfieldValues"] = "true";
            capabilities["type"] = "Phone.com 3.x Browser";
            this.Up3ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Up3ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Up3ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Up3ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Up4Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "4"))
            {
                return false;
            }
            capabilities["requiresAbsolutePostbackUrl"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            this.Up4ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Up4ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Up4ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Up4ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Up5Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "5"))
            {
                return false;
            }
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            this.Up5ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Up5ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Up5ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Up5ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Up61plusProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["minorVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^\.[^0]"))
            {
                return false;
            }
            capabilities["preferredRenderingMime"] = "application/xhtml+xml";
            this.Up61plusProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Up61plusProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Up61plusProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Up61plusProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Up6Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["majorVersion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "6"))
            {
                return false;
            }
            capabilities["cookies"] = "true";
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "xhtml-mp";
            capabilities["supportsStyleElement"] = "true";
            capabilities["type"] = worker["Openwave ${majorVersion}.x Browser"];
            browserCaps.HtmlTextWriter = "System.Web.UI.XhtmlTextWriter";
            this.Up6ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Up61plusProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.Up6ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Up6ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Up6ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpdefaultscreencharactersProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-SCREENCHARS"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^$"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["defaultScreenCharactersWidth"] = "15";
            capabilities["defaultScreenCharactersHeight"] = "4";
            this.UpdefaultscreencharactersProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpdefaultscreencharactersProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpdefaultscreencharactersProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpdefaultscreencharactersProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpdefaultscreenpixelsProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-SCREENPIXELS"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^$"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["defaultScreenPixelsWidth"] = "120";
            capabilities["defaultScreenPixelsHeight"] = "40";
            this.UpdefaultscreenpixelsProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpdefaultscreenpixelsProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpdefaultscreenpixelsProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpdefaultscreenpixelsProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Upg1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "UPG1"))
            {
                return false;
            }
            capabilities["canSendMail"] = "false";
            capabilities["mobileDeviceManufacturer"] = "OpenWave";
            capabilities["mobileDeviceModel"] = "Generic Simulator";
            browserCaps.AddBrowser("Upg1");
            this.Upg1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Upg1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Upg1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Upg1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpgatewayProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"UP\.Link/"))
            {
                return false;
            }
            worker.ProcessRegex(browserCaps[string.Empty], @"(?'goWebUPGateway'Go\.Web)");
            capabilities["isGoWebUpGateway"] = worker["${goWebUPGateway}"];
            this.UpgatewayProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.UpnongogatewayProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.UpgatewayProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpgatewayProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpgatewayProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpmaxpduProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-MAX-PDU"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'maxDeckSize'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["maximumRenderedPageSize"] = worker["${maxDeckSize}"];
            this.UpmaxpduProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpmaxpduProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpmaxpduProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpmaxpduProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpmsizeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-MSIZE"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'width'\d+),(?'height'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["characterHeight"] = worker["${height}"];
            capabilities["characterWidth"] = worker["${width}"];
            this.UpmsizeProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpmsizeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpmsizeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpmsizeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpnongogatewayProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"UP\.Link/(?'gatewayMajorVersion'\d*)(?'gatewayMinorVersion'\.\d*)(?'other'\S*)"))
            {
                return false;
            }
            target = (string) capabilities["isGoWebUpGateway"];
            if (!worker.ProcessRegex(target, "^$"))
            {
                return false;
            }
            capabilities["gatewayMajorVersion"] = worker["${gatewayMajorVersion}"];
            capabilities["gatewayMinorVersion"] = worker["${gatewayMinorVersion}"];
            capabilities["gatewayVersion"] = worker["UP.Link/${gatewayMajorVersion}${gatewayMinorVersion}${other}"];
            this.UpnongogatewayProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpnongogatewayProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpnongogatewayProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpnongogatewayProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpnumsoftkeysProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-NUMSOFTKEYS"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'softkeys'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["numberOfSoftkeys"] = worker["${softkeys}"];
            this.UpnumsoftkeysProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpnumsoftkeysProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpnumsoftkeysProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpnumsoftkeysProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(UP\.Browser)|(UP/)"))
            {
                return false;
            }
            target = browserCaps[string.Empty];
            if (worker.ProcessRegex(target, @"Go\.Web"))
            {
                return false;
            }
            worker.ProcessRegex(browserCaps[string.Empty], @"((?'deviceID'\S*) UP/\S* UP\.Browser/((?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*) UP\.Link/)|((?'deviceID'\S*)/\S* UP(\.Browser)*/((?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*))|(UP\.Browser/((?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*)-(?'deviceID'\S*) UP\.Link/)|((?'deviceID'\S*) UP\.Browser/((?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*) UP\.Link/)|((?'deviceID'\S*)/(?'DeviceVersion'\S*) UP/((?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*))|((?'deviceID'\S*)/(?'DeviceVersion'\S*) UP.Browser/((?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*))|((?'deviceID'\S*) UP.Browser/((?'browserMajorVersion'\d*)(?'browserMinorVersion'\.\d*)\S*))");
            capabilities["browser"] = "Phone.com";
            capabilities["canInitiateVoiceCall"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["deviceID"] = worker["${deviceID}"];
            capabilities["deviceVersion"] = worker["${deviceVersion}"];
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "1492";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["numberOfSoftkeys"] = "2";
            capabilities["optimumPageWeight"] = "700";
            capabilities["preferredImageMime"] = "image/vnd.wap.wbmp";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml11";
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["rendersBreakBeforeWmlSelectAndInput"] = "true";
            capabilities["rendersWmlDoAcceptsInline"] = "false";
            capabilities["rendersWmlSelectsAsMenuCards"] = "true";
            capabilities["requiresFullyQualifiedRedirectUrl"] = "true";
            capabilities["requiresNoescapedPostUrl"] = "true";
            capabilities["requiresPostRedirectionHandling"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["type"] = worker["Phone.com${browserMajorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.AddBrowser("Up");
            this.UpProcessGateways(headers, browserCaps);
            this.UpdefaultscreencharactersProcess(headers, browserCaps);
            this.UpdefaultscreenpixelsProcess(headers, browserCaps);
            this.UpscreendepthProcess(headers, browserCaps);
            this.UpscreencharsProcess(headers, browserCaps);
            this.UpscreenpixelsProcess(headers, browserCaps);
            this.UpmsizeProcess(headers, browserCaps);
            this.IscolorProcess(headers, browserCaps);
            this.UpnumsoftkeysProcess(headers, browserCaps);
            this.UpsoftkeysizeProcess(headers, browserCaps);
            this.UpmaxpduProcess(headers, browserCaps);
            this.UpversionProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((((((((!this.AumicProcess(headers, browserCaps) && !this.Alcatelbe4Process(headers, browserCaps)) && (!this.Alcatelbe5Process(headers, browserCaps) && !this.Alcatelbe3Process(headers, browserCaps))) && ((!this.Alcatelbf3Process(headers, browserCaps) && !this.Alcatelbf4Process(headers, browserCaps)) && (!this.MotcbProcess(headers, browserCaps) && !this.Motf5Process(headers, browserCaps)))) && (((!this.Motd8Process(headers, browserCaps) && !this.MotcfProcess(headers, browserCaps)) && (!this.Motf6Process(headers, browserCaps) && !this.MotbcProcess(headers, browserCaps))) && ((!this.MotdcProcess(headers, browserCaps) && !this.MotpancProcess(headers, browserCaps)) && (!this.Motc4Process(headers, browserCaps) && !this.MccaProcess(headers, browserCaps))))) && ((((!this.Mot2000Process(headers, browserCaps) && !this.Motp2kcProcess(headers, browserCaps)) && (!this.MotafProcess(headers, browserCaps) && !this.Motc2Process(headers, browserCaps))) && ((!this.XeniumProcess(headers, browserCaps) && !this.Sagem959Process(headers, browserCaps)) && (!this.Sgha300Process(headers, browserCaps) && !this.Sghn100Process(headers, browserCaps)))) && (((!this.C304saProcess(headers, browserCaps) && !this.Sy11Process(headers, browserCaps)) && (!this.St12Process(headers, browserCaps) && !this.Sy14Process(headers, browserCaps))) && ((!this.Sies40Process(headers, browserCaps) && !this.Siesl45Process(headers, browserCaps)) && (!this.Sies35Process(headers, browserCaps) && !this.Sieme45Process(headers, browserCaps)))))) && (((((!this.Sies45Process(headers, browserCaps) && !this.Gm832Process(headers, browserCaps)) && (!this.Gm910iProcess(headers, browserCaps) && !this.Mot32Process(headers, browserCaps))) && ((!this.Mot28Process(headers, browserCaps) && !this.D2Process(headers, browserCaps)) && (!this.PpatProcess(headers, browserCaps) && !this.AlazProcess(headers, browserCaps)))) && (((!this.Cdm9100Process(headers, browserCaps) && !this.Cdm135Process(headers, browserCaps)) && (!this.Cdm9000Process(headers, browserCaps) && !this.C303caProcess(headers, browserCaps))) && ((!this.C311caProcess(headers, browserCaps) && !this.C202deProcess(headers, browserCaps)) && (!this.C409caProcess(headers, browserCaps) && !this.C402deProcess(headers, browserCaps))))) && ((((!this.Ds15Process(headers, browserCaps) && !this.Tp2200Process(headers, browserCaps)) && (!this.Tp120Process(headers, browserCaps) && !this.Ds10Process(headers, browserCaps))) && ((!this.R280Process(headers, browserCaps) && !this.C201hProcess(headers, browserCaps)) && (!this.S71Process(headers, browserCaps) && !this.C302hProcess(headers, browserCaps)))) && (((!this.C309hProcess(headers, browserCaps) && !this.C407hProcess(headers, browserCaps)) && (!this.C451hProcess(headers, browserCaps) && !this.R201Process(headers, browserCaps))) && ((!this.P21Process(headers, browserCaps) && !this.Kyocera702gProcess(headers, browserCaps)) && (!this.Kyocera703gProcess(headers, browserCaps) && !this.Kyocerac307kProcess(headers, browserCaps))))))) && ((((((!this.Tk01Process(headers, browserCaps) && !this.Tk02Process(headers, browserCaps)) && (!this.Tk03Process(headers, browserCaps) && !this.Tk04Process(headers, browserCaps))) && ((!this.Tk05Process(headers, browserCaps) && !this.D303kProcess(headers, browserCaps)) && (!this.D304kProcess(headers, browserCaps) && !this.Qcp2035Process(headers, browserCaps)))) && (((!this.Qcp3035Process(headers, browserCaps) && !this.D512Process(headers, browserCaps)) && (!this.Dm110Process(headers, browserCaps) && !this.Tm510Process(headers, browserCaps))) && ((!this.Lg13Process(headers, browserCaps) && !this.P100Process(headers, browserCaps)) && (!this.Lgc875fProcess(headers, browserCaps) && !this.Lgp680fProcess(headers, browserCaps))))) && ((((!this.Lgp7800fProcess(headers, browserCaps) && !this.Lgc840fProcess(headers, browserCaps)) && (!this.Lgi2100Process(headers, browserCaps) && !this.Lgp7300fProcess(headers, browserCaps))) && ((!this.Sd500Process(headers, browserCaps) && !this.Tp1100Process(headers, browserCaps)) && (!this.Tp3000Process(headers, browserCaps) && !this.T250Process(headers, browserCaps)))) && (((!this.Mo01Process(headers, browserCaps) && !this.Mo02Process(headers, browserCaps)) && (!this.Mc01Process(headers, browserCaps) && !this.McccProcess(headers, browserCaps))) && ((!this.Mcc9Process(headers, browserCaps) && !this.Nk00Process(headers, browserCaps)) && (!this.Mai12Process(headers, browserCaps) && !this.Ma112Process(headers, browserCaps)))))) && (((((!this.Ma13Process(headers, browserCaps) && !this.Mac1Process(headers, browserCaps)) && (!this.Mat1Process(headers, browserCaps) && !this.Sc01Process(headers, browserCaps))) && ((!this.Sc03Process(headers, browserCaps) && !this.Sc02Process(headers, browserCaps)) && (!this.Sc04Process(headers, browserCaps) && !this.Sg08Process(headers, browserCaps)))) && (((!this.Sc13Process(headers, browserCaps) && !this.Sc11Process(headers, browserCaps)) && (!this.Sec01Process(headers, browserCaps) && !this.Sc10Process(headers, browserCaps))) && ((!this.Sy12Process(headers, browserCaps) && !this.St11Process(headers, browserCaps)) && (!this.Sy13Process(headers, browserCaps) && !this.Syc1Process(headers, browserCaps))))) && ((((!this.Sy01Process(headers, browserCaps) && !this.Syt1Process(headers, browserCaps)) && (!this.Sty2Process(headers, browserCaps) && !this.Sy02Process(headers, browserCaps))) && ((!this.Sy03Process(headers, browserCaps) && !this.Si01Process(headers, browserCaps)) && (!this.Sni1Process(headers, browserCaps) && !this.Sn11Process(headers, browserCaps)))) && (((!this.Sn12Process(headers, browserCaps) && !this.Sn134Process(headers, browserCaps)) && (!this.Sn156Process(headers, browserCaps) && !this.Snc1Process(headers, browserCaps))) && ((!this.Tsc1Process(headers, browserCaps) && !this.Tsi1Process(headers, browserCaps)) && (!this.Ts11Process(headers, browserCaps) && !this.Ts12Process(headers, browserCaps)))))))) && ((((((!this.Ts13Process(headers, browserCaps) && !this.Tst1Process(headers, browserCaps)) && (!this.Tst2Process(headers, browserCaps) && !this.Tst3Process(headers, browserCaps))) && ((!this.Ig01Process(headers, browserCaps) && !this.Ig02Process(headers, browserCaps)) && (!this.Ig03Process(headers, browserCaps) && !this.Qc31Process(headers, browserCaps)))) && (((!this.Qc12Process(headers, browserCaps) && !this.Qc32Process(headers, browserCaps)) && (!this.Sp01Process(headers, browserCaps) && !this.ShProcess(headers, browserCaps))) && ((!this.Upg1Process(headers, browserCaps) && !this.Opwv1Process(headers, browserCaps)) && (!this.AlavProcess(headers, browserCaps) && !this.Im1kProcess(headers, browserCaps))))) && ((((!this.Nt95Process(headers, browserCaps) && !this.Mot2001Process(headers, browserCaps)) && (!this.Motv200Process(headers, browserCaps) && !this.Mot72Process(headers, browserCaps))) && ((!this.Mot76Process(headers, browserCaps) && !this.Scp6000Process(headers, browserCaps)) && (!this.Motd5Process(headers, browserCaps) && !this.Motf0Process(headers, browserCaps)))) && (((!this.Sgha400Process(headers, browserCaps) && !this.Sec03Process(headers, browserCaps)) && (!this.Siec3iProcess(headers, browserCaps) && !this.Sn17Process(headers, browserCaps))) && ((!this.Scp4700Process(headers, browserCaps) && !this.Sec02Process(headers, browserCaps)) && (!this.Sy15Process(headers, browserCaps) && !this.Db520Process(headers, browserCaps)))))) && (((((!this.L430v03j02Process(headers, browserCaps) && !this.OpwvsdkProcess(headers, browserCaps)) && (!this.Kddica21Process(headers, browserCaps) && !this.Kddits21Process(headers, browserCaps))) && ((!this.Kddisa21Process(headers, browserCaps) && !this.Km100Process(headers, browserCaps)) && (!this.Lgelx5350Process(headers, browserCaps) && !this.Hitachip300Process(headers, browserCaps)))) && (((!this.Sies46Process(headers, browserCaps) && !this.Motorolav60gProcess(headers, browserCaps)) && (!this.Motorolav708Process(headers, browserCaps) && !this.Motorolav708aProcess(headers, browserCaps))) && ((!this.Motorolae360Process(headers, browserCaps) && !this.Sonyericssona1101sProcess(headers, browserCaps)) && (!this.Philipsfisio820Process(headers, browserCaps) && !this.Casioa5302Process(headers, browserCaps))))) && (((!this.Tcll668Process(headers, browserCaps) && !this.Kddits24Process(headers, browserCaps)) && (!this.Sies55Process(headers, browserCaps) && !this.Sharpgx10Process(headers, browserCaps))) && !this.BenqathenaProcess(headers, browserCaps)))))
            {
                ignoreApplicationBrowsers = false;
            }
            this.UpProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpscreencharsProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-SCREENCHARS"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'screenCharsWidth'[1-9]\d*),(?'screenCharsHeight'[1-9]\d*)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["screenCharactersHeight"] = worker["${screenCharsHeight}"];
            capabilities["screenCharactersWidth"] = worker["${screenCharsWidth}"];
            this.UpscreencharsProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpscreencharsProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpscreencharsProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpscreencharsProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpscreendepthProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-SCREENDEPTH"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'screenDepth'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["screenBitDepth"] = worker["${screenDepth}"];
            this.UpscreendepthProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpscreendepthProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpscreendepthProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpscreendepthProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpscreenpixelsProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-SCREENPIXELS"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'screenPixWidth'[1-9]\d*),(?'screenPixHeight'[1-9]\d*)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["screenPixelsHeight"] = worker["${screenPixHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenPixWidth}"];
            this.UpscreenpixelsProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpscreenpixelsProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpscreenpixelsProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpscreenpixelsProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpsoftkeysizeProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = headers["X-UP-DEVCAP-SOFTKEYSIZE"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"(?'softkeySize'\d+)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["maximumSoftkeyLabelLength"] = worker["${softkeySize}"];
            this.UpsoftkeysizeProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.UpsoftkeysizeProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpsoftkeysizeProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpsoftkeysizeProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool UpversionProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            RegexWorker worker = new RegexWorker(browserCaps);
            capabilities["type"] = worker["Phone.com ${majorVersion}.x Browser"];
            this.UpversionProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (((!this.BlazerProcess(headers, browserCaps) && !this.Up4Process(headers, browserCaps)) && (!this.Up5Process(headers, browserCaps) && !this.Up6Process(headers, browserCaps))) && !this.Up3Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.UpversionProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void UpversionProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void UpversionProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool VoiceProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = headers["UA-VOICE"];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            str = headers["UA-VOICE"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(str, "(?i:TRUE)"))
            {
                return false;
            }
            browserCaps.DisableOptimizedCacheKey();
            capabilities["canInitiateVoiceCall"] = "true";
            this.VoiceProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.VoiceProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void VoiceProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void VoiceProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool VrnaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "sony/model vrna"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Sony";
            capabilities["mobileDeviceModel"] = "CLIE PEG-TG50";
            capabilities["canInitiateVoiceCall"] = "false";
            capabilities["canSendMail"] = "true";
            capabilities["ExchangeOmaSupported"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["javascript"] = "true";
            capabilities["maximumRenderedPageSize"] = "65000";
            capabilities["preferredRenderingMime"] = "text/html";
            capabilities["preferredRenderingType"] = "html32";
            capabilities["requiresHtmlAdaptiveErrorReporting"] = "true";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["requiresPragmaNoCacheHeader"] = "true";
            capabilities["requiresUniqueFilePathSuffix"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "10";
            capabilities["screenCharactersWidth"] = "31";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "320";
            capabilities["supportsAccessKeyAttribute"] = "false";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["supportsMultilineTextboxDisplay"] = "true";
            browserCaps.AddBrowser("VRNA");
            this.VrnaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.VrnaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void VrnaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void VrnaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Webtv2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["minorversion"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "2"))
            {
                return false;
            }
            capabilities["css1"] = "true";
            capabilities["ecmascriptversion"] = "1.0";
            capabilities["isMobileDevice"] = "false";
            capabilities["javascript"] = "true";
            capabilities["supportsBold"] = "false";
            capabilities["supportsCss"] = "false";
            capabilities["supportsDivNoWrap"] = "false";
            capabilities["supportsFontName"] = "false";
            capabilities["supportsFontSize"] = "false";
            capabilities["supportsImageSubmit"] = "false";
            capabilities["supportsItalic"] = "false";
            browserCaps.AddBrowser("WebTV2");
            this.Webtv2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Webtv2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Webtv2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Webtv2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool WebtvbetaProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["letters"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "^b"))
            {
                return false;
            }
            capabilities["beta"] = "true";
            this.WebtvbetaProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.WebtvbetaProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void WebtvbetaProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void WebtvbetaProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool WebtvProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"WebTV/(?'version'(?'major'\d+)(?'minor'\.\d+)(?'letters'\w*))"))
            {
                return false;
            }
            capabilities["backgroundsounds"] = "true";
            capabilities["browser"] = "WebTV";
            capabilities["cookies"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["letters"] = worker["${letters}"];
            capabilities["majorversion"] = worker["${major}"];
            capabilities["minorversion"] = worker["${minor}"];
            capabilities["tables"] = "true";
            capabilities["type"] = worker["WebTV${major}"];
            capabilities["version"] = worker["${version}"];
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("WebTV");
            this.WebtvProcessGateways(headers, browserCaps);
            this.WebtvbetaProcess(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Webtv2Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.WebtvProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void WebtvProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void WebtvProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Win16Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"16bit|Win(dows 3\.1|16)"))
            {
                return false;
            }
            capabilities["win16"] = "true";
            this.Win16ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Win16ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Win16ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Win16ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Win32Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Win(dows )?(9[58]|NT|32)"))
            {
                return false;
            }
            capabilities["win32"] = "true";
            this.Win32ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Win32ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Win32ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Win32ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool WinceProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^Mozilla/\S* \(compatible; MSIE (?'majorVersion'\d*)(?'minorVersion'\.\d*);\D* Windows CE(;(?'deviceID' \D\w*))?(; (?'screenWidth'\d+)x(?'screenHeight'\d+))?"))
            {
                return false;
            }
            capabilities["activexcontrols"] = "true";
            capabilities["backgroundsounds"] = "true";
            capabilities["browser"] = "WinCE";
            capabilities["cookies"] = "true";
            capabilities["css1"] = "true";
            capabilities["defaultScreenCharactersHeight"] = "6";
            capabilities["defaultScreenCharactersWidth"] = "12";
            capabilities["defaultScreenPixelsHeight"] = "72";
            capabilities["defaultScreenPixelsWidth"] = "96";
            capabilities["deviceID"] = worker["${deviceID}"];
            capabilities["ecmascriptversion"] = "1.0";
            capabilities["frames"] = "true";
            capabilities["inputType"] = "telephoneKeypad";
            capabilities["isColor"] = "false";
            capabilities["isMobileDevice"] = "true";
            capabilities["javascript"] = "true";
            capabilities["jscriptversion"] = "1.0";
            capabilities["majorVersion"] = worker["${majorVersion}"];
            capabilities["minorVersion"] = worker["${minorVersion}"];
            capabilities["platform"] = "WinCE";
            capabilities["screenBitDepth"] = "1";
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            capabilities["supportsMultilineTextBoxDisplay"] = "true";
            capabilities["tables"] = "true";
            capabilities["version"] = worker["${majorVersion}${minorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("WinCE");
            this.WinceProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if ((!this.PieProcess(headers, browserCaps) && !this.Pie4Process(headers, browserCaps)) && !this.Pie5plusProcess(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.WinceProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void WinceProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void WinceProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool WinProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string str = browserCaps[string.Empty];
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            this.WinProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Win32Process(headers, browserCaps) && !this.Win16Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.WinProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void WinProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void WinProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool WinwapProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^WinWAP-(?'platform'\w*)/(?'browserMajorVersion'\w*)(?'browserMinorVersion'\.\w*)"))
            {
                return false;
            }
            capabilities["browser"] = "WinWAP";
            capabilities["canRenderAfterInputOrSelectElement"] = "false";
            capabilities["canSendMail"] = "false";
            capabilities["cookies"] = "false";
            capabilities["hasBackButton"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "3500";
            capabilities["maximumSoftkeyLabelLength"] = "21";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["platform"] = worker["${platform}"];
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["preferredRenderingMime"] = "text/vnd.wap.wml";
            capabilities["preferredRenderingType"] = "wml12";
            capabilities["rendersBreaksAfterWmlInput"] = "false";
            capabilities["rendersWmlSelectsAsMenuCards"] = "false";
            capabilities["requiresSpecialViewStateEncoding"] = "true";
            capabilities["requiresUniqueFilepathSuffix"] = "true";
            capabilities["screenBitDepth"] = "24";
            capabilities["screenCharactersHeight"] = "16";
            capabilities["screenCharactersWidth"] = "43";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "240";
            capabilities["supportsBold"] = "true";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsItalic"] = "true";
            capabilities["supportsRedirectWithCookie"] = "false";
            capabilities["type"] = worker["WinWAP ${browserMajorVersion}${browserMinorVersion}"];
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.Adapters["System.Web.UI.WebControls.Menu, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"] = "System.Web.UI.WebControls.Adapters.MenuAdapter";
            browserCaps.AddBrowser("WinWap");
            this.WinwapProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.WinwapProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void WinwapProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void WinwapProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool XeniumProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["deviceID"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "Philips-Xenium9@9"))
            {
                return false;
            }
            capabilities["mobileDeviceManufacturer"] = "Philips";
            capabilities["mobileDeviceModel"] = "Xenium 9";
            browserCaps.AddBrowser("Xenium");
            this.XeniumProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.XeniumProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void XeniumProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void XeniumProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool XiinoProcess(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"Xiino/(?'browserMajorVersion'\d+)(?'browserMinorVersion'\.\d+).* (?'screenWidth'\d+)x(?'screenHeight'\d+);"))
            {
                return false;
            }
            capabilities["browser"] = "Xiino";
            capabilities["canRenderEmptySelects"] = "true";
            capabilities["canSendMail"] = "false";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "false";
            capabilities["inputType"] = "virtualKeyboard";
            capabilities["isColor"] = "true";
            capabilities["isMobileDevice"] = "true";
            capabilities["majorVersion"] = worker["${browserMajorVersion}"];
            capabilities["maximumRenderedPageSize"] = "65000";
            capabilities["minorVersion"] = worker["${browserMinorVersion}"];
            capabilities["requiresAdaptiveErrorReporting"] = "true";
            capabilities["requiresAttributeColonSubstitution"] = "false";
            capabilities["screenBitDepth"] = "8";
            capabilities["screenCharactersHeight"] = "12";
            capabilities["screenCharactersWidth"] = "30";
            capabilities["screenPixelsHeight"] = worker["${screenHeight}"];
            capabilities["screenPixelsWidth"] = worker["${screenWidth}"];
            capabilities["supportsBold"] = "true";
            capabilities["supportsCharacterEntityEncoding"] = "false";
            capabilities["supportsFontSize"] = "true";
            capabilities["type"] = "Xiino";
            capabilities["version"] = worker["${browserMajorVersion}${browserMinorVersion}"];
            browserCaps.HtmlTextWriter = "System.Web.UI.Html32TextWriter";
            browserCaps.AddBrowser("Xiino");
            this.XiinoProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = true;
            if (!this.Xiinov2Process(headers, browserCaps))
            {
                ignoreApplicationBrowsers = false;
            }
            this.XiinoProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void XiinoProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void XiinoProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Xiinov2Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = (string) capabilities["version"];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, @"^2\.0$"))
            {
                return false;
            }
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["supportsItalic"] = "true";
            browserCaps.AddBrowser("XiinoV2");
            this.Xiinov2ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Xiinov2ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Xiinov2ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Xiinov2ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Zaurusmie1Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MI-E1"))
            {
                return false;
            }
            capabilities["defaultCharacterHeight"] = "18";
            capabilities["defaultCharacterWidth"] = "7";
            capabilities["frames"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["javascript"] = "false";
            capabilities["mobileDeviceModel"] = "Zaurus MI-E1";
            capabilities["requiresDBCSCharacter"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenPixelsHeight"] = "240";
            capabilities["screenPixelsWidth"] = "320";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("ZaurusMiE1");
            this.Zaurusmie1ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Zaurusmie1ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Zaurusmie1ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Zaurusmie1ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Zaurusmie21Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MI-E21"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["canRenderEmptySelects"] = "false";
            capabilities["cookies"] = "true";
            capabilities["hidesRightAlignedMultiselectScrollbars"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "60000";
            capabilities["mobileDeviceModel"] = "Zaurus MI-E21";
            capabilities["requiresAttributeColonSubstitution"] = "true";
            capabilities["requiresDBCSCharacter"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "18";
            capabilities["screenCharactersWidth"] = "40";
            capabilities["screenPixelsHeight"] = "320";
            capabilities["screenPixelsWidth"] = "240";
            capabilities["supportsFontSize"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("ZaurusMiE21");
            this.Zaurusmie21ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Zaurusmie21ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Zaurusmie21ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Zaurusmie21ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        private bool Zaurusmie25Process(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
            IDictionary capabilities = browserCaps.Capabilities;
            string target = browserCaps[string.Empty];
            RegexWorker worker = new RegexWorker(browserCaps);
            if (!worker.ProcessRegex(target, "MI-E25"))
            {
                return false;
            }
            capabilities["cachesAllResponsesWithExpires"] = "true";
            capabilities["cookies"] = "true";
            capabilities["inputType"] = "keyboard";
            capabilities["isColor"] = "true";
            capabilities["maximumRenderedPageSize"] = "60000";
            capabilities["mobileDeviceModel"] = "Zaurus MI-E25DC";
            capabilities["preferredImageMime"] = "image/jpeg";
            capabilities["requiresContentTypeMetaTag"] = "false";
            capabilities["requiresDBCSCharacter"] = "true";
            capabilities["requiresOutputOptimization"] = "true";
            capabilities["screenBitDepth"] = "16";
            capabilities["screenCharactersHeight"] = "11";
            capabilities["screenCharactersWidth"] = "50";
            capabilities["screenPixelsHeight"] = "240";
            capabilities["screenPixelsWidth"] = "320";
            capabilities["supportsFontSize"] = "true";
            capabilities["supportsImageSubmit"] = "true";
            capabilities["tables"] = "true";
            browserCaps.AddBrowser("ZaurusMiE25");
            this.Zaurusmie25ProcessGateways(headers, browserCaps);
            bool ignoreApplicationBrowsers = false;
            this.Zaurusmie25ProcessBrowsers(ignoreApplicationBrowsers, headers, browserCaps);
            return true;
        }

        protected virtual void Zaurusmie25ProcessBrowsers(bool ignoreApplicationBrowsers, NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }

        protected virtual void Zaurusmie25ProcessGateways(NameValueCollection headers, HttpBrowserCapabilities browserCaps)
        {
        }
    }
}

