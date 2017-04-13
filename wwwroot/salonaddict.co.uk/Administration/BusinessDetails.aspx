<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessDetails.aspx.cs" Inherits="SalonAddict.Administration.BusinessDetails" %>
<%@ Register TagPrefix="SA" TagName="BusinessDetails" Src="~/Administration/Modules/BusinessDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:BusinessDetails ID="cntlBusinessDetails" runat="server" Action="Edit" />
</asp:Content>

