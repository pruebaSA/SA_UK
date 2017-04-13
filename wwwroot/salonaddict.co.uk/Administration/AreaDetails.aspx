<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="AreaDetails.aspx.cs" Inherits="SalonAddict.Administration.AreaDetails" %>
<%@ Register TagPrefix="SA" TagName="AreaDetails" Src="~/Administration/Modules/AreaDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:AreaDetails ID="cntlAreaDetails" runat="server" Action="Edit" />
</asp:Content>
