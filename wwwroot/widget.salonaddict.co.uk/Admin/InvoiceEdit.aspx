<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoiceEdit.aspx.cs" Inherits="IFRAME.Admin.InvoiceEditPage" %>
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
                   <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                   <asp:Button ID="btnCarryForward" runat="server" SkinID="SubmitButton" OnClick="btnCarryForward_Click" meta:resourceKey="btnCarryForward" />
                   <asp:Button ID="btnPaid" runat="server" SkinID="SubmitButton" OnClick="btnPaid_Click" meta:resourceKey="btnPaid" />
                   <asp:Button ID="btnVoid" runat="server" SkinID="SubmitButton" OnClick="btnVoid_Click" meta:resourceKey="btnVoid" />
              </td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" >
           <tr>
              <td style="width:500px;vertical-align:top;" >
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrSalon.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrSalonName" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrInvoiceNumber.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrInvoiceNumber" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrInvoiceDate.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrInvoicedDate" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrPlan.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrPlanType" runat="server" EnableViewState="false" ></asp:Literal>
                         (<asp:Literal ID="ltrPlanFee" runat="server" EnableViewState="false" ></asp:Literal>)
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrBillingPeriod.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrBillingPeriod" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                  <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrTotalPlan.Text") %>
                      </td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrTotalPlan" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrTotalWidget.Text") %>
                      </td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrTotalWidget" runat="server" EnableViewState="false" ></asp:Literal>
                          (<asp:Literal ID="ltrTotalWidgetCount" runat="server" EnableViewState="false" ></asp:Literal>)
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrSubtotal.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrSubtotalExclTax" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrTotalAdjustment.Text")%>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrTotalAdjustment" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrTotalTax.Text")%>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrTotalTax" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrOverdue.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrOverdue" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrTotal.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrTotalInclTax" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrTotalAmountDue.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrTotalAmountDue" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrPaymentDate.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrPaymentDate" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrStatus.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:Literal ID="ltrStatus" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                </table>
              </td>
              <td style="vertical-align:top;" >
 
              </td>
           </tr>
        </table>
     </asp:Panel>
</asp:Content>