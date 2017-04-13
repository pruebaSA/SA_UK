<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ServiceCategoryDetails.aspx.cs" Inherits="SalonAddict.Administration.ServiceCategoryDetails" %>
<%@ Register TagPrefix="SA" TagName="ServiceCategoryDetails" Src="~/Administration/Modules/ServiceCategoryDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:ServiceCategoryDetails ID="cntlServiceCategoryDetails" runat="server" Action="Edit" />
</asp:Content>

