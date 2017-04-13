<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="ReportInactivePaymentMethods.aspx.cs" Inherits="IFRAME.Admin.ReportInactivePaymentMethodsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Billing" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-reports.png" %>' alt="reports" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            EnableViewState="false" 
            OnRowCreated="gv_RowCreated" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("SalonName") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Alias") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <asp:Literal ID="ltrCardNumber" runat="server" ></asp:Literal>
                 </ItemTemplate>
                 <ItemStyle Width="90px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <asp:Literal ID="ltrExpiry" runat="server" ></asp:Literal>
                 </ItemTemplate>
                 <ItemStyle Width="70px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <a href='<%# IFRMHelper.GetURL("SalonPaymentMethod-Activate.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_PAYMENT_METHOD_ID, Eval("SalonPaymentMethodId"))) %>' ><u>Activate</u></a>
                 </ItemTemplate>
                 <ItemStyle Width="50px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
     </asp:Panel>
</asp:Content>
