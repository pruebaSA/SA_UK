<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSP-WT.aspx.cs" Inherits="IFRAME.Admin.WSP_WTPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Account" />
       <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><asp:Literal ID="ltrHeader" runat="server" ></asp:Literal></h1></td>
              <td style="vertical-align:middle;text-align:right;" >
              
              </td>
           </tr>
        </table>
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
                 <%= base.GetLocaleResourceString("ltrTransFee.Text") %>
              </td>
              <td class="data-item" >
                 <asp:TextBox ID="txtExcessFeeWT" runat="server" MaxLength="10" SkinID="TextBox" Width="60px" ></asp:TextBox>
                 <ajaxToolkit:FilteredTextBoxExtender ID="fltExcessFeeWT" runat="server" TargetControlID="txtExcessFeeWT" FilterType="Custom, Numbers" ValidChars="." />
                 <asp:RequiredFieldValidator ID="valExcessFeeWT" ControlToValidate="txtExcessFeeWT" runat="server" Display="None" meta:resourceKey="valExcessFeeWT" ></asp:RequiredFieldValidator>
                 <asp:RangeValidator ID="valExcessFeeWTRange" runat="server" ControlToValidate="txtExcessFeeWT" Type="Double" MinimumValue="0.00" MaximumValue="2.99" Display="None" meta:resourceKey="valExcessFeeWTRange" ></asp:RangeValidator>
                 <ajaxToolkit:ValidatorCalloutExtender ID="valExcessFeeWTEx" runat="Server" TargetControlID="valExcessFeeWT" HighlightCssClass="validator-highlight" />
                 <ajaxToolkit:ValidatorCalloutExtender ID="valExcessFeeWTRangeEx" runat="Server" TargetControlID="valExcessFeeWTRange" HighlightCssClass="validator-highlight" />
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
