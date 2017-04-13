<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.DefaultPage" %>
<%@ Register TagPrefix="IFRM" TagName="Overview" Src="~/Admin/Modules/SalonOverview.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <h1><asp:Literal ID="ltrHeader" runat="server" ></asp:Literal></h1>
       <IFRM:Overview ID="cntlOverview" runat="server" />
    </asp:Panel>
</asp:Content>
