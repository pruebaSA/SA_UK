<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSP-History.aspx.cs" Inherits="IFRAME.SecureArea.WSP_HistoryPage" %>
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
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False"
            OnRowCreated="gv_RowCreated"
            OnRowEditing="gv_RowEditing" 
            DataKeyNames="PlanId" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("Description") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# IFRMHelper.FromUrlFriendlyDate(Eval("PlanStartDate").ToString()).ToString("dd MMM yyyy") %> - <%# IFRMHelper.FromUrlFriendlyDate(Eval("PlanEndDate").ToString()).ToString("dd MMM yyyy") %>
                 </ItemTemplate>
                 <ItemStyle Width="220px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
                        <asp:View ID="v0" runat="server" >
                            <%# Eval("Status") %>
                        </asp:View>
                        <asp:View ID="v1" runat="server" >
                            <asp:Button ID="btnPay" runat="server" SkinID="SubmitButtonMiniSecure" CommandName="Edit" meta:resourceKey="btnPay" />
                        </asp:View>
                     </asp:MultiView>
                 </ItemTemplate>
                 <ItemStyle Width="80px" HorizontalAlign="Center" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
