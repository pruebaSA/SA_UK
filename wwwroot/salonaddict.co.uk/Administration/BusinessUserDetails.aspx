﻿<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessUserDetails.aspx.cs" Inherits="SalonAddict.Administration.BusinessUserDetails" %>
<%@ Register TagPrefix="SA" TagName="BusinessUserDetails" Src="~/Administration/Modules/BusinessUserDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:BusinessUserDetails ID="cntlBusinessUserDetails" runat="server" Action="Edit" />
</asp:Content>
