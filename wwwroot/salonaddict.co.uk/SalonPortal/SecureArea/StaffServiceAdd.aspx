<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="StaffServiceAdd.aspx.cs" Inherits="SalonPortal.SecureArea.StaffServiceAdd" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/SecureArea/Modules/DecimalTextBox.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
    <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-staff.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" CausesValidation="false" meta:resourceKey="btnCancel" />
        </div>
    </div>
    <br />
    <ajaxToolkit:TabContainer runat="server" ID="StaffTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel ID="pnlServices" runat="server" >
           <ContentTemplate>
                <table class="details">
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblStaffDisplayName" 
                                runat="server"  
                                meta:resourceKey="lblStaffDisplayName"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:Literal ID="txtStaffDisplayName" runat="server" ></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblAdditionalFee" 
                                runat="server"  
                                meta:resourceKey="lblAdditionalFee"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <SA:DecimalTextBox 
                                ID="txtAdditionalFee" 
                                runat="server" 
                                MinimumValue="0"
                                MaximumValue="999"
                                Value="0.00"
                                meta:resourceKey="txtAdditionalFee" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDuration" 
                                runat="server"  
                                meta:resourceKey="lblDuration"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:DropDownList ID="ddlDuration" Width="50px" runat="server" ></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblService" 
                                runat="server"  
                                meta:resourceKey="lblService"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:DropDownList ID="ddlServices" runat="server" ></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvServices" runat="server" ControlToValidate="ddlServices" Display="None" meta:resourceKey="rfvServices" ></asp:RequiredFieldValidator>
                            <ajaxToolkit:ValidatorCalloutExtender ID="rfvServicesE" runat="Server" TargetControlID="rfvServices" HighlightCssClass="validator-highlight" />
                        </td>
                    </tr>
               </table>
           </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Content>
