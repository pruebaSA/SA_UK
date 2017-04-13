<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxProviderCreate.aspx.cs" Inherits="SalonAddict.Administration.TaxProviderCreate" %>
<%@ Register TagPrefix="SA" TagName="TaxProviderDetails" Src="~/Administration/Modules/TaxProviderDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:TaxProviderDetails ID="cntlTaxProviderDetails" runat="server" Action="Create" />
</asp:Content>