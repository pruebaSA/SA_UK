<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxCategoryCreate.aspx.cs" Inherits="SalonAddict.Administration.TaxCategoryCreate" %>
<%@ Register TagPrefix="SA" TagName="TaxCategoryDetails" Src="~/Administration/Modules/TaxCategoryDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:TaxCategoryDetails ID="cntlTaxCategoryDetails" runat="server" Action="Create" />
</asp:Content>
