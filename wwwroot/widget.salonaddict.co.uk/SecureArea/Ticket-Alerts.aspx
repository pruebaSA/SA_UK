<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Ticket-Alerts.aspx.cs" Inherits="IFRAME.SecureArea.Ticket_AlertsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;">Ticket Alerts</h1></td>
              <td style="vertical-align:middle" >
                  <asp:Button ID="btnAdd" runat="server" SkinID="SubmitButtonSecure" OnClick="btnAdd_Click" Text="Add" />
              </td>
           </tr>
        </table>
        <%--<p>Ticket alerts allow you to set an alert at which you are automatically notified via either SMS or Email when tickets are created through your Widget.</p>--%>
        <p>Ticket alerts allow you to set an alert at which you are automatically notified via Email when tickets are created through your Widget.</p>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="AlertId"
            OnRowEditing="gv_RowEditing"
            OnRowDeleting="gv_RowDeleting" >
           <Columns>
              <asp:TemplateField HeaderText="Recipient Name" >
                 <ItemTemplate>
                     <%# Eval("DisplayText") %>
                 </ItemTemplate>
                 <ItemStyle Width="220px" />
              </asp:TemplateField>
              <asp:TemplateField HeaderText="Email" >
                 <ItemTemplate>
                     <%# ((bool)Eval("ByEmail"))?  Eval("Email") : "<center>x</center>" %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField HeaderText="SMS" Visible="false" >
                 <ItemTemplate>
                     <%# ((bool)Eval("BySMS")) ? Eval("Mobile") : "<center>x</center>"%>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField HeaderText="Active" >
                 <ItemTemplate>
                     <center><%# ((bool)Eval("Active"))? "&check;" : "x" %></center>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                    <center>
                        <asp:ImageButton ID="ibEdit" runat="server" SkinID="GridEditImageButton" CommandName="Edit" ToolTip="Edit" />
                        &nbsp;
                        <asp:ImageButton ID="ibRemove" runat="server" SkinID="GridRemoveImageButton" CommandName="Delete" ToolTip="Remove" />
                    </center>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
