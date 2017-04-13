<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxRateDetails.aspx.cs" Inherits="SalonAddict.Administration.TaxRateDetails" %>
<%@ Register TagPrefix="SA" TagName="TaxRateDetails" Src="~/Administration/Modules/TaxRateDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:TaxRateDetails ID="cntlTaxRateDetails" runat="server" Action="Edit" />
</asp:Content>
