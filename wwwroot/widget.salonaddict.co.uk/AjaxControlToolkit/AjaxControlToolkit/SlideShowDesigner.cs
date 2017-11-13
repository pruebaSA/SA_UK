namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Design;
    using System;
    using System.Runtime.CompilerServices;

    public class SlideShowDesigner : ExtenderControlBaseDesigner<SlideShowExtender>
    {
        [PageMethodSignature("SlideShow", "SlideShowServicePath", "SlideShowServiceMethod", "UseContextKey")]
        private delegate Slide[] GetSlides(string contextKey);
    }
}

