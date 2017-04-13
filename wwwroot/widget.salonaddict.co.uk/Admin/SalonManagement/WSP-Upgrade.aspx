<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSP-Upgrade.aspx.cs" Inherits="IFRAME.Admin.WSP_UpgradePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
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
        <table cellpadding="0" cellspacing="0" >
           <tr>
              <td width="480px" >
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
                         <%= base.GetLocaleResourceString("ltrPlanType.Text") %>
                      </td>
                      <td class="data-item" >
                        <asp:DropDownList ID="ddlPlanType" runat="server" SkinID="DropDownList">
                             <asp:ListItem Text="Trial Plan" Value="10" ></asp:ListItem>
                             <asp:ListItem Text="Monthly Plan" Value="30" ></asp:ListItem>
                             <asp:ListItem Text="Annual Plan" Value="100" ></asp:ListItem>
                         </asp:DropDownList>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrDescription.Text") %>
                      </td>
                      <td class="data-item" >
                        <asp:TextBox ID="txtDescription" runat="server" MaxLength="30" SkinID="TextBox" Text="Trial Plan" ></asp:TextBox>
                         <asp:RequiredFieldValidator ID="valDescription" ControlToValidate="txtDescription" runat="server" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                         <ajaxToolkit:ValidatorCalloutExtender ID="valDescriptionEx" runat="Server" TargetControlID="valDescription" HighlightCssClass="validator-highlight" />
                        <script type="text/javascript" language="javascript" >
                            var ddlplan = document.getElementById('<%= ddlPlanType.ClientID %>');
                            var txtdesc = document.getElementById('<%= txtDescription.ClientID %>');

                            ddlplan.onchange = function() {
                                txtdesc.value = this.options[this.selectedIndex].text;
                            }
                        </script>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrPlanPrice.Text")%>
                      </td>
                      <td class="data-item" >
                         <asp:TextBox ID="txtPrice" runat="server" MaxLength="10" SkinID="TextBox" Width="60px" Text="0.00" ></asp:TextBox>
                         <ajaxToolkit:FilteredTextBoxExtender ID="ftrPrice" runat="server" TargetControlID="txtPrice" FilterType="Custom, Numbers" ValidChars="." />
                         <asp:RequiredFieldValidator ID="valPrice" ControlToValidate="txtPrice" runat="server" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                         <asp:RangeValidator ID="valPriceRange" runat="server" ControlToValidate="txtPrice" Type="Double" MinimumValue="0.00" MaximumValue="999.00" Display="None" meta:resourceKey="valPriceRange" ></asp:RangeValidator>
                         <ajaxToolkit:ValidatorCalloutExtender ID="valPriceEx" runat="Server" TargetControlID="valPrice" HighlightCssClass="validator-highlight" />
                         <ajaxToolkit:ValidatorCalloutExtender ID="valPriceRangeEx" runat="Server" TargetControlID="valExcessFeeWTRange" HighlightCssClass="validator-highlight" />
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
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrTransFee.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:TextBox ID="txtExcessFeeWT" runat="server" MaxLength="10" SkinID="TextBox" Width="60px" Text="0.39" ></asp:TextBox>
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
              </td>
              <td style="padding-top:10px;">
                 <%= base.GetLocaleResourceString("ltrHelp.Text") %>
              </td>
           </tr>
        </table>
     </asp:Panel>
</asp:Content>
