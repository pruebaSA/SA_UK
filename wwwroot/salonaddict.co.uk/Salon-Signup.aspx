<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master"  AutoEventWireup="true" CodeBehind="Salon-Signup.aspx.cs" Inherits="SalonAddict.Salon_Signup" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="CountryDropDownList" Src="~/Modules/DropDownLists/CountryDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Modules/TextBoxes/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <SA:Message ID="lblMessage" runat="server" IsError="false" />
    <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" >
        <asp:View ID="v1" runat="server" >
            <SA:Topic ID="ContentTopic1" runat="server" meta:resourceKey="ContentTopic1" />
            <SA:Message ID="Message" runat="server" IsError="false" />
            <asp:Panel ID="pnlStep1" runat="server"  Width="420px" CssClass="grey-panel" DefaultButton="btnStep1" >
                <h2><%= base.GetLocalResourceObject("lblHeaderCompany.Text") %></h2>
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblCompany.Text") %></td>
                      <td class="data-item" >
                         <SA:TextBox ID="txtCompany" runat="server" MaxLength="100" Width="200px" ValidationGroup="signup1" meta:resourceKey="txtCompany" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblAddress1.Text") %></td>
                      <td class="data-item" >
                         <SA:TextBox ID="txtAddress1" runat="server" MaxLength="100" Width="200px" ValidationGroup="signup1" meta:resourceKey="txtAddress1" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblAddress2.Text") %></td>
                      <td class="data-item" >
                         <SA:TextBox ID="txtAddress2" runat="server" IsRequired="false" MaxLength="100" Width="200px" ValidationGroup="signup1" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblCountry.Text") %></td>
                      <td class="data-item" >
                         <SA:CountryDropDownList ID="Country" runat="server" AutoPostback="false" ValidationGroup="signup1" />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocalResourceObject("lblPhone.Text") %></td>
                      <td class="data-item" >
                         <SA:TextBox ID="txtSalonPhone" runat="server" MaxLength="50" Width="200px" ValidationGroup="signup1" meta:resourceKey="txtSalonPhone" />
                      </td>
                   </tr>
                </table>
                <div style="text-align:right;padding:10px;" >
                   <asp:Button ID="btnStep1" runat="server" SkinID="BlackButtonSmall" OnClick="btnStep1_Click"  ValidationGroup="signup1" meta:resourceKey="btnStep1" />
                </div>
            </asp:Panel>
        </asp:View>
        <asp:View ID="v2" runat="server" >
            <SA:Topic ID="ContentTopic2a" runat="server" />
            <SA:Topic ID="ContentTopic2b" runat="server" meta:resourceKey="ContentTopic2b" />
            <SA:Topic ID="ContentTopic2c" runat="server" meta:resourceKey="ContentTopic2c" />
            <SA:Topic ID="ContentTopic2d" runat="server" meta:resourceKey="ContentTopic2d" />
            <SA:Topic ID="ContentTopic2e" runat="server" meta:resourceKey="ContentTopic2e" />
            <SA:Topic ID="ContentTopic2f" runat="server" meta:resourceKey="ContentTopic2f" />
            <SA:Topic ID="ContentTopic2g" runat="server" meta:resourceKey="ContentTopic2g" />
            <div style="text-align:right;padding:10px;" >
                <asp:Button ID="btnStep2" runat="server" SkinID="BlackButtonLarge" OnClick="btnStep2_Click" CausesValidation="false" meta:resourceKey="btnStep2" />
            </div>
        </asp:View>
        <asp:View ID="v3" runat="server" >
            <SA:Topic ID="ContentTopic3" runat="server" meta:ResourceKey="ContentTopic3" />
            <table cellpadding="0" cellspacing="0" width="950px" >
               <tr>
                  <td style="width:520px;">
                    <asp:Panel ID="pnlStep31" runat="server" CssClass="grey-panel" Height="300px"  >
                        <h2><asp:Literal ID="lblHeaderCompany3" runat="server" EnableViewState="false" ></asp:Literal></h2>
                        <table class="details" cellpadding="0" cellspacing="0" >
                           <tr>
                              <td class="title" ><%= base.GetLocalResourceObject("lblEmail3.Text") %></td>
                           </tr>
                           <tr>
                              <td class="data-item" >
                                 <SA:EmailTextBox ID="txtEmail3" runat="server" MaxLength="100" Width="350px" IsRequired="false" ValidationGroup="signup3" />
                              </td>
                           </tr>
                           <tr>
                              <td class="title" ><%= base.GetLocalResourceObject("lblWebsite3.Text") %></td>
                           </tr>
                           <tr>
                              <td class="data-item" >
                                 <SA:TextBox ID="txtWebsite" runat="server" MaxLength="400" Width="350px" IsRequired="false" ValidationGroup="signup3" />
                              </td>
                           </tr>
                           <tr>
                              <td class="title" ><%= base.GetLocalResourceObject("lblShortDescription.Text") %></td>
                           </tr>
                           <tr>
                              <td class="data-item" >
                                  <SA:TextBox ID="txtShortDescription" runat="server" MaxLength="200" Width="350px" Height="100px" TextMode="MultiLine" IsRequired="false" OnKeyUp="SalonAddict.Utilities.CharacterCounter(this, '#counter_1')" />
                                  <span id="counter_1" >200</span>
                              </td>
                           </tr>
                        </table>
                    </asp:Panel>
                  </td>
                  <td style="padding-left:20px;" >
                     <asp:Panel ID="pnlStep32" runat="server" CssClass="pink-panel" DefaultButton="btnStep3" >
                        <h2><%= base.GetLocalResourceObject("lblHeaderContact.Text") %></h2>
                        <table class="details" cellpadding="0" cellspacing="0" >
                           <tr>
                              <td class="title" ><%= base.GetLocalResourceObject("lblContactFirstName.Text") %></td>
                              <td class="title" ><%= base.GetLocalResourceObject("lblContactSurname.Text") %></td>
                           </tr>
                           <tr>
                              <td class="data-item" >
                                 <SA:TextBox ID="txtContactFirstName" runat="server" MaxLength="50" Width="150px" ValidationGroup="signup3" meta:resourceKey="txtContactFirstName" />
                              </td>
                              <td class="data-item" >
                                 <SA:TextBox ID="txtContactSurname" runat="server" MaxLength="50" Width="150px" ValidationGroup="signup3" meta:resourceKey="txtContactSurname" />
                              </td>
                           </tr>
                           <tr>
                              <td class="title" colspan="2" >
                                  <%= base.GetLocalResourceObject("lblContactEmail.Text") %>
                              </td>
                           </tr>
                           <tr>
                              <td class="data-item" colspan="2" >
                                  <SA:EmailTextBox ID="txtContactEmail" runat="server" MaxLength="100" Width="324px" IsRequired="false" />
                              </td>
                           </tr>
                           <tr>
                              <td class="title" >
                                  <%= base.GetLocalResourceObject("lblContactPhone.Text") %>
                              </td>
                              <td></td>
                           </tr>
                           <tr>
                              <td class="data-item">
                                  <SA:TextBox ID="txtContactPhone" runat="server" MaxLength="24" Width="150px" IsRequired="false" />
                              </td>
                              <td></td>
                           </tr>
                        </table>
                        <div style="text-align:right;padding:10px;" >
                           <asp:Button ID="btnStep3" runat="server" SkinID="BlackButtonSmall" OnClick="btnStep3_Click"  ValidationGroup="signup3" meta:resourceKey="btnStep3" />
                        </div>
                     </asp:Panel>
                  </td>
               </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
