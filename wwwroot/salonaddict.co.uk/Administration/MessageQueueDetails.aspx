<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="MessageQueueDetails.aspx.cs" Inherits="SalonAddict.Administration.MessageQueueDetails" %>
<%@ Register TagPrefix="SA" TagName="MessageQueueDetails" Src="~/Administration/Modules/MessageQueueDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:MessageQueueDetails ID="cntlMessageQueueDetails" runat="server" Action="Edit" />
</asp:Content>
