<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CustomerCreate.aspx.cs" Inherits="SalonAddict.Administration.CustomerCreate" %>
<%@ Register TagPrefix="SA" TagName="CustomerDetails" Src="~/Administration/Modules/CustomerDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CustomerDetails ID="cntlCustomerDetails" runat="server" Action="Create" />
</asp:Content>
