﻿<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PaymentProviderCreate.aspx.cs" Inherits="SalonAddict.Administration.PaymentProviderCreate" %>
<%@ Register TagPrefix="SA" TagName="PaymentProviderDetails" Src="~/Administration/Modules/PaymentProviderDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:PaymentProviderDetails ID="cntlPaymentProviderDetails" runat="server" Action="Create" />
</asp:Content>