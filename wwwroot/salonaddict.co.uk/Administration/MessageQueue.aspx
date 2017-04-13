<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="MessageQueue.aspx.cs" Inherits="SalonAddict.Administration.MessageQueue" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-system.png" alt="System" />
        Message Queue
    </div>
    <div class="options">
        <asp:Button ID="btnLoad" runat="server" Text="Load" ToolTip="Load messages in queue" onclick="btnLoad_Click" CausesValidation="true" />
        <% if(Roles.IsUserInRole("SYS_ADMIN"))
           { %>
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new Message" OnClientClick="location.href='MessageQueueCreate.aspx';return false" />
        <% } %>
    </div>
</div>
<br />
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblStartDate"
                runat="server"  
                Text="Start date:" 
                IsRequired="true"
                ToolTip="The start date for the search in Coordinated Universal Time (UTC)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox runat="server" ID="txtStartDate" MaxLength="10" />
            <asp:ImageButton runat="Server" ID="iStartDate" ImageUrl="~/Administration/images/ico-calendar.png" AlternateText="Click to show calendar" /><br />
            <ajaxToolkit:CalendarExtender ID="cStartDateButtonExtender" runat="server" TargetControlID="txtStartDate" PopupButtonID="iStartDate" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblEndDate"
                runat="server"  
                Text="End date:" 
                IsRequired="true"
                ToolTip="The end date for the search in Coordinated Universal Time (UTC)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox runat="server" ID="txtEndDate" MaxLength="10" />
            <asp:ImageButton runat="Server" ID="iEndDate" ImageUrl="~/Administration/images/ico-calendar.png" AlternateText="Click to show calendar" /><br />
            <ajaxToolkit:CalendarExtender ID="cEndDateButtonExtender" runat="server" TargetControlID="txtEndDate" PopupButtonID="iEndDate" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblFromEmail"
                runat="server"  
                Text="From address:" 
                ToolTip="From address."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtFromEmail" runat="server" MaxLength="100" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblToEmail"
                runat="server"  
                Text="To address:" 
                ToolTip="To address."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtToEmail" runat="server" MaxLength="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblNotSentItemsOnly"
                runat="server"  
                Text="Load not sent items only:" 
                ToolTip="Only load emails into queue that have been sent."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbNotSentItemsOnly" runat="server" Checked="true" ></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblMaxSendTries"
                runat="server"  
                Text="Maximum send attempts:" 
                ToolTip="The maximum number of attempts to send a message."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox 
                ID="txtMaxSendTries" 
                runat="server" 
                RequiredErrorMessage="Enter maximum send tries" 
                MinimumValue="0" 
                MaximumValue="999999"
                Value="10" 
                RangeErrorMessage="The value must be from 0 to 999999">
             </SA:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblGoDirectlyToEmailNumber"
                runat="server"  
                Text="Go directly to email #:" 
                ToolTip="Go directly to email #"
                IsRequired="true"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox 
                ID="txtEmailID" 
                runat="server" 
                RequiredErrorMessage="Email number is a required field." 
                MinimumValue="0" 
                MaximumValue="999999"
                ValidationGroup="GoDirectly" 
                RangeErrorMessage="The value must be from 0 to 999999">
             </SA:NumericTextBox>
            <asp:Button runat="server" Text="Go" ID="btnGoDirectlyToEmailNumber" OnClick="btnGoDirectlyToEmailNumber_Click" ValidationGroup="GoDirectly" ToolTip="Go directly to email #" />
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gvMessageQueue" 
    runat="server" 
    Width="100%"
    PageSize="15"
    AllowPaging="true"
    DataKeyNames="QueuedEmailID"
    OnPageIndexChanging="gvMessageQueue_OnPageIndexChanging"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Queued Email ID" >
            <ItemTemplate>
                <%# Eval("QueuedEmailID") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Priority">
            <ItemTemplate>
               <%# Eval("Priority") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="From" >
            <ItemTemplate>
               <%# Eval("From") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="To" >
            <ItemTemplate>
                <%# Eval("To") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="View" >
            <ItemTemplate>
               <a href="MessageQueueDetails.aspx?QueuedEmailID=<%# Eval("QueuedEmailID") %>" >view</a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created on" >
            <ItemTemplate>
               <%# Eval("CreatedOn") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Sent on" >
            <ItemTemplate>
               <%# (Convert.ToDateTime(Eval("SentOn").ToString()) == DateTime.MinValue) ? String.Empty : Eval("SentOn") %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
