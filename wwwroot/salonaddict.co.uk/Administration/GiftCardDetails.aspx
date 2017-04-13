<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="GiftCardDetails.aspx.cs" Inherits="SalonAddict.Administration.GiftCardDetails" %>
<%@ Register TagPrefix="SA" TagName="GiftCardDetails" Src="~/Administration/Modules/GiftCardDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <SA:GiftCardDetails ID="cntlGiftCard" runat="server" Action="Edit" />
</asp:Content>
