<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ProfileDetails.aspx.cs" Inherits="SalonAddict.Administration.ProfileDetails" %>
<%@ Register TagPrefix="SA" TagName="ProfileDetails" Src="~/Administration/Modules/ProfileDetails.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:ProfileDetails ID="cntlProfileDetails" runat="server" Action="Edit" />
</asp:Content>
