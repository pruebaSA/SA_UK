<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoiceVoid.aspx.cs" Inherits="IFRAME.Admin.InvoiceVoidPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Billing" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-billing.png" %>' alt="billing" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  
              </td> 
           </tr>
        </table>
        <asp:Label ID="lblError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrInvoiceNumber.Text") %>
              </td>
              <td class="data-item" >
                 <asp:Literal ID="ltrInvoiceNumber" runat="server" EnableViewState="true" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrAdminComment.Text") %>
              </td>
              <td class="data-item" >
                 <asp:TextBox ID="txtAdminComment" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valAdminComment" runat="server" ControlToValidate="txtAdminComment" Display="None" meta:resourceKey="valAdminComment" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valAdminCommentEx" runat="Server" TargetControlID="valAdminComment" EnableViewState="false" />
              </td>
           </tr>
          <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrPassword.Text") %></td>
              <td class="data-item" >
                 <asp:TextBox ID="txtPassword" runat="server" SkinID="TextBox" TextMode="Password" MaxLength="50" ></asp:TextBox>
                 <asp:RequiredFieldValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" Display="None" meta:resourceKey="valPassword" ></asp:RequiredFieldValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valPasswordEx" runat="Server" TargetControlID="valPassword" EnableViewState="false" />
              </td>
          </tr>
          <tr>
             <td class="title" ></td>
             <td class="data-item" >
                 <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" OnClick="btnCancel_Click" CausesValidation="false" meta:resourceKey="btnCancel" />
                 <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
             </td>
          </tr>
        </table>
    </asp:Panel>
</asp:Content>