<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ReviewSessionDetails.aspx.cs" Inherits="SalonAddict.Administration.ReviewSessionDetails" %>
<%@ Register TagPrefix="SA" TagName="ReviewSession" Src="~/Administration/Modules/ReviewSessionDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:ReviewSession ID="cntlReviewSession" runat="server" Action="Edit" />
</asp:Content>
