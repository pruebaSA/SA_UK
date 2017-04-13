<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Billing.aspx.cs" Inherits="IFRAME.Admin.BillingPage" %>
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
        <table cellpadding="0" cellspacing="0" >
            <tr>
               <td>
                    <ul>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("invoicing.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlInvoicing.Text")%> (<asp:Literal ID="ltrInvoicingCount" runat="server" ></asp:Literal>)</u>
                          </a>
                       </li>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("invoicesissued.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlIssueBills.Text")%> (<asp:Literal ID="ltrIssuedBillsCount" runat="server" ></asp:Literal>)</u>
                          </a>
                       </li>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("invoicespaymentdue.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlBillSettlement.Text") %> (<asp:Literal ID="ltrPaymentDueCount" runat="server" ></asp:Literal>)</u>
                          </a>
                       </li>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("invoicesearch.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlInvoiceSearch.Text") %> </u>
                          </a>
                       </li>
                    </ul>
               </td>
               <td style="padding-left:35px" >
                    <ul>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("reportbillingtotals.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlBillingReport.Text") %></u>
                          </a>
                       </li>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("invoicespaymentoverdue.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlPaymentsOverdue.Text") %> (<asp:Literal ID="ltrPaymentOverdueCount" runat="server" ></asp:Literal>)</u>
                          </a>
                       </li>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("reportinactivepaymentmethods.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlInactivePaymentMethodsReport.Text") %></u>
                          </a>
                       </li>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("reportvoids.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlReportVoids.Text") %></u>
                          </a>
                       </li>
                       <li style="margin-bottom:10px;" >
                          <a href='<%= IFRMHelper.GetURL("reportnonbillablesalons.aspx") %>' >
                             <u><%= base.GetLocaleResourceString("hlNonBillableReport.Text")%></u>
                          </a>
                       </li>
                    </ul>
               </td>
            </tr>
        </table>
     </asp:Panel>
</asp:Content>
