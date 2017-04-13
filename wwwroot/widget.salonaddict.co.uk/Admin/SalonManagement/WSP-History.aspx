<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSP-History.aspx.cs" Inherits="IFRAME.Admin.WSP_HistoryPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Account" />
       <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;text-align:right;" >
                  
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="PlanId" 
            OnRowEditing="gv_RowEditing" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("Description") %>
                 </ItemTemplate>
              </asp:TemplateField>
             <asp:TemplateField>
                 <ItemTemplate>
                     <%# ((double)(int)Eval("PlanPrice") / 100).ToString("C") %>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# IFRMHelper.FromUrlFriendlyDate(Eval("PlanStartDate").ToString()).ToString("dd MMM yyyy") %> - <%# IFRMHelper.FromUrlFriendlyDate(Eval("PlanEndDate").ToString()).ToString("dd MMM yyyy") %>
                 </ItemTemplate>
                 <ItemStyle Width="190px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# ((double)(int)Eval("ExcessFeeWT") / 100).ToString("C")%>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <asp:ImageButton ID="ibEdit" runat="server" SkinID="GridEditImageButton" CommandName="Edit" meta:resourceKey="ibEdit" />
                 </ItemTemplate>
                 <ItemStyle Width="30px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
