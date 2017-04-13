<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ReviewSessions.aspx.cs" Inherits="SalonAddict.Administration.ReviewSessions" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-marketing.gif" alt="Marketing" />
        Manage Review Sessions
    </div>
    <div class="options">
       <asp:Button ID="btnLoad" runat="server" Text="Load" OnClick="btnLoad_Click" />
       <% if(Roles.IsUserInRole("SYS_ADMIN"))
           { %>
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new Session" OnClientClick="location.href='ReviewSessionCreate.aspx';return false" />
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
                ID="lblNotSentItemsOnly"
                runat="server"  
                Text="Load not sent items only:" 
                ToolTip="Only load SMS into queue that have been sent."
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
                ID="lblGoDirectlyToSession"
                runat="server"  
                Text="Go directly to Session #:" 
                ToolTip="Go directly to Session #"
                IsRequired="true"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtSessionGUID" 
                runat="server" 
                RequiredErrorMessage="Session number is a required field." 
                ValidationGroup="GoDirectly" >
             </SA:TextBox>
            <asp:Button runat="server" Text="Go" ID="btnGoDirectlyToSession" OnClick="btnGoDirectlyToSession_Click" ValidationGroup="GoDirectly" ToolTip="Go directly to Session GUID" />
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    DataKeyNames="ReviewSessionID, ReviewSessionGUID"
    AutoGenerateColumns="False" 
    PageIndex="0"
    PageSize="10"
    AllowPaging="true"
    OnPageIndexChanging="gv_PageIndexChanging" 
    Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="Reference No." >
            <ItemTemplate>
                <%# Eval("ReferenceNo") %>
            </ItemTemplate>
            <ItemStyle Width="100px" HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Customer" >
            <ItemTemplate>
                <%# Eval("BillingFirstName") %> <%# Eval("BillingLastName") %>
            </ItemTemplate>
            <ItemStyle Width="160px" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Business" >
            <ItemTemplate>
                <%# Eval("BusinessName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Service" >
            <ItemTemplate>
               <%# Eval("ServiceName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Appointment Time" >
            <ItemTemplate>
                 <%# ((DateTime)Eval("Time.StartsOn")).ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f").ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Sent On" >
            <ItemTemplate>
                 <%# ((DateTime)Eval("SentOn") == DateTime.MinValue) ? String.Empty : ((DateTime)Eval("SentOn")).ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f").ToString()%>
            </ItemTemplate>
            <ItemStyle Width="110px" HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Send Tries" >
            <ItemTemplate>
                <%# Eval("SendTries") %>
            </ItemTemplate>
            <ItemStyle Width="80px" HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Active" >
            <ItemTemplate>
                <%# Eval("Active") %>
            </ItemTemplate>
            <ItemStyle Width="80px" HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created on" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%# ((DateTime)Eval("CreatedOn")).ToFriendlyTimeFormat(TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).ToString("f").ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="View" >
            <ItemTemplate>
               <a href="ReviewSessionDetails.aspx?ReviewSessionGUID=<%# Eval("ReviewSessionGUID") %>" >view</a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
