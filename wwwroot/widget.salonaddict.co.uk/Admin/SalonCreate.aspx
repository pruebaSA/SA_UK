<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="SalonCreate.aspx.cs" Inherits="IFRAME.Admin.SalonCreatePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
       <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;text-align:right;" >
                 <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                 <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
              </td>
           </tr>
        </table>
        <ajaxToolkit:TabContainer ID="tc" runat="server" ActiveTabIndex="0" >
           <ajaxToolkit:TabPanel ID="p1" runat="server" >
              <ContentTemplate>
                  <asp:Panel ID="pnl2" runat="server" >
                      <table cellpadding="0" cellspacing="0" width="100%" >
                         <tr>
                            <td>
                             <table class="details" cellpadding="0" cellspacing="0" >
                                 <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrName.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtName" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                                       <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                                       <ajaxToolkit:ValidatorCalloutExtender ID="valNameEx" runat="server" TargetControlID="valName" EnableViewState="false" />
                                    </td>
                                 </tr>
                                <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrSalonId.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtSalonId" runat="server" SkinID="TextBox" MaxLength="3" Width="35px" ></asp:TextBox>
                                       <asp:RequiredFieldValidator ID="valSalonId" runat="server" ControlToValidate="txtSalonId" Display="None" meta:resourceKey="valSalonId" ></asp:RequiredFieldValidator>
                                       <ajaxToolkit:ValidatorCalloutExtender ID="valSalonIdEx" runat="server" TargetControlID="valSalonId" EnableViewState="false" />
                                    </td>
                                 </tr>
                                 <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrPhone.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtPhone" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                                       <asp:RequiredFieldValidator ID="valPhone" runat="server" ControlToValidate="txtPhone" Display="None" meta:resourceKey="valPhone" ></asp:RequiredFieldValidator>
                                       <ajaxToolkit:ValidatorCalloutExtender ID="valPhoneEx" runat="server" TargetControlID="valPhone" EnableViewState="false" />
                                    </td>
                                 </tr>
                                 <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrAddressLine1.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtAddressLine1" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                                       <asp:RequiredFieldValidator ID="valAddressLine1" runat="server" ControlToValidate="txtAddressLine1" Display="None" meta:resourceKey="valAddressLine1" ></asp:RequiredFieldValidator>
                                       <ajaxToolkit:ValidatorCalloutExtender ID="valAddressLine1Ex" runat="server" TargetControlID="valAddressLine1" EnableViewState="false" />
                                    </td>
                                 </tr>
                                 <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrAddressLine2.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtAddressLine2" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                                    </td>
                                 </tr>
                                 <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrCity.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtCity" runat="server" MaxLength="30" SkinID="TextBox" ></asp:TextBox>
                                       <asp:RequiredFieldValidator ID="valAddressLine3" runat="server" ControlToValidate="txtCity" Display="None" meta:resourceKey="valAddressLine3" ></asp:RequiredFieldValidator>
                                       <ajaxToolkit:ValidatorCalloutExtender ID="valAddressLine3Ex" runat="server" TargetControlID="valAddressLine3" EnableViewState="false" />
                                    </td>
                                 </tr>
                                 <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrPostalCode.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtPostalCode" runat="server" MaxLength="15" ></asp:TextBox>
                                       <asp:RequiredFieldValidator ID="valAddressLine5" runat="server" ControlToValidate="txtPostalCode" Display="None" meta:resourceKey="valAddressLine5" ></asp:RequiredFieldValidator>
                                       <ajaxToolkit:ValidatorCalloutExtender ID="valAddressLine5Ex" runat="server" TargetControlID="valAddressLine5" EnableViewState="false" />
                                    </td>
                                 </tr>
                                 <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrLatitude.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtLatitude" runat="server" SkinID="TextBox" MaxLength="20" ></asp:TextBox>
                                       <asp:RequiredFieldValidator ID="valLatitude" runat="server" ControlToValidate="txtLatitude" Display="None" meta:resourceKey="valLatitude" ></asp:RequiredFieldValidator>
                                       <ajaxToolkit:ValidatorCalloutExtender ID="valLatitudeEx" runat="server" TargetControlID="valLatitude" EnableViewState="false" />
                                    </td>
                                 </tr>
                                 <tr>
                                    <td class="title" >
                                       <%= base.GetLocaleResourceString("ltrLongitude.Text") %>
                                    </td>
                                    <td class="data-item" >
                                       <asp:TextBox ID="txtLongitude" runat="server" SkinID="TextBox" MaxLength="20" ></asp:TextBox>
                                       <asp:RequiredFieldValidator ID="valLongitude" runat="server" ControlToValidate="txtLongitude" Display="None" meta:resourceKey="valLongitude" ></asp:RequiredFieldValidator>
                                       <ajaxToolkit:ValidatorCalloutExtender ID="valLongitudeEx" runat="server" TargetControlID="valLongitude" EnableViewState="false" />
                                    </td>
                                 </tr>
                              </table>
                            </td>
                            <td style="vertical-align:bottom;" >
                                <center>
                                    <a href="http://itouchmap.com/latlong.html" target="_blank" ><u><%= base.GetLocaleResourceString("hlLatLong.Text") %></u></a>
                                </center>
                                <p>&nbsp;</p>
                                <p>&nbsp;</p>
                            </td>
                         </tr>
                      </table>
                  </asp:Panel>
              </ContentTemplate>
           </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </asp:Panel>
</asp:Content>