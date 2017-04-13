<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessLocalityCreate.aspx.cs" Inherits="SalonAddict.Administration.BusinessLocalityCreatePage" %>
<%@ Register TagPrefix="SA" TagName="LocalityDetails" Src="~/Administration/Modules/BusinessLocalityDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LocalityDetails ID="cntlLocalityDetails" runat="server" Action="Create" />
</asp:Content>

