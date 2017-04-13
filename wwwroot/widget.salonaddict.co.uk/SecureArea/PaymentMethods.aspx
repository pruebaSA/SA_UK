<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="PaymentMethods.aspx.cs" Inherits="IFRAME.SecureArea.PaymentMethodsPage" %>
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
                  <asp:Button ID="btnAdd" runat="server" SkinID="SubmitButtonSecure" OnClick="btnAdd_Click" meta:resourceKey="btnAdd" />
              </td>
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            AutoGenerateColumns="False" 
            DataKeyNames="SalonPaymentMethodId" 
            OnRowEditing="gv_RowEditing"
            OnRowDeleting="gv_RowDeleting">
           <Columns>
               <asp:TemplateField>
                 <ItemTemplate>
                      <%# System.Web.HttpUtility.HtmlEncode(Eval("Alias").ToString()) %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <center><%# Eval("CardExpirationMonth") + "/" + Eval("CardExpirationYear") %></center>
                 </ItemTemplate>
                 <ItemStyle Width="50px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                      <center><asp:RadioButton ID="rb" runat="server" Checked='<%# (bool)Eval("IsPrimary") %>' GroupName="IS_PRIMARY" AutoPostBack="true" OnCheckedChanged="rb_CheckedChanged" /></center>
                 </ItemTemplate>
                 <ItemStyle Width="50px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# ((bool)Eval("Active")) ? "&check;" : "x" %></center>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
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