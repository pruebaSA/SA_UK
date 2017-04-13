<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Privacy-Policy.aspx.cs" Inherits="SalonAddict.Privacy_Policy" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
    <SA:Topic ID="ContentTopic1" runat="server" meta:resourceKey="ContentTopic1" />
    <SA:Topic ID="ContentTopic2" runat="server" meta:resourceKey="ContentTopic2" />
    <SA:Topic ID="ContentTopic3" runat="server" meta:resourceKey="ContentTopic3" />
    <SA:Topic ID="ContentTopic4" runat="server" meta:resourceKey="ContentTopic4" />
    <SA:Topic ID="ContentTopic5" runat="server" meta:resourceKey="ContentTopic5" />
    <SA:Topic ID="ContentTopic6" runat="server" meta:resourceKey="ContentTopic6" />
    <SA:Topic ID="ContentTopic7" runat="server" meta:resourceKey="ContentTopic7" />
    <SA:Topic ID="ContentTopic8" runat="server" meta:resourceKey="ContentTopic8" />
</asp:Content>
