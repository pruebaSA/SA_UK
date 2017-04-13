<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessLocalityDetails.aspx.cs" Inherits="SalonAddict.Administration.BusinessLocalityDetailsPage" %>
<%@ Register TagPrefix="SA" TagName="LocalityDetails" Src="~/Administration/Modules/BusinessLocalityDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LocalityDetails ID="cntlLocalityDetails" runat="server" Action="Edit" />
</asp:Content>

