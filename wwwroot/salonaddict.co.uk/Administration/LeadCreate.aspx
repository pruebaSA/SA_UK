<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LeadCreate.aspx.cs" Inherits="SalonAddict.Administration.LeadCreate" %>
<%@ Register TagPrefix="SA" TagName="LeadDetails" Src="~/Administration/Modules/LeadDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:LeadDetails ID="cntlLeadDetails" runat="server" Action="Create" />
</asp:Content>