<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="MessageTemplateLocalizedDetails.aspx.cs" Inherits="SalonAddict.Administration.MessageTemplateLocalizedDetails" %>
<%@ Register TagPrefix="SA" TagName="MessageTemplateLocalizedDetails" Src="~/Administration/Modules/MessageTemplateLocalizedDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:MessageTemplateLocalizedDetails ID="cntlMessageTemplateLocalizedDetails" runat="server" Action="Edit" />
</asp:Content>