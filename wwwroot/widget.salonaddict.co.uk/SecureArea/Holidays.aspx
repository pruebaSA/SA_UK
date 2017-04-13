<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Holidays.aspx.cs" Inherits="IFRAME.SecureArea.HolidaysPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  <asp:Button ID="btnAdd" runat="server" SkinID="SubmitButtonSecure" OnClick="btnAdd_Click" meta:resourceKey="btnAdd" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="ClosingDayId"
            OnRowDeleting="gv_RowDeleting" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# ((DateTime)Eval("Date")).ToString("MMMM dd yyyy") %>
                 </ItemTemplate>
                 <ItemStyle Width="150px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# System.Web.HttpUtility.HtmlEncode(Eval("Description").ToString()) %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                    <center><asp:ImageButton ID="ibRemove" runat="server" SkinID="GridRemoveImageButton" CommandName="Delete" meta:resourceKey="ibRemove" /></center>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
