<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CountryDetails.aspx.cs" Inherits="SalonAddict.Administration.CountryDetails" %>
<%@ Register TagPrefix="SA" TagName="CountryDetails" Src="~/Administration/Modules/CountryDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CountryDetails ID="cntlCountryDetails" runat="server" Action="Edit" />
</asp:Content>
