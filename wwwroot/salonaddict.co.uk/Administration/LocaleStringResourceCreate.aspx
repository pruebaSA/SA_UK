<%@ Page Language="C#" ValidateRequest="false" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LocaleStringResourceCreate.aspx.cs" Inherits="SalonAddict.Administration.LocaleStringResourceCreate" %>
<%@ Register TagPrefix="SA" TagName="LocaleStringResourceDetails" Src="~/Administration/Modules/LocaleStringResourceDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LocaleStringResourceDetails ID="cntlLocaleStringResource" runat="server" Action="Create" />
</asp:Content>

