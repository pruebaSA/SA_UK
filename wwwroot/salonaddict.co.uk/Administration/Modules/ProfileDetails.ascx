<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.ProfileDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/Administration/Modules/TextBoxes/DecimalTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DateTimeTextBox" Src="~/Administration/Modules/TextBoxes/DateTimeTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="Businesses" Src="~/Administration/Modules/DropDownLists/BusinessDropDownList.ascx" %>

<style type="text/css" >
    .banner { position:relative; border:solid 1px #999; padding:8px; margin-bottom:15px; }
</style>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-partner.png" alt="" />
        Add a new profile <a href="Profiles.aspx" title="Back to profiles list"> (back to profiles list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save profile" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-partner.png" alt="" />
        Edit profile details <a href="Profiles.aspx" title="Back to profiles list">(back to profiles list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save profile" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete profile" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<% } %>
<% if (this.Action == ActionType.Edit)
   { %>
   <table class="banner" >
      <tr>
         <td><asp:Image ID="Banner" runat="server" /></td>
      </tr>
   </table>
<% } %>
<asp:UpdatePanel ID="up" runat="server" >
  <ContentTemplate>
    <table class="details">
        <tr>
           <td colspan="3" >
               <SA:Message ID="Message" runat="server" IsError="true" />
           </td>
        </tr>
        <tr>
            <td class="title">
                <SA:ToolTipLabel 
                    ID="lblName" 
                    runat="server" 
                    Text="Name:"
                    IsRequired="true"
                    ToolTip="Profile name." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item">
                <SA:TextBox ID="txtName" runat="server" MaxLength="100" ErrorMessage="Profile name is a required field." />
            </td>
            <td class="data-item" >
                <asp:Button ID="btnGenerate" runat="server" Text="Generate Profile URL" CausesValidation="false" OnClick="btnGenerate_Click" />
            </td>
        </tr>
        <tr>
            <td class="title" ></td>
            <td class="data-item" colspan="2" >
                <asp:Label ID="lblURL" runat="server" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="title">
                <SA:ToolTipLabel 
                    ID="lblBanner" 
                    runat="server" 
                    Text="Banner:"
                    IsRequired="true"
                    ToolTip="Banner (80px x 950px)." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" colspan="2" >
                <asp:FileUpload ID="fu" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="title" >
               <SA:ToolTipLabel 
                    ID="lblBusiness" 
                    runat="server" 
                    Text="Business:"
                    IsRequired="true"
                    ToolTip="Business." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" colspan="2" >
                <SA:Businesses ID="ddlBusinesses" runat="server" DefaultText="Choose a Business" DefaultValue="" ErrorMessage="Business is a required field." />
            </td>
        </tr>
        <tr>
            <td class="title" >
               <SA:ToolTipLabel 
                    ID="lblTheme" 
                    runat="server" 
                    Text="Theme:"
                    IsRequired="true"
                    ToolTip="Profile theme." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" colspan="2" >
                <asp:DropDownList ID="ddlTheme" runat="server" Width="180px" >
                   <asp:ListItem Text="Default" Value="Default" ></asp:ListItem>
                   <asp:ListItem Text="White" Value="White" ></asp:ListItem>
                   <asp:ListItem Text="Black" Value="Black" ></asp:ListItem>
                   <asp:ListItem Text="Blue" Value="Blue" ></asp:ListItem>
                   <asp:ListItem Text="Green" Value="Green" ></asp:ListItem>
                   <asp:ListItem Text="Red" Value="Red" ></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="title" style="vertical-align:top;" >
               <SA:ToolTipLabel 
                    ID="lblActiveOn" 
                    runat="server" 
                    Text="Billing Cycle:"
                    IsRequired="true"
                    ToolTip="Determines the period in which the profile is active." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" colspan="2" >
               <table cellpadding="0" cellspacing="0" >
                  <tr>
                     <td><SA:DateTimeTextBox ID="txtActiveOn" runat="server" ErrorMessage="Required!" /></td>
                     <td>&nbsp;&nbsp; <i>(starts)</i></td>
                  </tr>
                  <tr>
                     <td><SA:DateTimeTextBox ID="txtExpiresOn" runat="server" IsRequired="false" /></td>
                     <td>&nbsp;&nbsp; <i>(ends)</i></td>
                  </tr>
               </table>
            </td>
        </tr>
        <% if(this.Action == ActionType.Edit)
           { %>
        <tr>
            <td class="title" >
               <SA:ToolTipLabel 
                    ID="lblActive" 
                    runat="server" 
                    Text="Active:"
                    IsRequired="true"
                    ToolTip="Indicates whether this profile is active." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
            </td>
            <td class="data-item" colspan="2" >
                <asp:CheckBox ID="cbActive" runat="server" />
            </td>
        </tr>
        <% } %>
    </table>
  </ContentTemplate>
</asp:UpdatePanel>