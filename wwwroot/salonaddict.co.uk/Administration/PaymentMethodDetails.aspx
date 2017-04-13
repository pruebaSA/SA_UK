<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PaymentMethodDetails.aspx.cs" Inherits="SalonAddict.Administration.PaymentMethodDetails" %>
<%@ Register TagPrefix="SA" TagName="PaymentMethodDetails" Src="~/Administration/Modules/PaymentMethodDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:PaymentMethodDetails ID="cntlPaymentMethodDetails" runat="server" Action="Edit" />
</asp:Content>

