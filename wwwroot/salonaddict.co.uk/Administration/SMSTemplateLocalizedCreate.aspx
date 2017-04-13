<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="SMSTemplateLocalizedCreate.aspx.cs" Inherits="SalonAddict.Administration.SMSTemplateLocalizedCreate" %>
<%@ Register TagPrefix="SA" TagName="SMSTemplateLocalizedDetails" Src="~/Administration/Modules/SMSTemplateLocalizedDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:SMSTemplateLocalizedDetails ID="cntlSMSTemplateLocalizedDetails" runat="server" Action="Create" />
</asp:Content>