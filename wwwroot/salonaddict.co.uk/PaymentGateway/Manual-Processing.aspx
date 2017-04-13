<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Manual-Processing.aspx.cs" Inherits="SalonAddict.PaymentGateway.Manual_Processing" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
    <script type="text/javascript" language="javascript" >
        window.setTimeout("location.reload(true);", 4000);
    </script>
</asp:Content>
