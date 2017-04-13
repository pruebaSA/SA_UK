<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="StaffDetails.aspx.cs" Inherits="SalonPortal.SecureArea.StaffDetails" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/SecureArea/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/SecureArea/Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="GenderList" Src="~/SecureArea/Modules/GenderDropDownList.ascx" %>
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
            <asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click" CausesValidation="false" meta:resourceKey="btnDelete" />
        </div>
    </div>
    <br />
    <SA:Message ID="lblMessage" runat="server" />
    <ajaxToolkit:TabContainer runat="server" ID="StaffTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlStaffInfo" >
            <ContentTemplate>
                <table class="details">
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDisplayName" 
                                runat="server"  
                                meta:resourceKey="lblDisplayName"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtDisplayName" 
                                runat="server" 
                                meta:resourceKey="txtDisplayName"
                                MaxLength="100" >
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblGender" 
                                runat="server"  
                                meta:resourceKey="lblGender"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:GenderList ID="ddlGender" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblEmployeeNo" 
                                runat="server" 
                                meta:resourceKey="lblEmployeeNo"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtEmployeeNumber" runat="server" MaxLength="80" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblTitle" 
                                runat="server"  
                                meta:resourceKey="lblTitle"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtTitle" runat="server" MaxLength="80" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblMobile" 
                                runat="server" 
                                meta:resourceKey="lblMobile"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtMobile" runat="server" MaxLength="50" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblEmail" 
                                runat="server" 
                                meta:resourceKey="lblEmail"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <SA:EmailTextBox ID="txtEmail" runat="server" MaxLength="100" IsRequired="false" />
                        </td>
                    </tr>
                 </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="pnlServices" runat="server" >
           <ContentTemplate>
                <div style="text-align:right;margin-bottom:10px;" >
                    <asp:Button ID="btnAddService" runat="server" meta:resourceKey="btnAddService" OnClick="btnAddService_Click" />
                </div>
                <asp:GridView 
                    ID="gvServices" 
                    runat="server" 
                    Width="100%"
                    DataKeyNames="ServiceID"
                    OnRowCommand="gvServices_RowCommand"
                    OnRowCreated="gvServices_RowCreated"
                    AutoGenerateColumns="False" >
                    <Columns> 
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="ServiceDetails.aspx?ServiceGUID=<%# Eval("ServiceGUID") %>" >
                                    <%# Eval("DisplayName") %>
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <%# SalonAddict.BusinessAccess.Implementation.CurrencyManager.FormatPrice((decimal)Eval("Price")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Literal ID="ltrAdditionalFee" runat="server" ></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Literal ID="ltrTotal" runat="server" ></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Literal ID="ltrDuration" runat="server" ></asp:Literal>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="50px">
                            <ItemTemplate>
                            <asp:LinkButton ID="lbRemove" runat="server" CommandName="REMOVE" CommandArgument='<%# Eval("ServiceID") %>' >
                                <%= base.GetGlobalResourceObject("Global", "GridView_Link_Remove").ToString() %>
                            </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
           </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlNotes" >
            <ContentTemplate>
                <asp:UpdatePanel ID="upNotes" runat="server" >
                   <ContentTemplate>
                         <div style="text-align:right;margin-bottom:10px;" >
                            <asp:Button ID="btnAddNote" runat="server" meta:resourceKey="btnAddNote" OnClick="btnAddNote_Click" CausesValidation="false" />
                         </div>
                         <asp:GridView 
                            ID="gvNotes" 
                            runat="server" 
                            Width="100%"
                            DataKeyNames="StaffNoteID"
                            OnRowDeleting="gvNotes_RowDeleting"
                            OnRowCreated="gvNotes_RowCreated"
                            AutoGenerateColumns="False" >
                            <Columns> 
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%# Server.HtmlEncode(Eval("Note").ToString())  %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="110px">
                                    <ItemTemplate>
                                        <%# Eval("CreatedOn").ToString()  %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="50px" >
                                    <ItemTemplate>
                                       <asp:LinkButton ID="lbDelete" runat="server" CommandName="Delete" CausesValidation="false" ></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                           </Columns>
                        </asp:GridView>
                   </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Content>
