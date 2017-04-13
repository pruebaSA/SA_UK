<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="LocaleStringResourceDetails.aspx.cs" Inherits="SalonAddict.Administration.LocaleStringResourceDetails" %>
<%@ Register TagPrefix="SA" TagName="LocaleStringResourceDetails" Src="~/Administration/Modules/LocaleStringResourceDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LocaleStringResourceDetails ID="cntlLocaleStringResource" runat="server" Action="Edit" />
</asp:Content>

