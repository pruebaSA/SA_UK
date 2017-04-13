<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SMSTemplateCreate.aspx.cs" Inherits="SalonAddict.Administration.SMSTemplateCreate" %>
<%@ Register TagPrefix="SA" TagName="SMSTemplateDetails" Src="~/Administration/Modules/SMSTemplateDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:SMSTemplateDetails ID="cntlSMSTemplateDetails" runat="server" Action="Create" />
</asp:Content>

