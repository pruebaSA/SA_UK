<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="StaffNoteAdd.aspx.cs" Inherits="SalonPortal.SecureArea.StaffNoteAdd" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
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
    <SA:Message ID="lblMessage" runat="server" />
    <ajaxToolkit:TabContainer runat="server" ID="NoteTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel ID="pnlNotes" runat="server" >
           <ContentTemplate>
                <table class="details">
                     <tr>
                        <td class="title" style="vertical-align:top;">
                            <SA:ToolTipLabel 
                                ID="lblNote" 
                                runat="server" 
                                meta:resourceKey="lblNote"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <div>
                                <b id="note_chars">1000</b>
                            </div>
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" style="width:550px;" Height="120px" onkeyUp="txtNote_onkeyUp(this)" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvNote" ControlToValidate="txtNote" runat="server" Display="None" meta:resourceKey="rfvNote" ></asp:RequiredFieldValidator>
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvNoteE" TargetControlID="rfvNote" HighlightCssClass="validator-highlight" />
                        </td>
                    </tr>
                 </table>
            <script type="text/javascript" language="javascript" >
                function txtNote_onkeyUp(sender)
                {
                    if(sender != null)
                    {
                        var max = 1000;
                        var label = document.getElementById("note_chars");
                        if(label != null)
                        {
                           var used = sender.value.length;
                           if(used > max)
                           {
                               sender.value = sender.value.substring(0,max);
                               used = max;
                           }
                           var left = max - used;
                           label.innerHTML = left + "";
                        }
                    }
                }
            </script>
           </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Content>
