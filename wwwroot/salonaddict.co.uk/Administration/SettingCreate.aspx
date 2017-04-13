<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="SettingCreate.aspx.cs" Inherits="SalonAddict.Administration.SettingCreate" %>
<%@ Register TagPrefix="SA" TagName="SettingDetails" Src="~/Administration/Modules/SettingDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:SettingDetails ID="cntlSettingDetails" runat="server" Action="Create" />
</asp:Content>
