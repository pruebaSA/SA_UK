﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Careers.aspx.cs" Inherits="SalonAddict.Careers" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
</asp:Content>
