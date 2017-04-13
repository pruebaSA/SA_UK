<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoiceWTs.aspx.cs" Inherits="IFRAME.Admin.InvoiceWTsPage" %>
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
              </td> 
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server"
            EnableViewState="false" 
            AutoGenerateColumns="False" 
            DataKeyNames="InvoiceItemId" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# ((DateTime)Eval("Timestamp")).ToString("dd.MM.yyyy HH:mm") %>
                 </ItemTemplate>
                 <ItemStyle Width="120px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("FirstName") + " " + Eval("LastName")%>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("ServiceName") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                  <ItemTemplate>
                      <%# IFRMHelper.FromUrlFriendlyDate(Eval("AppointmentDate").ToString()).Date.ToString("ddd dd MMM") %>
                  </ItemTemplate>
                  <ItemStyle Width="75px" />
              </asp:TemplateField>
              <asp:TemplateField>
                  <ItemTemplate>
                      <center><%# Eval("AppointmentTime") %></center>
                  </ItemTemplate>
                  <ItemStyle Width="40px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# ((double)(int)Eval("ItemPrice")/100).ToString("C") %>
                 </ItemTemplate>
                 <ItemStyle Width="40px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
        <div style="margin-top:10px;" >
            <IFRM:IFRMPager ID="cntrlPager" runat="server" PageSize="10" CssClass="pager" OnPageCreated="cntrlPager_PageCreated" meta:resourceKey="Pager" ></IFRM:IFRMPager>
        </div>
     </asp:Panel>
</asp:Content>