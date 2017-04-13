<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PromotionDetails.aspx.cs" Inherits="SalonAddict.Administration.PromotionDetails" %>
<%@ Register TagPrefix="SA" TagName="PromotionDetails" Src="~/Administration/Modules/PromotionDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:PromotionDetails ID="cntlPromotionDetails" runat="server" Action="Edit" />
</asp:Content>