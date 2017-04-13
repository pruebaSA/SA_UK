<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IFRAME.Admin.DefaultPage" %>
<%@ Register TagPrefix="IFRM" TagName="Overview" Src="~/Admin/Modules/Overview.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <h1><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1>
       <IFRM:Overview ID="cntlOverview" runat="server" />
    </asp:Panel>
</asp:Content>
