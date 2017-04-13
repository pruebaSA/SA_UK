<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxCategoryDetails.aspx.cs" Inherits="SalonAddict.Administration.TaxCategoryDetails" %>
<%@ Register TagPrefix="SA" TagName="TaxCategoryDetails" Src="~/Administration/Modules/TaxCategoryDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:TaxCategoryDetails ID="cntlTaxCategoryDetails" runat="server" Action="Edit" />
</asp:Content>
