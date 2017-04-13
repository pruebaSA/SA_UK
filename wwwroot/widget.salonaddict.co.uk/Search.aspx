<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" EnableViewState="false" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="IFRAME.SearchPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" EnableViewState="false" >
        <h1><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1>
        <center>
           <asp:Image ID="imgLoader" runat="server" meta:resourceKey="imgLoader" />
        </center>
        <img src='<%= "App_Themes/" + base.Theme + "/images/salonaddict.png" %>' alt="http://www.salonaddict.co.uk" />
    </asp:Panel>
    <script type="text/javascript" language="javascript" >
        var redirect = function() {
            window.location = '<%= Page.ResolveUrl(String.Format("~/dayview.aspx{0}", base.Request.Url.Query)) %>';
        }
        window.setTimeout(redirect, 1000);
    </script>
</asp:Content>
