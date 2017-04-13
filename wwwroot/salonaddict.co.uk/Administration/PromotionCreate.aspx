<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PromotionCreate.aspx.cs" Inherits="SalonAddict.Administration.PromotionCreate" %>
<%@ Register TagPrefix="SA" TagName="PromotionDetails" Src="~/Administration/Modules/PromotionDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:PromotionDetails ID="cntlPromotionDetails" runat="server" Action="Create" />
</asp:Content>