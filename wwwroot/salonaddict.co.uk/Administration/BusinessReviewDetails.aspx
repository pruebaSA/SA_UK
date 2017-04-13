<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessReviewDetails.aspx.cs" Inherits="SalonAddict.Administration.BusinessReviewDetails" %>
<%@ Register TagPrefix="SA" TagName="BusinessReviewDetails" Src="~/Administration/Modules/BusinessReviewDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:BusinessReviewDetails ID="cntlBusinessReviewDetails" runat="server" Action="Edit" />
</asp:Content>
