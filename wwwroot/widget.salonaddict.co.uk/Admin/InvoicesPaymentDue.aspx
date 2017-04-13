<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoicesPaymentDue.aspx.cs" Inherits="IFRAME.Admin.InvoicesPaymentDuePage" %>
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
                  <asp:Button ID="btnTask" runat="server" SkinID="SubmitButton" OnClick="btnTask_Click" meta:resourceKey="btnTask" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="InvoiceId"  >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("InvoiceNumber") %><%# Eval("InvoiceType") %>
                 </ItemTemplate>
                 <ItemStyle Width="125px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("BillingCompany") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# IFRMHelper.FromUrlFriendlyDate(Eval("BillEndDate").ToString()).ToString("dd-MM-yyyy") %>
                 </ItemTemplate>
                 <ItemStyle Width="80px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# ((double)((int)Eval("TotalAmountDue")) / 100).ToString("C")%></center>
                 </ItemTemplate>
                 <ItemStyle Width="65px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# IFRMHelper.FromUrlFriendlyDate(Eval("PaymentDueDate").ToString()).ToString("MMM dd yyyy") %>
                 </ItemTemplate>
                 <ItemStyle Width="90px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
     </asp:Panel>
</asp:Content>
