<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="404.aspx.cs" Inherits="SalonPortal._04" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="OneColumnContentPlaceHolder" runat="server">
    <SA:Topic ID="Topic" runat="server" meta:resourceKey="Topic" />
</asp:Content>

