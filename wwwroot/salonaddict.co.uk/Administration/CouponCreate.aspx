<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CouponCreate.aspx.cs" Inherits="SalonAddict.Administration.CouponCreatePage" %>
<%@ Register TagPrefix="SA" TagName="CouponDetails" Src="~/Administration/Modules/CouponDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <SA:CouponDetails ID="cntrlCounpons" runat="server" Action="Create" />
</asp:Content>
