<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="TopicLocalizedDetails.aspx.cs" Inherits="SalonAddict.Administration.TopicLocalizedDetails" %>
<%@ Register TagPrefix="SA" TagName="TopicLocalizedDetails" Src="~/Administration/Modules/TopicLocalizedDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:TopicLocalizedDetails ID="cntlTopicLocalizedDetails" runat="server" />
</asp:Content>
