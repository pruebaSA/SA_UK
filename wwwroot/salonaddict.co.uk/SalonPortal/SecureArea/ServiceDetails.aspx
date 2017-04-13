﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="ServiceDetails.aspx.cs" Inherits="SalonPortal.SecureArea.ServiceDetails" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ServiceTypeList" Src="~/SecureArea/Modules/ServiceTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="ServiceCategoryList" Src="~/SecureArea/Modules/ServiceCategoryDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/SecureArea/Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/SecureArea/Modules/DecimalTextBox.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>
<%@ Import Namespace="System.Linq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
    <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-services.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
            <asp:Button ID="btnDelete" CausesValidation="false" runat="server" OnClick="btnDelete_Click" meta:resourceKey="btnDelete" />
        </div>
    </div>
    <SA:Message ID="lblMessage" runat="server" />
    <ajaxToolkit:TabContainer runat="server" ID="ServiceTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlServiceInfo" >
            <ContentTemplate>
                <asp:UpdatePanel ID="up" runat="server" >
                   <ContentTemplate>
                       <table class="details" >
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
                                        MaxLength="100"
                                        Width="450px" >
                                    </SA:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblShortDescription" 
                                        runat="server"  
                                        meta:resourceKey="lblShortDescription"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                </td>
                                <td class="data-item">
                                    <asp:TextBox 
                                        ID="txtShortDescription" 
                                        runat="server" 
                                        MaxLength="200"
                                        Width="450px" >
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblServiceType" 
                                        runat="server"  
                                        meta:resourceKey="lblServiceType"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                    <span class="required" >*</span>
                                </td>
                                <td class="data-item">
                                    <SA:ServiceTypeList 
                                        ID="ddlServiceType" 
                                        runat="server" 
                                        AutoPostback="true"
                                        OnSelectedIndexChanged="ddlServiceType_SelectedIndexChanged"
                                        meta:resourceKey="ddlServiceType" />
                                </td>
                            </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblServiceCategory" 
                                        runat="server"  
                                        meta:resourceKey="lblServiceCategory"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                    <span class="required" >*</span>
                                </td>
                                <td class="data-item">
                                    <SA:ServiceCategoryList 
                                        ID="ddlServiceCategory" 
                                        runat="server" 
                                        meta:resourceKey="ddlServiceCategory" />
                                </td>
                            </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblOldPrice" 
                                        runat="server"  
                                        meta:resourceKey="lblOldPrice"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                </td>
                                <td class="data-item">
                                    <SA:DecimalTextBox 
                                        ID="txtOldPrice" 
                                        runat="server" 
                                        MinimumValue="0" 
                                        MaximumValue="999"
                                        IsRequired="false"
                                        meta:resourceKey="txtOldPrice">
                                    </SA:DecimalTextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblPrice" 
                                        runat="server"  
                                        meta:resourceKey="lblPrice"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                </td>
                                <td class="data-item">
                                    <SA:DecimalTextBox 
                                        ID="txtPrice" 
                                        runat="server" 
                                        MinimumValue="0" 
                                        MaximumValue="999"
                                        meta:resourceKey="txtPrice">
                                    </SA:DecimalTextBox>
                                </td>
                            </tr>
                        </table>
                   </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="pnlStaff" runat="server" >
           <ContentTemplate>
              <div style="text-align:right;margin-bottom:10px;" >
                    <asp:Button ID="btnAddStaff" runat="server" meta:resourceKey="btnAddStaff" OnClick="btnAddStaff_Click" />
              </div>
              <asp:GridView 
                ID="gvServiceStaffMappings" 
                runat="server" 
                OnRowCommand="gvServiceStaffMappings_RowCommand"
                OnRowCreated="gvServiceStaffMappings_RowCreated"
                AutoGenerateColumns="false" >
                  <Columns>
                     <asp:TemplateField>
                        <ItemTemplate> 
                            <asp:HyperLink ID="hlStaff" runat="server" ></asp:HyperLink>
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField>
                        <ItemTemplate> 
                            <asp:Literal ID="ltrPrice" runat="server" ></asp:Literal>
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField>
                        <ItemTemplate> 
                            <%# SalonAddict.BusinessAccess.Implementation.CurrencyManager.FormatPrice(
                                decimal.Parse(Eval("AdditionalFee").ToString())) %>
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField>
                        <ItemTemplate> 
                            <asp:Literal ID="ltrTotal" runat="server" ></asp:Literal>
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" >
                        <ItemTemplate> 
                            <%# Eval("DurationInMinutes") %>
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField ItemStyle-Width="50px">
                        <ItemTemplate> 
                            <asp:LinkButton ID="lbRemove" runat="server" CommandName="REMOVE" CommandArgument='<%# Eval("StaffServiceMappingID") %>' >
                                <%= base.GetGlobalResourceObject("Global", "GridView_Link_Remove").ToString() %>
                            </asp:LinkButton>
                        </ItemTemplate>
                     </asp:TemplateField>
                  </Columns>
              </asp:GridView>
           </ContentTemplate>
        </ajaxToolkit:TabPanel>
     </ajaxToolkit:TabContainer>
</asp:Content>
