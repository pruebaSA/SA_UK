<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Overview.ascx.cs" Inherits="IFRAME.Admin.Modules.Overview" %>
<div class="module-overview" >
    <table style="margin:-20px" cellpadding="0" cellspacing="20" >
      <tr>
         <td>
            <a href='<%= IFRMHelper.GetURL(Page.ResolveUrl("~/")) %>' class="option-home" ><%= base.GetLocalResourceObject("hlHome.Text") %></a>
         </td>
         <td>
            <a href='<%= IFRMHelper.GetURL("salons.aspx") %>' class="option-salons" ><%= base.GetLocalResourceObject("hlSalons.Text") %></a>
         </td>
         <td>
            <div style="position:relative;">
                <a href='<%= IFRMHelper.GetURL("billing.aspx") %>' class="option-billing" ><%= base.GetLocalResourceObject("hlBilling.Text") %></a>
                <asp:Panel ID="pnlBillingBadge" runat="server" CssClass="billing-badge" Visible="false" > 
                    <asp:Literal ID="ltrBillingCount" runat="server" ></asp:Literal>
                </asp:Panel>
            </div>
         </td>
      </tr>
    </table>
    <div class="horizontal-line" ></div>
    <table style="margin:-20px" cellpadding="0" cellspacing="20" >
      <tr>
         <td>
            <a href='<%= IFRMHelper.GetURL("profile.aspx") %>' class="option-profile" ><%= base.GetLocalResourceObject("hlProfile.Text") %></a>
         </td>
         <td>
            <a href='<%= IFRMHelper.GetURL("reports.aspx") %>' class="option-reports" ><%= base.GetLocalResourceObject("hlReports.Text") %></a>
         </td>
         <td>
            <a href='<%= IFRMHelper.GetURL("audit.aspx") %>' class="option-audit" ><%= base.GetLocalResourceObject("hlLogs.Text") %></a>
         </td>
      </tr>
    </table>
</div>