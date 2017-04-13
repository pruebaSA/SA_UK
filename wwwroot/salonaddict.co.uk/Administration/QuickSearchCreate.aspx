<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="QuickSearchCreate.aspx.cs" Inherits="SalonAddict.Administration.QuickSearchCreate" %>
<%@ Register TagPrefix="SA" TagName="QuickSearchDetails" Src="~/Administration/Modules/QuickSearchDetails.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <SA:QuickSearchDetails ID="cntlQuickSearch" runat="server" Action="Create" />
</asp:Content>