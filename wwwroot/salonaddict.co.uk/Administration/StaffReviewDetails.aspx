<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="StaffReviewDetails.aspx.cs" Inherits="SalonAddict.Administration.StaffReviewDetails" %>
<%@ Register TagPrefix="SA" TagName="StaffReviewDetails" Src="~/Administration/Modules/StaffReviewDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:StaffReviewDetails ID="cntlStaffReviewDetails" runat="server" Action="Edit" />
</asp:Content>

