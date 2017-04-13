<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeadDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.LeadDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Administration/Modules/TextBoxes/EmailTextBox.ascx" %>

<% if(this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-marketing.gif" alt="" />
        Add a new lead <a href="Leads.aspx" title="Back to lead list">(back to lead list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ToolTip="Add lead" />
    </div>
</div>
<% } 
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-marketing.gif" alt="" />
        Edit lead details <a href="Leads.aspx" title="Back to lead list">(back to lead list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save lead changes" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete lead" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<% } %>
<ajaxToolkit:TabContainer runat="server" ID="UserTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlLeadInfo" HeaderText="Lead Info">
        <ContentTemplate>
            <table class="details">
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblLeadStatus" 
                            runat="server" 
                            Text="Status:"
                            IsRequired="true"
                            ToolTip="The lead status." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:DropDownList ID="ddlStatusTypes" runat="server" ></asp:DropDownList>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblTopic" 
                            runat="server" 
                            Text="Topic:"
                            ToolTip="Lead topic." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtTopic" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblFirstName" 
                            runat="server" 
                            Text="First name:"
                            ToolTip="The lead's contact first name (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtFirstName" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblLastName" 
                            runat="server" 
                            Text="Last name:"
                            ToolTip="The lead's contact last name (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtLastName" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblJobTitle" 
                            runat="server" 
                            Text="Job title:"
                            ToolTip="The lead's contact job title (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtJobTitle" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCompany" 
                            runat="server" 
                            Text="Company:"
                            IsRequired="true"
                            ToolTip="Company name." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox ID="txtCompany" runat="server" MaxLength="100" ErrorMessage="Company is a required field." />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblWebsite" 
                            runat="server" 
                            Text="Website:"
                            ToolTip="The lead's website (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtWebsite" runat="server" MaxLength="400" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblAddressLine1" 
                            runat="server" 
                            Text="Address:"
                            ToolTip="The lead's address (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtAddressLine1" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblAddressLine2" 
                            runat="server" 
                            Text="Address continued:"
                            ToolTip="The leads's address continued (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtAddressLine2" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCityTown" 
                            runat="server" 
                            Text="City/Town:"
                            IsRequired="true"
                            ToolTip="The leads's city/town (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtCityTown" 
                            runat="server" 
                            MaxLength="100"  
                            ErrorMessage="City/town is a required field." />
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblStateProvince" 
                            runat="server" 
                            Text="State/Province:"
                            ToolTip="The leads's state/province (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtStateProvince" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblZipPostalCode" 
                            runat="server" 
                            Text="Zip/Postal code:"
                            ToolTip="The lead's zip/postal code (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtZipPostalCode" runat="server" MaxLength="10" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCountry" 
                            runat="server" 
                            Text="Country:"
                            IsRequired="true"
                            ToolTip="The lead's country (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtCountry" 
                            runat="server" 
                            MaxLength="100"  
                            ErrorMessage="Country is a required field." />
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblPhoneNumber" 
                            runat="server" 
                            Text="Phone number:"
                            ToolTip="The lead's phone number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtPhoneNumber" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblFaxNumber" 
                            runat="server" 
                            Text="Fax number:"
                            ToolTip="The lead's fax number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtFaxNumber" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblEmail" 
                            runat="server" 
                            Text="Email address:"
                            ToolTip="The lead's email address." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:EmailTextBox 
                            ID="txtEmail" 
                            runat="server" 
                            IsRequired="false" />
                    </td>
                </tr>
                <% if (this.Action == ActionType.Edit)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCreatedOn" 
                            runat="server" 
                            Text="Registration Date:"
                            ToolTip="Date when the user account was created." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Literal ID="ltrCreatedOn" runat="server" ></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblUpdatedOn" 
                            runat="server" 
                            Text="Last Updated:"
                            ToolTip="Date when the user account was last updated." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Literal ID="ltrUpdatedOn" runat="server" ></asp:Literal>
                    </td>
                </tr> 
                <% } %>
             </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlSource" HeaderText="Source" >
       <ContentTemplate>
            <table class="details">
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSourceIPAddress" 
                            runat="server" 
                            Text="IP Address:"
                            IsRequired="true"
                            ToolTip="The source's ip address." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtIPAddress" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSourcePageURL" 
                            runat="server" 
                            Text="Page URL:"
                            IsRequired="true"
                            ToolTip="The source's page URL (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtSourcePageURL" runat="server" MaxLength="400" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSourceFirstName" 
                            runat="server" 
                            Text="First name:"
                            IsRequired="true"
                            ToolTip="The source's first name (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtSourceFirstName" 
                            runat="server" 
                            MaxLength="100"
                            ErrorMessage="Source first name is a required field." />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSourceLastName" 
                            runat="server" 
                            Text="Last name:"
                            IsRequired="true"
                            ToolTip="The source's last name (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtSourceLastName" 
                            runat="server" 
                            MaxLength="100"
                            ErrorMessage="Source last name is a required field." />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSourcePhone" 
                            runat="server" 
                            Text="Phone number:"
                            ToolTip="The source's phone number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtSourcePhoneNumber" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSourceMobile" 
                            runat="server" 
                            Text="Mobile number:"
                            ToolTip="The source's mobile number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtSourceMobile" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSourceEmail" 
                            runat="server" 
                            Text="Email address:"
                            ToolTip="The source's email address." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:EmailTextBox 
                            ID="txtSourceEmail" 
                            runat="server" 
                            IsRequired="false" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSourceComment" 
                            runat="server" 
                            Text="Comment:"
                            ToolTip="The source's comment." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtSourceComment" runat="server" TextMode="MultiLine" Height="80px" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblContactSource" 
                            runat="server" 
                            Text="Allows contact:"
                            ToolTip="The value indicating whether or not the source wants to be contacted." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbContactSource" runat="server" />
                    </td>
                </tr>
           </table>
       </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlNotes" HeaderText="Notes" >
       <ContentTemplate>
            <asp:UpdatePanel ID="upNotes" runat="server" >
               <ContentTemplate>
                     <asp:GridView 
                        ID="gvNotes" 
                        runat="server" 
                        Width="100%"
                        DataKeyNames="LeadNoteID"
                        OnRowDeleting="gvNotes_RowDeleting"
                        AutoGenerateColumns="False" >
                        <Columns> 
                            <asp:TemplateField HeaderText="Note" >
                                <ItemTemplate>
                                    <%# Server.HtmlEncode(Eval("Note").ToString())  %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="50px" >
                                <ItemTemplate>
                                   <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClientClick="return confirm('Are you sure?')" CommandName="Delete" CausesValidation="false" ></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                       </Columns>
                    </asp:GridView>
                    <br /><hr /><br />
                    <table style="margin-left:-10px;" cellpadding="0" cellspacing="10" >
                       <tr>
                          <td>
                             <asp:TextBox 
                                ID="txtNewNote" 
                                runat="server" 
                                TextMode="MultiLine" 
                                Height="80px" 
                                EnableViewState="false"
                                Width="600px" >
                             </asp:TextBox>
                          </td>
                       </tr>
                       <tr>
                          <td style="text-align:right">
                             &nbsp; <asp:Button ID="btnAddNote" runat="server" Text="Add New Note" OnClick="btnAddNote_Click" CausesValidation="false" />
                          </td>
                       </tr>
                    </table>
               </ContentTemplate>
            </asp:UpdatePanel>
       </ContentTemplate>
    </ajaxToolkit:TabPanel>
 </ajaxToolkit:TabContainer>