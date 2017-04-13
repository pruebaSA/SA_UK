<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SMSQueueCreate.aspx.cs" Inherits="SalonAddict.Administration.SMSQueueCreate" %>
<%@ Register TagPrefix="SA" TagName="SMSQueueDetails" Src="~/Administration/Modules/SMSQueueDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:SMSQueueDetails ID="cntlSMSQueueDetails" runat="server" Action="Create" />
</asp:Content>
