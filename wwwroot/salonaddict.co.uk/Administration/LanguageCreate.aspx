<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LanguageCreate.aspx.cs" Inherits="SalonAddict.Administration.LanguageCreate" %>
<%@ Register TagPrefix="SA" TagName="LanguageDetails" Src="~/Administration/Modules/LanguageDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LanguageDetails ID="cntlLanguageDetails" runat="server" Action="Create" />
</asp:Content>

