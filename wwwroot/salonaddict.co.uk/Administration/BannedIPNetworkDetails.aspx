<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BannedIPNetworkDetails.aspx.cs" Inherits="SalonAddict.Administration.BannedIPNetworkDetails" %>
<%@ Register TagPrefix="SA" TagName="BannedIPNetworkDetails" Src="~/Administration/Modules/BannedIPNetworkDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:BannedIPNetworkDetails ID="cntlBannedIPNetworkDetails" runat="server" Action="Edit" />
</asp:Content>

