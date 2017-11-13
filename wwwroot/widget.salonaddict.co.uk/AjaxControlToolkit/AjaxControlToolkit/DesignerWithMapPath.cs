namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Web.UI.Design;

    public class DesignerWithMapPath : ControlDesigner
    {
        public string MapPath(string originalPath)
        {
            string relativeUrl = null;
            ISite site = base.Component.Site;
            if (site != null)
            {
                IWebApplication service = (IWebApplication) site.GetService(typeof(IWebApplication));
                if (service == null)
                {
                    return relativeUrl;
                }
                string str2 = originalPath.Replace("/", @"\");
                bool flag = false;
                while ((str2.Length > 0) && ((str2.Substring(0, 1) == @"\") || (str2.Substring(0, 1) == "~")))
                {
                    flag = true;
                    str2 = str2.Substring(1);
                    if (str2.Length == 0)
                    {
                        break;
                    }
                }
                string physicalPath = service.RootProjectItem.PhysicalPath;
                if (flag)
                {
                    relativeUrl = Path.Combine(physicalPath, str2);
                }
                else
                {
                    string str4 = Path.GetDirectoryName(base.RootDesigner.DocumentUrl).Replace("/", @"\");
                    while ((str4.Length > 0) && ((str4.Substring(0, 1) == @"\") || (str4.Substring(0, 1) == "~")))
                    {
                        str4 = str4.Substring(1);
                        if (str4.Length == 0)
                        {
                            break;
                        }
                    }
                    relativeUrl = Path.Combine(Path.Combine(physicalPath, str4), str2);
                }
                relativeUrl = base.RootDesigner.ResolveUrl(relativeUrl).Substring(8).Replace("/", @"\");
                if (relativeUrl.IndexOf(physicalPath, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    relativeUrl = null;
                }
            }
            return relativeUrl;
        }
    }
}

