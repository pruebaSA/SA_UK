<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Press.aspx.cs" Inherits="SalonAddict.PressPage" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
<style type="text/css">
.press-wrapper b { font-size:15px; }
.press-wrapper ul li { line-height:17px; }
.press-wrapper p { line-height:15px; }
</style>
<div class="press-wrapper" >
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
</div>
</asp:Content>
