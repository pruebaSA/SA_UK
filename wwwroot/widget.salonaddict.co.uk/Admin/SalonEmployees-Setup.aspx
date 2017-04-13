<%@ Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="SalonEmployees-Setup.aspx.cs" Inherits="IFRAME.Admin.SalonEmployees_SetupPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
       <IFRM:Menu ID="cntrlMenu" runat="server" SelectedItem="Salons" />
       <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-salons.png" %>' alt="salons" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><asp:Literal ID="ltrHeader" runat="server" ></asp:Literal></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td width="500px" >
                <asp:UpdatePanel ID="up" runat="server" >
                   <ContentTemplate>
                    <table style="margin-top:-8px;border-collapse:collapse; width:370px" class="gridview" cellpadding="0" cellspacing="0" border="1" >
                       <tr class="header-style">
                           <th scope="col" width="30px" ></th>
		                   <th scope="col" width="190px" ><%= base.GetLocaleResourceString("gv.Columns[0].HeaderText") %></th>
		                   <th scope="col" ><%= base.GetLocaleResourceString("gv.Columns[1].HeaderText") %></th>
		               </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb1" runat="server" Checked="true" Enabled="false" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName1" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName1" runat="server" ControlToValidate="txtName1" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName1Ex" runat="Server" TargetControlID="valName1" EnableViewState="false" />
                            </td>
                            <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader1" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent1" runat="server" >
                                 <asp:Repeater ID="rptrServices1" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx1" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent1" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader1"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb2" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName2" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName2" runat="server" ControlToValidate="txtName2" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName2Ex" runat="Server" TargetControlID="valName2" EnableViewState="false" />
                            </td>
                            <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader2" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent2" runat="server" >
                                 <asp:Repeater ID="rptrServices2" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx2" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent2" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader2"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb3" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName3" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName3" runat="server" ControlToValidate="txtName3" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName3Ex" runat="Server" TargetControlID="valName3" EnableViewState="false" />
                            </td>
                            <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader3" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent3" runat="server" >
                                 <asp:Repeater ID="rptrServices3" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx3" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent3" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader3"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb4" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName4" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName4" runat="server" ControlToValidate="txtName4" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName4Ex" runat="Server" TargetControlID="valName4" EnableViewState="false" />
                            </td>
                            <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader4" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent4" runat="server" >
                                 <asp:Repeater ID="rptrServices4" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx4" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent4" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader4"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb5" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName5" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName5" runat="server" ControlToValidate="txtName5" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName5Ex" runat="Server" TargetControlID="valName5" EnableViewState="false" />
                            </td>
			                <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader5" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent5" runat="server" >
                                 <asp:Repeater ID="rptrServices5" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx5" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent5" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader5"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb6" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName6" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName6" runat="server" ControlToValidate="txtName6" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName6Ex" runat="Server" TargetControlID="valName6" EnableViewState="false" />
                            </td>
			                <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader6" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent6" runat="server" >
                                 <asp:Repeater ID="rptrServices6" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx6" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent6" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader6"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb7" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName7" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName7" runat="server" ControlToValidate="txtName7" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName7Ex" runat="Server" TargetControlID="valName7" EnableViewState="false" />
                            </td>
			                <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader7" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent7" runat="server" >
                                 <asp:Repeater ID="rptrServices7" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx7" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent7" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader7"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb8" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName8" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName8" runat="server" ControlToValidate="txtName8" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName8Ex" runat="Server" TargetControlID="valName8" EnableViewState="false" />
                            </td>
			                <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader8" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent8" runat="server" >
                                 <asp:Repeater ID="rptrServices8" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx8" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent8" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader8"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb9" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName9" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName9" runat="server" ControlToValidate="txtName9" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName9Ex" runat="Server" TargetControlID="valName9" EnableViewState="false" />
                            </td>
			                <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader9" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent9" runat="server" >
                                 <asp:Repeater ID="rptrServices9" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx9" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent9" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader9"
                                UseShadow="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:12px 10px 5px 10px;" >
                               <asp:CheckBox ID="cb10" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="cb_CheckedChanged" />
                            </td>
                            <td style="padding:5px 10px 5px 10px;" >
                               <asp:TextBox ID="txtName10" runat="server" SkinID="TextBox" MaxLength="30" ></asp:TextBox>
                               <asp:RequiredFieldValidator ID="valName10" runat="server" ControlToValidate="txtName10" Display="None" meta:resourceKey="valName" ></asp:RequiredFieldValidator>
                               <ajaxToolkit:ValidatorCalloutExtender ID="valName10Ex" runat="Server" TargetControlID="valName10" EnableViewState="false" />
                            </td>
			                <td style="padding-top:4px;">
                              <center>
                                 <asp:LinkButton ID="lbServiceHeader10" runat="server" Enabled="false" SkinID="SchedulingAddButton" meta:resourceKey="lbAdd" ></asp:LinkButton>
                              </center>
                              <asp:Panel ID="pnlServiceContent10" runat="server" >
                                 <asp:Repeater ID="rptrServices10" runat="server" >
                                    <HeaderTemplate>
                                        <table style="margin:-4px" cellpadding="0" cellspacing="4" >
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                           <td style="padding-top:2px" >
                                              <asp:CheckBox ID="cb" runat="server" />
                                           </td>
                                           <td>
                                              <%# String.Format("{0} £{1}", System.Web.HttpUtility.HtmlEncode(Eval("Name").ToString()), ((Decimal)(Eval("Price"))).ToString("#,#.00#"))%>
                                              <asp:Literal ID="ltrServiceId" runat="server" Visible="false" Text='<%# Eval("ServiceId") %>' ></asp:Literal>
                                           </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                 </asp:Repeater>
                              </asp:Panel>
                              <ajaxToolkit:BalloonPopupExtender 
                                ID="pnlServiceEx10" 
                                runat="server"
                                BalloonPopupControlID="pnlServiceContent10" 
                                BalloonSize="Medium" 
                                BalloonStyle="Rectangle"
                                CacheDynamicResults="false"
                                DisplayOnClick="true"
                                DisplayOnMouseOver="false"
                                DisplayOnFocus="false"
                                Position="TopLeft"
                                OffsetX="15"
                                OffsetY="10"
                                TargetControlID="lbServiceHeader10"
                                UseShadow="false" />
                            </td>
                        </tr>
                    </table>
                   </ContentTemplate>
                </asp:UpdatePanel>
                <table style="margin-top:10px" class="details" runat="server" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td class="title" >
                            <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                            <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
                        </td>
                    </tr>
                </table>
              </td>
              <td>
         
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>