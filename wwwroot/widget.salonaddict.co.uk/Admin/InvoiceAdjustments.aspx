<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoiceAdjustments.aspx.cs" Inherits="IFRAME.Admin.InvoiceAdjustmentsPage" %>
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
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" OnClick="btnCancel_Click" CausesValidation="false" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnAdd" runat="server" SkinID="SubmitButton" OnClick="btnAdd_Click" meta:resourceKey="btnAdd" />
              </td> 
           </tr>
        </table>
        <asp:GridView 
            ID="gv" 
            runat="server"
            AutoGenerateColumns="False" 
            OnRowDeleting="gv_RowDeleting"
            DataKeyNames="AdjustmentId" >
           <Columns>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# Eval("Description") %>
                 </ItemTemplate>
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <center><%# ((double)(int)Eval("Amount") / 100).ToString("C") %></center>
                 </ItemTemplate>
                 <ItemStyle Width="60px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                     <%# ((DateTime)Eval("CreatedOn")).ToString("dd MMM yyyy") %>
                 </ItemTemplate>
                 <ItemStyle Width="85px" />
              </asp:TemplateField>
              <asp:TemplateField>
                 <ItemTemplate>
                    <center>
                        <asp:ImageButton ID="ibRemove" runat="server" SkinID="GridRemoveImageButton" CommandName="Delete" meta:resourceKey="ibRemove" />
                    </center>
                 </ItemTemplate>
                 <ItemStyle Width="30px" />
              </asp:TemplateField>
           </Columns>
        </asp:GridView>
     </asp:Panel>
</asp:Content>