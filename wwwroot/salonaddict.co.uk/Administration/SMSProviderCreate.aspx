<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SMSProviderCreate.aspx.cs" Inherits="SalonAddict.Administration.SMSProviderCreate" %>
<%@ Register TagPrefix="SA" TagName="SMSProviderDetails" Src="~/Administration/Modules/SMSProviderDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:SMSProviderDetails ID="cntlSMSProviderDetails" runat="server" Action="Create" />
</asp:Content>
