<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="PaymentMethod-Delete.aspx.cs" Inherits="IFRAME.SecureArea.PaymentMethod_DeletePage" %>
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

              </td>
           </tr>
        </table>
        <asp:Label ID="lblError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
        <table style="margin-top:10px;" class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrAlias.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrAlias" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCardType.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCardType" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCardName.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCardName" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrCardNumber.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrCardNumber" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ><%= base.GetLocaleResourceString("ltrExpiry.Text") %></td>
              <td class="data-item" >
                 <asp:Literal ID="ltrExpiry" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr><td colspan="2" >&nbsp;</td></tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" >
                 <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                 <asp:Button ID="btnDelete" runat="server" SkinID="SubmitButton" OnClick="btnDelete_Click" meta:resourceKey="btnDelete" />
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
