<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CurrencyCreate.aspx.cs" Inherits="SalonAddict.Administration.CurrencyCreate" %>
<%@ Register TagPrefix="SA" TagName="CurrencyDetails" Src="~/Administration/Modules/CurrencyDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CurrencyDetails ID="cntlCurrencyDetails" runat="server" Action="Create" />
</asp:Content>

