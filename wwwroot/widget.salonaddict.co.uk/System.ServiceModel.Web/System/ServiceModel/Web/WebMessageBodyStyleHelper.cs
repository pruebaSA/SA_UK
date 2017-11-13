namespace System.ServiceModel.Web
{
    using System;

    internal static class WebMessageBodyStyleHelper
    {
        internal static bool IsDefined(WebMessageBodyStyle style)
        {
            if (((style != WebMessageBodyStyle.Bare) && (style != WebMessageBodyStyle.Wrapped)) && (style != WebMessageBodyStyle.WrappedRequest))
            {
                return (style == WebMessageBodyStyle.WrappedResponse);
            }
            return true;
        }
    }
}

