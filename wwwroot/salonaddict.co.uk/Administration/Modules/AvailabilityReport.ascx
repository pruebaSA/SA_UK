<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AvailabilityReport.ascx.cs" Inherits="SalonAddict.Administration.Modules.AvailabilityReport" %>

<table cellpadding="0" cellspacing="0" width="100%" class="details" >
    <tr>
        <td style="width:24px;">
           <img src="images/ico-attention.jpg" />
        </td>
        <td class="data-item">
            <b>No Availability</b>
            &nbsp; - <i><asp:Literal ID="ltrCount" runat="server" EnableViewState="false" ></asp:Literal> salon(s)</i>.
        </td>
        <td style="text-align:right;" class="data-item" >
            Alert me on: &nbsp;
            <asp:DropDownList ID="ddlWeek" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlWeek_SelectedIndexChanged">
                <asp:ListItem Text="week one" value="1" ></asp:ListItem>
                <asp:ListItem Text="week two" value="2" ></asp:ListItem>
                <asp:ListItem Text="week three" value="3" ></asp:ListItem>
                <asp:ListItem Text="week four" value="4" ></asp:ListItem>
                <asp:ListItem Text="week five" value="5" ></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
       <td colspan="3" >
            <asp:GridView 
                ID="gv" 
                runat="server" 
                Width="100%"
                PageSize="10"
                AllowPaging="true"
                OnPageIndexChanging="gv_PageIndexChanging"
                AutoGenerateColumns="False" >
                <Columns>
                    <asp:TemplateField HeaderText="Business" >
                        <ItemTemplate>
                            <a href="<%# "BusinessDetails.aspx?BusinessGUID=" + Eval("BusinessGUID") %>" >
                                <%# Eval("BusinessName") %>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Phone" >
                        <ItemTemplate>
                            <%# Eval("PhoneNumber") %>
                        </ItemTemplate>
                        <ItemStyle Width="100px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Email" >
                        <ItemTemplate>
                            <%# Eval("Email") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Week One (hr)">
                        <ItemTemplate>
                             <%# ((double)Eval("SumWeekOneHours") == 0) ? "<img src=\"images/ico-warnings.jpg\" />" : ((double)Eval("SumWeekOneHours") == -1) ? String.Empty : ((double)Eval("SumWeekOneHours")).ToString("N")%>
                        </ItemTemplate>
                        <ItemStyle Width="100px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Week Two (hr)">
                        <ItemTemplate>
                           <%# ((double)Eval("SumWeekTwoHours") == 0) ? "<img src=\"images/ico-warnings.jpg\" />" : ((double)Eval("SumWeekTwoHours") == -1) ? String.Empty : ((double)Eval("SumWeekTwoHours")).ToString("N")%>
                        </ItemTemplate>
                        <ItemStyle Width="100px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Week Three (hr)">
                        <ItemTemplate>
                           <%# ((double)Eval("SumWeekThreeHours") == 0) ? "<img src=\"images/ico-warnings.jpg\" />" : ((double)Eval("SumWeekThreeHours") == -1) ? String.Empty : ((double)Eval("SumWeekThreeHours")).ToString("N")%>
                        </ItemTemplate>
                        <ItemStyle Width="100px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Week Four (hr)">
                        <ItemTemplate>
                           <%# ((double)Eval("SumWeekFourHours") == 0) ? "<img src=\"images/ico-warnings.jpg\" />" : ((double)Eval("SumWeekFourHours") == -1) ? String.Empty : ((double)Eval("SumWeekFourHours")).ToString("N")%>
                        </ItemTemplate>
                        <ItemStyle Width="100px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Week Five (hr)">
                        <ItemTemplate>
                           <%# ((double)Eval("SumWeekFiveHours") == 0) ? "<img src=\"images/ico-warnings.jpg\" />" : ((double)Eval("SumWeekFiveHours") == -1) ? String.Empty : ((double)Eval("SumWeekFiveHours")).ToString("N")%>
                        </ItemTemplate>
                        <ItemStyle Width="100px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
       </td>
    </tr>
</table>