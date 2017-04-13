<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoiceSearch.aspx.cs" Inherits="IFRAME.Admin.InvoiceSearchPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSearch" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Billing" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-billing.png" %>' alt="billing" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                 <asp:Button ID="btnSearch" runat="server" SkinID="SubmitButton" OnClick="btnSearch_Click" meta:resourceKey="btnSearch" />
              </td> 
           </tr>
        </table>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" >
                 Invoice #:
              </td>
              <td class="data-item" >
                  <asp:TextBox ID="txtNumber" runat="server" SkinID="TextBox" MaxLength="20" ></asp:TextBox>
                  <asp:RequiredFieldValidator ID="valNumber" runat="server" ControlToValidate="txtNumber" Display="None" meta:resourceKey="valNumber" ></asp:RequiredFieldValidator>
                  <ajaxToolkit:ValidatorCalloutExtender ID="valNumberEx" runat="server" TargetControlID="valNumber" EnableViewState="false" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="InvoiceId" 
            OnRowEditing="gv_RowEditing" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("InvoiceNumber") %><%# Eval("InvoiceType") %>
                 </ItemTemplate>
                 <ItemStyle Width="130px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("BillingCompany") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# Eval("TotalWidgetCount") %></center>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
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
              <asp:TemplateField>
                 <ItemTemplate>
                      <center>
                        <asp:ImageButton ID="ibEdit" runat="server" SkinID="GridEditImageButton" CommandName="Edit" meta:resourceKey="ibEdit" />
                      </center>
                 </ItemTemplate>
                 <ItemStyle Width="35px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
        <div style="margin-top:10px;" >
            <IFRM:IFRMPager ID="cntrlPager" runat="server" PageSize="15" CssClass="pager" OnPageCreated="cntrlPager_PageCreated" meta:resourceKey="Pager" ></IFRM:IFRMPager>
        </div>
    </asp:Panel>
</asp:Content>
