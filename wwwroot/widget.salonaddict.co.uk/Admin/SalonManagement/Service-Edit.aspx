<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Service-Edit.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.Service_EditPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Services" />
        <div class="horizontal-line" ></div>
        <table style="margin-bottom:20px" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:60px" ><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-services.png" %>' alt="services" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle;text-align:right;" >
                 <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                 <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
              </td>
           </tr>
        </table>
        <ajaxToolkit:TabContainer ID="pnlTabs" runat="server" AutoPostBack="false" >
           <ajaxToolkit:TabPanel ID="pnlInfo" runat="server" HeaderText="General" >
             <ContentTemplate>
                <table cellpadding="0" cellspacing="0" >
                   <tr>
                      <td style="width:500px" >
                        <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
                           <tr>
                              <td class="title" ><%= base.GetLocaleResourceString("ltrName.Text") %></td>
                              <td class="data-item" >
                                 <asp:TextBox ID="txtName" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                                 <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                                 <ajaxToolkit:ValidatorCalloutExtender ID="valNameEx" runat="server" TargetControlID="valName" EnableViewState="false" />
                              </td>
                           </tr>
                           <tr>
                              <td class="title" ><%= base.GetLocaleResourceString("ltrCategory1.Text") %></td>
                              <td class="data-item" >
                                  <asp:DropDownList ID="ddlCategory1" runat="server" SkinID="DropDownList" Width="280px" ></asp:DropDownList>
                                 <asp:RequiredFieldValidator ID="valCategory1" runat="server" ControlToValidate="ddlCategory1" Display="None" meta:resourceKey="valCategory" ></asp:RequiredFieldValidator>
                                 <ajaxToolkit:ValidatorCalloutExtender ID="valCategory1Ex" runat="server" TargetControlID="valCategory1" EnableViewState="false" />
                              </td>
                           </tr>
                           <tr>
                              <td class="title" ><%= base.GetLocaleResourceString("ltrDescripton.Text") %></td>
                              <td class="data-item" >
                                 <asp:TextBox ID="txtDescription" runat="server" SkinID="TextBox" MaxLength="200" ></asp:TextBox>
                              </td>
                           </tr>
                           <tr>
                              <td class="title" ><%= base.GetLocaleResourceString("ltrPrice.Text") %></td>
                              <td class="data-item" >
                                 <asp:TextBox ID="txtPrice" runat="server" SkinID="TextBox" MaxLength="7" ></asp:TextBox>
                                 <asp:RequiredFieldValidator ID="valPrice" runat="server" ControlToValidate="txtPrice" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                                 <ajaxToolkit:ValidatorCalloutExtender ID="valPriceEx" runat="Server" TargetControlID="valPrice" EnableViewState="false" />
                                 <asp:RegularExpressionValidator ID="valPriceRegEx" runat="server" ControlToValidate="txtPrice" Display="None" ValidationExpression="\d+(\.\d{1,2})?" meta:resourceKey="valPriceRegEx"></asp:RegularExpressionValidator>
                                 <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="valPriceRegExEx" TargetControlID="valPriceRegEx" EnableViewState="false" />
                              </td>
                           </tr>
                           <tr>
                              <td class="title" ><%= base.GetLocaleResourceString("ltrDuration.Text") %></td>
                              <td class="data-item" >
                                   <asp:TextBox ID="txtLength" runat="server" SkinID="TextBox" Width="40px" MaxLength="4" ></asp:TextBox>
                                   <asp:RequiredFieldValidator ID="valLength" runat="server" ControlToValidate="txtLength" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                                   <ajaxToolkit:ValidatorCalloutExtender ID="valLengthEx" runat="Server" TargetControlID="valLength" EnableViewState="false" />
                                   <asp:RegularExpressionValidator ID="valLengthRegex" runat="server" ControlToValidate="txtLength" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                                   <ajaxToolkit:ValidatorCalloutExtender ID="valLengthRegexEx" runat="Server" TargetControlID="valLengthRegex" EnableViewState="false" />
                              </td>
                           </tr>
                           <tr>
                              <td class="title" ><%= base.GetLocaleResourceString("ltrActive.Text") %></td>
                              <td class="data-item" >
                                 <asp:CheckBox ID="cbActive" runat="server" />
                              </td>
                           </tr>
                        </table>
                      </td>
                      <td>
                         <%= base.GetLocaleResourceString("ltrHelp.Text") %>
                      </td>
                  </tr>
                </table>
             </ContentTemplate>
           </ajaxToolkit:TabPanel>
           <ajaxToolkit:TabPanel ID="pnlOther" runat="server" HeaderText="Other" >
               <ContentTemplate>
                   <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
                      <tr>
                          <td class="title" ><%= base.GetLocaleResourceString("ltrCategory2.Text") %></td>
                          <td class="data-item" >
                              <asp:DropDownList ID="ddlCategory2" runat="server" SkinID="DropDownList" Width="280px" ></asp:DropDownList>
                          </td>
                       </tr>
                       <tr>
                          <td class="title" ><%= base.GetLocaleResourceString("ltrCategory3.Text") %></td>
                          <td class="data-item" >
                              <asp:DropDownList ID="ddlCategory3" runat="server" SkinID="DropDownList" Width="280px" ></asp:DropDownList>
                          </td>
                       </tr>
                    </table>
               </ContentTemplate>
           </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
     </asp:Panel>
</asp:Content>
