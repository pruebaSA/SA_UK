<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ProfileRenewal.aspx.cs" Inherits="SalonAddict.Administration.ProfileRenewal" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DateTimeTextBox" Src="~/Administration/Modules/TextBoxes/DateTimeTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="ProcessPayment" Src="~/Administration/Modules/ProcessPayment.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section-header">
        <div class="title">
            <img src="images/ico-partner.png" alt="" />
            Process Payment <a href="ProfileRenewals.aspx" title="Back to profiles list"> (back to profiles list)</a>
        </div>
        <div class="options">
            <asp:Button ID="Renew" runat="server" Text="Renew" OnClick="Renew_Click" ToolTip="Renew without payment." />
        </div>
    </div>
    <SA:Message ID="Message" runat="server" IsError="true" />
    <table class="details" cellpadding="0" cellspacing="0" >
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
               <asp:Label ID="txtName" runat="server" ></asp:Label>
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
        <tr>
           <td class="title" >
                <SA:ToolTipLabel 
                    ID="lblPayment" 
                    runat="server" 
                    Text="Process Payment:"
                    IsRequired="true"
                    ToolTip="Process Payment." 
                    ToolTipImage="~/Administration/images/ico-help.gif" />
           </td>
           <td class="data-item" >
                <asp:CheckBox ID="cbPayment" runat="server" Checked="true" OnCheckedChanged="cbPayment_CheckedChanged" AutoPostBack="true" />
           </td>
        </tr>
    </table>
    <asp:Panel ID="pnlPayment" runat="server" Visible="true" >
        <SA:ProcessPayment ID="Payment" runat="server" />
    </asp:Panel>
</asp:Content>
