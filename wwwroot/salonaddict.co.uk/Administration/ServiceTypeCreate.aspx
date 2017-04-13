<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ServiceTypeCreate.aspx.cs" Inherits="SalonAddict.Administration.ServiceTypeCreate" %>
<%@ Register TagPrefix="SA" TagName="ServiceTypeDetails" Src="~/Administration/Modules/ServiceTypeDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <SA:ServiceTypeDetails ID="cntlServiceTypeDetails" runat="server" Action="Create" />
</asp:Content>

