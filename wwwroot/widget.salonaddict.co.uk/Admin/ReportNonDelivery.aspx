<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="ReportNonDelivery.aspx.cs" Inherits="IFRAME.Admin.ReportNonDeliveryPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
<asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Reports" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-reports.png" %>' alt="reports" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  <asp:Button ID="btnResend" runat="server" SkinID="SubmitButton" UseSubmitBehavior="false" OnClientClick="this.disabled=true;" meta:resourceKey="btnResend" OnClick="btnResend_Click" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="QueuedMessageId"
            OnRowEditing="gv_RowEditing"
            OnRowDeleting="gv_RowDeleting" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Sender") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Recipient") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <center><%# Eval("SendTries") %></center>
                 </ItemTemplate>
                 <ItemStyle Width="80px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                    <center><%# ((DateTime)Eval("CreatedOn")).ToString("MMM dd yyyy") %></center>
                 </ItemTemplate>
                 <ItemStyle Width="100px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                    <center>
                        <asp:ImageButton ID="ibEdit" runat="server" SkinID="GridEditImageButton" CommandName="Edit" meta:resourceKey="ibEdit" />
                        &nbsp;
                        <asp:ImageButton ID="ibRemove" runat="server" SkinID="GridRemoveImageButton" CommandName="Delete" meta:resourceKey="ibRemove" />
                    </center>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
</asp:Panel>
</asp:Content>
