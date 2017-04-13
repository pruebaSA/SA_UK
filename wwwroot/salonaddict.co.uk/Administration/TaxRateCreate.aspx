<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxRateCreate.aspx.cs" Inherits="SalonAddict.Administration.TaxRateCreate" %>
<%@ Register TagPrefix="SA" TagName="TaxRateDetails" Src="~/Administration/Modules/TaxRateDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:TaxRateDetails ID="cntlTaxRateDetails" runat="server" Action="Create" />
</asp:Content>

