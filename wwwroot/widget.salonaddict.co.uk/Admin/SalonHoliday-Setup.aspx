<%@ Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonHoliday-Setup.aspx.cs" Inherits="IFRAME.Admin.SalonHoliday_SetupPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="DateTextBox" Src="~/Modules/DateTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
       <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><asp:Literal ID="ltrHeader" runat="server" ></asp:Literal></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td width="500px" >
                <asp:UpdatePanel ID="up" runat="server" >
                   <ContentTemplate>
                    <table style="margin-top:-8px;border-collapse:collapse; width:420px;" class="gridview" cellpadding="0" cellspacing="0" border="1" >
                       <tr class="header-style">
                           <th scope="col" ></th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[0].HeaderText") %></th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[1].HeaderText") %></th>
		               </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb1" runat="server" Checked="true" Enabled="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate1" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate1" runat="server" ControlToValidate="txtDate1" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate1Ex" runat="Server" TargetControlID="valDate1" EnableViewState="false" />
                            </td>
                            <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription1" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription1" runat="server" ControlToValidate="txtDescription1" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription1Ex" runat="Server" TargetControlID="valDescription1" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb2" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate2" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate2" runat="server" ControlToValidate="txtDate2" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate2Ex" runat="Server" TargetControlID="valDate2" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription2" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription2" runat="server" ControlToValidate="txtDescription2" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription2Ex" runat="Server" TargetControlID="valDescription2" EnableViewState="false" />
                           </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb3" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate3" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate3" runat="server" ControlToValidate="txtDate3" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate3Ex" runat="Server" TargetControlID="valDate3" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription3" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription3" runat="server" ControlToValidate="txtDescription3" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription3Ex" runat="Server" TargetControlID="valDescription3" EnableViewState="false" />
                           </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb4" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate4" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate4" runat="server" ControlToValidate="txtDate4" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate4Ex" runat="Server" TargetControlID="valDate4" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription4" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription4" runat="server" ControlToValidate="txtDescription4" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription4Ex" runat="Server" TargetControlID="valDescription4" EnableViewState="false" />
                           </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb5" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate5" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate5" runat="server" ControlToValidate="txtDate5" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate5Ex" runat="Server" TargetControlID="valDate5" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription5" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription5" runat="server" ControlToValidate="txtDescription5" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription5Ex" runat="Server" TargetControlID="valDescription5" EnableViewState="false" />
                           </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb6" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate6" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate6" runat="server" ControlToValidate="txtDate6" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate6Ex" runat="Server" TargetControlID="valDate6" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription6" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription6" runat="server" ControlToValidate="txtDescription6" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription6Ex" runat="Server" TargetControlID="valDescription6" EnableViewState="false" />
                           </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb7" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate7" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate7" runat="server" ControlToValidate="txtDate7" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate7Ex" runat="Server" TargetControlID="valDate7" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription7" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription7" runat="server" ControlToValidate="txtDescription7" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription7Ex" runat="Server" TargetControlID="valDescription7" EnableViewState="false" />
                           </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb8" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate8" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate8" runat="server" ControlToValidate="txtDate8" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate8Ex" runat="Server" TargetControlID="valDate8" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription8" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription8" runat="server" ControlToValidate="txtDescription8" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription8Ex" runat="Server" TargetControlID="valDescription8" EnableViewState="false" />
                           </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb9" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate9" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate9" runat="server" ControlToValidate="txtDate9" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate9Ex" runat="Server" TargetControlID="valDate9" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription9" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription9" runat="server" ControlToValidate="txtDescription9" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription9Ex" runat="Server" TargetControlID="valDescription9" EnableViewState="false" />
                           </td>
                        </tr>
                        <tr>
                           <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb10" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                           </td>
                           <td style="padding:5px 10px 5px 10px;" >
                               <center><IFRM:DateTextBox ID="txtDate10" runat="server" /></center>
                               <asp:RequiredFieldValidator ID="valDate10" runat="server" ControlToValidate="txtDate10" Display="None" meta:resourceKey="valDate" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDate10Ex" runat="Server" TargetControlID="valDate10" EnableViewState="false" />
                           </td>
                           <td style="padding:8px 10px 5px 10px;">
                               <center><asp:TextBox ID="txtDescription10" runat="server" SkinID="TextBox" MaxLength="50" /></center>
                               <asp:RequiredFieldValidator ID="valDescription10" runat="server" ControlToValidate="txtDescription10" Display="None" meta:resourceKey="valDescription" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valDescription10Ex" runat="Server" TargetControlID="valDescription10" EnableViewState="false" />
                           </td>
                        </tr>
                    </table>
                   </ContentTemplate>
                </asp:UpdatePanel>
                <table id="Table1" style="margin-top:10px" class="details" runat="server" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td class="title" >
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