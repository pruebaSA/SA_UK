<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ServiceTypeLocalizedDetails.aspx.cs" Inherits="SalonAddict.Administration.ServiceTypeLocalizedDetails" %>
<%@ Register TagPrefix="SA" TagName="ServiceTypeLocalizedDetails" Src="~/Administration/Modules/ServiceTypeLocalizedDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <SA:ServiceTypeLocalizedDetails ID="cntlServiceTypeLocalizedDetails" runat="server" />
</asp:Content>

