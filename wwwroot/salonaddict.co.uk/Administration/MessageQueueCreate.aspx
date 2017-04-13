<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="MessageQueueCreate.aspx.cs" Inherits="SalonAddict.Administration.MessageQueueCreate" %>
<%@ Register TagPrefix="SA" TagName="MessageQueueDetails" Src="~/Administration/Modules/MessageQueueDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:MessageQueueDetails ID="cntlMessageQueueDetails" runat="server" Action="Create" />
</asp:Content>
