<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoicesIssued.aspx.cs" Inherits="IFRAME.Admin.InvoicesIssuedPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Billing" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-billing.png" %>' alt="billing" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="padding-top:3px;" >
                  <table class="details" cellpadding="0" cellpadding="0" >
                     <tr>
                         <td class="data-item" >
                            &nbsp;&nbsp;
                            <asp:DropDownList ID="ddlInvoiceType" runat="server" SkinID="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="ddlInvoiceType_SelectedIndexChanged" >
                               <asp:ListItem Text="Invoice Type - All" Value="" ></asp:ListItem>
                               <asp:ListItem Text="Invoice Type - R" Value="R" ></asp:ListItem>
                               <asp:ListItem Text="Invoice Type - U" Value="U" ></asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;&nbsp;
                            <asp:DropDownList ID="ddlSortBy" runat="server" SkinID="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="ddlSortBy_SelectedIndexChanged" >
                               <asp:ListItem Text="Sort By - Salon Name" Value="10" ></asp:ListItem>
                               <asp:ListItem Text="Sort By - Lowest Amount" Value="60" ></asp:ListItem>
                               <asp:ListItem Text="Sort By - Highest Amount" Value="70" ></asp:ListItem>
                            </asp:DropDownList>
                         </td>
                     </tr>
                  </table>
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
