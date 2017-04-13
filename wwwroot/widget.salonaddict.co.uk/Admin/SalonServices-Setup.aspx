<%@ Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonServices-Setup.aspx.cs" Inherits="IFRAME.Admin.SalonServices_SetupPage" %>
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
              <td width="500px" >
                <asp:UpdatePanel ID="up" runat="server" >
                   <ContentTemplate>
                    <table style="margin-top:-8px;border-collapse:collapse; width:420px;" class="gridview" cellpadding="0" cellspacing="0" border="1" >
                       <tr class="header-style">
                           <th scope="col" ></th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[0].HeaderText") %></th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[1].HeaderText") %></th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[2].HeaderText") %></th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[3].HeaderText") %></th>
		               </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb1" runat="server" Checked="true" Enabled="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName1" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName1" runat="server" ControlToValidate="txtName1" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName1Ex" runat="Server" TargetControlID="valName1" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription1" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice1" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice1" runat="server" ControlToValidate="txtPrice1" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice1Ex" runat="Server" TargetControlID="valPrice1" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice1Regex" runat="server" ControlToValidate="txtPrice1" Display="None" ValidationExpression="\d+(\.\d{1,2})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice1RegexEx" runat="Server" TargetControlID="valPrice1Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength1" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength1" runat="server" ControlToValidate="txtLength1" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength1Ex" runat="Server" TargetControlID="valLength1" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength1Regex" runat="server" ControlToValidate="txtLength1" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength1RegexEx" runat="Server" TargetControlID="valLength1Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb2" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName2" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName2" runat="server" ControlToValidate="txtName2" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName2Ex" runat="Server" TargetControlID="valName2" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription2" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice2" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice2" runat="server" ControlToValidate="txtPrice2" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice2Ex" runat="Server" TargetControlID="valPrice2" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice2Regex" runat="server" ControlToValidate="txtPrice2" Display="None" ValidationExpression="\d+(\.\d{2,2})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice2RegexEx" runat="Server" TargetControlID="valPrice2Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength2" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength2" runat="server" ControlToValidate="txtLength2" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength2Ex" runat="Server" TargetControlID="valLength2" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength2Regex" runat="server" ControlToValidate="txtLength2" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength2RegexEx" runat="Server" TargetControlID="valLength2Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb3" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName3" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName3" runat="server" ControlToValidate="txtName3" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName3Ex" runat="Server" TargetControlID="valName3" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription3" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice3" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice3" runat="server" ControlToValidate="txtPrice3" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice3Ex" runat="Server" TargetControlID="valPrice3" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice3Regex" runat="server" ControlToValidate="txtPrice3" Display="None" ValidationExpression="\d+(\.\d{3,3})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice3RegexEx" runat="Server" TargetControlID="valPrice3Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength3" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength3" runat="server" ControlToValidate="txtLength3" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength3Ex" runat="Server" TargetControlID="valLength3" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength3Regex" runat="server" ControlToValidate="txtLength3" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength3RegexEx" runat="Server" TargetControlID="valLength3Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb4" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName4" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName4" runat="server" ControlToValidate="txtName4" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName4Ex" runat="Server" TargetControlID="valName4" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription4" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice4" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice4" runat="server" ControlToValidate="txtPrice4" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice4Ex" runat="Server" TargetControlID="valPrice4" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice4Regex" runat="server" ControlToValidate="txtPrice4" Display="None" ValidationExpression="\d+(\.\d{4,4})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice4RegexEx" runat="Server" TargetControlID="valPrice4Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength4" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength4" runat="server" ControlToValidate="txtLength4" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength4Ex" runat="Server" TargetControlID="valLength4" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength4Regex" runat="server" ControlToValidate="txtLength4" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength4RegexEx" runat="Server" TargetControlID="valLength4Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb5" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName5" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName5" runat="server" ControlToValidate="txtName5" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName5Ex" runat="Server" TargetControlID="valName5" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription5" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice5" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice5" runat="server" ControlToValidate="txtPrice5" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice5Ex" runat="Server" TargetControlID="valPrice5" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice5Regex" runat="server" ControlToValidate="txtPrice5" Display="None" ValidationExpression="\d+(\.\d{5,5})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice5RegexEx" runat="Server" TargetControlID="valPrice5Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength5" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength5" runat="server" ControlToValidate="txtLength5" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength5Ex" runat="Server" TargetControlID="valLength5" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength5Regex" runat="server" ControlToValidate="txtLength5" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength5RegexEx" runat="Server" TargetControlID="valLength5Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb6" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName6" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName6" runat="server" ControlToValidate="txtName6" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName6Ex" runat="Server" TargetControlID="valName6" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription6" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice6" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice6" runat="server" ControlToValidate="txtPrice6" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice6Ex" runat="Server" TargetControlID="valPrice6" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice6Regex" runat="server" ControlToValidate="txtPrice6" Display="None" ValidationExpression="\d+(\.\d{6,6})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice6RegexEx" runat="Server" TargetControlID="valPrice6Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength6" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength6" runat="server" ControlToValidate="txtLength6" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength6Ex" runat="Server" TargetControlID="valLength6" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength6Regex" runat="server" ControlToValidate="txtLength6" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength6RegexEx" runat="Server" TargetControlID="valLength6Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb7" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName7" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName7" runat="server" ControlToValidate="txtName7" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName7Ex" runat="Server" TargetControlID="valName7" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription7" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice7" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice7" runat="server" ControlToValidate="txtPrice7" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice7Ex" runat="Server" TargetControlID="valPrice7" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice7Regex" runat="server" ControlToValidate="txtPrice7" Display="None" ValidationExpression="\d+(\.\d{7,7})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice7RegexEx" runat="Server" TargetControlID="valPrice7Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength7" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength7" runat="server" ControlToValidate="txtLength7" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength7Ex" runat="Server" TargetControlID="valLength7" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength7Regex" runat="server" ControlToValidate="txtLength7" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength7RegexEx" runat="Server" TargetControlID="valLength7Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb8" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName8" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName8" runat="server" ControlToValidate="txtName8" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName8Ex" runat="Server" TargetControlID="valName8" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription8" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice8" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice8" runat="server" ControlToValidate="txtPrice8" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice8Ex" runat="Server" TargetControlID="valPrice8" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice8Regex" runat="server" ControlToValidate="txtPrice8" Display="None" ValidationExpression="\d+(\.\d{8,8})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice8RegexEx" runat="Server" TargetControlID="valPrice8Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength8" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength8" runat="server" ControlToValidate="txtLength8" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength8Ex" runat="Server" TargetControlID="valLength8" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength8Regex" runat="server" ControlToValidate="txtLength8" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength8RegexEx" runat="Server" TargetControlID="valLength8Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb9" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName9" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName9" runat="server" ControlToValidate="txtName9" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName9Ex" runat="Server" TargetControlID="valName9" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription9" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice9" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice9" runat="server" ControlToValidate="txtPrice9" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice9Ex" runat="Server" TargetControlID="valPrice9" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice9Regex" runat="server" ControlToValidate="txtPrice9" Display="None" ValidationExpression="\d+(\.\d{9,9})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice9RegexEx" runat="Server" TargetControlID="valPrice9Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength9" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength9" runat="server" ControlToValidate="txtLength9" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength9Ex" runat="Server" TargetControlID="valLength9" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength9Regex" runat="server" ControlToValidate="txtLength9" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength9RegexEx" runat="Server" TargetControlID="valLength9Regex" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb10" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName10" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName10" runat="server" ControlToValidate="txtName10" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName10Ex" runat="Server" TargetControlID="valName10" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtDescription10" runat="server" SkinID="TextBox" Width="280px" MaxLength="400" ></asp:TextBox>
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtPrice10" runat="server" SkinID="TextBox" Width="40px" MaxLength="6" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valPrice10" runat="server" ControlToValidate="txtPrice10" Display="None" meta:resourceKey="valPrice" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice10Ex" runat="Server" TargetControlID="valPrice10" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valPrice10Regex" runat="server" ControlToValidate="txtPrice10" Display="None" ValidationExpression="\d+(\.\d{10,10})?" meta:resourceKey="valPriceRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valPrice10RegexEx" runat="Server" TargetControlID="valPrice10Regex" EnableViewState="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;">
                               <asp:TextBox ID="txtLength10" runat="server" SkinID="TextBox" Width="30px" MaxLength="4" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valLength10" runat="server" ControlToValidate="txtLength10" Display="None" meta:resourceKey="valLength" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength10Ex" runat="Server" TargetControlID="valLength10" EnableViewState="false" />
                               <asp:RegularExpressionValidator ID="valLength10Regex" runat="server" ControlToValidate="txtLength10" Display="None" ValidationExpression="[0-9]*" meta:resourceKey="valLengthRegex" ></asp:RegularExpressionValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valLength10RegexEx" runat="Server" TargetControlID="valLength10Regex" EnableViewState="false" />
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