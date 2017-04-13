<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="SMSTemplateLocalizedDetails.aspx.cs" Inherits="SalonAddict.Administration.SMSTemplateLocalizedDetails" %>
<%@ Register TagPrefix="SA" TagName="SMSTemplateLocalizedDetails" Src="~/Administration/Modules/SMSTemplateLocalizedDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:SMSTemplateLocalizedDetails ID="cntlSMSTemplateLocalizedDetails" runat="server" Action="Edit" />
</asp:Content>