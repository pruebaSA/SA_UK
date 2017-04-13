<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CountyDetails.aspx.cs" Inherits="SalonAddict.Administration.CountyDetails" %>
<%@ Register TagPrefix="SA" TagName="CountyDetails" Src="~/Administration/Modules/CountyDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CountyDetails ID="cntlCountyDetails" runat="server" Action="Edit" />
</asp:Content>


