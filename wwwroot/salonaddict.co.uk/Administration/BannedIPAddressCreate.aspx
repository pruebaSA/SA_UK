<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BannedIPAddressCreate.aspx.cs" Inherits="SalonAddict.Administration.BannedIPAddressCreate" %>
<%@ Register TagPrefix="SA" TagName="BannedIPAddressDetails" Src="~/Administration/Modules/BannedIPAddressDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:BannedIPAddressDetails ID="cntlBannedIPAddressDetails" runat="server" Action="Create" />
</asp:Content>
