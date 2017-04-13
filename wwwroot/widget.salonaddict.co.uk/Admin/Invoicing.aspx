<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Invoicing.aspx.cs" Inherits="IFRAME.Admin.InvoicingPage" %>
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
                  <asp:Button ID="btnGenerate" runat="server" SkinID="SubmitButton" OnClick="btnGenerate_Click" meta:resourceKey="btnGenerate" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="SalonId" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("Name") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("AddressLine1") %> <%# Eval("AddressLine2") %>
                 </ItemTemplate>
                 <ItemStyle Width="280px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# IFRMHelper.FromUrlFriendlyDate(Eval("BillEndDate").ToString()).ToString("MMM dd yyyy") %></center>
                 </ItemTemplate>
                 <ItemStyle Width="115px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
     </asp:Panel>
</asp:Content>
