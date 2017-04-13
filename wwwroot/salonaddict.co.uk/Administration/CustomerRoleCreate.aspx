<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CustomerRoleCreate.aspx.cs" Inherits="SalonAddict.Administration.CustomerRoleCreate" Title="Untitled Page" %>
<%@ Register TagPrefix="SA" TagName="CustomerRoleDetails" Src="~/Administration/Modules/CustomerRoleDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CustomerRoleDetails ID="cntlCustomerRoleDetails" runat="server" Action="Create" />
</asp:Content>
