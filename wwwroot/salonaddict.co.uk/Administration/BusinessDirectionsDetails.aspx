<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessDirectionsDetails.aspx.cs" Inherits="SalonAddict.Administration.BusinessDirectionsDetails" %>
<%@ Register TagPrefix="SA" TagName="DirectionDetails" Src="~/Administration/Modules/BusinessDirectionsDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:DirectionDetails ID="cntlDirectionDetails" runat="server" Action="Edit" />
</asp:Content>