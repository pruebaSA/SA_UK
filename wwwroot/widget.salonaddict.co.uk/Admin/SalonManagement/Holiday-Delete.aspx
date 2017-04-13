﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Holiday-Delete.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.Holiday_DeletePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
           </tr>
        </table>
        <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
            <tr>
                <td class="title" ><%= base.GetLocaleResourceString("ltrDate.Text") %></td>
                <td class="data-item" >
                   <asp:Literal ID="ltrDate" runat="server" ></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="title" ><%= base.GetLocaleResourceString("ltrDescription.Text") %></td>
                <td class="data-item" >
                    <asp:Literal ID="ltrDescription" runat="server" ></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="title" ></td>
                <td class="data-item" >
                    <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                    <asp:Button ID="btnDelete" runat="server" SkinID="SubmitButton" OnClick="btnDelete_Click" meta:resourceKey="btnDelete" />
                </td>
            </tr>
        </table>
     </asp:Panel>
</asp:Content>
