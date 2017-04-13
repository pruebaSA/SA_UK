<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LogDetails.aspx.cs" Inherits="SalonAddict.Administration.LogDetails" %>
<%@ Register TagPrefix="SA" TagName="LogDetails" Src="~/Administration/Modules/LogDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <SA:LogDetails ID="cntlLogDetails" runat="server" Action="Edit" />
</asp:Content>
