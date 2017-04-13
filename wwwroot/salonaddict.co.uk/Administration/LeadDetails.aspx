<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LeadDetails.aspx.cs" Inherits="SalonAddict.Administration.LeadDetails" %>
<%@ Register TagPrefix="SA" TagName="LeadDetails" Src="~/Administration/Modules/LeadDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LeadDetails ID="cntlLeadDetails" runat="server" Action="Edit" />
</asp:Content>