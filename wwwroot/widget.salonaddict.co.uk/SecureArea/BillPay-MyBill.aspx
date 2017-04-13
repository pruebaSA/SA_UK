<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="BillPay-MyBill.aspx.cs" Inherits="IFRAME.SecureArea.BillPay_MyBillPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Account" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-account.png" %>' alt="account" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrBillingPeriod.Text") %></td>
              <td class="data-item" >
                   <asp:DropDownList ID="ddlBillingPeriod" runat="server" SkinID="DropDownList" Width="180px" AutoPostBack="true" OnSelectedIndexChanged="ddlBillingPeriod_SelectedIndexChanged"  ></asp:DropDownList>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrInvoiceDate.Text") %></td>
              <td class="data-item" >
                   <asp:Literal ID="ltrInvoiceDate" runat="server" EnableViewState="true" Text="N/A" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrInvoiceNumber.Text") %></td>
              <td class="data-item" >
                   <asp:Literal ID="ltrInvoiceNumber" runat="server" EnableViewState="true" Text="N/A" ></asp:Literal>
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            EnableViewState="true" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("PreviousBalance") %></center>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("PaymentReceived")%></center>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("OverdueAmount")%></center>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("TotalCharges")%></center>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("TotalAmountDue")%></center>
                 </ItemTemplate>
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
        <p>
           <asp:LinkButton ID="lbDownLoad" runat="server" OnClick="lbDownload_Click" ><u><%= base.GetLocaleResourceString("hlDownload.Text") %></u></asp:LinkButton>
        </p>
    </asp:Panel>
</asp:Content>
