<%@ Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonSchedule-Setup.aspx.cs" Inherits="IFRAME.Admin.SalonSchedule_SetupPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

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
              <td style="vertical-align:top;" width="570px" >
                <asp:UpdatePanel ID="up" runat="server" >
                   <ContentTemplate>
                    <table style="margin-top:-8px;border-collapse:collapse;" class="gridview" cellpadding="0" cellspacing="0" border="1" >
                       <tr class="header-style">
                           <th scope="col" >Day</th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[0].HeaderText") %></th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[1].HeaderText") %></th>
		               </tr>
                        <tr>
                            <td style="padding:5px 10px 5px 10px;" >
                               <%= base.GetLocaleResourceString("ltrMonday.Text") %>
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtMonday" runat="server" SkinID="TextBox" MaxLength="500" Width="320px" ></asp:TextBox>
                               <asp:RegularExpressionValidator ID="valMondayRegex" runat="server" ControlToValidate="txtMonday" Display="None" ValidationExpression="(([0-2])([0-9])(\:)(00|30)(\([1-9]\))( )*)*" meta:resourceKey="valMondayRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valMondayRegexEx" runat="Server" TargetControlID="valMondayRegex" EnableViewState="false" />
                            </td>
                            <td style="padding-top:5px" >
                               <center><asp:Literal ID="ltrMondayHours" runat="server" ></asp:Literal></center>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:5px 10px 5px 10px;" >
                               <%= base.GetLocaleResourceString("ltrTuesday.Text") %>
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtTuesday" runat="server" SkinID="TextBox" MaxLength="500" Width="320px" ></asp:TextBox>
                               <asp:RegularExpressionValidator ID="valTuesdayRegex" runat="server" ControlToValidate="txtTuesday" Display="None" ValidationExpression="(([0-2])([0-9])(\:)(00|30)(\([1-9]\))( )*)*" meta:resourceKey="valTuesdayRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valTuesdayRegexEx" runat="Server" TargetControlID="valTuesdayRegex" EnableViewState="false" />
                            </td>
                            <td style="padding-top:5px" >
                               <center><asp:Literal ID="ltrTuesdayHours" runat="server" ></asp:Literal></center>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:5px 10px 5px 10px;" >
                               <%= base.GetLocaleResourceString("ltrWednesday.Text") %>
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtWednesday" runat="server" SkinID="TextBox" MaxLength="500" Width="320px" ></asp:TextBox>
                               <asp:RegularExpressionValidator ID="valWednesdayRegex" runat="server" ControlToValidate="txtWednesday" Display="None" ValidationExpression="(([0-2])([0-9])(\:)(00|30)(\([1-9]\))( )*)*" meta:resourceKey="valWednesdayRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valWednesdayRegexEx" runat="Server" TargetControlID="valWednesdayRegex" EnableViewState="false" />
                            </td>
                            <td style="padding-top:5px" >
                               <center><asp:Literal ID="ltrWednesdayHours" runat="server" ></asp:Literal></center>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:5px 10px 5px 10px;" >
                               <%= base.GetLocaleResourceString("ltrThursday.Text") %>
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtThursday" runat="server" SkinID="TextBox" MaxLength="500" Width="320px" ></asp:TextBox>
                               <asp:RegularExpressionValidator ID="valThursdayRegex" runat="server" ControlToValidate="txtThursday" Display="None" ValidationExpression="(([0-2])([0-9])(\:)(00|30)(\([1-9]\))( )*)*" meta:resourceKey="valThursdayRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valThursdayRegexEx" runat="Server" TargetControlID="valThursdayRegex" EnableViewState="false" />
                            </td>
                            <td style="padding-top:5px" >
                               <center><asp:Literal ID="ltrThursdayHours" runat="server" ></asp:Literal></center>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:5px 10px 5px 10px;" >
                               <%= base.GetLocaleResourceString("ltrFriday.Text") %>
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtFriday" runat="server" SkinID="TextBox" MaxLength="500" Width="320px" ></asp:TextBox>
                               <asp:RegularExpressionValidator ID="valFridayRegex" runat="server" ControlToValidate="txtFriday" Display="None" ValidationExpression="(([0-2])([0-9])(\:)(00|30)(\([1-9]\))( )*)*" meta:resourceKey="valFridayRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valFridayRegexEx" runat="Server" TargetControlID="valFridayRegex" EnableViewState="false" />
                            </td>
                            <td style="padding-top:5px" >
                               <center><asp:Literal ID="ltrFridayHours" runat="server" ></asp:Literal></center>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:5px 10px 5px 10px;" >
                               <%= base.GetLocaleResourceString("ltrSaturday.Text") %>
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtSaturday" runat="server" SkinID="TextBox" MaxLength="500" Width="320px" ></asp:TextBox>
                               <asp:RegularExpressionValidator ID="valSaturdayRegex" runat="server" ControlToValidate="txtSaturday" Display="None" ValidationExpression="(([0-2])([0-9])(\:)(00|30)(\([1-9]\))( )*)*" meta:resourceKey="valSaturdayRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valSaturdayRegexEx" runat="Server" TargetControlID="valSaturdayRegex" EnableViewState="false" />
                            </td>
                            <td style="padding-top:5px" >
                               <center><asp:Literal ID="ltrSaturdayHours" runat="server" ></asp:Literal></center>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:5px 10px 5px 10px;" >
                               <%= base.GetLocaleResourceString("ltrSunday.Text") %>
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtSunday" runat="server" SkinID="TextBox" MaxLength="500" Width="320px" ></asp:TextBox>
                               <asp:RegularExpressionValidator ID="valSundayRegex" runat="server" ControlToValidate="txtSunday" Display="None" ValidationExpression="(([0-2])([0-9])(\:)(00|30)(\([1-9]\))( )*)*" meta:resourceKey="valSundayRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valSundayRegexEx" runat="Server" TargetControlID="valSundayRegex" EnableViewState="false" />
                            </td>
                            <td style="padding-top:5px" >
                               <center><asp:Literal ID="ltrSundayHours" runat="server" ></asp:Literal></center>
                            </td>
                        </tr>
                    </table>
                   </ContentTemplate>
                </asp:UpdatePanel>
                <table style="margin-top:10px" class="details" runat="server" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td class="title" >
                            <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                            <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
                        </td>
                    </tr>
                </table>
              </td>
              <td style="vertical-align:top;" >
                  <%= base.GetLocaleResourceString("ltrHelp.Text") %>
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>