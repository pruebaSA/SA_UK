<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BannedIPNetworkCreate.aspx.cs" Inherits="SalonAddict.Administration.BannedIPNetworkCreate" %>
<%@ Register TagPrefix="SA" TagName="BannedIPNetworkDetails" Src="~/Administration/Modules/BannedIPNetworkDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:BannedIPNetworkDetails ID="cntlBannedIPNetworkDetails" runat="server" Action="Create" />
</asp:Content>

