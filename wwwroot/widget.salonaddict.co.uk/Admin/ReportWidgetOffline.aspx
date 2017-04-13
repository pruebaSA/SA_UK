<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="ReportWidgetOffline.aspx.cs" Inherits="IFRAME.Admin.ReportWidgetOfflinePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Reports" />
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
            EnableViewState="false"  >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Name") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("AddressLine1") + "<div>" + Eval("AddressLine3") + "<div>" %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("PhoneNumber")%>
                 </ItemTemplate>
                 <ItemStyle Width="90px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# ((DateTime)Eval("CreatedOnUtc")).ToString("MMM dd yyyy") %>
                 </ItemTemplate>
                 <ItemStyle Width="90px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
     </asp:Panel>
</asp:Content>
