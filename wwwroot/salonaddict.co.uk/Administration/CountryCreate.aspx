<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CountryCreate.aspx.cs" Inherits="SalonAddict.Administration.CountryCreate" %>
<%@ Register TagPrefix="SA" TagName="CountryDetails" Src="~/Administration/Modules/CountryDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CountryDetails ID="cntlCountryDetails" runat="server" Action="Create" />
</asp:Content>

