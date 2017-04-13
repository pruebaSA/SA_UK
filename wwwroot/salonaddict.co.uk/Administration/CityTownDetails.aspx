<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CityTownDetails.aspx.cs" Inherits="SalonAddict.Administration.CityTownDetails" %>
<%@ Register TagPrefix="SA" TagName="CityTownDetails" Src="~/Administration/Modules/CityTownDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CityTownDetails ID="cntlCityTown" runat="server" Action="Edit" />
</asp:Content>

