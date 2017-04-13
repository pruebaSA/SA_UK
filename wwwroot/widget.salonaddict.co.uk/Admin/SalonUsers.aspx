<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonUsers.aspx.cs" Inherits="IFRAME.Admin.SalonUsersPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
       <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><asp:Literal ID="ltrHeader" runat="server" ></asp:Literal></h1></td>
              <td style="vertical-align:middle" >
                  
              </td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" >
           <tr>
              <td width="80px" ><h2><%= base.GetLocaleResourceString("ltrAPIKeys.Text") %></h2></td>
              <td style="vertical-align:middle;" >
                 <asp:Button ID="btnAddAPIKey" runat="server" SkinID="SubmitButtonMini" OnClick="btnAddAPIKey_Click" meta:resourceKey="btnAdd" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gvAPIKeys" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="KeyId"
            OnRowEditing="gvAPIKeys_RowEditing" 
            OnRowDeleting="gvAPIKeys_RowDeleting" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("VerificationToken") %>
                 </ItemTemplate>
                 <ItemStyle Width="100px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("HttpReferer") %>
                 </ItemTemplate>
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
        <table cellpadding="0" cellspacing="0" >
           <tr>
              <td width="80px" ><h2><%= base.GetLocaleResourceString("ltrUsers.Text") %></h2></td>
              <td style="vertical-align:middle;" >
                 <asp:Button ID="btnAddUser" runat="server" SkinID="SubmitButtonMini" OnClick="btnAddUser_Click" meta:resourceKey="btnAdd" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gvUsers" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="UserId"
            OnRowCreated="gvUsers_RowCreated"
            OnRowEditing="gvUsers_RowEditing" 
            OnRowDeleting="gvUsers_RowDeleting" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Username") %>
                 </ItemTemplate>
                 <ItemStyle Width="85px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# (Eval("FirstName") + " " + Eval("LastName")).Trim() %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Mobile")%>
                 </ItemTemplate>
                 <ItemStyle Width="90px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <%# Eval("Email")%>
                 </ItemTemplate>
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