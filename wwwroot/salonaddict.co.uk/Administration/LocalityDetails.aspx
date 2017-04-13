<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LocalityDetails.aspx.cs" Inherits="SalonAddict.Administration.LocalityDetailsPage" %>
<%@ Register TagPrefix="SA" TagName="LocalityDetails" Src="~/Administration/Modules/LocalityDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LocalityDetails ID="cntlLocalityDetails" runat="server" Action="Edit" />
</asp:Content>
