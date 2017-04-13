<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReviewSessionDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.ReviewSessionDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Administration/Modules/TextBoxes/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DateTimeTextBox" Src="~/Administration/Modules/TextBoxes/DateTimeTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="LanguageDropDownList" Src="~/Administration/Modules/DropDownLists/LanguageDropDownList.ascx" %>

<div class="section-header">
<% if (this.Action == ActionType.Edit)
   { %>
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit review session details <a href="ReviewSessions.aspx" title="Back to session list">(back to session list)</a>
    </div>
    <div class="options">
        <asp:Button ID="RequeueButton" runat="server" Text="Requeue" OnClick="RequeueButton_Click" CausesValidation="false" ToolTip="Requeue the message" />
        <% if(Roles.IsUserInRole("SYS_ADMIN"))
           { %>
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save message" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete message" OnClientClick="return confirm('Are you sure?')" />
        <% } %>
    </div>
<% } 
   else
   {%>
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Add a new session <a href="ReviewSessions.aspx" title="Back to session list">(back to session list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Add session" />
    </div>
<% } %>
</div>
<table class="details">
 <tr>
    <td class="title">
        <SA:ToolTipLabel 
            ID="lblTopic" 
            runat="server" 
            Text="Topic:"
            ToolTip="Session topic." 
            ToolTipImage="~/Administration/images/ico-help.gif" />
    </td>
    <td class="data-item">
        <asp:TextBox ID="txtTopic" runat="server" MaxLength="50" ></asp:TextBox>
    </td>
 </tr>
 <tr>
    <td class="title">
        <SA:ToolTipLabel 
            ID="lblAppointmentID" 
            runat="server" 
            Text="Appointment ID:"
            ToolTip="The appointment identifier" 
            IsRequired="true"
            ToolTipImage="~/Administration/images/ico-help.gif" />
    </td>
    <td class="data-item">
        <SA:NumericTextBox 
            ID="txtAppointmentID" 
            runat="server" 
            RequiredErrorMessage="Appointment identifier is a required field"
            MinimumValue="0" 
            MaximumValue="999999999" 
            RangeErrorMessage="The value must be from 0 to 999999999" >
        </SA:NumericTextBox>
    </td>
</tr>
 <tr>
    <td class="title">
        <SA:ToolTipLabel 
            ID="lblCustomerID" 
            runat="server" 
            Text="Customer ID:"
            ToolTip="The customer identifier" 
            IsRequired="false"
            ToolTipImage="~/Administration/images/ico-help.gif" />
    </td>
    <td class="data-item">
        <SA:NumericTextBox 
            ID="txtCustomerID" 
            runat="server" 
            MinimumValue="0" 
            MaximumValue="999999999" 
            IsRequired="false"
            RangeErrorMessage="The value must be from 0 to 999999999" >
        </SA:NumericTextBox>
    </td>
</tr>
<tr>
    <td class="title">
        <SA:ToolTipLabel 
            ID="lblLanguage" 
            runat="server" 
            Text="Language:" 
            IsRequired="true"
            ToolTip="The customer language"
            ToolTipImage="~/Administration/images/ico-help.gif" />
    </td>
    <td class="data-item">
        <SA:LanguageDropDownList ID="ddlLanguages" runat="server" IsRequired="true" ErrorMessage="Language is a required field" />
    </td>
</tr>
 <tr>
    <td class="title">
        <SA:ToolTipLabel 
            ID="lblBusinessiD" 
            runat="server" 
            Text="Business ID:"
            ToolTip="The business identifier" 
            IsRequired="true"
            ToolTipImage="~/Administration/images/ico-help.gif" />
    </td>
    <td class="data-item">
        <SA:NumericTextBox 
            ID="txtBusinessID" 
            runat="server" 
            IsRequired="false"
            MinimumValue="0" 
            MaximumValue="999999999" 
            RangeErrorMessage="The value must be from 0 to 999999999" >
        </SA:NumericTextBox>
    </td>
</tr>
 <tr>
    <td class="title">
        <SA:ToolTipLabel 
            ID="lblStaffID" 
            runat="server" 
            Text="Staff ID:"
            ToolTip="The staff identifier" 
            IsRequired="true"
            ToolTipImage="~/Administration/images/ico-help.gif" />
    </td>
    <td class="data-item">
        <SA:NumericTextBox 
            ID="txtStaffID" 
            runat="server" 
            IsRequired="false"
            MinimumValue="0" 
            MaximumValue="999999999" 
            RangeErrorMessage="The value must be from 0 to 999999999" >
        </SA:NumericTextBox>
    </td>
</tr>
<tr>
    <td class="title">
        <SA:ToolTipLabel 
            ID="lblEmail" 
            runat="server" 
            Text="Email address:"
            IsRequired="true"
            ToolTip="The user's email address." 
            ToolTipImage="~/Administration/images/ico-help.gif" />
    </td>
    <td class="data-item">
        <SA:EmailTextBox 
            ID="txtEmail" 
            runat="server" 
            IsRequired="true"  />
    </td>
</tr>
    <% if(this.Action == ActionType.Edit)
       { %>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSendTries" 
                runat="server" 
                Text="Send attempts" 
                ToolTip="The number of times to attempt to send this message."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox 
                ID="txtSendTries"
                runat="server" 
                RequiredErrorMessage="Enter send tries" 
                MinimumValue="0" 
                MaximumValue="999999"
                Value="0" 
                RangeErrorMessage="The value must be from 0 to 999999">
            </SA:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSendOn" 
                runat="server" 
                Text="Sent on:" 
                ToolTip="The date/time message was sent."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrSentOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <% } %>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblActiveOn" 
                runat="server"
                Text="Active on:"
                ToolTip="Date/Time the session becomes active." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:DateTimeTextBox ID="txtActiveOn" runat="server" IsRequired="true" ErrorMessage="Active on is a required field." />
        </td>
    </tr>
   <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblActive" 
                runat="server"
                IsRequired="true"
                Text="Active:"
                ToolTip="Determines whether a session is active." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbActive" runat="server" Checked="true" />
        </td>
    </tr>
    <% if(this.Action == ActionType.Edit)
       { %>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCreatedOn" 
                runat="server"
                Text="Created on:"
                ToolTip="Date/Time session added to queue." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblUpdatedOn" 
                runat="server"
                Text="Updated on:"
                ToolTip="Date/Time session was last updated." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrUpdatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <% } %>
</table>