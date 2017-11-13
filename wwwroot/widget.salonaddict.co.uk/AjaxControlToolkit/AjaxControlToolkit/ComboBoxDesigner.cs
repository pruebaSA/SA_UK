namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web.UI.Design.WebControls;
    using System.Web.UI.WebControls;

    public class ComboBoxDesigner : ListControlDesigner
    {
        public override string GetDesignTimeHtml()
        {
            ListControl viewControl = (ListControl) base.ViewControl;
            System.Web.UI.WebControls.ListItem[] array = new System.Web.UI.WebControls.ListItem[viewControl.Items.Count];
            viewControl.Items.CopyTo(array, 0);
            string designTimeHtml = base.GetDesignTimeHtml();
            viewControl.Items.Clear();
            viewControl.Items.AddRange(array);
            string input = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ComboBox.ComboBox.css")).ReadToEnd();
            input = new Regex("(<%=)\\s*(WebResource\\(\")(?<resourceName>.+)\\s*(\"\\)%>)").Replace(input, new MatchEvaluator(this.PerformWebResourceSubstitution));
            return ("<style>" + input + "</style>" + designTimeHtml);
        }

        protected virtual string PerformWebResourceSubstitution(Match match) => 
            match.ToString().Replace(match.Value, base.ViewControl.Page.ClientScript.GetWebResourceUrl(base.GetType(), match.Groups["resourceName"].Value));

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection lists = new DesignerActionListCollection();
                lists.AddRange(base.ActionLists);
                lists.Add(new ComboBoxDesignerActionList(base.Component));
                return lists;
            }
        }

        public override bool AllowResize =>
            false;
    }
}

