<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeadReport.ascx.cs" Inherits="SalonAddict.Administration.Modules.LeadReport" %>
<table cellpadding="0" cellspacing="0" class="details" >
    <tr>
        <td>
           <img src="images/ico-attention.jpg" />
        </td>
        <td class="data-item">
            <b>Opportunities</b>&nbsp;&nbsp; - <i><asp:Literal ID="ltrCount" runat="server" EnableViewState="false" ></asp:Literal> new lead(s)</i>
        </td>
    </tr>
</table>
<div style="height:200px; overflow:auto;" >
<asp:GridView 
    ID="gv" 
    runat="server" 
    Width="400px"
    AutoGenerateColumns="False" >
    <Columns>
        <asp:TemplateField HeaderText="Business" >
            <ItemTemplate>
                <%# Eval("Company") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Source">
            <ItemTemplate>
               <%# Eval("SourceFullName")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <a href="<%# "LeadDetails.aspx?LeadID=" + Eval("LeadID") %>" title="Manage lead">
                    manage
                </a>
            </ItemTemplate>
            <ItemStyle Width="60px" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</div>