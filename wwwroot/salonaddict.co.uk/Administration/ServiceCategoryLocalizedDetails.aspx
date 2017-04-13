<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ServiceCategoryLocalizedDetails.aspx.cs" Inherits="SalonAddict.Administration.ServiceCategoryLocalizedDetails" %>
<%@ Register TagPrefix="SA" TagName="ServiceCategoryLocalizedDetails" Src="~/Administration/Modules/ServiceCategoryLocalizedDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <SA:ServiceCategoryLocalizedDetails ID="cntlServiceCategoryLocalizedDetails" runat="server" />
</asp:Content>
