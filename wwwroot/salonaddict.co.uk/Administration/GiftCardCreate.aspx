<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="GiftCardCreate.aspx.cs" Inherits="SalonAddict.Administration.GiftCardCreate" %>
<%@ Register TagPrefix="SA" TagName="GiftCardDetails" Src="~/Administration/Modules/GiftCardDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <SA:GiftCardDetails ID="cntlGiftCard" runat="server" Action="Create" />
</asp:Content>
