<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ServiceTypeDetails.aspx.cs" Inherits="SalonAddict.Administration.ServiceTypeDetails" %>
<%@ Register TagPrefix="SA" TagName="ServiceTypeDetails" Src="~/Administration/Modules/ServiceTypeDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <SA:ServiceTypeDetails ID="cntlServiceTypeDetails" runat="server" Action="Edit" />
</asp:Content>

