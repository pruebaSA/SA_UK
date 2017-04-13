<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CurrencyDetails.aspx.cs" Inherits="SalonAddict.Administration.CurrencyDetails" %>
<%@ Register TagPrefix="SA" TagName="CurrencyDetails" Src="~/Administration/Modules/CurrencyDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CurrencyDetails ID="cntlCurrencyDetails" runat="server" Action="Edit" />
</asp:Content>
