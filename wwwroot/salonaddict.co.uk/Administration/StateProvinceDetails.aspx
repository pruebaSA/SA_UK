<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="StateProvinceDetails.aspx.cs" Inherits="SalonAddict.Administration.StateProvinceDetails" %>
<%@ Register TagPrefix="SA" TagName="StateProvinceDetails" Src="~/Administration/Modules/StateProvinceDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:StateProvinceDetails ID="cntlStateProvinceDetails" runat="server" Action="Edit" />
</asp:Content>

