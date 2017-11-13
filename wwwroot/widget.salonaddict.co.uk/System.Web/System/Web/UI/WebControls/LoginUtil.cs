namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.Net.Mail;
    using System.Runtime.CompilerServices;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;

    internal static class LoginUtil
    {
        private const string _passwordReplacementKey = @"<%\s*Password\s*%>";
        private const string _templateDesignerRegion = "0";
        private const string _userNameReplacementKey = @"<%\s*UserName\s*%>";

        internal static void ApplyStyleToLiteral(Literal literal, string text, Style style, bool setTableCellVisible)
        {
            bool visible = false;
            if (!string.IsNullOrEmpty(text))
            {
                literal.Text = text;
                if (style != null)
                {
                    SetTableCellStyle(literal, style);
                }
                visible = true;
            }
            if (setTableCellVisible)
            {
                SetTableCellVisible(literal, visible);
            }
            else
            {
                literal.Visible = visible;
            }
        }

        internal static void CopyBorderStyles(WebControl control, Style style)
        {
            if ((style != null) && !style.IsEmpty)
            {
                control.BorderStyle = style.BorderStyle;
                control.BorderColor = style.BorderColor;
                control.BorderWidth = style.BorderWidth;
                control.BackColor = style.BackColor;
                control.CssClass = style.CssClass;
            }
        }

        internal static void CopyStyleToInnerControl(WebControl control, Style style)
        {
            if ((style != null) && !style.IsEmpty)
            {
                control.ForeColor = style.ForeColor;
                control.Font.CopyFrom(style.Font);
            }
        }

        internal static Table CreateChildTable(bool convertingToTemplate)
        {
            if (convertingToTemplate)
            {
                return new Table();
            }
            return new ChildTable(2);
        }

        private static MailMessage CreateMailMessage(string email, string userName, string password, MailDefinition mailDefinition, string defaultBody, Control owner)
        {
            ListDictionary replacements = new ListDictionary();
            if (mailDefinition.IsBodyHtml)
            {
                userName = HttpUtility.HtmlEncode(userName);
                password = HttpUtility.HtmlEncode(password);
            }
            replacements.Add(@"<%\s*UserName\s*%>", userName);
            replacements.Add(@"<%\s*Password\s*%>", password);
            if (string.IsNullOrEmpty(mailDefinition.BodyFileName) && (defaultBody != null))
            {
                return mailDefinition.CreateMailMessage(email, replacements, defaultBody, owner);
            }
            return mailDefinition.CreateMailMessage(email, replacements, owner);
        }

        internal static MembershipProvider GetProvider(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return Membership.Provider;
            }
            MembershipProvider provider = Membership.Providers[providerName];
            if (provider == null)
            {
                throw new HttpException(System.Web.SR.GetString("WebControl_CantFindProvider"));
            }
            return provider;
        }

        internal static IPrincipal GetUser(Control c)
        {
            IPrincipal user = null;
            Page page = c.Page;
            if (page != null)
            {
                return page.User;
            }
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                user = current.User;
            }
            return user;
        }

        internal static string GetUserName(Control c)
        {
            string name = null;
            IPrincipal user = GetUser(c);
            if (user != null)
            {
                IIdentity identity = user.Identity;
                if (identity != null)
                {
                    name = identity.Name;
                }
            }
            return name;
        }

        internal static void SendPasswordMail(string email, string userName, string password, MailDefinition mailDefinition, string defaultSubject, string defaultBody, OnSendingMailDelegate onSendingMailDelegate, OnSendMailErrorDelegate onSendMailErrorDelegate, Control owner)
        {
            try
            {
                new MailAddress(email);
            }
            catch (Exception exception)
            {
                SendMailErrorEventArgs e = new SendMailErrorEventArgs(exception) {
                    Handled = true
                };
                onSendMailErrorDelegate(e);
                return;
            }
            try
            {
                using (MailMessage message = CreateMailMessage(email, userName, password, mailDefinition, defaultBody, owner))
                {
                    if ((mailDefinition.SubjectInternal == null) && (defaultSubject != null))
                    {
                        message.Subject = defaultSubject;
                    }
                    MailMessageEventArgs args2 = new MailMessageEventArgs(message);
                    onSendingMailDelegate(args2);
                    if (!args2.Cancel)
                    {
                        new SmtpClient().Send(message);
                    }
                }
            }
            catch (Exception exception2)
            {
                SendMailErrorEventArgs args3 = new SendMailErrorEventArgs(exception2);
                onSendMailErrorDelegate(args3);
                if (!args3.Handled)
                {
                    throw;
                }
            }
        }

        internal static void SetTableCellStyle(Control control, Style style)
        {
            Control parent = control.Parent;
            if (parent != null)
            {
                ((TableCell) parent).ApplyStyle(style);
            }
        }

        internal static void SetTableCellVisible(Control control, bool visible)
        {
            Control parent = control.Parent;
            if (parent != null)
            {
                parent.Visible = visible;
            }
        }

        internal sealed class DisappearingTableRow : TableRow
        {
            protected internal override void Render(HtmlTextWriter writer)
            {
                bool flag = false;
                foreach (TableCell cell in this.Cells)
                {
                    if (cell.Visible)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    base.Render(writer);
                }
            }
        }

        internal abstract class GenericContainer<ControlType> : WebControl where ControlType: WebControl
        {
            private Table _borderTable;
            private Table _layoutTable;
            private ControlType _owner;
            private bool _renderDesignerRegion;

            public GenericContainer(ControlType owner)
            {
                this._owner = owner;
            }

            private Control FindControl<RequiredType>(string id, bool required, string errorResourceKey)
            {
                Control control = this.FindControl(id);
                if (control is RequiredType)
                {
                    return control;
                }
                if (required && !this.Owner.DesignMode)
                {
                    throw new HttpException(System.Web.SR.GetString(errorResourceKey, new object[] { this.Owner.ID, id }));
                }
                return null;
            }

            protected Control FindOptionalControl<RequiredType>(string id) => 
                this.FindControl<RequiredType>(id, false, null);

            protected Control FindRequiredControl<RequiredType>(string id, string errorResourceKey) => 
                this.FindControl<RequiredType>(id, true, errorResourceKey);

            public sealed override void Focus()
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoFocusSupport", new object[] { base.GetType().Name }));
            }

            protected internal sealed override void Render(HtmlTextWriter writer)
            {
                if (this.UsingDefaultTemplate)
                {
                    if (!this.ConvertingToTemplate)
                    {
                        this.BorderTable.CopyBaseAttributes(this);
                        if (base.ControlStyleCreated)
                        {
                            LoginUtil.CopyBorderStyles(this.BorderTable, base.ControlStyle);
                            LoginUtil.CopyStyleToInnerControl(this.LayoutTable, base.ControlStyle);
                        }
                    }
                    this.LayoutTable.Height = this.Height;
                    this.LayoutTable.Width = this.Width;
                    this.RenderContents(writer);
                }
                else
                {
                    this.RenderContentsInUnitTable(writer);
                }
            }

            private void RenderContentsInUnitTable(HtmlTextWriter writer)
            {
                System.Web.UI.WebControls.LayoutTable table = new System.Web.UI.WebControls.LayoutTable(1, 1, this.Page);
                if (this.RenderDesignerRegion)
                {
                    table[0, 0].Attributes["_designerRegion"] = "0";
                }
                else
                {
                    foreach (Control control in this.Controls)
                    {
                        table[0, 0].Controls.Add(control);
                    }
                }
                string iD = this.Parent.ID;
                if ((iD != null) && (iD.Length != 0))
                {
                    table.ID = this.Parent.ClientID;
                }
                table.CopyBaseAttributes(this);
                table.ApplyStyle(base.ControlStyle);
                table.CellPadding = 0;
                table.CellSpacing = 0;
                table.RenderControl(writer);
            }

            protected void VerifyControlNotPresent<RequiredType>(string id, string errorResourceKey)
            {
                if ((this.FindOptionalControl<RequiredType>(id) != null) && !this.Owner.DesignMode)
                {
                    throw new HttpException(System.Web.SR.GetString(errorResourceKey, new object[] { this.Owner.ID, id }));
                }
            }

            internal Table BorderTable
            {
                get => 
                    this._borderTable;
                set
                {
                    this._borderTable = value;
                }
            }

            protected abstract bool ConvertingToTemplate { get; }

            internal Table LayoutTable
            {
                get => 
                    this._layoutTable;
                set
                {
                    this._layoutTable = value;
                }
            }

            internal ControlType Owner =>
                this._owner;

            internal bool RenderDesignerRegion
            {
                get => 
                    (base.DesignMode && this._renderDesignerRegion);
                set
                {
                    this._renderDesignerRegion = value;
                }
            }

            private bool UsingDefaultTemplate =>
                (this.BorderTable != null);
        }

        internal delegate void OnSendingMailDelegate(MailMessageEventArgs e);

        internal delegate void OnSendMailErrorDelegate(SendMailErrorEventArgs e);
    }
}

