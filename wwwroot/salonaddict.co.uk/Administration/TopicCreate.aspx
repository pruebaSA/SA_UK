<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TopicCreate.aspx.cs" Inherits="SalonAddict.Administration.TopicCreate" %>
<%@ Register TagPrefix="SA" TagName="TopicDetails" Src="~/Administration/Modules/TopicDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:TopicDetails ID="cntlTopicDetails" runat="server" Action="Create" />
</asp:Content>