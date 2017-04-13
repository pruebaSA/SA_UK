<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Terms-And-Conditions.aspx.cs" Inherits="SalonPortal.Terms_And_Conditions" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="OneColumnContentPlaceHolder" runat="server">
    <SA:Topic ID="Topic" runat="server" meta:resourceKey="Topic" />
</asp:Content>
