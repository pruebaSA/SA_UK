<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Savings.aspx.cs" Inherits="SalonAddict.Savings" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <SA:Topic ID="ContentTopic1" runat="server" meta:resourceKey="ContentTopic1" />
    <SA:Topic ID="ContentTopic2" runat="server" meta:resourceKey="ContentTopic2" />
    <SA:Topic ID="ContentTopic3" runat="server" meta:resourceKey="ContentTopic3" />
</asp:Content>
