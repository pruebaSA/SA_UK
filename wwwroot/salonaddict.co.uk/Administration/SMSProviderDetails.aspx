<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SMSProviderDetails.aspx.cs" Inherits="SalonAddict.Administration.SMSProviderDetails" %>
<%@ Register TagPrefix="SA" TagName="SMSProviderDetails" Src="~/Administration/Modules/SMSProviderDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <SA:SMSProviderDetails ID="cntlSMSProviderDetails" runat="server" Action="Edit" />
</asp:Content>
