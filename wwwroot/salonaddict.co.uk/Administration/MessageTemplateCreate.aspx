<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="MessageTemplateCreate.aspx.cs" Inherits="SalonAddict.Administration.MessageTemplateCreate" %>
<%@ Register TagPrefix="SA" TagName="MessageTemplateDetails" Src="~/Administration/Modules/MessageTemplateDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:MessageTemplateDetails ID="cntlMessageTemplateDetails" runat="server" Action="Create" />
</asp:Content>

