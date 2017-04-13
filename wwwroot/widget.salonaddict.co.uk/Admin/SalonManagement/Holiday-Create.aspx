<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Holiday-Create.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.Holiday_CreatePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="DateTextBox" Src="~/Modules/DateTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:450px;" >
                <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td class="title" ><%= base.GetLocaleResourceString("ltrDate.Text") %></td>
                        <td class="data-item" >
                           <IFRM:DateTextBox ID="txtDate" runat="server" />
                           <asp:RequiredFieldValidator ID="valDate" runat="server" ControlToValidate="txtDate" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                           <ajaxToolkit:ValidatorCalloutExtender ID="valDateEx" runat="Server" TargetControlID="valDate" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title" ><%= base.GetLocaleResourceString("ltrDescription.Text") %></td>
                        <td class="data-item" >
                           <asp:TextBox ID="txtDescription" runat="server" SkinID="TextBox" MaxLength="50" />
                           <asp:RequiredFieldValidator ID="valDescription" runat="server" ControlToValidate="txtDescription" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                           <ajaxToolkit:ValidatorCalloutExtender ID="valDescriptionEx" runat="Server" TargetControlID="valDescription" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title" ></td>
                        <td class="data-item" >
                            <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                            <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
                        </td>
                    </tr>
                </table>
              </td>
              <td>
                 <%= base.GetLocaleResourceString("ltrHelp.Text") %>
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
