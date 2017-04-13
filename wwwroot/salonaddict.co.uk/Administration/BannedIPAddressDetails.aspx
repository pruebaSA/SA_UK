<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BannedIPAddressDetails.aspx.cs" Inherits="SalonAddict.Administration.BannedIPAddressDetails" %>
<%@ Register TagPrefix="SA" TagName="BannedIPAddressDetails" Src="~/Administration/Modules/BannedIPAddressDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:BannedIPAddressDetails ID="cntlBannedIPAddressDetails" runat="server" Action="Edit" />
</asp:Content>

