<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SMSProviderDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.SMSProviderDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Add a new SMS provider <a href="SMSProviders.aspx" title="Back to SMS providers list"> (back to SMS provider list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save SMS provider" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit SMS provider details <a href="SMSProviders.aspx" title="Back to SMS provider list">(back to SMS provider list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save SMS provider" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete SMS provider" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<% } %>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server"
                Text="Name:" 
                IsRequired="true"
                ToolTip="The SMS provider name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox  
                ID="txtName"  
                runat="server"
                ErrorMessage="Name is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblClassName"
                runat="server"  
                Text="Class name:" 
                IsRequired="true"
                ToolTip="The fully qualified class name for this SMS provider."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtClassName"
                runat="server"
                ErrorMessage="Class name is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSystemKeyword"
                runat="server"  
                Text="System Keyword:" 
                IsRequired="true"
                ToolTip="The system name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtSystemKeyword"
                runat="server"
                ErrorMessage="System keyword is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblActive"
                runat="server"  
                Text="Active:" 
                ToolTip="Value indicating whether the provider is active."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbActive" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server"
                Text="Display order:"
                ToolTip="The display order of this payment provider. 1 represents the top of the list."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox 
                ID="txtDisplayOrder" 
                runat="server"
                Value="1" 
                RequiredErrorMessage="Display order is required" 
                MinimumValue="1"
                MaximumValue="99999"
                RangeErrorMessage="The value must be from 1 to 99999">
            </SA:NumericTextBox>
        </td>
    </tr>
</table>
<br /><br />
<% if (this.Action == ActionType.Edit)
   { %>
     <p><b>Send Test SMS (save settings first by clicking "Save" button)</b></p>
     <table class="details" cellpadding="0" cellspacing="0" >
        <tr>
            <td class="title">
                <SA:ToolTipLabel 
                    ID="lblPhoneNumber"
                    runat="server"  
                    Text="Send SMS to:" 
                    ToolTip="Phone number to send the test SMS to"
                    ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td style="width:40px;" class="data-item" >
                <asp:DropDownList ID="ddlAreaCode" runat="server" >
                   <asp:ListItem Text="--" Value="" ></asp:ListItem>
                   <asp:ListItem Text="074" Value="074" ></asp:ListItem>
                   <asp:ListItem Text="075" Value="075" ></asp:ListItem>
                   <asp:ListItem Text="077" Value="077" ></asp:ListItem>
                   <asp:ListItem Text="078" Value="078" ></asp:ListItem>
                   <asp:ListItem Text="079" Value="079" ></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvAreaCode" runat="server" ControlToValidate="ddlAreaCode" ErrorMessage="Area code is a required field." Display="None" ValidationGroup="SMS" ></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvAreaCodeExtender" TargetControlID="rfvAreaCode" HighlightCssClass="validator-highlight" />
            </td>
            <td class="data-item" >
                <SA:TextBox 
                    ID="txtPhoneNumber"
                    runat="server"
                    Width="120px"
                    MaxLength="8"
                    ValidationGroup="SMS"
                    ValidationExpression="\d{8}"
                    ValidationMessage="Invalid phone number"
                    ErrorMessage="Phone number is a required field.">
                </SA:TextBox>
            </td>
        </tr>
        <tr>
            <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblMessage"
                    runat="server"  
                    Text="Message:" 
                    ToolTip="Body of the SMS"
                    ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" colspan="2" >
                <SA:TextBox 
                    ID="txtMessage"
                    runat="server"
                    Width="280px"
                    MaxLength="400"
                    ValidationGroup="SMS"
                    ErrorMessage="Message is a required field.">
                </SA:TextBox>
            </td>
        </tr>
        <tr>
            <td class="title" ></td>
            <td class="data-item" colspan="2" >
                <asp:Button ID="btnSendSMS" runat="server" Text="Send Test SMS" ValidationGroup="SMS" OnClick="btnSendSMS_Click" />
            </td>
        </tr>
        <tr>
           <td class="title" ></td>
           <td class="data-item" colspan="2" >
                  <asp:Label ID="lblError" runat="server" EnableViewState="false" Font-Bold="true" ForeColor="Red" ></asp:Label>
           </td>
        </tr>
     </table>
<% } %>