<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="IFRAME.DetailsPage" %>
<%@ Register TagPrefix="IFRM" TagName="PhoneList" Src="~/Modules/PhoneDropDownList.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="YesNoOptions" Src="~/Modules/YesNoOptions.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <script type="text/javascript" language="javascript" src="js/jquery.popupWin.js" ></script>
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <div style="position:relative;" >
          <h1 style="margin-bottom:25px;" ><asp:Literal ID="ltrHeader" runat="server" EnableViewState="false" ></asp:Literal></h1>
          <div style="position:absolute;top:26px;left:0px;font-size:11px;" >
             <asp:Literal ID="ltrAddress" runat="server" EnableViewState="false" ></asp:Literal>
          </div>
       </div>
       <ajaxToolkit:Accordion 
            ID="mv" 
            runat="server" 
            Height="255px"
            AutoSize="Fill" 
            SelectedIndex="0"
            FadeTransitions="false" 
            TransitionDuration="250" 
            FramesPerSecond="40" 
            SuppressHeaderPostbacks="true" 
            RequireOpenedPane="true" >
          <Panes>
             <ajaxToolkit:AccordionPane ID="v0" runat="server">
                <Header>
                   <asp:Label ID="lblService" runat="server" EnableViewState="false" ></asp:Label>
                   <asp:Label ID="lblEmployee" runat="server" EnableViewState="false" ></asp:Label>
                   <asp:Label ID="lblDate" runat="server" EnableViewState="false" Font-Bold="false" ></asp:Label>
                </Header>
                <Content>
                   <table class="details" cellpadding="0" cellspacing="0" >
                      <tr>
                         <td class="title" ><%= base.GetLocaleResourceString("ltrName.Text") %></td>
                         <td class="data-item" >
                           <asp:TextBox ID="txtFirstName" runat="server" SkinID="TextBox" MaxLength="20" Font-Italic="true" meta:resourceKey="txtFirstName" ></asp:TextBox>
                           <asp:RequiredFieldValidator ID="valFirstName" runat="server" ControlToValidate="txtFirstName" Display="None" meta:resourceKey="valFirstName" ></asp:RequiredFieldValidator>
                           <ajaxToolkit:ValidatorCalloutExtender ID="valFirstNameEX" runat="Server" TargetControlID="valFirstName" EnableViewState="false" />
                           <asp:RegularExpressionValidator ID="valFirstNameRegEx1" runat="server" ControlToValidate="txtFirstName" Display="None" ValidationExpression="[^0-9]+" meta:resourceKey="valFirstNameRegEx1"></asp:RegularExpressionValidator>
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="valFirstNameRegExEx1" TargetControlID="valFirstNameRegEx1" EnableViewState="false" />
                           <asp:RegularExpressionValidator ID="valFirstNameRegEx2" runat="server" ControlToValidate="txtFirstName" Display="None" ValidationExpression="^((?!First|first).)*$" meta:resourceKey="valFirstNameRegEx2"></asp:RegularExpressionValidator>
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="valFirstNameRegEx2Ex" TargetControlID="valFirstNameRegEx2" EnableViewState="false" />
                         </td>
                         <td class="data-item" >
                           <asp:TextBox ID="txtLastName" runat="server" SkinID="TextBox" MaxLength="20" Font-Italic="true" meta:resourceKey="txtLastName" ></asp:TextBox>
                           <asp:RequiredFieldValidator ID="valLastName" runat="server" ControlToValidate="txtLastName" Display="None" meta:resourceKey="valLastName" ></asp:RequiredFieldValidator>
                           <ajaxToolkit:ValidatorCalloutExtender ID="valLastNameEx" runat="Server" TargetControlID="valLastName" EnableViewState="false" />
                           <asp:RegularExpressionValidator ID="valLastNameRegEx1" runat="server" ControlToValidate="txtLastName" Display="None" ValidationExpression="[^0-9]+" meta:resourceKey="valLastNameRegEx1"></asp:RegularExpressionValidator>
                           <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="valLastNameRegEx1Ex" TargetControlID="valLastNameRegEx1" EnableViewState="false" />
                           <asp:RegularExpressionValidator ID="valLastNameRegEx2" runat="server" ControlToValidate="txtLastName" Display="None" ValidationExpression="^((?!Last|last).)*$" meta:resourceKey="valLastNameRegEx2"></asp:RegularExpressionValidator>
                           <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="valLastNameRegEx2Ex" TargetControlID="valLastNameRegEx2" EnableViewState="false" />
                         </td>
                      </tr>
                      <tr>
                         <td class="title" ><%= base.GetLocaleResourceString("ltrPhone.Text") %></td>
                         <td class="data-item" >
                            <IFRM:PhoneList ID="ddlPhoneType" runat="server" />
                         </td>
                         <td class="data-item" >
                           <asp:TextBox ID="txtPhone" runat="server" SkinID="TextBox" MaxLength="20" ></asp:TextBox>
                           <asp:RequiredFieldValidator ID="valPhone" runat="server" ControlToValidate="txtPhone" Display="None" meta:resourceKey="valPhone" ></asp:RequiredFieldValidator>
                           <ajaxToolkit:ValidatorCalloutExtender ID="valPhoneEx" runat="server" TargetControlID="valPhone" EnableViewState="false" />
                           <asp:RegularExpressionValidator ID="valPhoneRegEx" runat="server" ControlToValidate="txtPhone" Display="None" ValidationExpression="^[0-9 \-\+\(\)]*" meta:resourceKey="valPhoneRegEx"></asp:RegularExpressionValidator>
                           <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="valPhoneRegExEx" TargetControlID="valPhoneRegEx" EnableViewState="false" />
                         </td>
                      </tr>
                      <tr>
                         <td class="title" ><%= base.GetLocaleResourceString("ltrEmail.Text") %></td>
                         <td class="data-item" >
                           <asp:TextBox ID="txtEmail" runat="server" SkinID="TextBox" MaxLength="50" ></asp:TextBox>
                           <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" Display="None" meta:resourceKey="valEmail" ></asp:RequiredFieldValidator>
                           <ajaxToolkit:ValidatorCalloutExtender ID="valEmailEx" runat="server" TargetControlID="valEmail" EnableViewState="false" />
                           <asp:RegularExpressionValidator ID="valEmailRegEx" runat="server" ControlToValidate="txtEmail" Display="None" ValidationExpression=".+@.+\..+" meta:resourceKey="valEmailRegEx" ></asp:RegularExpressionValidator>
                           <ajaxToolkit:ValidatorCalloutExtender ID="valEmailRegExEx" runat="server" TargetControlID="valEmailRegEx" EnableViewState="false" />
                         </td>
                         <td class="data-item" >
                           <asp:TextBox ID="txtEmailConfirm" runat="server" SkinID="TextBox" MaxLength="50" meta:resourceKey="txtEmailConfirm" ></asp:TextBox>
                           <asp:CompareValidator ID="valEmailConfirmComp" runat="server" ControlToValidate="txtEmailConfirm" ControlToCompare="txtEmail" Operator="Equal" Display="None" meta:resourceKey="valEmailConfirmComp" ></asp:CompareValidator>
                           <ajaxToolkit:ValidatorCalloutExtender ID="valEmailConfirmCompEx" runat="server" TargetControlID="valEmailConfirmComp" EnableViewState="false" />
                         </td>
                      </tr>
                      <tr>
                         <td class="title" colspan="2" >
                            <table cellpadding="0" cellspacing="0" >
                               <tr>
                                  <td>
                                     <%= base.GetLocaleResourceString("ltrQuestion1.Text") %>
                                  </td>
                                  <td style="padding-left:15px;" >
                                     <IFRM:YesNoOptions ID="rblYesNo" runat="server" />
                                     <asp:RequiredFieldValidator ID="valYesNo" runat="server" ControlToValidate="rblYesNo" Display="None" meta:resourceKey="valYesNo" ></asp:RequiredFieldValidator>
                                     <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="valYesNoEx" TargetControlID="valYesNo" EnableViewState="false" />
                                  </td>
                               </tr>
                            </table>
                         </td>
                      </tr>
                   </table>
                </Content>
             </ajaxToolkit:AccordionPane>
             <ajaxToolkit:AccordionPane ID="v1" runat="server" >
                <Header><%= base.GetLocaleResourceString("ltrHeader[1].Text") %></Header>
                <Content>
                    <br />
                    <table class="details" cellpadding="0" cellspacing="0" >
                       <tr>
                          <td class="title" >
                             <%= base.GetLocaleResourceString("ltrSpecialRequests.Text") %>
                          </td>
                          <td class="data-item" >
                             <asp:TextBox ID="txtSpecialRequests" runat="server" TextMode="MultiLine" Height="100px" Width="400px" ></asp:TextBox>
                          </td>
                       </tr>
                    </table>
                </Content>
             </ajaxToolkit:AccordionPane>
          </Panes>
       </ajaxToolkit:Accordion>
       <table class="details" cellpadding="0" cellspacing="0" width="100%" >
          <tr>
             <td class="title" >
                <p style="text-align:center;margin-top:12px;margin-bottom:10px;" ><%= base.GetLocaleResourceString("ltrUserAgreement.Text") %> <a href='<%= IFRMHelper.GetURL("agreement.aspx") %>' class="user-agreement-link" target="_blank" ><u><%= base.GetLocaleResourceString("hlUserAgreement.Text") %></u></a>.</p>
                <script type="text/javascript" language="javascript" >
                    $('.user-agreement-link').popupWindow({ scrollbars: 1 });
                </script>
             </td>
          </tr>
          <tr>
             <td class="title" style="text-align:center" >
                <asp:Button ID="btnSubmit" runat="server" SkinID="SubmitButton" OnClick="btnSubmit_Click" OnClientClick="if(Page_ClientValidate()){this.disabled=true;}" UseSubmitBehavior="false" meta:resourceKey="btnSubmit" />
             </td>
          </tr>
       </table>
    </asp:Panel>
    <script type="text/javascript" language="javascript" >
        var __txt_fn = document.getElementById('<%= txtFirstName.ClientID %>');
        var __txt_ln = document.getElementById('<%= txtLastName.ClientID %>');
        var __txt_ec = document.getElementById('<%= txtEmailConfirm.ClientID %>');

        var __default_fn_text = '<%= base.GetLocaleResourceString("txtFirstName.Text") %>';
        var __default_ln_text = '<%= base.GetLocaleResourceString("txtLastName.Text") %>';
        var __default_ec_text = '<%= base.GetLocaleResourceString("txtEmailConfirm.Text") %>';

        __txt_fn.onfocus = function() {
            if (this.value == __default_fn_text) {
                this.value = '';
                this.style.fontStyle = 'normal';
            }
        }
        __txt_fn.onblur = function() {
            if (this.value == '') {
                this.value = __default_fn_text;
                this.style.fontStyle = 'italic';
            }
        }

        __txt_ln.onfocus = function() {
            if (this.value == __default_ln_text) {
                this.value = '';
                this.style.fontStyle = 'normal';
            }
        }
        __txt_ln.onblur = function() {
            if (this.value == '') {
                this.value = __default_ln_text;
                this.style.fontStyle = 'italic';
            }
        }

        __txt_ec.onfocus = function() {
            if (this.value == __default_ec_text) {
                this.value = '';
                this.style.fontStyle = 'normal';
            }
        }
        __txt_ec.onblur = function() {
            if (this.value == '') {
                this.value = __default_ec_text;
                this.style.fontStyle = 'italic';
            }
        }
    </script>
</asp:Content>
