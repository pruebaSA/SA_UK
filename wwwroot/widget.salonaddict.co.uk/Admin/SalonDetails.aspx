<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="SalonDetails.aspx.cs" Inherits="IFRAME.Admin.SalonDetailsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
       <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><asp:Literal ID="ltrHeader" runat="server" ></asp:Literal></h1></td>
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
                                 <tr>
                                    <td class="title" ><%= base.GetLocaleResourceString("ltrCreatedOn.Text") %></td>
                                    <td class="data-item" >
                                        <asp:Literal ID="ltrCreatedOn" runat="server" ></asp:Literal>
                                    </td>
                                 </tr>
                              </table>
                          </td>
                          <td>
                             <ul>
                                <li><p><a href='<%= IFRMHelper.GetURL("salonusers.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><u><%= base.GetLocaleResourceString("hlAPICount.Text") %> (<asp:Literal ID="ltrAPICount" runat="server" ></asp:Literal>) &nbsp;/&nbsp; <%= base.GetLocaleResourceString("hlUserCount.Text") %> (<asp:Literal ID="ltrUserCount" runat="server" ></asp:Literal>)</u></a></p></li>
                                <li><p><a href='<%= IFRMHelper.GetURL("salonsetup.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><u><%= base.GetLocaleResourceString("hlSalonSetup.Text") %></u></a></li>
                                <li><p><a href='<%= IFRMHelper.GetURL("salonmanagement/default.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><u><%= base.GetLocaleResourceString("hlSalonManagement.Text") %></u></a></p></li>
                             </ul>
                             <p>&nbsp;</p>
                             <p>&nbsp;</p>
                             <p>&nbsp;</p>
                             <p>&nbsp;</p>
                             <p>&nbsp;</p>
                             <center>
                                 <a href="http://itouchmap.com/latlong.html" target="_blank" ><u><%= base.GetLocaleResourceString("hlLatLong.Text") %></u></a>
                             </center>
                          </td>
                       </tr>
                    </table>
                </asp:Panel>
              </ContentTemplate>
           </ajaxToolkit:TabPanel>
           <ajaxToolkit:TabPanel ID="p2" runat="server" >
              <ContentTemplate>
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrSEName.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:UpdatePanel ID="upSEName" runat="server" >
                            <ContentTemplate>
                                 <asp:TextBox ID="txtSEName" runat="server" SkinID="TextBox" Width="300px" MaxLength="100" AutoPostBack="true" OnTextChanged="txtSEName_TextChanged" ></asp:TextBox>
                                 <table cellpadding="0" cellspacing="0" >
                                    <tr>
                                       <td>
                                          <asp:RequiredFieldValidator ID="valSEName" runat="server" ControlToValidate="txtSEName" Display="None" meta:resourceKey="valSEName" ></asp:RequiredFieldValidator>
                                          <ajaxToolkit:ValidatorCalloutExtender ID="valSENameEx" runat="server" TargetControlID="valSEName" EnableViewState="false" />
                                       </td>
                                       <td>
                                          <asp:Label ID="lblSENameAvail" runat="server" EnableViewState="false" ForeColor="#c80d0d" ></asp:Label>
                                       </td>
                                       <td></td>
                                    </tr>
                                 </table>
                            </ContentTemplate>
                            <Triggers>
                               <asp:AsyncPostBackTrigger ControlID="txtSEName" />
                            </Triggers>
                         </asp:UpdatePanel>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <%= base.GetLocaleResourceString("ltrShortDescription.Text") %>
                      </td>
                      <td class="data-item" >
                         <asp:TextBox ID="txtDescription" runat="server" SkinID="TextBox" TextMode="MultiLine" Height="80px" Width="500px" ></asp:TextBox>
                      </td>
                   </tr>
                </table>
              </ContentTemplate>
           </ajaxToolkit:TabPanel>
           <ajaxToolkit:TabPanel ID="p3" runat="server" >
               <ContentTemplate>
                   <asp:UpdatePanel ID="upLocation" runat="server" >
                      <Triggers>
                          <asp:AsyncPostBackTrigger ControlID="btnLocationAdd" />
                      </Triggers>
                      <ContentTemplate>
                           <table class="details" cellpadding="0" cellspacing="0" >
                               <tr>
                                  <td class="title"  >
                                       <asp:DropDownList ID="ddlLocationCC" runat="server" Width="220px" SkinID="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="ddlLocationCC_SelectedIndexChanged" >
                                          <asp:ListItem Text="City or County" Value="" ></asp:ListItem>
                                          <asp:ListItem Text="Dublin" Value="D" ></asp:ListItem>
                                          <asp:ListItem Text="Galway" Value="G" ></asp:ListItem>
                                       </asp:DropDownList>
                                  </td>
                                  <td class="data-item" >
                                       <asp:DropDownList ID="ddlLocationAP" runat="server" Width="220px" SkinID="DropDownList" >
                                          <asp:ListItem Text="Area or Postal code" Value="" ></asp:ListItem>
                                          <asp:ListItem Text="Dublin" Value="D" ></asp:ListItem>
                                          <asp:ListItem Text="Galway" Value="G" ></asp:ListItem>
                                       </asp:DropDownList>
                                  </td>
                                  <td style="vertical-align:top;" class="data-item" >
                                        &nbsp;&nbsp;
                                       <asp:Button ID="btnLocationAdd" runat="server" SkinID="SubmitButton" Text="Add Location" OnClick="btnLocationAdd_Click" />
                                  </td>
                               </tr>
                               <tr>
                                  <td class="title" colspan="3" >
                                     <asp:Label ID="lblLocationError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
                                  </td>
                               </tr>
                           </table>
                            <asp:GridView 
                                ID="gvLocations" 
                                runat="server" 
                                AutoGenerateColumns="False" 
                                DataKeyNames="MappingId"
                                OnRowDeleting="gvLocation_RowDeleting" >
                               <Columns>
                                  <asp:TemplateField>
                                     <ItemTemplate>
                                         <%# Eval("Name") %>
                                     </ItemTemplate>
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
                      </ContentTemplate>
                   </asp:UpdatePanel>
               </ContentTemplate>
           </ajaxToolkit:TabPanel>
           <ajaxToolkit:TabPanel ID="p4" runat="server" HeaderText="Pictures" >
               <ContentTemplate>
                    <table class="details" cellspacing="0" cellpadding="0" >
                        <tr>
                           <td class="title" style="padding:20px;" >
                              <div style="border:solid 1px #eee;padding:6px;min-height:110px;max-width:110px;background:#f7f7f7;">
                                <asp:Image ID="imgPicture" runat="server" EnableViewState="false" Width="110px" Height="110px" Visible="false" />
                                <asp:Literal ID="imgNone" runat="server" Text="<p><center><i>no image</i></center></p>" ></asp:Literal>
                              </div>
                              <p style="padding-left:20px;" >
                                 <asp:Button ID="btnPictureDelete" runat="server" SkinID="SubmitButtonMini" Text="Delete Picture" OnClick="btnPictureDelete_Click" Visible="false" />
                              </p>
                           </td>
                           <td class="data-item" style="padding-left:50px;padding-top:20px;" >
                              <asp:FileUpload ID="fuPicture" runat="server" Width="300px" />
                              <asp:Label ID="lblError" runat="server" SkinID="ErrorLabel" Font-Italic="true" Font-Size="11px" Font-Bold="false" ></asp:Label>
                              <p>
                                <asp:Button ID="btnPictureSave" runat="server" SkinID="SubmitButton" Text="Save & Replace Picture" OnClick="btnPictureSave_Click" />
                              </p>
                              <p>
                                  <b>Pictures must meet the following requirements:</b>
                                  <ul>
                                     <li>It must be in jpg/jpeg or png format</li>
                                     <li>It must be minimum 110px x 110px</li>
                                     <li>Pixel dimensions must be square; width must equal height.</li>
                                     <li>It must have a file size of less than 100k</li>
                                  </ul>
                              </p>
                           </td>
                        </tr>
                    </table>
               </ContentTemplate>
           </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </asp:Panel>
</asp:Content>
