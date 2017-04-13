<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CustomerDetails.aspx.cs" Inherits="SalonAddict.Administration.CustomerDetails" %>
<%@ Register TagPrefix="SA" TagName="CustomerDetails" Src="~/Administration/Modules/CustomerDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CustomerDetails ID="cntlCustomerDetails" runat="server" Action="Edit" />
</asp:Content>
