<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="GuestDashboard.aspx.cs" Inherits="SalonAddict.Administration.GuestDashboard" %>
<%@ Register TagPrefix="SA" TagName="AppointmentReport" Src="~/Administration/Modules/AppointmentAverageReport.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-dashboard.png" alt="Dashboard" />
    Dashboard
</div>
<SA:AppointmentReport ID="cntlAppointmentReport" runat="server" IsGuest="true" />
</asp:Content>
