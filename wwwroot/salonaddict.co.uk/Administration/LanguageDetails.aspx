<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LanguageDetails.aspx.cs" Inherits="SalonAddict.Administration.LanguageDetails" %>
<%@ Register TagPrefix="SA" TagName="LanguageDetails" Src="~/Administration/Modules/LanguageDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LanguageDetails ID="cntlLanguageDetails" runat="server" Action="Edit" />
</asp:Content>

