<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoicesCreateTask.aspx.cs" Inherits="IFRAME.Admin.InvoicesCreateTaskPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Billing" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-billing.png" %>' alt="billing" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  
              </td>
           </tr>
        </table>
        <center>
           <asp:Image ID="imgLoader" runat="server" ImageUrl="~/App_Themes/Admin/images/ajax-loader.gif" AlternateText="loading..." />
        </center>
        <div style="display:none;" >
            <asp:LinkButton ID="lbStartTask" runat="server" OnClick="lbStartTask_Click" ></asp:LinkButton>
        </div>
    </asp:Panel>
</asp:Content>
