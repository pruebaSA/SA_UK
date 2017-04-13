<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="IFRAME.ErrorPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <h1 style="margin-top:25px" ><%= base.GetLocaleResourceString("ltrContent.Text") %></h1>
       <p style="text-align:center" >
          <a href='<%= IFRMHelper.GetURL(Page.ResolveUrl("~/")) %>' ><u><%= base.GetLocaleResourceString("hlHomepage.Text") %></u></a>
       </p>
    </asp:Panel>
</asp:Content>
