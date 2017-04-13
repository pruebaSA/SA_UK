<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="InvoiceAdjustment-Create.aspx.cs" Inherits="IFRAME.Admin.InvoiceAdjustment_CreatePage" %>
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
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" OnClick="btnCancel_Click" CausesValidation="false" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
              </td> 
           </tr>
        </table>
        <asp:Label ID="lblError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
        <table cellpadding="0" cellspacing="0" >
           <tr>
              <td style="width:410px" >
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                       <td class="title" >
                           <%= base.GetLocaleResourceString("ltrDescription.Text") %>
                       </td>
                       <td class="data-item" >
                           <asp:TextBox ID="txtDescription" runat="server" MaxLength="50" SkinID="TextBox" ></asp:TextBox>
                           <asp:RequiredFieldValidator ID="valDescription" ControlToValidate="txtDescription" runat="server" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="valDescriptionEx" TargetControlID="valDescription" HighlightCssClass="validator-highlight" />
                       </td>
                   </tr>
                   <tr>
                       <td class="title" >
                           <%= base.GetLocaleResourceString("ltrAdjustment.Text") %>
                       </td>
                       <td class="data-item" >
                           <asp:TextBox ID="txtValue" runat="server" MaxLength="6" SkinID="TextBox" Width="50px" ></asp:TextBox>
                           <ajaxToolkit:FilteredTextBoxExtender ID="ftbeValue" runat="server" TargetControlID="txtValue" FilterType="Custom, Numbers" ValidChars=".-" />
                           <asp:RequiredFieldValidator ID="rfvValue" ControlToValidate="txtValue" runat="server" Display="None" meta:resourceKey="rfvValue" ></asp:RequiredFieldValidator>
                           <asp:RangeValidator ID="rvValue" runat="server" ControlToValidate="txtValue" Type="Double" Display="None" MinimumValue="-500.00" MaximumValue="500.00" ></asp:RangeValidator>
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueE" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rvValueE" TargetControlID="rvValue" HighlightCssClass="validator-highlight" />
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
                </table>
              </td>
              <td>
                  <ul>
                     <li>
                        <%= base.GetLocaleResourceString("ltrHelp.Text") %>
                     </li>
                  </ul>
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>