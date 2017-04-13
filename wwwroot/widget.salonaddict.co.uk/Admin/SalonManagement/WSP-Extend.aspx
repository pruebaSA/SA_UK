<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSP-Extend.aspx.cs" Inherits="IFRAME.Admin.WSP_ExtendPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Account" />
       <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;text-align:right;" >
              
              </td>
           </tr>
        </table>
        <asp:Label ID="lblError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
        <table class="details" cellpadding="0" cellspacing="0" >
            <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrSalon.Text") %>
              </td>
              <td class="data-item" >
                <asp:Literal ID="ltrSalon" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrPeriod.Text") %>
              </td>
              <td class="data-item" >
                <table cellpadding="0" cellspacing="0" >
                    <tr>
                       <td style="padding-right:6px" >
                           <asp:DropDownList ID="ddlDay" runat="server" SkinID="DropDownList" ></asp:DropDownList>
                       </td>
                       <td style="padding-right:6px" >
                           <asp:DropDownList ID="ddlMonth" runat="server" SkinID="DropDownList" ></asp:DropDownList>
                       </td>
                       <td>
                           <asp:DropDownList ID="ddlYear" runat="server" SkinID="DropDownList" ></asp:DropDownList>
                       </td>
                    </tr>
                 </table>
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" colspan="2" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                  <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
