<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="SettingDetails.aspx.cs" Inherits="SalonAddict.Administration.SettingDetails" %>
<%@ Register TagPrefix="SA" TagName="SettingDetails" Src="~/Administration/Modules/SettingDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:SettingDetails ID="cntlSettingDetails" runat="server" Action="Edit" />
</asp:Content>
