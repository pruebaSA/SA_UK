<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Expired.aspx.cs" Inherits="IFRAME.ExpiredPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <h1 style="margin-top:25px" ><%= base.GetLocaleResourceString("ltrContent.Text") %></h1>
    </asp:Panel>
</asp:Content>
