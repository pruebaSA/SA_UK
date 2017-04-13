<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SMSTemplateDetails.aspx.cs" Inherits="SalonAddict.Administration.SMSTemplateDetails" %>
<%@ Register TagPrefix="SA" TagName="SMSTemplateDetails" Src="~/Administration/Modules/SMSTemplateDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:SMSTemplateDetails ID="cntlSMSTemplateDetails" runat="server" Action="Edit" />
</asp:Content>

