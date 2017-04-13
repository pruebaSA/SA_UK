<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="DiscountCreate.aspx.cs" Inherits="SalonAddict.Administration.DiscountCreate" %>
<%@ Register TagPrefix="SA" TagName="DiscountDetails" Src="~/Administration/Modules/DiscountDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <SA:DiscountDetails ID="cntrlDiscounts" runat="server" Action="Create" />
</asp:Content>
