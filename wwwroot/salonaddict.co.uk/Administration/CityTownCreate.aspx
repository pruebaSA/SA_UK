<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CityTownCreate.aspx.cs" Inherits="SalonAddict.Administration.CityTownCreate" %>
<%@ Register TagPrefix="SA" TagName="CityTownDetails" Src="~/Administration/Modules/CityTownDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:CityTownDetails ID="cntlCityTown" runat="server" Action="Create" />
</asp:Content>